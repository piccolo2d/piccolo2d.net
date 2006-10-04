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
using UMD.HCIL.RangeSlider;

namespace RangeSliderTester {
	public class RangeSliderTester : System.Windows.Forms.Form {
		private UMD.HCIL.RangeSlider.RangeSlider rangeSlider1;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.Container components = null;

		public RangeSliderTester() {
			InitializeComponent();

			rangeSlider1.RangeChanged += new UMD.HCIL.RangeSlider.RangeSlider.RangeEventHandler(rangeSlider1_RangeChanged);
		}

		private void rangeSlider1_RangeChanged(object sender, RangeEventArgs e) {
			label1.Text = "Range: [" + (int)e.Low + ", " + (int)e.High + "]";
		}

		#region Dispose
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.rangeSlider1 = new UMD.HCIL.RangeSlider.RangeSlider();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// rangeSlider1
			// 
			this.rangeSlider1.AllowDrop = true;
			this.rangeSlider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rangeSlider1.BackColor = System.Drawing.Color.White;
			this.rangeSlider1.High = 90;
			this.rangeSlider1.Location = new System.Drawing.Point(64, 88);
			this.rangeSlider1.Low = 30;
			this.rangeSlider1.Maximum = 100;
			this.rangeSlider1.Minimum = 0;
			this.rangeSlider1.Name = "rangeSlider1";
			this.rangeSlider1.Size = new System.Drawing.Size(144, 23);
			this.rangeSlider1.TabIndex = 0;
			this.rangeSlider1.Text = "rangeSlider1";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(64, 136);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Range:";
			// 
			// RangeSliderTester
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rangeSlider1);
			this.Name = "RangeSliderTester";
			this.Text = "Range Slider Tester";
			this.ResumeLayout(false);

		}
		#endregion

		#region Main
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.Run(new RangeSliderTester());
		}
		#endregion
	}
}