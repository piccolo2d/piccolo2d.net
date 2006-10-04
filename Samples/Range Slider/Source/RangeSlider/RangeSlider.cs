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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Event;

namespace UMD.HCIL.RangeSlider {
	public class RangeSlider : PCanvas {
		#region Internal Fields
		internal static int   HEIGHT = 20;
		internal static int   GRAB_WIDTH = 15;
		internal static Brush TROUGH_BRUSH = new SolidBrush(Color.FromArgb(160, 158, 150));
		internal static Brush THUMB_BRUSH = new SolidBrush(Color.FromArgb(239, 235, 222));

		protected float minimum, maximum;
		protected float low, high;
		internal RangeSliderNode rangeSliderNode;
		#endregion

		//////////////////////////////////
		// PUBLIC API
		//////////////////////////////////

		#region Public Fields
		public delegate void RangeEventHandler(object sender, RangeEventArgs e);
		public event RangeEventHandler RangeChanged;
		#endregion

		#region Initialization
		public RangeSlider() : this(0, 100, 0, 100) {
		}

		public RangeSlider(float minimum, float maximum, float low, float high) {
			this.minimum = minimum;
			this.maximum = maximum;
			this.low = low;
			this.high = high;

			rangeSliderNode = new RangeSliderNode(this);
			Layer.AddChild(rangeSliderNode);

			// Force anti-aliasing all the time
			DefaultRenderQuality = RenderQuality.HighQuality;
			InteractingRenderQuality = RenderQuality.HighQuality;

			PanEventHandler = null;
			ZoomEventHandler = null;
			rangeSliderNode.AddInputEventListener(new RangeSliderEventHandler(this));
			Resize += new EventHandler(RangeSlider_Resize);
		}
		#endregion

		#region Properties
		/** 
		 * Gets/sets the current "low" value shown by the range slider's bar. The low
		 * value meets the constraint minimum <= low <= high <= maximum. 
		 */
		public float Low {
			get { return low; }
			set {
				if (value > high) {
					low = high;
				} else if (value < minimum) {
					low = minimum;
				} else {
					low = value;
				}

				rangeSliderNode.InvalidateLayout();
				OnRangeChanged(new RangeEventArgs(low, high));
			}
		}

		/** 
		 * Gets/sets the current "high" value shown by the range slider's bar. The high
		 * value meets the constraint minimum <= low <= high <= maximum. 
		 */
		public float High {
			get { return high; }
			set {
				if (value < low) {
					high = low;
				} else if (value > maximum) {
					high = maximum;
				} else {
					high = value;
				}

				rangeSliderNode.InvalidateLayout();
				OnRangeChanged(new RangeEventArgs(low, high));
			}
		}

		/**
		 * Gets/sets the minimum possible value for either the low value or the high value.
		 */
		public float Minimum {
			get { return minimum; }
			set {
				minimum = value;
				rangeSliderNode.InvalidateLayout();
			}
		}

		/**
		 * Gets/sets the maximum possible value for either the low value or the high value.
		 */
		public float Maximum {
			get { return maximum; }
			set {
				maximum = value;
				rangeSliderNode.InvalidateLayout();
			}
		}
		#endregion

		//////////////////////////////////
		// INTERNAL IMPLEMENTATION
		//////////////////////////////////
		
		internal RangeSliderNode RangeSliderNode {
			get { return rangeSliderNode; }
		}

		#region PCanvas Event Code
		/**
		 * Fires a RangeEvent. This is called whenever either the low value or the
		 * high value on the range slider changes.
		 */
		protected void OnRangeChanged(RangeEventArgs e) {
			if (RangeChanged != null) {
				RangeChanged(this, e);
			}
		}

		protected void RangeSlider_Resize(object sender, EventArgs e) {
			rangeSliderNode.Bounds = new RectangleF(0, 0, Width, Height);
			rangeSliderNode.InvalidateLayout();
		}
		#endregion
	}

	//////////////////////////////////
	// COMPONENTS
	//////////////////////////////////
	
	#region RangeSliderNode Class
	internal class RangeSliderNode : PNode {
		internal RangeSlider rangeSlider;
		internal RangeSliderArrow leftArrow;
		internal RangeSliderThumb thumb;
		internal RangeSliderArrow rightArrow;

		internal RangeSliderNode(RangeSlider rangeSlider) {
			this.rangeSlider = rangeSlider;

			thumb = new RangeSliderThumb();
			leftArrow = new RangeSliderArrow(rangeSlider, RangeSliderArrow.Directions.Left);
				rightArrow = new RangeSliderArrow(rangeSlider, RangeSliderArrow.Directions.Right);
			AddChild(thumb);
			AddChild(leftArrow);
			AddChild(rightArrow);

			Brush = RangeSlider.TROUGH_BRUSH;
		}

		internal float ModelToCanvas(float mx) {
			return RangeSlider.GRAB_WIDTH + (rangeSlider.Width - 2*RangeSlider.GRAB_WIDTH) * (mx / (rangeSlider.Maximum - rangeSlider.Minimum));
		}

		internal float CanvasToModel(float cx) {
			return (cx - RangeSlider.GRAB_WIDTH) * (rangeSlider.Maximum - rangeSlider.Minimum) / (rangeSlider.Width - 2*RangeSlider.GRAB_WIDTH);
		}
		
		public override void LayoutChildren() {
			float grabWidth = RangeSlider.GRAB_WIDTH;

			leftArrow.Bounds = new RectangleF(ModelToCanvas(rangeSlider.Low) - grabWidth, 0, grabWidth, rangeSlider.Height);
			rightArrow.Bounds = new RectangleF(ModelToCanvas(rangeSlider.High), 0, grabWidth, rangeSlider.Height);
			thumb.Bounds = new RectangleF(leftArrow.Bounds.Left, 0, rightArrow.Bounds.Right - leftArrow.Bounds.Left, rangeSlider.Height);
		}
	}
	#endregion

	#region RangeSliderThumb Class
	////////// Thumb class ////////////
	internal class RangeSliderThumb : PNode {
		internal RangeSliderThumb() {
			Brush = null;
		}

		protected override void Paint(PPaintContext paintContext) {
			base.Paint (paintContext);

			if ((int)Width > 0) {
				ControlPaint.DrawButton(paintContext.Graphics, (int)X, (int)Y, (int)Width, (int)Height, ButtonState.Flat);
			}
		}
	}
	#endregion

	#region RangeSliderArrow Class
	////////// Arrow class ////////////
	internal class RangeSliderArrow : PNode {
		internal enum Directions {Left, Right};

		protected Directions direction;
		protected RangeSlider rangeSlider;

		internal RangeSliderArrow(RangeSlider rangeSlider, Directions direction) {
			this.rangeSlider = rangeSlider;
			this.direction = direction;
			Brush = null;
		}

		internal Directions Direction {
			get { return direction; }
		}

		protected override void Paint(PPaintContext paintContext) {
			base.Paint (paintContext);
			ControlPaint.DrawButton(paintContext.Graphics, (int)X, (int)Y, (int)Width, (int)Height, ButtonState.Normal);
		}
	}
	#endregion

	//////////////////////////////////
	// EVENT HANDLING
	//////////////////////////////////
	
	#region RangeSliderEventHandler Class
	internal class RangeSliderEventHandler : PDragSequenceEventHandler {
		protected RangeSlider rangeSlider;
		protected PNode pickNode;
		protected float pickLow;
		protected float pickHigh;

		public RangeSliderEventHandler(RangeSlider rangeSlider) {
			this.rangeSlider = rangeSlider;
		}

		public override void OnMouseMove(object sender, PInputEventArgs e) {
			base.OnMouseMove(sender, e);
			PNode node = e.PickedNode;

			if (node is RangeSliderThumb) {
				rangeSlider.Cursor = Cursors.Hand;
			} else if (node is RangeSliderArrow) {
				rangeSlider.Cursor = Cursors.SizeWE;
			} else {
				rangeSlider.Cursor = Cursors.Default;
			}
		}

		public override void OnMouseDown(object sender, PInputEventArgs e) {
			base.OnMouseDown(sender, e);

			// Slightly tricky - we want to make sure that the thing being dragged stays directly under the cursor,
			// even if the cursor goes too far one way or the other - when it comes back, we want it to remember
			// the position.
			pickNode = e.PickedNode;
			pickLow = rangeSlider.Low;
			pickHigh = rangeSlider.High;
		}

		protected override void OnDrag(object sender, PInputEventArgs e) {
			base.OnDrag(sender, e);

			if (pickNode != null) {
				float dmx = rangeSlider.RangeSliderNode.CanvasToModel(rangeSlider.RangeSliderNode.ModelToCanvas(0) + e.Delta.Width);
				pickLow += dmx;
				pickHigh += dmx;

				if (pickNode is RangeSliderThumb) {
					if (pickLow < rangeSlider.Minimum) {
						rangeSlider.Low = rangeSlider.Minimum;
						rangeSlider.High = rangeSlider.Minimum + (pickHigh - pickLow);
					} else if (pickHigh > rangeSlider.Maximum) {
						rangeSlider.High = rangeSlider.Maximum;
						rangeSlider.Low = rangeSlider.Maximum - (pickHigh - pickLow);
					} else {
						rangeSlider.Low = pickLow;
						rangeSlider.High = pickHigh;
					}
				} else if (pickNode is RangeSliderArrow) {
					switch (((RangeSliderArrow)pickNode).Direction) {
						case RangeSliderArrow.Directions.Left:
							rangeSlider.Low = pickLow;
							break;
						case RangeSliderArrow.Directions.Right:
							rangeSlider.High = pickHigh;
							break;
					}
				}
			}
		}
	}
	#endregion

	#region RangeEventArgs Class
	public class RangeEventArgs : EventArgs {
		protected float low;
		protected float high;

		public RangeEventArgs(float low, float high) {
			this.low = low;
			this.high = high;
		}

		public float Low {
			get { return low; }
		}

		public float High {
			get { return high; }
		}
	}
	#endregion
}