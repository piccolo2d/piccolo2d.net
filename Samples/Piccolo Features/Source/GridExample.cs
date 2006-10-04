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
using System.Windows.Forms;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.PiccoloFeatures {
	public class GridExample : UMD.HCIL.PiccoloX.PForm {
		static protected GraphicsPath gridLine = new GraphicsPath();
		static protected Pen gridPen = new Pen(Brushes.Black, 1);
		static protected float gridSpacing = 20;

		protected PLayer gridLayer = new GridLayer();

		private System.ComponentModel.IContainer components = null;

		public GridExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			PRoot root = Canvas.Root;
			PCamera camera = Canvas.Camera;
			//PLayer gridLayer = new GridLayer();

			// replace standard layer with grid layer.
			root.RemoveChild(camera.GetLayer(0));
			camera.RemoveLayer(0);
			root.AddChild(gridLayer);
			camera.AddLayer(gridLayer);

			// add constraints so that grid layers bounds always match cameras view bounds. This makes 
			// it look like an infinite grid.
			camera.BoundsChanged += new PPropertyEventHandler(camera_BoundsChanged);
			camera.ViewTransformChanged += new PPropertyEventHandler(camera_ViewTransformChanged);

			gridLayer.Bounds = camera.ViewBounds;

			PNode n = new PNode();
			n.Brush = Brushes.Blue;
			n.SetBounds(0, 0, 100, 80);
		
			Canvas.Layer.AddChild(n);
			Canvas.RemoveInputEventListener(Canvas.PanEventHandler);

			Canvas.AddInputEventListener(new GridDragHandler(Canvas));
		}

		protected void camera_BoundsChanged(object sender, PPropertyEventArgs e) {
			gridLayer.Bounds = Canvas.Camera.ViewBounds;
		}

		protected void camera_ViewTransformChanged(object sender, PPropertyEventArgs e) {
			gridLayer.Bounds = Canvas.Camera.ViewBounds;
		}

		class GridLayer : PLayer {
			public GridLayer() {
			}
			protected override void Paint(PPaintContext paintContext) {
				// make sure grid gets drawn on snap to grid boundaries. And 
				// expand a little to make sure that entire view is filled.
				float bx = (X - (X % gridSpacing)) - gridSpacing;
				float by = (Y - (Y % gridSpacing)) - gridSpacing;
				float rightBorder = X + Width + gridSpacing;
				float bottomBorder = Y + Height + gridSpacing;

				Graphics g = paintContext.Graphics;
				RectangleF clip = paintContext.LocalClip;

				for (float x = bx; x < rightBorder; x += gridSpacing) {
					gridLine.Reset();
					gridLine.AddLine(x, by, x, bottomBorder);
					if (PUtil.RectIntersectsPerpLine(clip, x, by, x, bottomBorder)) {
						g.DrawPath(gridPen, gridLine);
					}
				}

				for (float y = by; y < bottomBorder; y += gridSpacing) {
					gridLine.Reset();
					gridLine.AddLine(bx, y, rightBorder, y);
					if (PUtil.RectIntersectsPerpLine(clip, bx, y, rightBorder, y)) {
						g.DrawPath(gridPen, gridLine);
					}
				}
			}
		}

		class GridDragHandler : PDragSequenceEventHandler {
			protected PCanvas canvas;
			protected PNode draggedNode;
			protected PointF nodeStartPosition;

			public GridDragHandler(PCanvas canvas) {
				this.canvas = canvas;
			}

			protected override bool ShouldStartDragInteraction(PInputEventArgs e) {
				if (base.ShouldStartDragInteraction (e)) {
					return e.PickedNode != e.TopCamera && !(e.PickedNode is PLayer);
				}
				return false;
			}

			protected override void OnStartDrag(object sender, PInputEventArgs e) {
				base.OnStartDrag (sender, e);
				draggedNode = e.PickedNode;
				draggedNode.MoveToFront();
				nodeStartPosition = draggedNode.Offset;
			}

			protected override void OnDrag(object sender, PInputEventArgs e) {
				base.OnDrag (sender, e);

				PointF start = canvas.Camera.LocalToView((PointF)MousePressedCanvasPoint);
				PointF current = e.GetPositionRelativeTo(canvas.Layer);
				PointF dest = PointF.Empty;

				dest.X = nodeStartPosition.X + (current.X - start.X);
				dest.Y = nodeStartPosition.Y + (current.Y - start.Y);

				dest.X = dest.X - (dest.X % gridSpacing);
				dest.Y = dest.Y - (dest.Y % gridSpacing);

				draggedNode.SetOffset(dest.X, dest.Y);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			// 
			// GridExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "GridExample";
			this.Text = "GridExample";

		}
		#endregion
	}
}