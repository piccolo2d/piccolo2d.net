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
using UMD.HCIL.PiccoloX.Handles;
using UMD.HCIL.PiccoloX.Util;

namespace UMD.HCIL.PiccoloFeatures {
	public class PathExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public PathExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			PPath n1 = PPath.CreateRectangle(0, 0, 100, 80);
			PPath n2 = PPath.CreateEllipse(100, 100, 200, 34);
			PPath n3 = new PPath();
			n3.AddLine(0, 0, 20, 40);
			n3.AddLine(20, 40, 10, 200);
			n3.AddLine(10, 200, 155.444f, 33.232f);
			n3.CloseFigure();
			n3.Brush = Brushes.Yellow;
		
			n1.Pen = new Pen(Brushes.Red, 5);
			n2.Pen = new Pen(Brushes.Black, 0); //Fixed width stroke
			n3.Pen = new Pen(Brushes.Black, 0); //Fixed width stroke
			
			Canvas.Layer.AddChild(n1);
			Canvas.Layer.AddChild(n2);		
			Canvas.Layer.AddChild(n3);	
		
			// create a set of bounds handles for reshaping n3, and make them
			// sticky relative to the getCanvas().getCamera().
			PStickyHandleManager sm = new PStickyHandleManager(Canvas.Camera, n3);
		
			Canvas.RemoveInputEventListener(Canvas.PanEventHandler);
			Canvas.AddInputEventListener(new PDragEventHandler());	
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
			// PathExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "PathExample";
			this.Text = "PathExample";

		}
		#endregion
	}
}