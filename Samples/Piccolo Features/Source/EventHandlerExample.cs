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
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Util;


namespace UMD.HCIL.PiccoloFeatures {
	public class EventHandlerExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public EventHandlerExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			// Create a new event handler that creates new rectangles on
			// mouse pressed, dragged, release.
			PBasicInputEventHandler rectEventHandler = new RectEventHandler(Canvas);
				
			// Remove the pan event handler that is installed by default so that it
			// does not conflict with our new rectangle creation event handler.
			Canvas.RemoveInputEventListener(Canvas.PanEventHandler);
		
			// Register our new event handler.
			Canvas.AddInputEventListener(rectEventHandler);
		}

		class RectEventHandler : PBasicInputEventHandler {

			PCanvas canvas;

			public RectEventHandler(PCanvas canvas) {
				this.canvas = canvas;
			}

			// The rectangle that is currently getting created.
			protected PPath rectangle;
			
			// The mouse press location for the current pressed, drag, release sequence.
			protected PointF pressPoint;
			
			// The current drag location.
			protected PointF dragPoint;

			public override void OnMouseDown(object sender, PInputEventArgs e) {
				base.OnMouseDown (sender, e);

				PLayer layer = canvas.Layer;

				// Initialize the locations.
				pressPoint = e.Position;
				dragPoint = pressPoint; 			
				
				// create a new rectangle and add it to the canvas layer so that
				// we can see it.
				rectangle = new PPath();
				rectangle.Pen = new Pen(Brushes.Black, (float)(1/ e.Camera.ViewScale));
				layer.AddChild(rectangle);
				
				// update the rectangle shape.
				UpdateRectangle();
			}

			public override void OnMouseDrag(object sender, PInputEventArgs e) {
				base.OnMouseDrag (sender, e);
				// update the drag point location.
				dragPoint = e.Position;
				
				// update the rectangle shape.
				UpdateRectangle();
			}

			public override void OnMouseUp(object sender, PInputEventArgs e) {
				base.OnMouseUp (sender, e);
				// update the rectangle shape.
				UpdateRectangle();
				rectangle = null;
			}

			public void UpdateRectangle() {
				// create a new bounds that contains both the press and current
				// drag point.
				RectangleF r = RectangleF.Empty;
				r = PUtil.AddPointToRect(r, pressPoint);
				r = PUtil.AddPointToRect(r, dragPoint);
				
				// Set the rectangles bounds.
				rectangle.PathReference.Reset();
				rectangle.AddRectangle(r.X, r.Y, r.Width, r.Height);
			}

			// Make the event handler only work with BUTTON1 events, so that it does
			// not conflict with the zoom event handler that is installed by default.
			public override bool DoesAcceptEvent(PInputEventArgs e) {
				return (base.DoesAcceptEvent(e) && e.IsMouseEvent && e.Button == MouseButtons.Left);
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
			// EventHandlerExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "EventHandlerExample";
			this.Text = "EventHandlerExample";

		}
		#endregion
	}
}

