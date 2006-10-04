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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.PiccoloDirect3D;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.Piccolo.Activities;
using UMD.HCIL.PiccoloDirect3D.Nodes;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Direct3DTester {
	public class Direct3DTester : Form {
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.Container components = null;
		private P3Canvas canvas;

		public Direct3DTester() {
			InitializeComponent();

			InitSceneGraph();
		}

		public void InitSceneGraph() {
			Random rnd = new Random();

			CreateLines();
			CreateRects(rnd);
			CreateImages(rnd);
			CreatePath(rnd);
			CreateText(rnd);
		}

		protected void CreateLines() {
			// Create a bunch of lines
			for (int x=0; x<1000; x+=10) {
				P3Path line = P3Path.CreateLine(10, 60, 10+x, 200);
				line.Pen = new Pen(Color.Orange, 0);
				canvas.Layer.AddChild(line);

				line = P3Path.CreateLine(500, 60, 1000-x, 200);
				line.Pen = new Pen(Color.Purple, 0);
				canvas.Layer.AddChild(line);
			}
		}

		protected void CreateRects(Random rnd) {
			PTransformActivity rotActivity;
			P3Path rect;

			// Create a bunch of animated rectangles
			for (int x=0; x<2000; x += 100) {
				for (int y=200; y<1500; y += 100) {
					int w = 200;
					int h = 200;
					rect = P3Path.CreateRectangle(x, y, w, h);
					rect.Brush =  new SolidBrush(Color.FromArgb(50, Color.Purple.R, Color.Purple.G, Color.Purple.B));
					rect.Pen = new Pen(Color.Red, 0);
					canvas.Layer.AddChild(rect);

					PMatrix matrix = new PMatrix();
					matrix.RotateBy(90, x+w/2, y+h/2);
					rotActivity = rect.AnimateToMatrix(matrix, 5000 + (long)(2000 * rnd.NextDouble()));
					rotActivity.LoopCount = 1000;
					rotActivity.Mode = ActivityMode.SourceToDestinationToSource;
				}
			}
		}

		protected void CreateImages(Random rnd) {
			PTransformActivity rotActivity;
			P3Image image;

			// Create a bunch of animated images
			for (int x=0; x<2000; x += 500) {
				for (int y=1500; y<3000; y += 500) {
					int w = 200;
					int h = 200;
					image = new P3Image("hcil-logo.png");
					canvas.Layer.AddChild(image);

					PMatrix matrix = new PMatrix();
					matrix.RotateBy(90, x+w/2, y+h/2);
					rotActivity = image.AnimateToMatrix(matrix, 5000 + (long)(2000 * rnd.NextDouble()));
					rotActivity.LoopCount = 1000;
					rotActivity.Mode = ActivityMode.SourceToDestinationToSource;
				}
			}
		}

		protected void CreatePath(Random rnd) {
			PTransformActivity rotActivity;

			// Create a path
			P3Path path = P3Path.CreateEllipse(0, 0, 100, 100);
			path.Brush = Brushes.Red;
			path.AddLine(0, 0, 20, 20);
			path.AddLine(20, 20, 34,67);
			path.AddArc(0, 30, 30, 30, 30, 30);
			path.Tolerance = .002f;
			canvas.Layer.AddChild(path);

			PMatrix rMatrix = new PMatrix();
			PointF center = PUtil.CenterOfRectangle(path.Bounds);
			rMatrix.RotateBy(90, center.X, center.Y);
			rotActivity = path.AnimateToMatrix(rMatrix, 2000 + (long)(2000 * rnd.NextDouble()));
			rotActivity.LoopCount = 1000;
			rotActivity.Mode = ActivityMode.SourceToDestinationToSource;
		}

		protected void CreateText(Random rnd) {
			// Create some text
			P3Text text = new P3Text("Direct3D Piccolo Renderer");
			text.Brush = Brushes.Blue;
			text.TextBrush = Brushes.Yellow;
			text.Font = new System.Drawing.Font("Arial", 120);
			canvas.Layer.AddChild(text);

			PMatrix tMatrix = text.Matrix;
			tMatrix.TranslateBy(canvas.Width-text.Width, 0);
			PTransformActivity translateActivity = text.AnimateToMatrix(tMatrix, 10000 + (long)(2000 * rnd.NextDouble()));
			translateActivity.LoopCount = 1000;
			translateActivity.Mode = ActivityMode.SourceToDestinationToSource;
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.canvas = new UMD.HCIL.PiccoloDirect3D.P3Canvas();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(248, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(200, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "This shows Piccolo.NET using the Direct3D graphics renderer";
			// 
			// canvas
			// 
			this.canvas.AllowDrop = true;
			this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.canvas.BackColor = System.Drawing.Color.White;
			this.canvas.Location = new System.Drawing.Point(8, 56);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(680, 464);
			this.canvas.TabIndex = 1;
			this.canvas.Text = "8";
			// 
			// Direct3DTester
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 526);
			this.Controls.Add(this.canvas);
			this.Controls.Add(this.label1);
			this.Name = "Direct3DTester";
			this.Text = "Piccolo Direct3D Tester";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() {
			using (Direct3DTester tester = new Direct3DTester()) {
				Application.Run(tester);
			}
		}
	}
}