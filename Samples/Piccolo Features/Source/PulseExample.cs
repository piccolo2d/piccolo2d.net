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
using UMD.HCIL.Piccolo.Nodes;

namespace UMD.HCIL.PiccoloFeatures {
	public class PulseExample : UMD.HCIL.PiccoloX.PForm {
		private System.ComponentModel.IContainer components = null;

		public PulseExample() {
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		public override void Initialize() {
			PRoot root = Canvas.Root;
			PLayer layer = Canvas.Layer;
			PActivityScheduler scheduler = root.ActivityScheduler;
		
			PNode singlePulse = PPath.CreateRectangle(0, 0, 100, 80);;
			PPath repeatPulse = PPath.CreateRectangle(100, 80, 100, 80);;
			PNode repeatReversePulse = PPath.CreateRectangle(200, 160, 100, 80);;

			layer.AddChild(singlePulse);
			layer.AddChild(repeatPulse);
			layer.AddChild(repeatReversePulse);

			PColorActivity singlePulseActivity = new PColorActivity(1000, 0, 1, ActivityMode.SourceToDestination, new PulseTarget(singlePulse), Color.Orange);
			PColorActivity repeatPulseActivity = new PColorActivity(1000, 0, 5, ActivityMode.SourceToDestination, new PulseTarget(repeatPulse), Color.Blue);
			PColorActivity repeatReversePulseActivity = new PColorActivity(500, 0, 10, ActivityMode.SourceToDestination, new PulseTarget(repeatReversePulse), Color.Green);

			scheduler.AddActivity(singlePulseActivity);
			scheduler.AddActivity(repeatPulseActivity);
			scheduler.AddActivity(repeatReversePulseActivity);
		}

		class PulseTarget : PColorActivity.Target {
			PNode node;
			public PulseTarget(PNode node) {
				this.node = node;
			}

			public Color Color {
				get {
					return ((SolidBrush)node.Brush).Color;
				}
				set {
					node.Brush = new SolidBrush(value);
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
			// PulseExample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "PulseExample";
			this.Text = "PulseExample";

		}
		#endregion
	}
}