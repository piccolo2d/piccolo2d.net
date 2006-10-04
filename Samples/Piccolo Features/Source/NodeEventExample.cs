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
using UMD.HCIL.PiccoloX;

namespace UMD.HCIL.PiccoloFeatures
{
	public class NodeEventExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public NodeEventExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			PLayer layer = Canvas.Layer;
			PNode aNode = new PNode();

			aNode.MouseDown += new PInputEventHandler(aNode_MouseDown);
			aNode.MouseDrag += new PInputEventHandler(aNode_MouseDrag);
			aNode.MouseUp += new PInputEventHandler(aNode_MouseUp);

			aNode.SetBounds(0, 0, 200, 200);
			aNode.Brush = new SolidBrush(Color.Green);

			// add another node to the canvas that does not handle events as a reference
			// point, so that we can make sure that our green node is getting dragged.				
			layer.AddChild(PPath.CreateRectangle(0, 0, 100, 80));
			layer.AddChild(aNode);		
		}
		
		protected void aNode_MouseDown(object sender, PInputEventArgs e) {
			PNode aNode = (PNode)sender;
			aNode.Brush = new SolidBrush(Color.Orange);
			PrintEventCoords(e);
			e.Handled = true;
		}

		protected void aNode_MouseDrag(object sender, PInputEventArgs e) {
			PNode aNode = (PNode)sender;
			SizeF delta = e.GetDeltaRelativeTo(aNode);
			aNode.TranslateBy(delta.Width, delta.Height);
			PrintEventCoords(e);
			e.Handled = true;
		}

		protected void aNode_MouseUp(object sender, PInputEventArgs e) {
			PNode aNode = (PNode)sender;
			aNode.Brush = new SolidBrush(Color.Green);
			PrintEventCoords(e);
			e.Handled = true;
		}

		public void PrintEventCoords(PInputEventArgs e) {
			System.Console.WriteLine("Canvas Location: " + e.CanvasPosition);
			//System.out.println("Global Location: " + aEvent.getGlobalLocation());
			//System.Console.WriteLine("Local Location: " + e.GetPositionRelativeTo(aNode));
			System.Console.WriteLine("Canvas Delta: " + e.CanvasDelta);
			//System.out.println("Global Delta: " + aEvent.getGlobalDelta());
			//System.Console.WriteLine("Local Delta: " + e.GetDeltaRelativeTo(aNode));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
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
		private void InitializeComponent()
		{
			// 
			// NodeEventExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "NodeEventExample";
			this.Text = "NodeEventExample";

		}
		#endregion
	}
}

