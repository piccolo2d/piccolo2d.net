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

namespace UMD.HCIL.PiccoloFeatures {
	public class KeyEventFocusExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public KeyEventFocusExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		class RedInputEventListener : PBasicInputEventHandler {
			public override void OnKeyDown(object sender, PInputEventArgs e) {
				base.OnKeyDown(sender, e);
				System.Console.WriteLine("red keydown");
			}

			public override void OnMouseDown(object sender, PInputEventArgs e) {
				base.OnMouseDown(sender, e);
				e.InputManager.KeyboardFocus = e.Path;
				System.Console.WriteLine("red mousedown");
			}

			public override void OnGotFocus(object sender, PInputEventArgs e) {
				base.OnGotFocus(sender, e);
				System.Console.WriteLine("red got focus");
			}

			public override void OnLostFocus(object sender, PInputEventArgs e) {
				base.OnLostFocus(sender, e);
				System.Console.WriteLine("red lost focus");
			}
		}

		class GreenInputEventListener : PBasicInputEventHandler {
			public override void OnKeyDown(object sender, PInputEventArgs e) {
				base.OnKeyDown(sender, e);
				System.Console.WriteLine("green keydown");
			}

			public override void OnMouseDown(object sender, PInputEventArgs e) {
				base.OnMouseDown(sender, e);
				e.InputManager.KeyboardFocus = e.Path;
				System.Console.WriteLine("green mousedown");
			}

			public override void OnGotFocus(object sender, PInputEventArgs e) {
				base.OnGotFocus(sender, e);
				System.Console.WriteLine("green got focus");
			}

			public override void OnLostFocus(object sender, PInputEventArgs e) {
				base.OnLostFocus(sender, e);
				System.Console.WriteLine("green lost focus");
			}
		}

		public override void Initialize() {
			// Create a green and red node and add them to canvas layer.
			PCanvas canvas = Canvas;
			PNode nodeGreen = PPath.CreateRectangle(0, 0, 100, 100);
			PNode nodeRed = PPath.CreateRectangle(0, 0, 100, 100);
			nodeRed.TranslateBy(200, 0);
			nodeGreen.Brush = Brushes.Green;
			nodeRed.Brush = Brushes.Red;
			canvas.Layer.AddChild(nodeGreen);
			canvas.Layer.AddChild(nodeRed);

			nodeGreen.AddInputEventListener(new GreenInputEventListener());
			nodeRed.AddInputEventListener(new RedInputEventListener());

			// Or, delegates could be used to add each eventhandler such as
			// nodeGreen.MouseDown += new PInputEventHandler(nodeGreen_MouseDown);
			// However, overriding the PBasicInputEventHandler is the preferred
			// approach
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
			// KeyEventFocusExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "KeyEventFocusExample";
			this.Text = "KeyEventFocusExample";

		}
		#endregion
	}
}