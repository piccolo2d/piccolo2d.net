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

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using Direct3D = Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.PiccoloDirect3D.Util;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.PiccoloDirect3D.Nodes {
	/// <summary>
	/// <b>P3Text</b> is a multi-line Direct3D text node.  The text will wrap based on the width
	/// of the node's bounds.
	/// </summary>
	public class P3Text : P3Node {
		#region Fields
		private static int DEFAULT_FONT_SIZE = 12;
		private static float DISPLAY_FONT_SIZE = DEFAULT_FONT_SIZE;

		/// <summary>
		/// The default font to use when rendering this P3Text node.
		/// </summary>
		public static Font DEFAULT_FONT = new Font("Arial", DEFAULT_FONT_SIZE);

		// The padding values relative to the default font size.
		private static int TOP_PADDING = 1;
		private static int BOTTOM_PADDING = 3;
		private static int LEFT_PADDING = 2;
		private static int RIGHT_PADDING = 3;

		private String text;
		private Brush textBrush;
		private Font font;
		private StringFormat stringFormat = new StringFormat();
		private bool constrainHeightToTextHeight = true;
		private bool constrainWidthToTextWidth = true;
		private Direct3D.Font D3Dfont;
		private Direct3D.Font measureFont;
		private Sprite textSprite;

		private float displayFontSize = DISPLAY_FONT_SIZE;
		private int currLeftPadding = LEFT_PADDING;
		private int currRightPadding = RIGHT_PADDING;
		private int currTopPadding = TOP_PADDING;
		private int currBottomPadding = BOTTOM_PADDING;
		private bool fontChanged = false;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3Text with an empty string.
		/// </summary>
		public P3Text() {
			textBrush = Brushes.Black;
		}

		/// <summary>
		/// Constructs a new P3Text with the given string.
		/// </summary>
		/// <param name="aText">The desired text string for this P3Text.</param>
		public P3Text(String aText) : this() {
			Text = aText;
		}
		#endregion

		#region Basic
		//****************************************************************
		// Basic - Methods for manipulating the underlying text.
		//****************************************************************

		/// <summary>
		/// Gets or sets a value indicating whether this node changes its width to fit
		/// the width of its text.
		/// </summary>
		/// <value>
		/// True if this node changes its width to fit its text width; otherwise, false.
		/// </value>
		public virtual bool ConstrainWidthToTextWidth {
			get { return constrainWidthToTextWidth; }
			set {
				constrainWidthToTextWidth = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this node changes its height to fit
		/// the height of its text.
		/// </summary>
		/// <value>
		/// True if this node changes its height to fit its text height; otherwise, false.
		/// </value>
		public virtual bool ConstrainHeightToTextHeight {
			get { return constrainHeightToTextHeight; }
			set {
				constrainHeightToTextHeight = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets the text for this node.
		/// </summary>
		/// <value>This node's text.</value>
		/// <remarks>
		/// The text will be broken up into multiple lines based on the size of the text
		/// and the bounds width of this node.
		/// </remarks>
		public virtual String Text {
			get { return text; }
			set {
				text = value;

				InvalidatePaint();
				InvalidateVertices();  // Need to preload the new characters.
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets a value specifiying the alignment to use when rendering this
		/// node's text.
		/// </summary>
		/// <value>The alignment to use when rendering this node's text.</value>
		public virtual StringAlignment TextAlignment {
			get { return stringFormat.Alignment; }
			set {
				stringFormat.Alignment = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets the brush to use when rendering this node's text.
		/// </summary>
		/// <value>The brush to use when rendering this node's text.</value>
		public virtual Brush TextBrush {
			get { return textBrush; }
			set {
				textBrush = value;
				InvalidatePaint();
				RecomputeBounds();  // Recalculate since MeasureString takes a color.
			}
		}

		/// <summary>
		/// Gets or sets the font to use when rendering this node's text.
		/// </summary>
		/// <value>The font to use when rendering this node's text.</value>
		/// <remarks>
		/// <para>
		/// This font will not necessarily determine the actual render size of the
		/// text.  The actual render size can be set through the
		/// <see cref="DisplayFontSize">DisplayFontSize</see> property.
		/// </para>
		/// <para>
		/// Set the size of the font larger than the DisplayFontSize to keep the text
		/// smooth at larger scale factors.
		/// </para>
		/// </remarks>
		public virtual Font Font {
			get {
				if (font == null) {
					Font = DEFAULT_FONT;
				}
				return font;
			}
			set {
				font = value;
				measureFont = new Direct3D.Font(P3Util.NullDevice, Font);
				fontChanged = true;

				InvalidatePaint();
				InvalidateVertices();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// The size that the text is actually rendered at.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use this property to keep the text smooth at larger scale factors.
		/// </para>
		/// <para>
		/// The text will be scaled to the DisplayFontSize at render time.  Setting the
		/// size of the <see cref="Font">Font</see> larger than the display font size
		/// will keep the text smooth as the user zooms in.
		/// </para>
		/// </remarks>
		public virtual float DisplayFontSize {
			get { return displayFontSize; }
			set {
				displayFontSize = value;
				InvalidatePaint();
			}
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Overridden.  Disposes the Direct3D font and sprite.
		/// </summary>
		public override void Dispose() {
			base.Dispose();
			DisposeFont();
			DisposeSprite();
		}

		/// <summary>
		/// Dispose the Direct3D font.
		/// </summary>
		protected virtual void DisposeFont() {
			if (D3Dfont != null) {
				D3Dfont.Dispose();
				D3Dfont = null;
			}
		}

		/// <summary>
		/// Dispose the Direct3D sprite.
		/// </summary>
		protected virtual void DisposeSprite() {
			if (textSprite != null) {
				textSprite.Dispose();
				textSprite = null;
			}
		}
		#endregion

		#region Vertex Buffer
		/// <summary>
		/// Overridden.  See <see cref="P3Node.ValidateVertices">P3Node.ValidateVertices</see>.
		/// </summary>
		public override void ValidateVertices(Device device) {
			if (fontChanged) {
				D3Dfont = new Direct3D.Font(device, Font);
			}

			if (textSprite == null) {
				textSprite = new Sprite(device);
			}

			D3Dfont.PreloadText(text);

			base.ValidateVertices (device);
		}
		#endregion

		#region Painting
		//****************************************************************
		// Painting - Methods for painting a PText.
		//****************************************************************

		/// <summary>
		/// Overridden.  See <see cref="PNode.Paint">PNode.Paint</see>.
		/// </summary>
		protected override void Paint(UMD.HCIL.Piccolo.Util.PPaintContext paintContext) {
			base.Paint (paintContext);
			Device device = (paintContext as P3PaintContext).Device;

			PMatrix currMatrix = (paintContext as P3PaintContext).Transform;

			// Scale the matrix down to display font units
			float scale = displayFontSize / font.Size;
			currMatrix.ScaleBy(scale, X, Y);

			float[] piccoloMatrixElements = currMatrix.Elements;
			if (!currMatrix.IsIdentity) {
				Matrix m = new Matrix();
				m.M11 = piccoloMatrixElements[0];
				m.M12 = piccoloMatrixElements[1];
				m.M21 = piccoloMatrixElements[2];
				m.M22 = piccoloMatrixElements[3];
				m.M41 = piccoloMatrixElements[4];
				m.M42 = piccoloMatrixElements[5];
				m.M33 = 1;
				m.M44 = 1;
				textSprite.Transform = m;
			}

			textSprite.Begin(SpriteFlags.None);
			DrawTextFormat D3DAlignment = P3Util.GetD3DAlignment(stringFormat.Alignment);
			
			// Calculate the rectangle with no padding, in actual font units
			scale = 1 / scale;
			int totHzPadding = currLeftPadding+currRightPadding;
			int totVtPadding = currTopPadding+currBottomPadding;

			Rectangle dstRect = new Rectangle((int)(Bounds.X+currLeftPadding*scale), (int)(Bounds.Y+currTopPadding*scale),
				(int)((Bounds.Width-totHzPadding)*scale), (int)((Bounds.Height-totVtPadding)*scale));

			// Wrap the string ourselves, instead of letting the draw method do it, since we want to make
			// sure it's consistent with our own MeasureString method.
			String str = P3Util.WrapString(textSprite, D3Dfont, Text, dstRect.Width, (TextBrush as SolidBrush).Color);
			D3Dfont.DrawText(textSprite, str, dstRect, D3DAlignment, (TextBrush as SolidBrush).Color);
			textSprite.End();
		}
		#endregion

		#region Bounds
		/// <summary>
		/// Overridden.  See <see cref="PNode.InternalUpdateBounds">PNode.InternalUpdateBounds</see>.
		/// </summary>
		protected override void InternalUpdateBounds(float x, float y, float width, float height) {
			RecomputeBounds();
		}

		/// <summary>
		/// Override this method to change the way bounds are computed. For example
		/// this is where you can control how lines are wrapped.
		/// </summary>
		public virtual void RecomputeBounds() {
			if (text != null && (ConstrainWidthToTextWidth || ConstrainHeightToTextHeight)) {
				currLeftPadding = currRightPadding = currTopPadding = currBottomPadding = 0;

				// Calculate the padding values in display units
				float scaleFactor = displayFontSize / DEFAULT_FONT_SIZE;
				currLeftPadding = (int)(scaleFactor * LEFT_PADDING);
				currRightPadding = (int)(scaleFactor * RIGHT_PADDING);
				currTopPadding = (int)(scaleFactor * TOP_PADDING);
				currBottomPadding = (int)(scaleFactor * BOTTOM_PADDING);

				// Set the scale factor relative to the current font size
				scaleFactor = displayFontSize / Font.Size;

				float textWidth;
				float textHeight;
				if (ConstrainWidthToTextWidth) {
					Rectangle rect = measureFont.MeasureString(null, Text, DrawTextFormat.None, (textBrush as SolidBrush).Color);
					textWidth = rect.Width * scaleFactor + currLeftPadding + currRightPadding;
					textHeight = rect.Height * scaleFactor + currTopPadding + currBottomPadding;
				}
				else {
					textWidth = Width;
					int scaledWidth = (int)((textWidth-(currLeftPadding+currRightPadding)) * (1/scaleFactor));
					float scaledHeight = P3Util.MeasureString(measureFont, null, Text, scaledWidth, (textBrush as SolidBrush).Color).Height;
					textHeight = scaledHeight * scaleFactor + currTopPadding + currBottomPadding;;
				}

				float newWidth = Width;
				float newHeight = Height;
				if (ConstrainWidthToTextWidth) newWidth = textWidth;
				if (ConstrainHeightToTextHeight) newHeight = textHeight;

				base.SetBounds(X, Y, newWidth, newHeight);
			}
		}
		#endregion
	}
}
