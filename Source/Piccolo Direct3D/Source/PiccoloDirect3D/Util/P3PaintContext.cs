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
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.PiccoloDirect3D.Util {
	/// <summary>
	/// A simple paint context for Direct3D rendering.
	/// </summary>
	public class P3PaintContext : PPaintContext {
		#region Fields
		Device device;
		static Tesselator tesselator;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3PaintContext.
		/// </summary>
		/// <param name="g">The GDI+ graphics context to associate with this paint context.</param>
		/// <param name="device">The device to associate with this paint context</param>
		/// <param name="canvas">The canvas that the paint context will render on.</param>
		public P3PaintContext(Graphics g, Device device, P3Canvas canvas) : base(g, canvas) {
			this.device = device;
		}

		/// <summary>
		/// Overridden.  Pushes the initial transform and clips onto their respective stacks.
		/// </summary>
		protected override void InitializeStacks() {
			localClipStack.Push((RectangleF)canvas.ClientRectangle);
			clipStack.Push(localClipStack.Peek());
			transformStack.Push(new PMatrix());
		}
		#endregion

		#region Context Attributes
		/// <summary>
		/// Gets the device associated with this paint context.
		/// </summary>
		/// <value>The device associated with this paint context.</value>
		public virtual Device Device {
			get { return device; }
		}

		/// <summary>
		/// 
		/// </summary>
		public static Tesselator Tesselator {
			get {
				if (tesselator == null) {
					tesselator = new Tesselator();
				}
				return tesselator; 
			}
		}

		/// <summary>
		/// Gets the current transform.
		/// </summary>
		/// <value>The current transform.</value>
		public PMatrix Transform {
			get {
				return (PMatrix)transformStack.Peek();
			}
		}

		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.Scale">PPaintContext.Scale</see>.
		/// </summary>
		public override float Scale {
			get {
				PTS[0] = new PointF(0, 0);
				PTS[1] = new PointF(1, 0);
				TEMP_MATRIX.Reset();
				TEMP_MATRIX.Multiply((Transform));
				TEMP_MATRIX.TransformPoints(PTS);
				return PUtil.DistanceBetweenPoints(PTS[0], PTS[1]);
			}
		}

		#endregion

		#region Context Attribute Stacks
		/// <summary>
		/// Overridden.  PiccoloDirect3D only supports rectangular clip regions.  So, if a
		/// non-rectangular region is pushed, the bounds of that region will be used instead.
		/// </summary>
		/// <param name="aClip">The clip to push.</param>
		public override void PushClip(Region aClip) {
			PushClip(aClip.GetBounds(graphics));
		}

		/// <summary>
		/// Pushes the current clip onto the clip stack and sets clip of the device to
		/// the intersection of the current clip and the given clip.
		/// </summary>
		/// <remarks>
		/// The intersection of the current local clip and the new local clip is also pushed onto
		/// the local clip stack.
		/// </remarks>
		/// <param name="aClip">The clip to push.</param>
		public virtual void PushClip(RectangleF aClip) {
			RectangleF newLocalClip = RectangleF.Intersect(LocalClip, aClip);
			localClipStack.Push(newLocalClip);

			RectangleF currentClip = (RectangleF)clipStack.Peek();
			currentClip.Intersect(Transform.Transform(aClip));
			clipStack.Push(currentClip);
			DeviceClip = currentClip;
		}

		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.PopClip">PPaintContext.PopClip</see>.
		/// </summary>
		public override void PopClip() {
			clipStack.Pop();
			DeviceClip = (RectangleF)clipStack.Peek();
			localClipStack.Pop();
		}

		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.PushMatrix">PPaintContext.PushTransform</see>.
		/// </summary>
		public override void PushMatrix(PMatrix matrix) {
			if (matrix == null) return;
			RectangleF newLocalClip = matrix.InverseTransform(LocalClip);
			localClipStack.Push(newLocalClip);
			PMatrix newMatrix = (PMatrix)Transform.Clone();
			newMatrix.Multiply(matrix);
			transformStack.Push(newMatrix);

			SetWorldMatrix(newMatrix);
		}

		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.PopMatrix">PPaintContext.PopTransform</see>.
		/// </summary>
		public override void PopMatrix() {
			transformStack.Pop();
			localClipStack.Pop();

			SetWorldMatrix(Transform);
		}

		/// <summary>
		/// Sets the clip of the device.
		/// </summary>
		protected virtual RectangleF DeviceClip {
			set {
				Vector3 a = new Vector3(value.X, value.Y, 0);
				Vector3 b = new Vector3(value.Right, value.Y, 0);
				Vector3 c = new Vector3(value.X, value.Bottom, 0);
				Vector3 d = new Vector3(value.Right, value.Bottom, 0);
				Vector3 eyePart = new Vector3(canvas.ClientSize.Width/2, canvas.ClientSize.Height/2, -5.0f);

				device.ClipPlanes[0].Plane = Plane.FromPoints(a, b, eyePart);
				device.ClipPlanes[1].Plane = Plane.FromPoints(b, d, eyePart);
				device.ClipPlanes[2].Plane = Plane.FromPoints(d, c, eyePart);
				device.ClipPlanes[3].Plane = Plane.FromPoints(c, a, eyePart);

				device.ClipPlanes.EnableAll();
			}
		}

		/// <summary>
		/// Sets the world transform of the device.
		/// </summary>
		/// <param name="matrix"></param>
		protected virtual void SetWorldMatrix(PMatrix matrix) {
			float[] piccoloMatrixElements = matrix.Elements;
			Matrix worldMatrix = new Matrix();
			worldMatrix.M11 = piccoloMatrixElements[0];
			worldMatrix.M12 = piccoloMatrixElements[1];
			worldMatrix.M21 = piccoloMatrixElements[2];
			worldMatrix.M22 = piccoloMatrixElements[3];
			worldMatrix.M41 = piccoloMatrixElements[4];
			worldMatrix.M42 = piccoloMatrixElements[5];
			worldMatrix.M33 = 1;
			worldMatrix.M44 = 1;
			device.Transform.World = worldMatrix;
		}
		#endregion

		#region Render Quality
		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.OnHighRenderQuality">
		/// PPaintContext.OnHighRenderQuality</see>.
		/// </summary>
		protected override void OnHighRenderQuality(Graphics graphics, PCanvas canvas) {
			Device device = ((P3Canvas)Canvas).Device;
			device.RenderState.AntiAliasedLineEnable = true;
			base.OnHighRenderQuality (graphics, canvas);
		}

		/// <summary>
		/// Overridden.  See <see cref="PPaintContext.OnLowRenderQuality">
		/// PPaintContext.OnLowRenderQuality</see>.
		/// </summary>
		protected override void OnLowRenderQuality(Graphics graphics, PCanvas canvas) {
			Device device = ((P3Canvas)Canvas).Device;
			device.RenderState.AntiAliasedLineEnable = false;
			base.OnLowRenderQuality (graphics, canvas);
		}
		#endregion

		#region Debugging
		// Region management is disabled for PiccoloDirect3D since presenting the entire backbuffer
		// is more efficient.
		/// <summary>
		/// Overridden.  This method will not paint anything.
		/// </summary>
		/// <remarks>
		/// Region management is disabled for PiccoloDirect3D since presenting the entire backbuffer
		/// is more efficient.  Thus, this method is left empty.
		/// </remarks>
		/// <param name="brush">The brush to use for painting the clipping region.</param>
		public override void PaintClipRegion(Brush brush) {
		}
		#endregion
	}
}