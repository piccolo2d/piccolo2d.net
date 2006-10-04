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
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.PiccoloFeatures {

	public class PiccoloFeatures : System.Windows.Forms.Form {
		private System.Windows.Forms.CheckBox printFrameRates;
		private System.Windows.Forms.CheckBox showRegionManagment;
		private System.Windows.Forms.CheckBox showFullBounds;
		private System.Windows.Forms.Button btnActivityExample;
		private System.Windows.Forms.Button btnAngleNodeExample;
		private System.Windows.Forms.Button btnCameraExample;
		private System.Windows.Forms.Button btnClipExample;
		private System.Windows.Forms.Button btnCenterExample;
		private System.Windows.Forms.Button btnDynamicExample;
		private System.Windows.Forms.Button btnEventHandlerExample;
		private System.Windows.Forms.Button btnGridExample;
		private System.Windows.Forms.Button btnGroupExample;
		private System.Windows.Forms.Button btnHandleExample;
		private System.Windows.Forms.Button btnHelloWorldExample;
		private System.Windows.Forms.Button btnKeyEventFocusExample;
		private System.Windows.Forms.Button btnLayoutExample;
		private System.Windows.Forms.Button btnLensExample;
		private System.Windows.Forms.Button btnNavigationExample;
		private System.Windows.Forms.Button btnNodeEventExample;
		private System.Windows.Forms.Button btnNodeExample;
		private System.Windows.Forms.Button btnPanToExample;
		private System.Windows.Forms.Button btnPathExample;
		private System.Windows.Forms.Button btnPositionExample;
		private System.Windows.Forms.Button btnPositionPathActivity;
		private System.Windows.Forms.Button btnPulseExample;
		private System.Windows.Forms.Button btnScrollingExample;
		private System.Windows.Forms.Button btnSelectionExample;
		private System.Windows.Forms.Button btnSquiggleExample;
		private System.Windows.Forms.Button btnStickyExample;
		private System.Windows.Forms.Button btnStickyHandleExample;
		private System.Windows.Forms.Button btnToolTipExample;
		private System.Windows.Forms.Button btnTwoCanvasExample;
		private System.Windows.Forms.Button btnWaitForActivitiesExample;
		private System.Windows.Forms.Button btnFullScreenNodeExample;
		private System.Windows.Forms.Button btnThreeDimRectExample;
		private System.Windows.Forms.Button btnNodeCacheExample;
		private System.Windows.Forms.Button btnCompositeExample;
		private System.Windows.Forms.Button btnControlExample;
		private System.Windows.Forms.Button btnHierarchyZoomExample;
		private System.Windows.Forms.Button btnBirdsEyeExample;
		private System.Windows.Forms.Button btnChartLabelExample;
		private System.Windows.Forms.Button btnNodeLinkExample;

		private System.ComponentModel.Container components = null;

		public PiccoloFeatures() {
			InitializeComponent();
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		[STAThread]
		static void Main() {
			Application.Run(new PiccoloFeatures());
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.btnActivityExample = new System.Windows.Forms.Button();
			this.btnCameraExample = new System.Windows.Forms.Button();
			this.btnSquiggleExample = new System.Windows.Forms.Button();
			this.btnDynamicExample = new System.Windows.Forms.Button();
			this.btnEventHandlerExample = new System.Windows.Forms.Button();
			this.btnClipExample = new System.Windows.Forms.Button();
			this.btnKeyEventFocusExample = new System.Windows.Forms.Button();
			this.btnHandleExample = new System.Windows.Forms.Button();
			this.btnHelloWorldExample = new System.Windows.Forms.Button();
			this.btnLayoutExample = new System.Windows.Forms.Button();
			this.btnLensExample = new System.Windows.Forms.Button();
			this.btnNavigationExample = new System.Windows.Forms.Button();
			this.btnNodeEventExample = new System.Windows.Forms.Button();
			this.btnNodeExample = new System.Windows.Forms.Button();
			this.btnPanToExample = new System.Windows.Forms.Button();
			this.btnPathExample = new System.Windows.Forms.Button();
			this.btnPositionExample = new System.Windows.Forms.Button();
			this.btnStickyExample = new System.Windows.Forms.Button();
			this.btnTwoCanvasExample = new System.Windows.Forms.Button();
			this.btnWaitForActivitiesExample = new System.Windows.Forms.Button();
			this.printFrameRates = new System.Windows.Forms.CheckBox();
			this.showRegionManagment = new System.Windows.Forms.CheckBox();
			this.showFullBounds = new System.Windows.Forms.CheckBox();
			this.btnAngleNodeExample = new System.Windows.Forms.Button();
			this.btnCenterExample = new System.Windows.Forms.Button();
			this.btnGridExample = new System.Windows.Forms.Button();
			this.btnGroupExample = new System.Windows.Forms.Button();
			this.btnPositionPathActivity = new System.Windows.Forms.Button();
			this.btnPulseExample = new System.Windows.Forms.Button();
			this.btnScrollingExample = new System.Windows.Forms.Button();
			this.btnSelectionExample = new System.Windows.Forms.Button();
			this.btnStickyHandleExample = new System.Windows.Forms.Button();
			this.btnToolTipExample = new System.Windows.Forms.Button();
			this.btnFullScreenNodeExample = new System.Windows.Forms.Button();
			this.btnThreeDimRectExample = new System.Windows.Forms.Button();
			this.btnNodeCacheExample = new System.Windows.Forms.Button();
			this.btnCompositeExample = new System.Windows.Forms.Button();
			this.btnControlExample = new System.Windows.Forms.Button();
			this.btnHierarchyZoomExample = new System.Windows.Forms.Button();
			this.btnBirdsEyeExample = new System.Windows.Forms.Button();
			this.btnChartLabelExample = new System.Windows.Forms.Button();
			this.btnNodeLinkExample = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnActivityExample
			// 
			this.btnActivityExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnActivityExample.Location = new System.Drawing.Point(8, 80);
			this.btnActivityExample.Name = "btnActivityExample";
			this.btnActivityExample.Size = new System.Drawing.Size(184, 23);
			this.btnActivityExample.TabIndex = 3;
			this.btnActivityExample.Text = "ActivityExample";
			this.btnActivityExample.Click += new System.EventHandler(this.btnActivityExample_Click);
			// 
			// btnCameraExample
			// 
			this.btnCameraExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnCameraExample.Location = new System.Drawing.Point(8, 152);
			this.btnCameraExample.Name = "btnCameraExample";
			this.btnCameraExample.Size = new System.Drawing.Size(184, 23);
			this.btnCameraExample.TabIndex = 5;
			this.btnCameraExample.Text = "CameraExample";
			this.btnCameraExample.Click += new System.EventHandler(this.btnCameraExample_Click);
			// 
			// btnSquiggleExample
			// 
			this.btnSquiggleExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSquiggleExample.Location = new System.Drawing.Point(8, 848);
			this.btnSquiggleExample.Name = "btnSquiggleExample";
			this.btnSquiggleExample.Size = new System.Drawing.Size(184, 23);
			this.btnSquiggleExample.TabIndex = 28;
			this.btnSquiggleExample.Text = "SquiggleExample";
			this.btnSquiggleExample.Click += new System.EventHandler(this.btnSquiggleExample_Click);
			// 
			// btnDynamicExample
			// 
			this.btnDynamicExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnDynamicExample.Location = new System.Drawing.Point(8, 296);
			this.btnDynamicExample.Name = "btnDynamicExample";
			this.btnDynamicExample.Size = new System.Drawing.Size(184, 23);
			this.btnDynamicExample.TabIndex = 8;
			this.btnDynamicExample.Text = "DynamicExample";
			this.btnDynamicExample.Click += new System.EventHandler(this.btnDynamicExample_Click);
			// 
			// btnEventHandlerExample
			// 
			this.btnEventHandlerExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnEventHandlerExample.Location = new System.Drawing.Point(8, 320);
			this.btnEventHandlerExample.Name = "btnEventHandlerExample";
			this.btnEventHandlerExample.Size = new System.Drawing.Size(184, 23);
			this.btnEventHandlerExample.TabIndex = 9;
			this.btnEventHandlerExample.Text = "EventHandlerExample";
			this.btnEventHandlerExample.Click += new System.EventHandler(this.btnEventHandlerExample_Click);
			// 
			// btnClipExample
			// 
			this.btnClipExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnClipExample.Location = new System.Drawing.Point(8, 224);
			this.btnClipExample.Name = "btnClipExample";
			this.btnClipExample.Size = new System.Drawing.Size(184, 23);
			this.btnClipExample.TabIndex = 7;
			this.btnClipExample.Text = "ClipExample";
			this.btnClipExample.Click += new System.EventHandler(this.btnClipExample_Click);
			// 
			// btnKeyEventFocusExample
			// 
			this.btnKeyEventFocusExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnKeyEventFocusExample.Location = new System.Drawing.Point(8, 488);
			this.btnKeyEventFocusExample.Name = "btnKeyEventFocusExample";
			this.btnKeyEventFocusExample.Size = new System.Drawing.Size(184, 23);
			this.btnKeyEventFocusExample.TabIndex = 15;
			this.btnKeyEventFocusExample.Text = "KeyEventFocusExample";
			this.btnKeyEventFocusExample.Click += new System.EventHandler(this.btnKeyEventFocusExample_Click);
			// 
			// btnHandleExample
			// 
			this.btnHandleExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnHandleExample.Location = new System.Drawing.Point(8, 416);
			this.btnHandleExample.Name = "btnHandleExample";
			this.btnHandleExample.Size = new System.Drawing.Size(184, 23);
			this.btnHandleExample.TabIndex = 13;
			this.btnHandleExample.Text = "HandleExample";
			this.btnHandleExample.Click += new System.EventHandler(this.btnHandleExample_Click);
			// 
			// btnHelloWorldExample
			// 
			this.btnHelloWorldExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnHelloWorldExample.Location = new System.Drawing.Point(8, 440);
			this.btnHelloWorldExample.Name = "btnHelloWorldExample";
			this.btnHelloWorldExample.Size = new System.Drawing.Size(184, 23);
			this.btnHelloWorldExample.TabIndex = 14;
			this.btnHelloWorldExample.Text = "HelloWorldExample";
			this.btnHelloWorldExample.Click += new System.EventHandler(this.btnHelloWorldExample_Click);
			// 
			// btnLayoutExample
			// 
			this.btnLayoutExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnLayoutExample.Location = new System.Drawing.Point(8, 512);
			this.btnLayoutExample.Name = "btnLayoutExample";
			this.btnLayoutExample.Size = new System.Drawing.Size(184, 23);
			this.btnLayoutExample.TabIndex = 16;
			this.btnLayoutExample.Text = "LayoutExample";
			this.btnLayoutExample.Click += new System.EventHandler(this.btnLayoutExample_Click);
			// 
			// btnLensExample
			// 
			this.btnLensExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnLensExample.Location = new System.Drawing.Point(8, 536);
			this.btnLensExample.Name = "btnLensExample";
			this.btnLensExample.Size = new System.Drawing.Size(184, 23);
			this.btnLensExample.TabIndex = 17;
			this.btnLensExample.Text = "LensExample";
			this.btnLensExample.Click += new System.EventHandler(this.btnLensExample_Click);
			// 
			// btnNavigationExample
			// 
			this.btnNavigationExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnNavigationExample.Location = new System.Drawing.Point(8, 560);
			this.btnNavigationExample.Name = "btnNavigationExample";
			this.btnNavigationExample.Size = new System.Drawing.Size(184, 23);
			this.btnNavigationExample.TabIndex = 18;
			this.btnNavigationExample.Text = "NavigationExample";
			this.btnNavigationExample.Click += new System.EventHandler(this.btnNavigationExample_Click);
			// 
			// btnNodeEventExample
			// 
			this.btnNodeEventExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnNodeEventExample.Location = new System.Drawing.Point(8, 608);
			this.btnNodeEventExample.Name = "btnNodeEventExample";
			this.btnNodeEventExample.Size = new System.Drawing.Size(184, 23);
			this.btnNodeEventExample.TabIndex = 19;
			this.btnNodeEventExample.Text = "NodeEventExample";
			this.btnNodeEventExample.Click += new System.EventHandler(this.btnNodeEventExample_Click);
			// 
			// btnNodeExample
			// 
			this.btnNodeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnNodeExample.Location = new System.Drawing.Point(8, 632);
			this.btnNodeExample.Name = "btnNodeExample";
			this.btnNodeExample.Size = new System.Drawing.Size(184, 23);
			this.btnNodeExample.TabIndex = 20;
			this.btnNodeExample.Text = "NodeExample";
			this.btnNodeExample.Click += new System.EventHandler(this.btnNodeExample_Click);
			// 
			// btnPanToExample
			// 
			this.btnPanToExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPanToExample.Location = new System.Drawing.Point(8, 680);
			this.btnPanToExample.Name = "btnPanToExample";
			this.btnPanToExample.Size = new System.Drawing.Size(184, 23);
			this.btnPanToExample.TabIndex = 21;
			this.btnPanToExample.Text = "PanToExample";
			this.btnPanToExample.Click += new System.EventHandler(this.btnPanToExample_Click);
			// 
			// btnPathExample
			// 
			this.btnPathExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPathExample.Location = new System.Drawing.Point(8, 704);
			this.btnPathExample.Name = "btnPathExample";
			this.btnPathExample.Size = new System.Drawing.Size(184, 23);
			this.btnPathExample.TabIndex = 22;
			this.btnPathExample.Text = "PathExample";
			this.btnPathExample.Click += new System.EventHandler(this.btnPathExample_Click);
			// 
			// btnPositionExample
			// 
			this.btnPositionExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPositionExample.Location = new System.Drawing.Point(8, 728);
			this.btnPositionExample.Name = "btnPositionExample";
			this.btnPositionExample.Size = new System.Drawing.Size(184, 23);
			this.btnPositionExample.TabIndex = 23;
			this.btnPositionExample.Text = "PositionExample";
			this.btnPositionExample.Click += new System.EventHandler(this.btnPositionExample_Click);
			// 
			// btnStickyExample
			// 
			this.btnStickyExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnStickyExample.Location = new System.Drawing.Point(8, 872);
			this.btnStickyExample.Name = "btnStickyExample";
			this.btnStickyExample.Size = new System.Drawing.Size(184, 23);
			this.btnStickyExample.TabIndex = 29;
			this.btnStickyExample.Text = "StickyExample";
			this.btnStickyExample.Click += new System.EventHandler(this.btnStickyExample_Click);
			// 
			// btnTwoCanvasExample
			// 
			this.btnTwoCanvasExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnTwoCanvasExample.Location = new System.Drawing.Point(8, 968);
			this.btnTwoCanvasExample.Name = "btnTwoCanvasExample";
			this.btnTwoCanvasExample.Size = new System.Drawing.Size(184, 23);
			this.btnTwoCanvasExample.TabIndex = 33;
			this.btnTwoCanvasExample.Text = "TwoCanvasExample";
			this.btnTwoCanvasExample.Click += new System.EventHandler(this.btnTwoCanvasExample_Click);
			// 
			// btnWaitForActivitiesExample
			// 
			this.btnWaitForActivitiesExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnWaitForActivitiesExample.Location = new System.Drawing.Point(8, 992);
			this.btnWaitForActivitiesExample.Name = "btnWaitForActivitiesExample";
			this.btnWaitForActivitiesExample.Size = new System.Drawing.Size(184, 23);
			this.btnWaitForActivitiesExample.TabIndex = 34;
			this.btnWaitForActivitiesExample.Text = "WaitForActivitiesExample";
			this.btnWaitForActivitiesExample.Click += new System.EventHandler(this.btnWaitForActivitiesExample_Click);
			// 
			// printFrameRates
			// 
			this.printFrameRates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.printFrameRates.Location = new System.Drawing.Point(8, 8);
			this.printFrameRates.Name = "printFrameRates";
			this.printFrameRates.Size = new System.Drawing.Size(184, 24);
			this.printFrameRates.TabIndex = 0;
			this.printFrameRates.Text = "Print Frame Rates";
			this.printFrameRates.CheckedChanged += new System.EventHandler(this.printFrameRates_CheckedChanged);
			// 
			// showRegionManagment
			// 
			this.showRegionManagment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.showRegionManagment.Location = new System.Drawing.Point(8, 32);
			this.showRegionManagment.Name = "showRegionManagment";
			this.showRegionManagment.Size = new System.Drawing.Size(176, 24);
			this.showRegionManagment.TabIndex = 1;
			this.showRegionManagment.Text = "Show Region Managment";
			this.showRegionManagment.CheckedChanged += new System.EventHandler(this.showRegionManagment_CheckedChanged);
			// 
			// showFullBounds
			// 
			this.showFullBounds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.showFullBounds.Location = new System.Drawing.Point(8, 56);
			this.showFullBounds.Name = "showFullBounds";
			this.showFullBounds.Size = new System.Drawing.Size(176, 24);
			this.showFullBounds.TabIndex = 2;
			this.showFullBounds.Text = "Show Full Bounds";
			this.showFullBounds.CheckedChanged += new System.EventHandler(this.showFullBounds_CheckedChanged);
			// 
			// btnAngleNodeExample
			// 
			this.btnAngleNodeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnAngleNodeExample.Location = new System.Drawing.Point(8, 104);
			this.btnAngleNodeExample.Name = "btnAngleNodeExample";
			this.btnAngleNodeExample.Size = new System.Drawing.Size(184, 23);
			this.btnAngleNodeExample.TabIndex = 4;
			this.btnAngleNodeExample.Text = "AngleNodeExample";
			this.btnAngleNodeExample.Click += new System.EventHandler(this.btnAngleNodeExample_Click);
			// 
			// btnCenterExample
			// 
			this.btnCenterExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnCenterExample.Location = new System.Drawing.Point(8, 176);
			this.btnCenterExample.Name = "btnCenterExample";
			this.btnCenterExample.Size = new System.Drawing.Size(184, 23);
			this.btnCenterExample.TabIndex = 6;
			this.btnCenterExample.Text = "CenterExample";
			this.btnCenterExample.Click += new System.EventHandler(this.btnCenterExample_Click);
			// 
			// btnGridExample
			// 
			this.btnGridExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnGridExample.Location = new System.Drawing.Point(8, 368);
			this.btnGridExample.Name = "btnGridExample";
			this.btnGridExample.Size = new System.Drawing.Size(184, 23);
			this.btnGridExample.TabIndex = 11;
			this.btnGridExample.Text = "GridExample";
			this.btnGridExample.Click += new System.EventHandler(this.btnGridExample_Click);
			// 
			// btnGroupExample
			// 
			this.btnGroupExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnGroupExample.Location = new System.Drawing.Point(8, 392);
			this.btnGroupExample.Name = "btnGroupExample";
			this.btnGroupExample.Size = new System.Drawing.Size(184, 23);
			this.btnGroupExample.TabIndex = 12;
			this.btnGroupExample.Text = "GroupExample";
			this.btnGroupExample.Click += new System.EventHandler(this.btnGroupExample_Click);
			// 
			// btnPositionPathActivity
			// 
			this.btnPositionPathActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPositionPathActivity.Location = new System.Drawing.Point(8, 752);
			this.btnPositionPathActivity.Name = "btnPositionPathActivity";
			this.btnPositionPathActivity.Size = new System.Drawing.Size(184, 23);
			this.btnPositionPathActivity.TabIndex = 24;
			this.btnPositionPathActivity.Text = "PositionPathActivityExample";
			this.btnPositionPathActivity.Click += new System.EventHandler(this.btnPositionPathActivity_Click);
			// 
			// btnPulseExample
			// 
			this.btnPulseExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPulseExample.Location = new System.Drawing.Point(8, 776);
			this.btnPulseExample.Name = "btnPulseExample";
			this.btnPulseExample.Size = new System.Drawing.Size(184, 23);
			this.btnPulseExample.TabIndex = 25;
			this.btnPulseExample.Text = "PulseExample";
			this.btnPulseExample.Click += new System.EventHandler(this.btnPulseExample_Click);
			// 
			// btnScrollingExample
			// 
			this.btnScrollingExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnScrollingExample.Location = new System.Drawing.Point(8, 800);
			this.btnScrollingExample.Name = "btnScrollingExample";
			this.btnScrollingExample.Size = new System.Drawing.Size(184, 23);
			this.btnScrollingExample.TabIndex = 26;
			this.btnScrollingExample.Text = "ScrollingExample";
			this.btnScrollingExample.Click += new System.EventHandler(this.btnScrollingExample_Click);
			// 
			// btnSelectionExample
			// 
			this.btnSelectionExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectionExample.Location = new System.Drawing.Point(8, 824);
			this.btnSelectionExample.Name = "btnSelectionExample";
			this.btnSelectionExample.Size = new System.Drawing.Size(184, 23);
			this.btnSelectionExample.TabIndex = 27;
			this.btnSelectionExample.Text = "SelectionExample";
			this.btnSelectionExample.Click += new System.EventHandler(this.btnSelectionExample_Click);
			// 
			// btnStickyHandleExample
			// 
			this.btnStickyHandleExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnStickyHandleExample.Location = new System.Drawing.Point(8, 896);
			this.btnStickyHandleExample.Name = "btnStickyHandleExample";
			this.btnStickyHandleExample.Size = new System.Drawing.Size(184, 23);
			this.btnStickyHandleExample.TabIndex = 30;
			this.btnStickyHandleExample.Text = "StickyHandleExample";
			this.btnStickyHandleExample.Click += new System.EventHandler(this.btnStickyHandleExample_Click);
			// 
			// btnToolTipExample
			// 
			this.btnToolTipExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnToolTipExample.Location = new System.Drawing.Point(8, 944);
			this.btnToolTipExample.Name = "btnToolTipExample";
			this.btnToolTipExample.Size = new System.Drawing.Size(184, 23);
			this.btnToolTipExample.TabIndex = 32;
			this.btnToolTipExample.Text = "ToolTipExample";
			this.btnToolTipExample.Click += new System.EventHandler(this.btnToolTipExample_Click);
			// 
			// btnFullScreenNodeExample
			// 
			this.btnFullScreenNodeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnFullScreenNodeExample.Location = new System.Drawing.Point(8, 344);
			this.btnFullScreenNodeExample.Name = "btnFullScreenNodeExample";
			this.btnFullScreenNodeExample.Size = new System.Drawing.Size(184, 23);
			this.btnFullScreenNodeExample.TabIndex = 10;
			this.btnFullScreenNodeExample.Text = "FullScreenNodeExample";
			this.btnFullScreenNodeExample.Click += new System.EventHandler(this.btnFullScreenNodeExample_Click);
			// 
			// btnThreeDimRectExample
			// 
			this.btnThreeDimRectExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnThreeDimRectExample.Location = new System.Drawing.Point(8, 920);
			this.btnThreeDimRectExample.Name = "btnThreeDimRectExample";
			this.btnThreeDimRectExample.Size = new System.Drawing.Size(184, 24);
			this.btnThreeDimRectExample.TabIndex = 36;
			this.btnThreeDimRectExample.Text = "ThreeDimRectExample";
			this.btnThreeDimRectExample.Click += new System.EventHandler(this.btnThreeDimRectExample_Click);
			// 
			// btnNodeCacheExample
			// 
			this.btnNodeCacheExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnNodeCacheExample.Location = new System.Drawing.Point(8, 584);
			this.btnNodeCacheExample.Name = "btnNodeCacheExample";
			this.btnNodeCacheExample.Size = new System.Drawing.Size(184, 23);
			this.btnNodeCacheExample.TabIndex = 37;
			this.btnNodeCacheExample.Text = "NodeCacheExample";
			this.btnNodeCacheExample.Click += new System.EventHandler(this.btnNodeCacheExample_Click);
			// 
			// btnCompositeExample
			// 
			this.btnCompositeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnCompositeExample.Location = new System.Drawing.Point(8, 248);
			this.btnCompositeExample.Name = "btnCompositeExample";
			this.btnCompositeExample.Size = new System.Drawing.Size(184, 23);
			this.btnCompositeExample.TabIndex = 38;
			this.btnCompositeExample.Text = "CompositeExample";
			this.btnCompositeExample.Click += new System.EventHandler(this.btnCompositeExample_Click);
			// 
			// btnControlExample
			// 
			this.btnControlExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnControlExample.Location = new System.Drawing.Point(8, 272);
			this.btnControlExample.Name = "btnControlExample";
			this.btnControlExample.Size = new System.Drawing.Size(184, 23);
			this.btnControlExample.TabIndex = 39;
			this.btnControlExample.Text = "ControlExample";
			this.btnControlExample.Click += new System.EventHandler(this.btnControlExample_Click);
			// 
			// btnHierarchyZoomExample
			// 
			this.btnHierarchyZoomExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnHierarchyZoomExample.Location = new System.Drawing.Point(8, 464);
			this.btnHierarchyZoomExample.Name = "btnHierarchyZoomExample";
			this.btnHierarchyZoomExample.Size = new System.Drawing.Size(184, 23);
			this.btnHierarchyZoomExample.TabIndex = 40;
			this.btnHierarchyZoomExample.Text = "HierarchyZoomExample";
			this.btnHierarchyZoomExample.Click += new System.EventHandler(this.btnHierarchyZoomExample_Click);
			// 
			// btnBirdsEyeExample
			// 
			this.btnBirdsEyeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnBirdsEyeExample.Location = new System.Drawing.Point(8, 128);
			this.btnBirdsEyeExample.Name = "btnBirdsEyeExample";
			this.btnBirdsEyeExample.Size = new System.Drawing.Size(184, 23);
			this.btnBirdsEyeExample.TabIndex = 41;
			this.btnBirdsEyeExample.Text = "BirdsEyeExample";
			this.btnBirdsEyeExample.Click += new System.EventHandler(this.btnBirdsEyeExample_Click);
			// 
			// btnChartLabelExample
			// 
			this.btnChartLabelExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnChartLabelExample.Location = new System.Drawing.Point(8, 200);
			this.btnChartLabelExample.Name = "btnChartLabelExample";
			this.btnChartLabelExample.Size = new System.Drawing.Size(184, 23);
			this.btnChartLabelExample.TabIndex = 42;
			this.btnChartLabelExample.Text = "ChartLabelExample";
			this.btnChartLabelExample.Click += new System.EventHandler(this.btnChartLabelExample_Click);
			// 
			// btnNodeLinkExample
			// 
			this.btnNodeLinkExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnNodeLinkExample.Location = new System.Drawing.Point(8, 656);
			this.btnNodeLinkExample.Name = "btnNodeLinkExample";
			this.btnNodeLinkExample.Size = new System.Drawing.Size(184, 23);
			this.btnNodeLinkExample.TabIndex = 43;
			this.btnNodeLinkExample.Text = "NodeLinkExample";
			this.btnNodeLinkExample.Click += new System.EventHandler(this.btnNodeLinkExample_Click);
			// 
			// PiccoloFeatures
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(200, 509);
			this.Controls.Add(this.btnNodeLinkExample);
			this.Controls.Add(this.btnChartLabelExample);
			this.Controls.Add(this.btnBirdsEyeExample);
			this.Controls.Add(this.btnHierarchyZoomExample);
			this.Controls.Add(this.btnControlExample);
			this.Controls.Add(this.btnCompositeExample);
			this.Controls.Add(this.btnNodeCacheExample);
			this.Controls.Add(this.btnThreeDimRectExample);
			this.Controls.Add(this.btnFullScreenNodeExample);
			this.Controls.Add(this.btnToolTipExample);
			this.Controls.Add(this.btnStickyHandleExample);
			this.Controls.Add(this.btnSelectionExample);
			this.Controls.Add(this.btnScrollingExample);
			this.Controls.Add(this.btnPulseExample);
			this.Controls.Add(this.btnPositionPathActivity);
			this.Controls.Add(this.btnGroupExample);
			this.Controls.Add(this.btnGridExample);
			this.Controls.Add(this.btnCenterExample);
			this.Controls.Add(this.btnAngleNodeExample);
			this.Controls.Add(this.showFullBounds);
			this.Controls.Add(this.showRegionManagment);
			this.Controls.Add(this.printFrameRates);
			this.Controls.Add(this.btnWaitForActivitiesExample);
			this.Controls.Add(this.btnTwoCanvasExample);
			this.Controls.Add(this.btnStickyExample);
			this.Controls.Add(this.btnPositionExample);
			this.Controls.Add(this.btnPathExample);
			this.Controls.Add(this.btnPanToExample);
			this.Controls.Add(this.btnNodeExample);
			this.Controls.Add(this.btnNodeEventExample);
			this.Controls.Add(this.btnNavigationExample);
			this.Controls.Add(this.btnLensExample);
			this.Controls.Add(this.btnLayoutExample);
			this.Controls.Add(this.btnHelloWorldExample);
			this.Controls.Add(this.btnHandleExample);
			this.Controls.Add(this.btnKeyEventFocusExample);
			this.Controls.Add(this.btnClipExample);
			this.Controls.Add(this.btnEventHandlerExample);
			this.Controls.Add(this.btnDynamicExample);
			this.Controls.Add(this.btnSquiggleExample);
			this.Controls.Add(this.btnCameraExample);
			this.Controls.Add(this.btnActivityExample);
			this.Name = "PiccoloFeatures";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ExampleRunner";
			this.ResumeLayout(false);

		}
		#endregion

		private void showFullBounds_CheckedChanged(object sender, System.EventArgs e) {
			PDebug.DebugFullBounds = showFullBounds.Checked;
		}

		private void showRegionManagment_CheckedChanged(object sender, System.EventArgs e) {
			PDebug.DebugRegionManagement = showRegionManagment.Checked;		
		}

		private void printFrameRates_CheckedChanged(object sender, System.EventArgs e) {
			PDebug.DebugPrintFrameRate = printFrameRates.Checked;
		}

		private void btnActivityExample_Click(object sender, System.EventArgs e) {
			(new ActivityExample()).Show();
		}

		private void btnAngleNodeExample_Click(object sender, System.EventArgs e) {
			(new AngleNodeExample()).Show();
		}

		private void btnCameraExample_Click(object sender, System.EventArgs e) {
			(new CameraExample()).Show();
		}

		private void btnCenterExample_Click(object sender, System.EventArgs e) {
			(new CenterExample()).Show();
		}

		private void btnClipExample_Click(object sender, System.EventArgs e) {
			(new ClipExample()).Show();
		}

		private void btnDynamicExample_Click(object sender, System.EventArgs e) {
			(new DynamicExample()).Show();
		}

		private void btnEventHandlerExample_Click(object sender, System.EventArgs e) {
			(new EventHandlerExample()).Show();
		}

		private void btnGridExample_Click(object sender, System.EventArgs e) {
			(new GridExample()).Show();
		}

		private void btnHandleExample_Click(object sender, System.EventArgs e) {
			(new HandleExample()).Show();
		}

		private void btnHelloWorldExample_Click(object sender, System.EventArgs e) {
			(new HelloWorldExample()).Show();
		}

		private void btnKeyEventFocusExample_Click(object sender, System.EventArgs e) {
			(new KeyEventFocusExample()).Show();
		}

		private void btnLayoutExample_Click(object sender, System.EventArgs e) {
			(new LayoutExample()).Show();
		}

		private void btnLensExample_Click(object sender, System.EventArgs e) {
			(new LensExample()).Show();
		}

		private void btnNavigationExample_Click(object sender, System.EventArgs e) {
			(new NavigationExample()).Show();
		}

		private void btnNodeEventExample_Click(object sender, System.EventArgs e) {
			(new NodeEventExample()).Show();
		}

		private void btnNodeExample_Click(object sender, System.EventArgs e) {
			(new NodeExample()).Show();
		}

		private void btnPanToExample_Click(object sender, System.EventArgs e) {
			(new PanToExample()).Show();
		}

		private void btnPathExample_Click(object sender, System.EventArgs e) {
			(new PathExample()).Show();
		}

		private void btnPositionExample_Click(object sender, System.EventArgs e) {
			(new PositionExample()).Show();
		}

		private void btnPositionPathActivity_Click(object sender, System.EventArgs e) {
			(new PositionPathActivityExample()).Show();
		}

		private void btnPulseExample_Click(object sender, System.EventArgs e) {
			(new PulseExample()).Show();
		}

		private void btnSquiggleExample_Click(object sender, System.EventArgs e) {
			(new SquiggleExample()).Show();
		}

		private void btnStickyExample_Click(object sender, System.EventArgs e) {
			(new StickyExample()).Show();
		}

		private void btnStickyHandleExample_Click(object sender, System.EventArgs e) {
			(new StickyHandleLayerExample()).Show();
		}

		private void btnWaitForActivitiesExample_Click(object sender, System.EventArgs e) {
			(new WaitForActivitiesExample()).Show();
		}

		private void btnTwoCanvasExample_Click(object sender, System.EventArgs e) {
			(new TwoCanvasExample()).Show();
		}

		private void btnToolTipExample_Click(object sender, System.EventArgs e) {
			(new TooltipExample()).Show();
		}

		private void btnSelectionExample_Click(object sender, System.EventArgs e) {
			(new SelectionExample()).Show();
		}

		private void btnGroupExample_Click(object sender, System.EventArgs e) {
			(new GroupExample()).Show();
		}

		private void btnScrollingExample_Click(object sender, System.EventArgs e) {
			(new ScrollingExample()).Show();
		}

		private void btnFullScreenNodeExample_Click(object sender, System.EventArgs e) {
			(new FullScreenExample()).Show();
		}

		private void btnThreeDimRectExample_Click(object sender, System.EventArgs e) {
			(new ThreeDimRectExample()).Show();
		}

		private void btnNodeCacheExample_Click(object sender, System.EventArgs e) {
			(new NodeCacheExample()).Show();
		}

		private void btnCompositeExample_Click(object sender, System.EventArgs e) {
			(new CompositeExample()).Show();
		}

		private void btnControlExample_Click(object sender, System.EventArgs e) {
			(new ControlExample()).Show();
		}

		private void btnHierarchyZoomExample_Click(object sender, System.EventArgs e) {
			(new HierarchyZoomExample()).Show();
		}

		private void btnBirdsEyeExample_Click(object sender, System.EventArgs e) {
			(new BirdsEyeViewExample()).Show();
		}

		private void btnChartLabelExample_Click(object sender, System.EventArgs e) {
			(new ChartLabelExample()).Show();
		}

		private void btnNodeLinkExample_Click(object sender, System.EventArgs e) {
			(new NodeLinkExample()).Show();
		}
	}
}
