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
using UMD.HCIL.Piccolo.Activities;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.Piccolo.Nodes;

namespace UMD.HCIL.PiccoloFeatures {
	public class ActivityExample : UMD.HCIL.PiccoloX.PForm {
		private bool fRed = true;
		private PNode aNode;
		private System.ComponentModel.IContainer components = null;

		public ActivityExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			long currentTime = PUtil.CurrentTimeMillis;

			// Create a new node that we will apply different activities to, and
			// place that node at location 200, 200.
			aNode = PPath.CreateRectangle(0, 0, 100, 80);
			PLayer layer = Canvas.Layer;
			layer.AddChild(aNode);
			aNode.SetOffset(200, 200);
		
			// Create a new custom "flash" activity. This activity will start running in
			// five seconds, and while it runs it will flash aNode's brush color between
			// red and green every half second.  The same effect could be achieved by
			// extending PActivity and override OnActivityStep.
			PActivity flash = new PActivity(-1, 500, currentTime + 5000);
			flash.ActivityStepped = new ActivitySteppedDelegate(ActivityStepped);

			Canvas.Root.AddActivity(flash);

			// Use the PNode animate methods to create three activities that animate
			// the node's position. Since our node already descends from the root node the
			// animate methods will automatically schedule these activities for us.
			PActivity a1 = aNode.AnimateToPositionScaleRotation(0f, 0f, 0.5f, 0f, 5000);
			PActivity a2 = aNode.AnimateToPositionScaleRotation(100f, 0f, 1.5f, 110f, 5000);
			PActivity a3 = aNode.AnimateToPositionScaleRotation(200f, 100f, 1f, 0f, 5000);

			// the animate activities will start immediately (in the next call to PRoot.processInputs)
			// by default. Here we set their start times (in PRoot global time) so that they start 
			// when the previous one has finished.
			a1.StartTime = currentTime;
		
			a2.StartAfter(a1);
			a3.StartAfter(a2);
		
			// or the previous three lines could be replaced with these lines for the same effect.
			//a2.setStartTime(currentTime + 5000);
			//a3.setStartTime(currentTime + 10000);
		}

		protected void ActivityStepped(PActivity activity) {
			if (fRed) {
				aNode.Brush = Brushes.Red;
			} else {
				aNode.Brush = Brushes.Green;
			}		
				
			fRed = !fRed;
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
			// ActivityExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "ActivityExample";
			this.Text = "ActivityExample";

		}
		#endregion
	}
}