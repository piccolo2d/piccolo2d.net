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
using UMD.HCIL.PiccoloX;
using UMD.HCIL.PiccoloX.Handles;
using UMD.HCIL.PiccoloX.Util;

namespace UMD.HCIL.PiccoloFeatures {
	public class HandleExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public HandleExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			PPath n = PPath.CreateRectangle(0, 0, 100, 80);
		
			// add another node the the root as a reference point so that we can 
			// tell that our node is getting dragged, as opposed the the canvas 
			// view being panned.
			Canvas.Layer.AddChild(PPath.CreateRectangle(0, 0, 100, 80));
		
			Canvas.Layer.AddChild(n);
		
			// tell the node to show its default handles.
			PBoundsHandle.AddBoundsHandlesTo(n);
		
			// The default PBoundsHandle implementation doesn't work well with PPaths that have strokes. The reason
			// for this is that the default PBoundsHandle modifies the bounds of an PNode, but when adding handles to
			// a PPath we really want it to be modifying the underlying geometry of the PPath, the shape without the
			// stroke. The solution is that we need to create handles specific to PPaths that locate themselves on the
			// paths internal geometry, not the external bounds geometry...
		
			n.Pen = new Pen(Color.Black, 10);
			n.Brush = Brushes.Green;

			// Here we create our own custom handle. This handle is located in the center of its parent
			// node and you can use it to drag the parent around. This handle also updates its color when
			// the is pressed/released in it. 
			PHandle h = new PathHandle(new PNodeLocator(n));
			h.AddInputEventListener(new HandleEventHandler(h));

			// make this handle appear a bit different then the default handle appearance.
			h.Brush = Brushes.Red;
			h.SetBounds(-10, -10, 20, 20);
		
			// also add our new custom handle to the node.
			n.AddChild(h);
		}

		class PathHandle : PHandle {
			public PathHandle(PLocator locator) : base(locator) {
			}
			public override void OnHandleDrag(object sender, SizeF size, PInputEventArgs e) {
				base.OnHandleDrag(sender, size, e);
				size = LocalToParent(size);
				Parent.TranslateBy(size.Width, size.Height);
			}
		}

		class HandleEventHandler : PBasicInputEventHandler {
			PHandle handle;

			public HandleEventHandler(PHandle handle) {
				this.handle = handle;
			}

			public override void OnMouseDown(object sender, PInputEventArgs e) {
				base.OnMouseDown(sender, e);
				handle.Brush = Brushes.Yellow;
			}

			public override void OnMouseUp(object sender, PInputEventArgs e) {
				base.OnMouseUp(sender, e);
				handle.Brush = Brushes.Red;
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
			// HandleExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "HandleExample";
			this.Text = "HandleExample";

		}
		#endregion
	}
}