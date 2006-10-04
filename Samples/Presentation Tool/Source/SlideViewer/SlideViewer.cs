/* 
 * Copyright (c) 2003-2006, University of Maryland
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided
 * that the following conditions are met:
 * 
 *		Redistributions of source code must retain the above copyright notice, this list of conditions
 *		and the following disclaimer.
 * 
 *		Redistributions in binary form must reproduce the above copyright notice, this list of conditions
 *		and the following disclaimer in the documentation and/or other materials provided with the
 *		distribution.
 * 
 *		Neither the name of the University of Maryland nor the names of its contributors may be used to
 *		endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 * TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * Piccolo was written at the Human-Computer Interaction Laboratory www.cs.umd.edu/hcil by Jesse Grosjean
 * and ported to C# by Aaron Clamage under the supervision of Ben Bederson.  The Piccolo website is
 * www.cs.umd.edu/hcil/piccolo.
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.IO;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Activities;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloX;

namespace UMD.HCIL.SlideViewer {
	/// <summary>
	/// A Simple Piccolo Presentation control.  It shows all the images in a specified directory
	/// in a style suitable for making a presentation.  There is a navigation bar that shows
	/// thumbnails of the slides and uses a simple "fisheye" view of the thumbnails to enable
	/// navigation, even when there are more slides than can easily fit.
	/// </summary>
	public class SlideViewer : PCanvas {
		#region Fields
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Constants which control layout and interaction
		protected const String  IMAGE_RESOURCE_STREAM = "UMD.HCIL.SlideViewer.Images";
		protected const int     DEFAULT_SLIDES_COUNT = 22;
		protected const long    ANIMATION_TIME_MILLIS = 600;
		protected const long    SLIDEBAR_ANIMATION_TIME_MILLIS = 300;
		protected const int     SLIDE_BAR_HEIGHT = 100;
		protected const int     THUMB_HEIGHT = 250;
		protected const long    IMAGE_PADDING = 10;

		// Components
		protected PNode slideBar;						// The navigation bar.
		protected PMultiSizeImage focusSlide;			// The currently focused slide.
		protected PNode hiliteSlide;                    // Slide currently selected in slidebar
		protected PNodeList slides;						// List of slides in order
		protected bool slideBarVisible = false;			// True to show slide bar
		protected PActivity[] slideActivities;			// Array of active activities per slide animation
		protected PNode currentPosition;				// Visual indicator of current position
		#endregion

		#region Initialization
		/// <summary>
		/// Constructs a new slideviewer control.
		/// </summary>
		public SlideViewer() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			slides = new PNodeList();
			slideBar = new PNode();
			slideBar.Brush = Brushes.DarkGray;
			slideBar.Bounds = new RectangleF(0, 0, Width, SLIDE_BAR_HEIGHT);

			// Add the navigation event handlers and set the keyboard focus.
			Camera.MouseDown += new PInputEventHandler(Camera_MouseDown);
			Camera.KeyDown += new PInputEventHandler(Camera_KeyDown);
			Camera.MouseMove += new PInputEventHandler(Camera_MouseMove);
			Root.DefaultInputManager.KeyboardFocus = Camera.ToPickPath();

			// Remove the default zoom and pan event handlers.
			ZoomEventHandler = null;
			PanEventHandler = null;

			this.HandleCreated += new EventHandler(SlideViewer_HandleCreated);
		}

		/// <summary>
		/// Loads the default Piccolo presentation
		/// </summary>
		public void LoadDefaultSlides() {
			LoadSlides(null);
		}

		/// <summary>
		/// Load all images in the specified directory as slides.
		/// </summary>
		public void LoadSlides(string folder) {
			hiliteSlide = null;
			slideBar.RemoveAllChildren();
			slides.Clear();
			Layer.AddChild(slideBar);

			if (folder == null) {
				AddDefaultSlides();
			} else {
				AddSlides(folder);
			}

			// Create visual indicator of current position in slide bar
			Bitmap arrowBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream(IMAGE_RESOURCE_STREAM + ".up-arrow.gif"));
			currentPosition = new PImage(arrowBitmap);
			currentPosition.Pickable = false;
			slideBar.AddChild(currentPosition);

			slideActivities = new PActivity[slides.Count];

			LayoutSlides();		
			GoToSlide((PMultiSizeImage)slideBar[0]);
		}

		/// <summary>
		/// Add all images in the default Piccolo presentation as slides.
		/// </summary>
		public void AddDefaultSlides() {
			int imageNum = 0;
			for (int i = 1; i <= DEFAULT_SLIDES_COUNT; i++) {
				try {
					Image hiRes = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream(IMAGE_RESOURCE_STREAM + ".Slide" + i + ".PNG"));
					AddSlide(hiRes, imageNum++);
				} catch (Exception) {
				}
			}
		}

		/// <summary>
		/// Add all images in the specified directory as slides.
		/// </summary>
		public void AddSlides(String folder) {
			String[] slideFiles = Directory.GetFiles(folder);
			// Special support for powerpoint generated slides which have filenames "slide##".  We want to sort by the "##".
			Array.Sort(slideFiles, new SlideComparer());
			int imageNum = 0;
			for (int i = 0; i < slideFiles.Length; i++) {
				try {
					Image hiRes = new Bitmap(slideFiles[i]);
					AddSlide(hiRes, imageNum++);
				} catch (Exception) {
				}
			}
		}

		/// <summary>
		/// Add the given slide at the given index.
		/// </summary>
		public void AddSlide(Image hiRes, int index) {
			int thumbHeight = THUMB_HEIGHT;		
			int thumbWidth = (int) (hiRes.Width * ((float)thumbHeight / hiRes.Height));

			// Create thumb manually.  Don't use Image.GetThumbnailImage() as it results in images that are very slow to render
			Image thumb = new Bitmap(thumbWidth, thumbHeight);
			Graphics g = Graphics.FromImage(thumb);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.DrawImage(hiRes, 0, 0, thumbWidth, thumbHeight);
			g.Dispose();

			PMultiSizeImage slide = new PMultiSizeImage(thumb, hiRes, true);
			slide.Tag = index;

			slideBar.AddChild(slide);
			slides.Add(slide);
		}

		// Special support for powerpoint generated slides which have filenames "Slide##.xxx".  We want to sort by the "##".
		class SlideComparer : IComparer {
			public int Compare(object o1, object o2) {
				string s1 = o1 as string;
				string s2 = o2 as string;
				int index1 = s1.LastIndexOf("Slide") + 5;
				int index2 = s2.LastIndexOf("Slide") + 5;
				int dotindex1 = s1.LastIndexOf(".");
				int dotindex2 = s2.LastIndexOf(".");
				if ((index1 != -1) && (index2 != -1)) {
					try {
						int i1 = Int32.Parse(s1.Substring(index1, (dotindex1 - index1)));
						int i2 = Int32.Parse(s2.Substring(index2, (dotindex2 - index2)));
						return i1.CompareTo(i2);
					} catch (Exception) {
						return s1.CompareTo(s2);
					}
				} else {
					return s1.CompareTo(s2);
				}
			}
		}
		#endregion

		#region Slide Layout
		/// <summary>
		/// Layout all the slides.
		/// </summary>
		public void LayoutSlides() {
			if (slideBar != null) {
				slideBar.Bounds = new RectangleF(slideBar.X, slideBar.Y, Width, SLIDE_BAR_HEIGHT);
				if (SlideBarVisible) {
					slideBar.SetOffset(0, Height - SLIDE_BAR_HEIGHT);
				} else {
					slideBar.SetOffset(0, Height);
				}
				for (int i = 0; i < slides.Count; i++) {	
					PNode slide = slides[i];
					slideBar.AddChild(i, slide);  // Enforce order

					if (slide != focusSlide) {
						slide.Matrix = GetUnfocusedMatrix(slide, (int)slide.Tag);
					} else {
						slide.Matrix = GetFocusedMatrix(slide);
					}
				}

				LayoutCurrentPosition();
			}
		}

		public void LayoutCurrentPosition() {
			// Move the current position indicator to the new focus slide's spot in the slide bar
			if (focusSlide == null) {
				currentPosition.Visible = false;
			} else {
				PMatrix matrix = GetUnfocusedMatrix(currentPosition, (int)focusSlide.Tag);
				matrix.TranslateBy(0, -IMAGE_PADDING);
				currentPosition.AnimateToMatrix(matrix, ANIMATION_TIME_MILLIS);
				currentPosition.Visible = true;
			}
		}

		/// <summary>
		/// Control whether the slide bar is visible or hidden
		/// </summary>
		public bool SlideBarVisible {
			get { return slideBarVisible; }
			set {
				if (slideBarVisible != value) {
					slideBarVisible = value;
					float y;
					if (slideBarVisible) {
						y = Height - SLIDE_BAR_HEIGHT;
					} else {
						y = Height;
					}
					slideBar.AnimateToPositionScaleRotation(0, y, 1, 0, ANIMATION_TIME_MILLIS);
					GoToSlide(focusSlide);  // Do this to recompute focus slide size
				}
			}
		}

		// Get the matrix that will transform the node to fill the main pane.
		public PMatrix GetFocusedMatrix(PNode slide) {
			float scale;
			int width = Width;
			int height = Height;
			if (SlideBarVisible) {
				height -= SLIDE_BAR_HEIGHT;
			}

			if ((width / slide.Width) < (height / slide.Height)) {
				scale = width / slide.Width;
				height = (int) (slide.Height * scale);
			} else {
				scale = height / slide.Height;
				width = (int) (slide.Width * scale);
			}

			PMatrix matrix = new PMatrix();
			matrix.Scale = scale;
			matrix.OffsetX = (Width - width) / 2;
			if (SlideBarVisible) {
				matrix.OffsetY = - (Height - SLIDE_BAR_HEIGHT);
			} else {
				matrix.OffsetY = - (Height);
			}
			return matrix;
		}

		/// <summary>
		/// Get the matrix that will transform the slide to fit in the slide bar.
		/// </summary>
		public PMatrix GetUnfocusedMatrix(PNode node, int index) {
			float scale = 1;
			if (node.Tag != null) {  // Only scale actual slides
				scale = (slideBar.Height - IMAGE_PADDING) / node.Height;
			}
			PMatrix matrix = new PMatrix();
			float maxSpacePerSlide = (Width - (node.Width * scale)) / (slides.Count - 1);
			matrix.Scale = scale;
			matrix.OffsetX = index * maxSpacePerSlide;
			matrix.OffsetY = IMAGE_PADDING;
			return matrix;
		}

		/// <summary>
		/// Go to the specified slide.  If it is already shown, then recompute its layout.
		/// </summary>
		public void GoToSlide(PMultiSizeImage slide) {
			if (hiliteSlide != slide) {
				HiliteSlide(-1);  // Unhilite any slide that might be hilited
			}

			// First unfocus any slide that has focus
			if ((focusSlide != null) && (focusSlide != slide)) {
				((PMultiSizeImage)focusSlide).ShowThumb = true;
				focusSlide.AnimateToMatrix(GetUnfocusedMatrix(focusSlide, (int)focusSlide.Tag), ANIMATION_TIME_MILLIS);
			}

			// Then set the focus to the new slide
			focusSlide = slide;
			if (focusSlide != null) {
				((PMultiSizeImage)focusSlide).Hilite = false;
				PTransformActivity activity = focusSlide.AnimateToMatrix(GetFocusedMatrix(focusSlide), ANIMATION_TIME_MILLIS);
				activity.ActivityFinished = new ActivityFinishedDelegate(FocusActivityFinished);

				// Move the current position indicator to the new focus slide's spot in the slide bar
				LayoutCurrentPosition();
			}
		}

		/// <summary>
		/// Hilite the specified slide - or unhilite all slides if hiliteIndex is -1.
		/// </summary>
		private void HiliteSlide(int newHiliteIndex) {
			PNode newSlide = null;
			if (newHiliteIndex >= 0) {
				newSlide = slides[newHiliteIndex];
			}
			if (newSlide != hiliteSlide) {
				// First unhilite previously hilited slide
				if ((hiliteSlide != null) && (hiliteSlide != focusSlide)) {
					// If this slide is currently animating, then kill that animation
					int index = (int)hiliteSlide.Tag;
					if (slideActivities[index] != null) {
						slideActivities[index].Terminate();
						slideActivities[index] = null;
					}
					(hiliteSlide as PMultiSizeImage).ShowThumb = true;
					(hiliteSlide as PMultiSizeImage).Hilite = false;
					PTransformActivity activity = hiliteSlide.AnimateToMatrix(GetUnfocusedMatrix(hiliteSlide, (int)hiliteSlide.Tag), SLIDEBAR_ANIMATION_TIME_MILLIS);
					// Put the slide in order when the animation finishes
					activity.ActivityFinished = new ActivityFinishedDelegate(HiliteActivityFinished);
					hiliteSlide = null;
				}
				// Then hilite new slide (as long is it isn't the currently focused slide)
				if (newSlide != focusSlide) {
					hiliteSlide = newSlide;
					if (hiliteSlide != null) {
						PMatrix matrix = GetUnfocusedMatrix(hiliteSlide, (int)hiliteSlide.Tag);
						matrix.ScaleBy(1.3f, (hiliteSlide.Bounds.Width / 2), hiliteSlide.Bounds.Height);
						(hiliteSlide as PMultiSizeImage).Hilite = true;
						hiliteSlide.MoveToFront();
						currentPosition.MoveToFront();
						slideActivities[newHiliteIndex] = hiliteSlide.AnimateToMatrix(matrix, SLIDEBAR_ANIMATION_TIME_MILLIS);
					}
				}
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Load the default slideshow.
		/// </summary>
		protected void SlideViewer_HandleCreated(object sender, EventArgs e) {
			LoadDefaultSlides();
		}

		/// <summary>
		/// When the focus animation finishes, show the hires version of the slide
		/// </summary>
		protected void FocusActivityFinished(PActivity activity) {
			PTransformActivity transformActivity = activity as PTransformActivity;
			PNode.PNodeTransformTarget nodeTarget = (PNode.PNodeTransformTarget)transformActivity.ActivityTarget;
			PNode slide = nodeTarget.Target;
			((PMultiSizeImage)slide).ShowThumb = false;
		}

		/// <summary>
		/// When the hilite animation finishes, put the slide back in its proper place
		/// </summary>
		protected void HiliteActivityFinished(PActivity activity) {
			PTransformActivity transformActivity = activity as PTransformActivity;
			PNode.PNodeTransformTarget nodeTarget = (PNode.PNodeTransformTarget)transformActivity.ActivityTarget;
			PNode slide = nodeTarget.Target;
			slideBar.AddChild((int)slide.Tag, slide);  // Reinsert slide in the right slot
		}

		/// <summary>
		/// On resize, we will relayout everything.
		/// </summary>
		protected override void OnResize(EventArgs e) {
			base.OnResize (e);
			if ((slides != null) && (slides.Count > 0)) {
				LayoutSlides();
			}
		}

		/// <summary>
		/// Go to the slide that was clicked.
		/// </summary>
		protected void Camera_MouseDown(object sender, PInputEventArgs e) {
			if ((slides == null) || (slides.Count == 0)) {
				return;
			}

			PNode picked = e.PickedNode;

			if (picked.Parent == slideBar) {
				if (picked == focusSlide) {
					int newIndex = (int)focusSlide.Tag + 1;
					if (newIndex < slides.Count) {
						GoToSlide((PMultiSizeImage)slides[newIndex]);
					}
				} else {
					GoToSlide((PMultiSizeImage)picked);
				}
			}
		}

		/// <summary>
		/// Let the user navigate between slides.
		/// Space, right arrow, PageDown is next slide.
		/// Left arrow, PageUp is previous slide.
		/// Home/End move to first/last slide, respectively
		/// </summary>
		protected void Camera_KeyDown(object sender, PInputEventArgs e) {
			int newIndex;

			if ((slides == null) || (slides.Count == 0)) {
				return;
			}

			switch (e.KeyCode) {
				case Keys.Space:
				case Keys.Right:
				case Keys.PageDown:
					newIndex = (int)focusSlide.Tag + 1;
					if (newIndex < slides.Count) {
						GoToSlide((PMultiSizeImage)slides[newIndex]);
					}
					break;
				case Keys.Left:
				case Keys.PageUp:
					newIndex = (int)focusSlide.Tag - 1;
					if (newIndex >= 0) {
						GoToSlide((PMultiSizeImage)slides[newIndex]);
					}
					break;
				case Keys.Home:
					GoToSlide((PMultiSizeImage)slides[0]);
					break;
				case Keys.End:
					GoToSlide((PMultiSizeImage)slides[slides.Count - 1]);
					break;
			}
		}

		/// <summary>
		/// When the user moves the mouse over the slide bar, the slide under the mouse gets raised and hilited.
		/// </summary>
		private void Camera_MouseMove(object sender, PInputEventArgs e) {
			if ((slides == null) || (slides.Count == 0)) {
				return;
			}

			// First make slide bar visible or not
			if ((e.Position.Y < (Height - SLIDE_BAR_HEIGHT)) && SlideBarVisible) {
				SlideBarVisible = false;
			} else if ((e.Position.Y > (Height - SLIDE_BAR_HEIGHT)) && !SlideBarVisible) {
				SlideBarVisible = true;
			}

			// Then, hilite slide mouse is over
			if (SlideBarVisible) {
				int hiliteIndex = Math.Min(slides.Count - 1, (int)(e.CanvasPosition.X / (e.Canvas.Width / slides.Count)));
				HiliteSlide(hiliteIndex);
			}
		}
		#endregion

		#region PMultiSizeImage
		/// <summary>
		/// An image node that supports both a thumbnail image and a high resolution image.
		/// </summary>
		public class PMultiSizeImage : PImage {
			static Font font = new Font("Arial", 30);
			Image thumb;
			Image hiRes;
			bool hilite = false;

			public PMultiSizeImage(Image thumb, Image hiRes, bool showThumb) :
				base(showThumb ? thumb : hiRes) {
				this.thumb = thumb;
				this.hiRes = hiRes;
			}

			public bool ShowThumb {
				get { return image == thumb; }
				set {
					image = value ? thumb : hiRes;
					InvalidatePaint();
				}
			}

			public bool Hilite {
				get { return hilite; }
				set {
					hilite = value;
					InvalidatePaint();
				}
			}

			public Image Thumb {
				get { return thumb; }
			}

			public Image HiRes {
				get { return hiRes; }
			}

			protected override void Paint(PPaintContext paintContext) {
				base.Paint (paintContext);

				Graphics g = paintContext.Graphics;
				if (hilite) {
					g.DrawRectangle(Pens.Red, X, Y, Width, Height);
					if (Tag != null) {
						g.DrawString(((int)Tag).ToString(), font, Brushes.Black, Bounds.Right-60, Bounds.Bottom-55);
					}
				}
			}
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region Control Border
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
			e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width-1, this.Height-1);
		}
		#endregion
	}
}