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
using System.Windows.Forms;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.PiccoloFeatures {
	/// <summary>
	/// This example shows how to create a vertical and a horizontal bar which can
	/// move with your graph and always stays on view.
	/// </summary>
	/// <remarks>
	/// Java code submitted by Tao and ported by Aaron Clamage
	/// </remarks>
	public class ChartLabelExample : UMD.HCIL.PiccoloX.PForm {
		const int NODE_HEIGHT = 15;
		const int NODE_WIDTH = 30;

		private System.ComponentModel.IContainer components = null;

		public ChartLabelExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			//create bar layers
			PLayer rowBarLayer = new PLayer();
			PLayer colBarLayer = new PLayer();

			//create bar nodes
			for (int i = 0; i < 10; i++) {
				//create row bar with node row1, row2,...row10
				PText p = new PText("Row " + i);
				p.X = 0;
				p.Y = NODE_HEIGHT * i + NODE_HEIGHT;
				p.Brush = Brushes.White;
				colBarLayer.AddChild(p);

				//create col bar with node col1, col2,...col10
				p = new PText("Col " + i);
				p.X = NODE_WIDTH * i + NODE_WIDTH;
				p.Y = 0;
				p.Brush = Brushes.White;
				rowBarLayer.AddChild(p);
			}

			//add bar layers to camera
			Canvas.Camera.AddChild(rowBarLayer);
			Canvas.Camera.AddChild(colBarLayer);

			//create matrix nodes
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					PPath path = PPath.CreateRectangle(NODE_WIDTH * j + NODE_WIDTH,
						NODE_HEIGHT * i + NODE_HEIGHT, NODE_WIDTH - 1,
						NODE_HEIGHT - 1);
					Canvas.Layer.AddChild(path);
				}
			}

			//catch drag event and move bars corresponding
			Canvas.AddInputEventListener(new BarDragEventHandler(Canvas, rowBarLayer, colBarLayer));
		}

		private class BarDragEventHandler : PDragSequenceEventHandler {
			PointF oldP, newP;
			PCanvas target;
			PLayer rowBarLayer, colBarLayer;

			public BarDragEventHandler(PCanvas target, PLayer rowBarLayer, PLayer colBarLayer) {
				this.target = target;
				this.rowBarLayer = rowBarLayer;
				this.colBarLayer = colBarLayer;
			}

			public override void OnMouseDown(object sender, PInputEventArgs e) {
				oldP = PUtil.CenterOfRectangle(target.Camera.ViewBounds);
			}

			public override void OnMouseUp(object sender, PInputEventArgs e) {
				newP = PUtil.CenterOfRectangle(target.Camera.ViewBounds);
				colBarLayer.TranslateBy(0, (oldP.Y - newP.Y)
					/ target.Layer.Scale);
				rowBarLayer.TranslateBy((oldP.X - newP.X)
					/ target.Layer.Scale, 0);
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
			// ChartLabelExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "ChartLabelExample";
			this.Text = "ChartLabelExample";

		}
		#endregion
	}
}