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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

using UMD.HCIL.PiccoloDirect3D.Nodes;

namespace UMD.HCIL.PiccoloDirect3D.Util {
	/// <summary>
	/// <b>Tesselator</b> is a wrapper around the GLU tesselation methods used to tesselate
	/// arbitrary flattened paths.
	/// </summary>
	public class Tesselator {
		#region Fields
		/// <summary>
		/// The tesselation object.
		/// </summary>
		protected IntPtr tess;

		/// <summary>
		/// The visitor object notified during a tesselation.
		/// </summary>
		protected TesselationVisitor visitor;

		/// <summary>
		/// Invoked to indicate the start of a (triangle) primitive.
		/// </summary>
		protected P3Util.BeginCallback beginCallback;

		/// <summary>
		/// Invoked between the begin and end callbacks to define the vertices of the triangles
		/// created by the tessellation process.
		/// </summary>
		protected P3Util.VertexCallback vertexCallback;

		/// <summary>
		/// Invoked to indicate the end of a primitive.
		/// </summary>
		protected P3Util.EndCallback endCallback;

		/// <summary>
		/// Invoked when an error is encountered.
		/// </summary>
		protected P3Util.ErrorCallback errorCallback;

		/// <summary>
		/// Invoked to create a new vertex when the tessellation detects an intersection, or
		/// wishes to merge features.
		/// </summary>
		protected P3Util.CombineCallback combineCallback;

		/// <summary>
		/// A collection of GCHandles used to pass objects the the GLU tesselation methods.
		/// </summary>
		protected ArrayList handles = new ArrayList();
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new Tesselator object.
		/// </summary>
		public Tesselator() {
			tess = P3Util.GluNewTess();

			vertexCallback = new P3Util.VertexCallback(TessVertex);
			beginCallback = new P3Util.BeginCallback(TessBegin);
			endCallback = new P3Util.EndCallback(TessEnd);
			errorCallback = new P3Util.ErrorCallback(TessError);
			combineCallback = new P3Util.CombineCallback(TessCombine);

			P3Util.GluTessVertexCallBack(tess, P3Util.GlCallbackName.Vertex, vertexCallback);
			P3Util.GluTessBeginCallBack(tess, P3Util.GlCallbackName.Begin, beginCallback);
			P3Util.GluTessEndCallBack(tess, P3Util.GlCallbackName.End, endCallback);
			P3Util.GluTessErrorCallBack(tess, P3Util.GlCallbackName.Error, errorCallback);
			P3Util.GluTessCombineCallBack(tess, P3Util.GlCallbackName.Combine, combineCallback);

			P3Util.GluTessNormal(tess, 0f, 0f, -1f);
		}
		#endregion

		#region Tesselation
		/// <summary>
		/// Tesselates the specified path, notifying the
		/// <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor">TesselationVisitor</see>
		/// as each new triangle primitive is added.
		/// </summary>
		/// <param name="path">The path to tesselate.</param>
		/// <param name="visitor">The tesselation visitor to notify.</param>
		public virtual void Tesselate(GraphicsPath path, TesselationVisitor visitor) {	
			this.visitor = visitor;

			switch (path.FillMode) {
				case FillMode.Alternate: //even/odd
					P3Util.SetWindingRule(tess, P3Util.GlTessWinding.WindingOdd);
					break;
				case FillMode.Winding: //nonzero
					P3Util.SetWindingRule(tess, P3Util.GlTessWinding.WindingNonzero);
					break;
			}

			P3Util.GluTessBeginPolygon(tess, IntPtr.Zero);

			bool isClosed;
			GraphicsPathIterator pi = new GraphicsPathIterator(path);
			PointF[] points = path.PathPoints;

			while(pi.NextSubpath(path, out isClosed)!=0) {
				byte type;
				int start, end;
				while(pi.NextPathType(out type, out start, out end)!=0) {
					PathPointType ppType = (PathPointType)type;

					P3Util.GluTessBeginContour(tess);

					for (int i = start; i <= end; i++) {
						PointF point = points[i];
						double[] coords = new double[3];
						coords[0] = point.X;
						coords[1] = point.Y;
						coords[2] = 0;
						GCHandle handle = GCHandle.Alloc(coords, GCHandleType.Pinned);
						P3Util.GluTessVertex(tess, coords, (IntPtr)handle);
						handles.Add(handle);
					}

					P3Util.GluTessEndContour(tess);
				}
			}

			P3Util.GluTessEndPolygon(tess);	

			ClearHandles();
		}

		/// <summary>
		/// Frees the list of GCHandles collected during tesselation.
		/// </summary>
		/// <remarks>
		/// GCHandles are used so that objects passed to the GLU tesselation methods
		/// won't get garbage collected during the tesselation process.  These handles
		/// must be explicitly freed after the tesselation is completed.
		/// </remarks>
		protected virtual void ClearHandles() {
			foreach (GCHandle handle in handles) {
				handle.Free();
			}
			handles.Clear();
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Disposes the tesselation object.
		/// </summary>
		public virtual void Dispose() {
			P3Util.GluDeleteTess(tess);
		}
		#endregion

		#region Tesselation Callbacks
		/// <summary>
		/// Notifies the <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor">
		/// TesselationVisitor</see> of the start of a (triangle) primitive.
		/// </summary>
		/// <param name="which">The type of the primitive.</param>
		public virtual void TessBegin(P3Util.GlPrimitiveType which) {
			visitor.TessBegin(which);
		}

		/// <summary>
		/// Notifies the <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor">
		/// TesselationVisitor</see> of new vertices created by the tessellation process.
		/// </summary>
		/// <param name="data">A pointer to the vertex data.</param>
		public virtual void TessVertex(IntPtr data) {
			GCHandle handle = ((GCHandle)data);
			double[] values = (double[])handle.Target;
			visitor.TessVertex(values);
		}

		/// <summary>
		/// Notifies the <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor">
		/// TesselationVisitor</see> of the end of a primitive.
		/// </summary>
		public virtual void TessEnd() {
			visitor.TessEnd();
		}

		/// <summary>
		/// Notifies the <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor">
		/// TesselationVisitor</see> when an error is encountered.
		/// </summary>
		/// <param name="error">The type of error that occured.</param>
		public virtual void TessError(P3Util.GlTessError error) {
			visitor.TessError(error);
		}

		/// <summary>
		/// Creates a new vertex when the tessellation detects an intersection, or wishes
		/// to merge features.
		/// </summary>
		/// <param name="coords">A pointer to the new vertex.</param>
		/// <param name="data">A pointer to the four vertices that surround the new vertex.</param>
		/// <param name="weight">
		/// A pointer to an array of values that indicate how much influence the surrounding
		/// vertices have on the newly created vertex.
		/// </param>
		/// <param name="dataOut">
		/// A pointer to the representation of the new vertex that should be used in subsequent calls
		/// to <see cref="UMD.HCIL.PiccoloDirect3D.Util.Tesselator.TessVertex">TessVertex</see>.
		/// </param>
		/// <remarks>
		/// This method will result in subsequent call(s) to
		/// <see cref="UMD.HCIL.PiccoloDirect3D.Util.Tesselator.TessVertex">TessVertex</see>
		/// where the data parameter will be the dataOut value set here.
		/// </remarks>
		public virtual void TessCombine(IntPtr coords, IntPtr data, IntPtr weight, out IntPtr dataOut) {
			unsafe {
				double *unsafeCoords = (double*)coords;
				double[] safeCoords = new double[3];
				safeCoords[0] = unsafeCoords[0];  // x
				safeCoords[1] = unsafeCoords[1];  // y
				safeCoords[2] = unsafeCoords[2];  // z

				GCHandle handle = GCHandle.Alloc(safeCoords, GCHandleType.Pinned);
				dataOut = (IntPtr)handle;
				handles.Add(handle);
			}
		}
		#endregion
	}
}