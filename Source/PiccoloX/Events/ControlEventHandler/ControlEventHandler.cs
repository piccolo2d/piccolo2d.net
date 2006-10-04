/* 
 * Copyright (c) 2003-2004, University of Maryland
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

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Activities;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloX.Nodes;

namespace UMD.HCIL.PiccoloX.Events {
	/// <summary>
	/// <b>PControlEventHandler</b> implements focus based navigation, strictly for
	/// <see cref="PControl"/> nodes.
	/// </summary>
	/// <remarks>
	/// A PControl is only editable when it is at it's natural size (the actual bounds of
	/// the underlying control).  The natural size of a PControl is the same as the bounds
	/// of the node at 100% scale.
	/// <para>
	/// When the a mouse button is pressed over a <see cref="PControl"/>, the camera's
	/// view is zoomed so that the node is at 100% scale (the natural size of the control),
	/// and panned so that the node is visible.  Finally, the PControl is switched to
	/// editing mode.
	/// </para>
	/// <para>
	/// If a the mouse button is subsequently pressed over another node, the previously
	/// edited control is switched to non-editing mode.
	/// </para>
	/// Note, this event handler only allows one PControl to be editable at a time.  That is,
	/// it only allows one focus node.  This is a logical approach and provides a simple way
	/// to turn off previously edited nodes.  Otherwise, we would have to listen to all
	/// transformations that could possibly affect a given node.  However, this will only work
	/// in the general case, where a user interaction changes the focus.  If the application
	/// itself modifies a transform that affects a PControl, it is the application's
	/// responsibility to turn off editing if the node is no longer at it's natural size.
	/// </remarks>
	public class PControlEventHandlerb : PNavigationEventHandler {
		private PControl lastEditedControl;

		public PControlEventHandlerb() {
			ZoomToDuration = 300;
		}

		// Ignore key events.
		public override bool DoesAcceptEvent(PInputEventArgs e) {
			return !(e.IsKeyEvent || e.IsKeyPressEvent);
		}

		public override void OnMouseDown(object sender, PInputEventArgs e) {
			if (lastEditedControl != null) lastEditedControl.Editing = false;

			PNode node = e.PickedNode;
			if (node is PControl) {
				e.Handled = true;
				base.OnMouseDown (sender, e);
				lastEditedControl = (PControl)node;
			}
		}

		public override PActivity DirectCameraViewToFocus(PCamera aCamera, PNode aFocusNode, int duration) {
			PActivity animateViewToMatrix = base.DirectCameraViewToFocus (aCamera, aFocusNode, duration);

			PControl controlNode = (PControl)aFocusNode;
			aCamera.Root.WaitForActivities();
			controlNode.CurrentCamera = aCamera;
			controlNode.Editing = true;

			return animateViewToMatrix;
		}
	}
}