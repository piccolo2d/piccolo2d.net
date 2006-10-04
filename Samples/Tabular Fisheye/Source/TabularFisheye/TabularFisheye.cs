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
using System.Data;
using System.Windows.Forms;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.TabularFisheye {
	/// <summary>
	/// Implements a fisheye calendar control, which lets the user click on a day
	/// to give that day the fisheye focus.
	/// <para>
	/// The implementation consists of a CalendarNode that holds a collection of
	/// DayNodes.  A simple event handler is attached to the CalendarNode to get
	/// MouseUp events.
	/// </para>
	/// </summary>
	public class TabularFisheye : PCanvas {
		#region Fields
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private CalendarNode calendar;
		#endregion

		#region Constructors
		public TabularFisheye() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			calendar = new CalendarNode();
			Layer.AddChild(calendar);

			this.PanEventHandler = null;
			this.ZoomEventHandler = null;

			// Since we don't zoom, we will use grid-fitted text
			// for the highest possible quality.
			this.GridFitText = true;
		}
		#endregion

		#region Resize
		protected override void OnResize(EventArgs e) {
			base.OnResize (e);
			calendar.SetBounds(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
			calendar.LayoutChildren(false);
		}
		#endregion

		#region Nodes
		class CalendarNode : PNode {
			public static int DEFAULT_NUM_DAYS = 7;
			public static int DEFAULT_NUM_WEEKS = 12;
			public static int TEXT_X_OFFSET = 1;
			public static int TEXT_Y_OFFSET = 1;
			public static int DEFAULT_ANIMATION_MILLIS = 250;
			public static float FOCUS_SIZE_PERCENT = 0.65f;
			public static Font DEFAULT_FONT = new Font("Arial", 7);
	
			int numDays = DEFAULT_NUM_DAYS;
			int numWeeks = DEFAULT_NUM_WEEKS;
			int daysExpanded = 0;
			int weeksExpanded = 0;
		
			public CalendarNode() {		
				for (int week = 0; week < numWeeks; week++) {
					for (int day = 0; day < numDays; day++) {
						AddChild(new DayNode(week, day));
					}
				}
			}

			public override void OnMouseUp(PInputEventArgs e) {
				DayNode pickedDay = (DayNode) e.PickedNode;
				if (pickedDay.HasWidthFocus && pickedDay.HasHeightFocus) {
					SetFocusDay(null, true);
				} else {
					SetFocusDay(pickedDay, true);
				}
			}

			public DayNode GetDay(int week, int day) {
				return (DayNode) GetChild((week * numDays) + day);
			}

			public void SetFocusDay(DayNode focusDay, bool animate) {
				for (int i = 0; i < ChildrenCount; i++) {
					DayNode each = (DayNode) GetChild(i);
					each.HasWidthFocus = false;
					each.HasHeightFocus = false;
				}

				if (focusDay == null) {
					daysExpanded = 0;
					weeksExpanded = 0;
				} else {
					focusDay.HasWidthFocus = true;					
					daysExpanded = 1;
					weeksExpanded = 1;		

					for (int i = 0; i < numDays; i++) {
						GetDay(focusDay.Week, i).HasHeightFocus = true;
					}			
		
					for (int i = 0; i < numWeeks; i++) {
						GetDay(i, focusDay.Day).HasWidthFocus = true;
					}			
				}
						
				LayoutChildren(animate);
			}

			public void LayoutChildren(bool animate) {
				float focusWidth = 0;
				float focusHeight = 0;
			
				if (daysExpanded != 0 && weeksExpanded != 0) {
					focusWidth = (Width * FOCUS_SIZE_PERCENT) / daysExpanded;
					focusHeight = (Height * FOCUS_SIZE_PERCENT) / weeksExpanded;
				}
			
				float collapsedWidth = (Width - (focusWidth * daysExpanded)) / (numDays - daysExpanded);
				float collapsedHeight = (Height - (focusHeight * weeksExpanded)) / (numWeeks - weeksExpanded);

				float xOffset = 0;
				float yOffset = 0;
				float rowHeight = 0;
				DayNode each = null;

				for (int week = 0; week < numWeeks; week++) {
					for (int day = 0; day < numDays; day++) {
						each = GetDay(week, day);
						float width = collapsedWidth;
						float height = collapsedHeight;

						if (each.HasWidthFocus) width = focusWidth;
						if (each.HasHeightFocus) height = focusHeight;
					
						if (animate) {
							each.AnimateToBounds(xOffset, yOffset, width, height, DEFAULT_ANIMATION_MILLIS).StepInterval = 0;
						} else {
							each.SetBounds(xOffset, yOffset, width, height);
						}
					
						xOffset += width;
						rowHeight = height;
					}
					xOffset = 0;
					yOffset += rowHeight;
				}
			}
		}

		class DayNode : PNode {
			bool hasWidthFocus;
			bool hasHeightFocus;
			ArrayList lines;
			int week;
			int day;
			String dayOfMonthString;
		
			public int Week {
				get {
					return week;
				}
			}

			public int Day {
				get {
					return day;
				}
			}

			public bool HasHeightFocus {
				get {
					return hasHeightFocus;
				}
				set {
					hasHeightFocus = value;
				}
			}

			public bool HasWidthFocus {
				get {
					return hasWidthFocus;
				}
				set {
					hasWidthFocus = value;
				}
			}

			public DayNode(int week, int day) {	
				lines = new ArrayList();
				lines.Add("7:00 AM Walk the dog.");
				lines.Add("9:30 AM Meet John for Breakfast.");
				lines.Add("12:00 PM Lunch with Peter.");
				lines.Add("3:00 PM Research Demo.");
				lines.Add("6:00 PM Pickup Sarah from gymnastics.");
				lines.Add("7:00 PM Pickup Tommy from karate.");
				this.week = week;
				this.day = day;
				this.dayOfMonthString = ((week * 7) + day) + "";
				Brush = Brushes.Black;
			}
		
			protected override void Paint(PPaintContext paintContext) {
				Graphics g = paintContext.Graphics;
				g.DrawRectangle(Pens.Black, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

				float y = (float) Y + CalendarNode.TEXT_Y_OFFSET;
				g.DrawString(dayOfMonthString, CalendarNode.DEFAULT_FONT, Brush, (float) X + CalendarNode.TEXT_X_OFFSET, y);

				if (hasWidthFocus && hasHeightFocus) {
					paintContext.PushClip(new Region(Bounds));
					for (int i = 0; i < lines.Count; i++) {
						y += 10;
						g.DrawString((String)lines[i], CalendarNode.DEFAULT_FONT, Brush, X + CalendarNode.TEXT_X_OFFSET, y);
					}
					paintContext.PopClip();
				}
			}
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}