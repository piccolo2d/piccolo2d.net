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
	/// Simple example showing one way to create a link between two nodes.
	/// </summary>
	public class NodeLinkExample : UMD.HCIL.PiccoloX.PForm {
		PNode node1;
		PNode node2;
		PPath link;

		private System.ComponentModel.IContainer components = null;

		public NodeLinkExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			Canvas.PanEventHandler = null;
			Canvas.AddInputEventListener(new PDragEventHandler());
		
			node1 = PPath.CreateEllipse(0, 0, 100, 100);
			node2 = PPath.CreateEllipse(0, 0, 100, 100);
			link = PPath.CreateLine(50, 50, 50, 50);
			link.Pickable = false;
			Canvas.Layer.AddChild(node1);
			Canvas.Layer.AddChild(node2);
			Canvas.Layer.AddChild(link);
		
			node2.TranslateBy(200, 200);

			node1.FullBoundsChanged += new PPropertyEventHandler(node1_FullBoundsChanged);
			node2.FullBoundsChanged += new PPropertyEventHandler(node2_FullBoundsChanged);
		}

		protected void node1_FullBoundsChanged(object sender, PPropertyEventArgs e) {
			UpdateLink();
		}

		protected void node2_FullBoundsChanged(object sender, PPropertyEventArgs e) {
			UpdateLink();
		}

		public void UpdateLink() {
			PointF p1 = PUtil.CenterOfRectangle(node1.FullBounds);
			PointF p2 = PUtil.CenterOfRectangle(node2.FullBounds);
			link.Reset();
			link.AddLine(p1.X, p1.Y, p2.X, p2.Y);
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
			// NodeLinkExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "NodeLinkExample";
			this.Text = "NodeLinkExample";

		}
		#endregion
	}
}