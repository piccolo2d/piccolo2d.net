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
using UMD.HCIL.PiccoloX;
using UMD.HCIL.PiccoloX.Events;
using UMD.HCIL.PiccoloX.Nodes;

namespace UMD.HCIL.PiccoloFeatures {
	public class ControlExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public ControlExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			// Add a standard pnode to the scene graph.
			PNode aNode = new PNode();
			aNode.SetBounds(0, 70, 15, 15);
			aNode.Brush = Brushes.Blue;
			Canvas.Layer.AddChild(aNode);

			// Create a button.
			Button button = new Button();
			button.Text = "Hello";
			button.Bounds = new Rectangle(10, 10, 10, 10);
			button.BackColor = SystemColors.Control;

			// Wrap the button in a PControl and
			// add it to the scene graph.
			PControl cn = new PControl(button);
			Canvas.Layer.AddChild(cn);
			cn.SetBounds(20, 20, 70, 70);

			// Create another button.
			Button otherButton = new Button();
			otherButton.Text = "123";
			otherButton.Bounds = new Rectangle(0, 0, 15, 45);
			otherButton.BackColor = SystemColors.Control;

			// Wrap the second button in another PControl and
			// add it to the scene graph.
			PControl cn2 = new PControl(otherButton, PCanvas.CURRENT_PCANVAS);
			cn2.ScaleBy(1.1f);
			Canvas.Layer.AddChild(cn2);

			// Create a tabcontrol
			TabControl tabControl = new TabControl();
			tabControl.Size = new Size(60, 60);
			tabControl.TabPages.Add(new TabPage("P1"));
			tabControl.TabPages.Add(new TabPage("P2"));

			// Wrap the tabcontrol in a PControl and
			// add it the scene graph.
			PControl cn3 = new PControl(tabControl);
			cn3.ScaleBy(1.2f);
			cn3.TranslateBy(0, 100);
			Canvas.Layer.AddChild(cn3);

			// Create an internal camera that looks at the main layer.
			PCamera internalCamera = new PCamera();
			internalCamera.TranslateViewBy(145, 145);
			internalCamera.ScaleViewBy(.5f);
			internalCamera.SetBounds(130, 130, 200, 200);
			internalCamera.Brush = Brushes.Yellow;
			internalCamera.AddLayer(Canvas.Layer);
			Canvas.Camera.AddChild(internalCamera);

			Canvas.Layer.ScaleBy(1.3f);

			// Create another canvas.
			PCamera otherCamera = new PCamera();
			otherCamera.AddLayer(Canvas.Layer);
			Canvas.Root.AddChild(otherCamera); 	
		
			PCanvas other = new PCanvas();
			other.Camera = otherCamera;
			PForm result = new PForm(false, other);
			result.StartPosition = FormStartPosition.Manual;
			result.Location = new Point(this.Location.X + this.Width, this.Location.Y);
			result.Size = this.Size;
			result.Show();

			// Add the control event handler to both canvas' cameras.
			Canvas.Camera.AddInputEventListener(new PControlEventHandler());
			other.Camera.AddInputEventListener(new PControlEventHandler());
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
			// ControlExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "ControlExample";
			this.Text = "ControlExample";

		}
		#endregion
	}
}