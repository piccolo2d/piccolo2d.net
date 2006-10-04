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
	public class NodeExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public NodeExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			NodeDemo();
			CreateNodeUsingExistingClasses();
			SubclassExistingClasses();
			ComposeOtherNodes();
			CreateCustomNode();

			Canvas.RemoveInputEventListener(Canvas.PanEventHandler);
			Canvas.AddInputEventListener(new PDragEventHandler());
		}

		// This method demonstrates the kinds of things that can be done with any node.
		public void NodeDemo() {
			PLayer layer = Canvas.Layer;
			PNode aNode = PPath.CreateRectangle(0, 0, 100, 80);
		
			// A node needs to be a descendent of the root to be displayed on the screen.
			layer.AddChild(aNode);
		
			// The default color for a node is blue, but you can change that with 
			// the setPaint method.
			aNode.Brush = Brushes.Red;
				
			// A node can have children nodes added to it.
			aNode.AddChild(PPath.CreateRectangle(0, 0, 100, 80));
				
			// The base bounds of a node is easy to change. Note that changing the base 
			// bounds of a node will not change it's children.
			aNode.SetBounds(-10, -10, 200, 110);
		
			// Each node has a transform that can be used to transform the node, and 
			// all its children on the screen.
			aNode.TranslateBy(100, 100);
			aNode.ScaleBy(1.5f);
			aNode.RotateBy(45);
		
			// The transparency of any node can be set, this transparency will be 
			// applied to any of the nodes children as well.
			//aNode.setTransparency(0.75f);
		
			// Its easy to copy nodes.
			PNode aCopy = (PNode) aNode.Clone();

			// Make is so that the copies children are not pickable. For this example
			// that means you will not be able to grab the child and remove it from
			// its parent.
			aNode.ChildrenPickable = false;

			// Change the look of the copy
			aNode.Brush = Brushes.Green;
			//aNode.setTransparency(1.0f);
		
			// Let's add the copy to the root, and translate it so that it does not 
			// cover the original node.
			layer.AddChild(aCopy);		
			aCopy.SetOffset(0, 0);
			aCopy.RotateBy(-45);
		}
		
		// So far we have just been using PNode, but of course PNode has many
		// subclasses that you can try out to.
		public void CreateNodeUsingExistingClasses() {
			PLayer layer = Canvas.Layer;
			layer.AddChild(PPath.CreateEllipse(0, 0, 100, 100));
			layer.AddChild(PPath.CreateRectangle(0, 100, 100, 100));
			layer.AddChild(new PText("Hello World"));

			// Here we create an image node that displays a thumbnail
			// image of the root node. Note that you can easily get a thumbnail
			// of any node by using PNode.ToImage().
			PImage image = new PImage(layer.ToImage(300, 300, null));
			layer.AddChild(image);
		}

		public void SubclassExistingClasses() {
			ToggleShape ts = new ToggleShape();
			ts.AddInputEventListener(new ToggleShapeListener(ts));
			ts.Brush = Brushes.Orange;
			Canvas.Layer.AddChild(ts);
		}

		class ToggleShape : PPath {
			private bool fIsPressed = false;
			public bool FIsPressed {
				set { fIsPressed = value; }
			}
			public ToggleShape() {
				this.AddEllipse(0, 0, 100, 80);
			}
			protected override void Paint(PPaintContext paintContext) {
				if (fIsPressed) {
					Graphics g = paintContext.Graphics;
					g.FillRectangle(this.Brush, this.Bounds);
				}
				else {
					base.Paint(paintContext);
				}
			}
		}

		class ToggleShapeListener : PBasicInputEventHandler {
			private ToggleShape target;
			public ToggleShapeListener(ToggleShape node) {
				target = node;
			}
			public override void OnMouseDown(object sender, PInputEventArgs e) {
				base.OnMouseDown (sender, e);
				target.FIsPressed = true;
				target.InvalidatePaint(); // this tells the framework that the node needs to be redisplayed.
			}
			public override void OnMouseUp(object sender, PInputEventArgs e) {
				base.OnMouseUp (sender, e);
				target.FIsPressed = false;
				target.InvalidatePaint();
			}
		}

		// Here a new "face" node is created. But instead of drawing the face directly
		// using Graphics2D we compose the face from other nodes.
		public void ComposeOtherNodes() {
			PNode myCompositeFace = PPath.CreateRectangle(0, 0, 100, 80);
		
			// create parts for the face.
			PNode eye1 = PPath.CreateEllipse(0, 0, 20, 20);
			eye1.Brush = Brushes.Yellow;
			PNode eye2 = (PNode) eye1.Clone();
			PNode mouth = PPath.CreateRectangle(0, 0, 40, 20);
			mouth.Brush = Brushes.Black;
		
			// add the face parts
			myCompositeFace.AddChild(eye1);
			myCompositeFace.AddChild(eye2);
			myCompositeFace.AddChild(mouth);
		
			// don't want anyone grabbing out our eye's.
			myCompositeFace.ChildrenPickable = false;
		
			// position the face parts.
			eye2.TranslateBy(25, 0);
			mouth.TranslateBy(0, 30);
		
			// set the face bounds so that it neatly contains the face parts.
			RectangleF b = myCompositeFace.UnionOfChildrenBounds;
			b.Inflate(5, 5);
			myCompositeFace.Bounds = b;
		
			// opps it to small, so scale it up.
			myCompositeFace.ScaleBy(1.5f);
		
			Canvas.Layer.AddChild(myCompositeFace);
		}

		public void CreateCustomNode() {
			CustomNode cn = new CustomNode();
			cn.SetBounds(0, 0, 100, 80);
			cn.Brush = Brushes.Black;
			Canvas.Layer.AddChild(cn);
		}

		class CustomNode : PNode {
			protected override void Paint(PPaintContext paintContext) {
				float bx = X;
				float by = Y;
				float rightBorder = bx + Width;
				float bottomBorder = by + Height;
				
				GraphicsPath line = new GraphicsPath();
				Graphics g = paintContext.Graphics;				
				Pen pen = new Pen(this.Brush, 0);

				// draw vertical lines
				for (float x = bx; x < rightBorder; x += 5) {
					line.Reset();
					line.AddLine(x, by, x, bottomBorder);
					g.DrawPath(pen, line);
				}
				
				for (float y = by; y < bottomBorder; y += 5) {
					line.Reset();
					line.AddLine(bx, y, rightBorder, y);
					g.DrawPath(pen, line);
				}
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
			// NodeExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "NodeExample";
			this.Text = "NodeExample";

		}
		#endregion
	}
}