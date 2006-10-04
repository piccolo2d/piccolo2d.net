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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using UMD.HCIL.SlideViewer;
using UMD.HCIL.PiccoloX;

namespace PresentationTool {
	/// <summary>
	/// A Simple Piccolo Presentation Tool.  It shows all the images in a specified directory
	/// in a style suitable for making a presentation.  There is a navigation bar that shows
	/// thumbnails of the slides and uses a simple "fisheye" view of the thumbnails to enable
	/// navigation, even when there are more slides than can easily fit.
	/// </summary>
	public class PresentationTool : PForm {
		#region Fields
		// Application variables
		protected string currentSlideFolder = ".";

		// Auto-generated Designer variables
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuOpen;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.MenuItem menuView;
		private System.Windows.Forms.MenuItem menuFullScreen;
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialization
		public PresentationTool() 
			: base(false, new SlideViewer()) {
			InitializeComponent();
		}
		#endregion

		#region Menu Handlers
		private void menuOpen_Click(object sender, System.EventArgs e) {
			FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
			folderBrowser.Description = "Select a folder that contains images to be displayed";
			folderBrowser.SelectedPath = currentSlideFolder;
			DialogResult dialogResult = folderBrowser.ShowDialog();
			if (dialogResult == DialogResult.OK) {
				String folder = folderBrowser.SelectedPath;
				((SlideViewer)Canvas).LoadSlides(folder);
				currentSlideFolder = folder;
			}		
		}

		private void menuExit_Click(object sender, System.EventArgs e) {
			Application.Exit();
		}

		private void menuFullScreen_Click(object sender, System.EventArgs e) {
			FullScreenMode = true;
		}

		public override bool FullScreenMode {
			set {
				if (value) {
					this.Menu = null;
				} else {
					this.Menu = mainMenu1;
				}
				base.FullScreenMode = value;
			}
		}
		#endregion

		#region Standard Windows stuff
		#region Dispose
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
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuOpen = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.menuView = new System.Windows.Forms.MenuItem();
			this.menuFullScreen = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFile,
																					  this.menuView});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuOpen,
																					 this.menuExit});
			this.menuFile.Text = "File";
			// 
			// menuOpen
			// 
			this.menuOpen.Index = 0;
			this.menuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuOpen.Text = "Open";
			this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
			// 
			// menuExit
			// 
			this.menuExit.Index = 1;
			this.menuExit.Text = "Exit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// menuView
			// 
			this.menuView.Index = 1;
			this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFullScreen});
			this.menuView.Text = "View";
			// 
			// menuFullScreen
			// 
			this.menuFullScreen.Index = 0;
			this.menuFullScreen.Shortcut = System.Windows.Forms.Shortcut.F11;
			this.menuFullScreen.Text = "Full Screen";
			this.menuFullScreen.Click += new System.EventHandler(this.menuFullScreen_Click);
			// 
			// PresentationTool
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(800, 600);
//			this.Layered = true;
			this.Location = new System.Drawing.Point(0, 0);
			this.Menu = this.mainMenu1;
			this.Name = "PresentationTool";
			this.Opacity = 0.99;
			this.Text = "Presentation Tool";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new PresentationTool());
		}
		#endregion
	}
}