using System;
using System.Drawing;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Activities;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloX.Nodes;

namespace UMD.HCIL.PiccoloX.Events {
	/// <summary>
	/// Summary description for PControlEventHandler2.
	/// </summary>
	public class PControlEventHandler2 : PBasicInputEventHandler {
		private PActivity navigationActivity;
		private PControl lastEditedControl;

		public PControlEventHandler2() {
			//
			// TODO: Add constructor logic here
			//
		}

		public override void OnMouseDown(object sender, PInputEventArgs e) {
			base.OnMouseDown (sender, e);

			if (lastEditedControl != null) lastEditedControl.Editing = false;

			PNode node = e.PickedNode;
			if (node is PControl) {
				e.Handled = true;
				DirectCameraViewToFocus(e.Camera, node, e.Path, 300);
				lastEditedControl = (PControl)node;
			}
		}

		public virtual PActivity DirectCameraViewToFocus(PCamera aCamera, PNode aFocusNode, PPickPath path, int duration) {
			PMatrix originalViewMatrix = aCamera.ViewMatrix;

			// Scale the canvas to include
			SizeF s = new SizeF(1, 0);
			s = aFocusNode.GlobalToLocal(s);
		
			float scaleFactor = s.Width / aCamera.ViewScale;
			PointF scalePoint = PUtil.CenterOfRectangle(aFocusNode.GlobalFullBounds);
			if (scaleFactor != 1) {
				aCamera.ScaleViewBy(scaleFactor, scalePoint.X, scalePoint.Y);
			}
		
			// Pan the canvas to include the view bounds with minimal canvas
			// movement.
			aCamera.AnimateViewToPanToBounds(aFocusNode.GlobalFullBounds, 0);

			// Get rid of any white space. The canvas may be panned and
			// zoomed in to do this. But make sure not stay constrained by max
			// magnification.
			//FillViewWhiteSpace(aCamera);

			PMatrix resultingMatrix = aCamera.ViewMatrix;
			aCamera.ViewMatrix = originalViewMatrix;

			// Animate the canvas so that it ends up with the given
			// view transform.
			PActivity animateCameraViewActivity = AnimateCameraViewMatrixTo(aCamera, resultingMatrix, duration);

			PControl controlNode = (PControl)aFocusNode;
			aCamera.Root.WaitForActivities();

			controlNode.CurrentCanvas = path.TopCamera.Canvas;
			PointF pf = path.GetPathTransformTo(controlNode).Transform(new PointF(controlNode.X, controlNode.Y));
			controlNode.ControlLocation = new Point((int)pf.X, (int)pf.Y);

			controlNode.Editing = true;

			return animateCameraViewActivity;
		}

		protected virtual PActivity AnimateCameraViewMatrixTo(PCamera aCamera, PMatrix aMatrix, int duration) {
			bool wasOldAnimation = false;
		
			// first stop any old animations.
			if (navigationActivity != null) {
				navigationActivity.Terminate();
				wasOldAnimation = true;
			}
			
			if (duration == 0) {
				aCamera.ViewMatrix = aMatrix;
				return null;
			}

			PMatrix source = aCamera.ViewMatrixReference;

			if (!source.MatrixReference.Equals(aMatrix.MatrixReference)) {
				navigationActivity = aCamera.AnimateViewToMatrix(aMatrix, duration);
				((PTransformActivity)navigationActivity).SlowInSlowOut = !wasOldAnimation;
				return navigationActivity;			
			}
		
			return null;
		}
	}
}
