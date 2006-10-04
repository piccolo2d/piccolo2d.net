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
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloDirect3D.Util;

using FillMode = System.Drawing.Drawing2D.FillMode;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace UMD.HCIL.PiccoloDirect3D.Nodes {
	/// <summary>
	/// <b>P3Path</b> is a Direct3D wrapper around a
	/// <see cref="System.Drawing.Drawing2D.GraphicsPath">
	/// System.Drawing.Drawing2D.GraphicsPath</see>.
	/// </summary>
	/// <remarks>
	/// The SetBounds method works by scaling the path to fit into the specified bounds.
	/// This normally works well, but if the specified base bounds get too small then it is
	/// impossible to expand the path shape again since all its numbers have tended to zero,
	/// so application code may need to take this into consideration. 
	/// <para>
	/// One option that applications have is to call <c>StartResizeBounds</c> before starting
	/// an interaction that may make the bounds very small, and calling <c>EndResizeBounds</c>
	/// when this interaction is finished. When this is done PPath will use a copy of the
	/// original path to do the resizing so the numbers in the path wont loose resolution.
	/// </para>
	/// <para>
	/// This class also provides methods for constructing common shapes using a GraphicsPath.
	/// </para>
	/// <para>
	/// <b>Performance Note</b>:  Checking for intersections between some paths and rectangles
	/// can be very slow.  This is due to the way .NET implements the IsVisible method.
	/// The problem generally occurs in extreme cases, when the path consists of numerous
	/// lines joined at very steep angles, which exhausts the intersection algorithm.
	/// One simple workaround is to break the figure up into several PPath nodes.  Also,
	/// remember to set the Brush to null if you do not want to a fill a path.  Otherwise,
	/// the path will be filled with a white brush, and picking will be more expensive.
	/// See <see cref="P3Path.Intersects">P3Path.Intersects</see>.
	/// </para>
	/// </remarks>
	public class P3Path : P3Node, TesselationVisitor {
		#region Fields
		private static GraphicsPath TEMP_PATH = new GraphicsPath();

		private static Region TEMP_REGION = new Region();
		private static PMatrix TEMP_MATRIX = new PMatrix();
		private static Pen DEFAULT_PEN = Pens.Black;
		private const FillMode DEFAULT_FILLMODE = FillMode.Alternate;
		private const float DEFAULT_TOLERANCE = .25f;

		private GraphicsPath path;
		private GraphicsPath resizePath;
		private PathRenderMode renderMode = PathRenderMode.Cached;
		private Pen pen;
		private bool updatingBoundsFromPath;
		private ArrayList renderList = new ArrayList();
		private ArrayList renderListTypes = new ArrayList();
		private float tolerance = DEFAULT_TOLERANCE;
		private float flatness = DEFAULT_TOLERANCE;
		private int penStartIndex;
		private int flattenedStartIndex;
		private int currentTesselationColor;
		private PPath.PathPickMode pickMode = PPath.PathPickMode.Fast;
		#endregion

		#region Structs
		/// <summary>
		/// Stores details about a cached primitive type.
		/// </summary>
		private struct PrimitiveTypeInfo {
			private int start;
			private int end;
			private int color;
			private PrimitiveType type;

			/// <summary>
			/// Constructs a new PrimitiveTypeInfo.
			/// </summary>
			/// <param name="start">The start index of the primitive.</param>
			/// <param name="end">The end index of the primitive.</param>
			/// <param name="color">The color of the primitive.</param>
			/// <param name="type">The type of the primitive.</param>
			public PrimitiveTypeInfo(int start, int end, int color, PrimitiveType type) {
				this.start = start;
				this.end = end;
				this.type = type;
				this.color = color;
			}
			/// <summary>
			/// The start index of the primitive.
			/// </summary>
			public int Start {
				get { return start; }
				set { start = value; }
			}
			/// <summary>
			/// The end index of the primitive.
			/// </summary>
			public int End {
				get { return end; }
				set { end = value; }
			}
			/// <summary>
			/// The color of the primitive.
			/// </summary>
			public int Color {
				get { return color; }
				set { color = value; }
			}
			/// <summary>
			/// The type of the primitive.
			/// </summary>
			public PrimitiveType Type {
				get { return type; }
				set { type = value; }
			}
		}
		#endregion

		#region Enums
		/// <summary>
		/// Represents the types of rendering modes for a P3Path object.
		/// </summary>
		public enum PathRenderMode {
			/// <summary>
			/// Faster Rendering.  Paths are tesselated and cached only when the
			/// underlying path data is modified.
			/// </summary>
			Cached,

			/// <summary>
			/// Slower and more accurate rendering.  Paths are tesselated on every
			/// render.
			/// </summary>
			Accurate
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3Path with an empty path.
		/// </summary>
		public P3Path() {
			pen = DEFAULT_PEN;
			path = new GraphicsPath();
		}

		/// <summary>
		/// Constructs a new P3Path wrapping the given
		/// <see cref="System.Drawing.Drawing2D.GraphicsPath">
		/// System.Drawing.Drawing2D.GraphicsPath</see>.
		/// </summary>
		/// <param name="path">The path to wrap.</param>
		public P3Path(GraphicsPath path) {
			pen = DEFAULT_PEN;
			this.path = (GraphicsPath)path.Clone();
			UpdateBoundsFromPath();
		}

		/// <summary>
		/// Constructs a new P3Path with the given points and point types.
		/// </summary>
		/// <param name="pts">The points in the path.</param>
		/// <param name="types">The types of the points in the path.</param>
		public P3Path(PointF[] pts, byte[] types)
			: this(pts, types, DEFAULT_PEN) {
		}

		/// <summary>
		/// Constructs a new P3Path with the given points, point types and pen.
		/// </summary>
		/// <param name="pts">The points in the path.</param>
		/// <param name="types">The types of the points in the path.</param>
		/// <param name="pen">The pen to use when rendering this node.</param>
		public P3Path(PointF[] pts, byte[] types, Pen pen)
			: this(pts, types, DEFAULT_FILLMODE, pen) {
		}

		/// <summary>
		/// Constructs a new P3Path with the given points, point types and fill mode.
		/// </summary>
		/// <param name="pts">The points in the path.</param>
		/// <param name="types">The types of the points in the path.</param>
		/// <param name="fillMode">The fill mode to use when rendering this node.</param>
		public P3Path(PointF[] pts, byte[] types, FillMode fillMode)
			: this(pts, types, fillMode, DEFAULT_PEN) {
		}

		/// <summary>
		/// Constructs a new P3Path with the given points, point types, fill mode and pen.
		/// </summary>
		/// <param name="pts">The points in the path.</param>
		/// <param name="types">The types of the points in the path.</param>
		/// <param name="fillMode">The fill mode to use when rendering this node.</param>
		/// <param name="pen">The pen to use when rendering this node.</param>
		public P3Path(PointF[] pts, byte[] types, FillMode fillMode, Pen pen) {
			path = new GraphicsPath(pts, types, fillMode);
			this.pen = pen;
			UpdateBoundsFromPath();
		}
		#endregion

		#region Basic Shapes
		//****************************************************************
		// Basic Shapes - Methods used for creating common paths.
		//****************************************************************

		/// <summary>
		/// Creates a new P3Path with the shape of a line specified by the given coordinates.
		/// </summary>
		/// <param name="x1">The x-coordinate of the start-point of the line.</param>
		/// <param name="y1">The y-coordinate of the start-point of the line.</param>
		/// <param name="x2">The x-coordinate of the end-point of the line.</param>
		/// <param name="y2">The y-coordinate of the end-point of the line.</param>
		/// <returns>The new PPath node.</returns>
		public static P3Path CreateLine(float x1, float y1, float x2, float y2) {
			P3Path result = new P3Path();
			result.AddLine(x1, y1, x2, y2);
			return result;
		}

		/// <summary>
		/// Creates a new P3Path with the shape of the rectangle specified by the given dimensions.
		/// </summary>
		/// <param name="x">The x-coordinate of the top left corner of the rectangle.</param>
		/// <param name="y">The y-coordinate of the top left corner of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		/// <returns>The new PPath node.</returns>
		public static P3Path CreateRectangle(float x, float y, float width, float height) {
			P3Path result = new P3Path();
			result.AddRectangle(x, y, width, height);
			result.Brush = Brushes.White;
			return result;
		}

		/// <summary>
		/// Creates a new P3Path with the shape of the ellipse specified by the given dimensions.
		/// </summary>
		/// <param name="x">
		/// The x-coordinate of the top left corner of the bounding box of the ellipse.
		/// </param>
		/// <param name="y">
		/// The y-coordinate of the top left corner of the bounding box of the ellipse.
		/// </param>
		/// <param name="width">The width of the ellipse.</param>
		/// <param name="height">The height of the ellipse.</param>
		/// <returns>The new PPath node.</returns>
		public static P3Path CreateEllipse(float x, float y, float width, float height) {
			P3Path result = new P3Path();
			result.AddEllipse(x, y, width, height);
			result.Brush = Brushes.White;
			return result;
		}

		/// <summary>
		/// Creates a new P3Path with the shape of the polygon specified by the given dimension.
		/// </summary>
		/// <param name="points">The points in the desired polygon.</param>
		/// <returns>The new PPath node.</returns>
		public static P3Path CreatePolygon(PointF[] points) {
			P3Path result = new P3Path();
			result.AddPolygon(points);
			result.Brush = Brushes.White;
			return result;
		}
		#endregion

		#region Pen
		/// <summary>
		/// Gets or sets the pen used when rendering this node.
		/// </summary>
		/// <value>The pen used when rendering this node.</value>
		public virtual Pen Pen {
			get { return pen; }
			set {
				pen = value;
				UpdateBoundsFromPath();
				InvalidatePaint();
				InvalidateVertices();
			}
		}
		#endregion

		#region Picking Mode
		/// <summary>
		/// Gets or sets the mode used to pick this node.
		/// <seealso cref="PPath.PathPickMode">PathPickMode</seealso>
		/// </summary>
		/// <value>The mode used to pick this node.</value>
		public virtual PPath.PathPickMode PickMode {
			get { return pickMode; }
			set {
				this.pickMode = value;
			}
		}
		#endregion

		#region Render Mode
		/// <summary>
		/// Gets or sets the mode used to render this node.
		/// </summary>
		/// <value>The mode used to render this node.</value>
		public virtual PathRenderMode RenderMode {
			get { return renderMode; }
			set {
				renderMode = value;
				InvalidatePaint();
				InvalidateVertices();
			}
		}

		/// <summary>
		/// Gets or sets the tolerance used to flatten and tesselate paths.
		/// </summary>
		/// <value>The tolerance used to flatten and tesselate paths.</value>
		/// <remarks>
		/// The flatness will be equal to the tolerance divided by the current scale.
		/// The tolerance has a default value of .25f.
		/// </remarks>
		public virtual float Tolerance {
			get { return tolerance; }
			set {
				tolerance = value;
				InvalidatePaint();
				InvalidateVertices();
			}
		}
		#endregion

		#region Bounds
		//****************************************************************
		// Bounds - Methods for manipulating/updating the bounds of a
		// PPath.
		//****************************************************************

		/// <summary>
		/// Overridden.  See <see cref="PNode.StartResizeBounds">PNode.StartResizeBounds</see>.
		/// </summary>
		public override void StartResizeBounds() {
			resizePath = new GraphicsPath();
			resizePath.AddPath(path, false);
		}

		/// <summary>
		/// Overridden.  See <see cref="PNode.EndResizeBounds">PNode.EndResizeBounds</see>.
		/// </summary>
		public override void EndResizeBounds() {
			resizePath = null;
		}

		/// <summary>
		/// Overridden.  Set the bounds of this path.
		/// </summary>
		/// <param name="x">The new x-coordinate of the bounds/</param>
		/// <param name="y">The new y-coordinate of the bounds.</param>
		/// <param name="width">The new width of the bounds.</param>
		/// <param name="height">The new height of the bounds.</param>
		/// <returns>True if the bounds have changed; otherwise, false.</returns>
		/// <remarks>
		/// This works by scaling the path to fit into the specified bounds.  This normally
		/// works well, but if the specified base bounds get too small then it is impossible
		/// to expand the path shape again since all its numbers have tended to zero, so
		/// application code may need to take this into consideration.
		/// </remarks>
		protected override void InternalUpdateBounds(float x, float y, float width, float height) {
			if (updatingBoundsFromPath || path == null) {
				return;
			}

			if (resizePath != null) {
				path.Reset();
				path.AddPath(resizePath, false);
			}

			RectangleF pathBounds = path.GetBounds();

			if (pen != null && path.PointCount > 0) {
				try {
					TEMP_PATH.Reset();
					TEMP_PATH.AddPath(path, false);

					TEMP_PATH.Widen(pen);
					RectangleF penPathBounds = TEMP_PATH.GetBounds();

					float strokeOutset = Math.Max(penPathBounds.Width - pathBounds.Width, 
						penPathBounds.Height - pathBounds.Height);

					x += strokeOutset / 2;
					y += strokeOutset / 2;
					width -= strokeOutset;
					height -= strokeOutset;
				} catch (OutOfMemoryException) {
					// Catch the case where the path is a single point
				}
			}

			float scaleX = (width == 0 || pathBounds.Width == 0) ? 1 : width / pathBounds.Width;
			float scaleY = (height == 0 || pathBounds.Height == 0) ? 1 : height / pathBounds.Height;

			TEMP_MATRIX.Reset();
			TEMP_MATRIX.TranslateBy(x, y);
			TEMP_MATRIX.ScaleBy(scaleX, scaleY);
			TEMP_MATRIX.TranslateBy(-pathBounds.X, -pathBounds.Y);
		
			path.Transform(TEMP_MATRIX.MatrixReference);
		}

		/// <summary>
		/// Returns true if this path intersects the given rectangle.
		/// </summary>
		/// <remarks>
		/// This method first checks if the interior of the path intersects with the rectangle.
		/// If not, the method then checks if the path bounding the pen stroke intersects with
		/// the rectangle.  If either of these cases are true, this method returns true.
		/// <para>
		/// <b>Performance Note</b>:  For some paths, this method can be very slow.  This is due
		/// to the implementation of IsVisible.  The problem usually occurs when many lines are
		/// joined at very steep angles.  See the documentation above for workarounds.
		/// </para>
		/// </remarks>>
		/// <param name="bounds">The rectangle to check for intersection.</param>
		/// <returns>True if this path intersects the given rectangle; otherwise, false.</returns>
		public override bool Intersects(RectangleF bounds) {
			// Call intersects with the identity matrix.
			return Intersects(bounds, new PMatrix());
		}

		/// <summary>
		/// Overridden.  Performs picking in canvas coordinates if <see cref="PickMode">PickMode</see>
		/// is false.
		/// </summary>
		/// <remarks>
		/// Due to the implementation of the GraphicsPath object, picking in canvas coordinates
		/// is more accurate, but will introduce a significant performance hit.
		/// </remarks>
		protected override bool PickAfterChildren(PPickPath pickPath) {
			if (pickMode == PPath.PathPickMode.Fast) {
				return base.PickAfterChildren(pickPath);
			} else {
				return Intersects(pickPath.PickBounds,  pickPath.GetPathTransformTo(this));
			}
		}

		/// <summary>
		/// Returns true if this path intersects the given rectangle.
		/// </summary>
		/// <remarks>
		/// This method first checks if the interior of the path intersects with the rectangle.
		/// If not, the method then checks if the path bounding the pen stroke intersects with
		/// the rectangle.  If either of these cases are true, this method returns true.
		/// <para>
		/// <b>Performance Note</b>:  For some paths, this method can be very slow.  This is due
		/// to the implementation of IsVisible.  The problem usually occurs when many lines are
		/// joined at very steep angles.  See the documentation above for workarounds.
		/// </para>
		/// </remarks>
		/// <param name="bounds">The rectangle to check for intersection.</param>
		/// <param name="matrix">
		/// A matrix object that specifies a transform to apply to the path and bounds before
		/// checking for an intersection.
		/// </param>
		/// <returns>True if this path intersects the given rectangle; otherwise, false.</returns>
		public virtual bool Intersects(RectangleF bounds, PMatrix matrix) {
			if (base.Intersects(bounds)) {
				// Transform the bounds.
				if (!matrix.IsIdentity) bounds = matrix.Transform(bounds);

				// Set the temp region to the transformed path.
				SetTempRegion(path, matrix, false);

				if (Brush != null && TEMP_REGION.IsVisible(bounds)) {
					return true;
				} else if (pen != null) {
					// Set the temp region to the transformed, widened path.
					SetTempRegion(path, matrix, true);
					return TEMP_REGION.IsVisible(bounds);
				}
			}
		
			return false;
		}

		/// <summary>
		/// Sets the temp region to the transformed path, widening the path if
		/// requested to do so.
		/// </summary>
		private void SetTempRegion(GraphicsPath path, PMatrix matrix, bool widen) {
			TEMP_PATH.Reset();

			if (path.PointCount > 0) {
				TEMP_PATH.AddPath(path, false);

				if (widen) {
					TEMP_PATH.Widen(pen, matrix.MatrixReference);
				} else {
					TEMP_PATH.Transform(matrix.MatrixReference);
				}
			}

			TEMP_REGION.MakeInfinite();
			TEMP_REGION.Intersect(TEMP_PATH);
		}

		/// <summary>
		/// This method is called to update the bounds whenever the underlying path changes.
		/// </summary>
		public virtual void UpdateBoundsFromPath() {
			updatingBoundsFromPath = true;
			if (path == null || path.PointCount == 0) {
				ResetBounds();
			} else {
				try {
					TEMP_PATH.Reset();
					TEMP_PATH.AddPath(path, false);
					if (pen != null && TEMP_PATH.PointCount > 0) TEMP_PATH.Widen(pen);
					RectangleF b = TEMP_PATH.GetBounds();
					SetBounds(b.X, b.Y, b.Width, b.Height);
				} catch (OutOfMemoryException) {
					//Catch the case where the path is a single point
				}
			}
			updatingBoundsFromPath = false;
		}
		#endregion

		#region Path Support
		//****************************************************************
		// Path Support - Methods for manipulating the underlying path.
		// See System.Drawing.Drawing2D.GraphicsPath documentation for
		// more information on using these methods.
		//****************************************************************

		/// <summary>
		/// Gets a reference to the underlying path object.
		/// </summary>
		/// <value>The underlying path object.</value>
		public virtual GraphicsPath PathReference {
			get { return path; }
		}

		
		/// <summary>
		/// See <see cref="GraphicsPath.FillMode">GraphicsPath.FillMode</see>.
		/// </summary>
		public virtual FillMode FillMode {
			get { return path.FillMode; }
			set {
				path.FillMode = value;
				InvalidatePaint();
				InvalidateVertices();
			}
		}

		/// <summary>
		/// See <see cref="GraphicsPath.PathData">GraphicsPath.PathData</see>.
		/// </summary>
		public virtual PathData PathData {
			get { return path.PathData; }
		}

		/// <summary>
		/// See <see cref="GraphicsPath.PointCount">GraphicsPath.PointCount</see>.
		/// </summary>
		public virtual int PointCount {
			get { return path.PointCount; }
		}

		
		/// <summary>
		/// See <see cref="GraphicsPath.AddArc">GraphicsPath.AddArc</see>.
		/// </summary>
		public virtual void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle) {
			path.AddArc(x, y, width, height, startAngle, sweepAngle);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddBezier">GraphicsPath.AddBezier</see>.
		/// </summary>
		public virtual void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
			path.AddBezier(x1, y1, x2, y2, x3, y3, x4, y4);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddClosedCurve">GraphicsPath.AddClosedCurve</see>.
		/// </summary>
		public virtual void AddClosedCurve(PointF[] points) {
			path.AddClosedCurve(points);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddCurve">GraphicsPath.AddCurve</see>.
		/// </summary>
		public virtual void AddCurve(PointF[] points) {
			path.AddCurve(points);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddEllipse">GraphicsPath.AddEllipse</see>.
		/// </summary>
		public virtual void AddEllipse(float x, float y, float width, float height) {
			path.AddEllipse(x, y, width, height);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddLine">GraphicsPath.AddLine</see>.
		/// </summary>
		public virtual void AddLine(float x1, float y1, float x2, float y2) {
			path.AddLine(x1, y1, x2, y2);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddPath">GraphicsPath.AddPath</see>.
		/// </summary>
		public virtual void AddPath(GraphicsPath path, bool connect) {
			this.path.AddPath(path, connect);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddPolygon">GraphicsPath.AddPolygon</see>.
		/// </summary>
		public virtual void AddPolygon(PointF[] points) {
			path.AddPolygon(points);
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.AddRectangle">GraphicsPath.AddRectangle</see>.
		/// </summary>
		public virtual void AddRectangle(float x, float y, float width, float height) {
			path.AddRectangle(new RectangleF(x, y, width, height));
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.CloseFigure">GraphicsPath.CloseFigure</see>.
		/// </summary>
		public virtual void CloseFigure() {
			path.CloseFigure();
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.CloseAllFigures">GraphicsPath.CloseAllFigures</see>.
		/// </summary>
		public virtual void CloseAllFigures() {
			path.CloseAllFigures();
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}

		/// <summary>
		/// See <see cref="GraphicsPath.Reset">GraphicsPath.Reset</see>.
		/// </summary>
		public virtual void Reset() {
			path.Reset();
			UpdateBoundsFromPath();
			InvalidatePaint();
			InvalidateVertices();
		}
		#endregion

		#region Vertex Buffer
		/// <summary>
		/// Overridden.  See <see cref="P3Node.ValidateVertices">P3Node.ValidateVertices</see>.
		/// </summary>
		public override void ValidateVertices(Device device) {
			// Make sure a new vertex buffer is always created.
			if (vb != null) {
				vb.Dispose();
				vb = null;
			}
			base.ValidateVertices(device);
		}

		/// <summary>
		/// Overridden.  See <see cref="P3Node.CreateVertexBuffer">P3Node.CreateVertexBuffer</see>.
		/// </summary>
		protected override VertexBuffer CreateVertexBuffer(Device device) {
			if (Brush != null) {
				CacheFill();
			}

			// Set the start index of the pen.
			this.penStartIndex = renderListTypes.Count;

			if (pen != null) {
				CacheStroke();
			}

			return new VertexBuffer(typeof(CustomVertex.PositionColored), renderList.Count, device, Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Managed);
		}

		/// <summary>
		/// Overridden.  See <see cref="P3Node.FillVertexBuffer">P3Node.FillVertexBuffer</see>.
		/// </summary>
		protected override void FillVertexBuffer(VertexBuffer vb) {
			GraphicsStream stm = vb.Lock(0, 0, 0);
			CustomVertex.PositionColored[] colVerts = new CustomVertex.PositionColored[renderList.Count];

			PointF[] points = (PointF[])renderList.ToArray(typeof(PointF));
			foreach (PrimitiveTypeInfo pti in renderListTypes) {
				P3Util.CreateColoredVertexList(colVerts, points, pti.Start, pti.Start, pti.End-pti.Start+1, pti.Color);
			}

			stm.Write(colVerts);
			vb.Unlock();
		}
		#endregion

		#region Cache Path
		/// <summary>
		/// Tesselate and cache the interior of the path.
		/// </summary>
		protected virtual void CacheFill() {
			// Tesselate fill.
			TEMP_PATH.Reset();
			TEMP_PATH.AddPath(path, false);
			TEMP_PATH.FillMode = FillMode;
			Tesselate(TEMP_PATH, (Brush as SolidBrush).Color.ToArgb(), false, true);
		}

		/// <summary>
		/// Tesselate and cache the contours of the path.
		/// </summary>
		protected virtual void CacheStroke() {
			// Tesselate stroke.
			TEMP_PATH.Reset();
			TEMP_PATH.AddPath(path, false);
			Tesselate(TEMP_PATH, pen.Color.ToArgb(), true, false);

			// Set the start index of the flattened path
			this.flattenedStartIndex = renderListTypes.Count;

			// Add flattened path.
			TEMP_PATH.Reset();
			TEMP_PATH.AddPath(path, false);
			TEMP_PATH.Flatten(new Matrix(), this.flatness);
			CacheFlatPath(TEMP_PATH);
		}

		/// <summary>
		/// Cache a flattened path.
		/// </summary>
		/// <param name="flattenedPath">The flattened path to cache.</param>
		protected virtual void CacheFlatPath(GraphicsPath flattenedPath) {
			bool isClosed;
			GraphicsPathIterator pi = new GraphicsPathIterator(flattenedPath);
			PointF[] points = flattenedPath.PathPoints;

			while(pi.NextSubpath(flattenedPath, out isClosed)!=0) {
				byte type;
				int start, end;
				int oldCount = renderList.Count;

				while(pi.NextPathType(out type, out start, out end)!=0) {
					for (int i = start; i <= end; i++) {
						renderList.Add(points[i]);
					}

					if (isClosed){
						renderList.Add(points[start]);
					}

					renderListTypes.Add(new PrimitiveTypeInfo(oldCount, renderList.Count-1, pen.Color.ToArgb(), PrimitiveType.LineStrip));
				}
			}
		}
		#endregion

		#region Tesselation
		/// <summary>
		/// Tesselate the path.
		/// </summary>
		/// <param name="path">The path to tesselate.</param>
		/// <param name="tesselationColor">The color to use for the vertices.</param>
		/// <param name="widen">Indicates whether or not to widen the path.</param>
		/// <param name="clearCache">Indicates whether or not to clear the cache.</param>
		protected virtual void Tesselate(GraphicsPath path, int tesselationColor, bool widen, bool clearCache) {
			currentTesselationColor = tesselationColor;

			if (clearCache) {
				renderList.Clear();
				renderListTypes.Clear();
			}

			if (widen) {
				path.Widen(pen, new Matrix(), flatness);
			} else {
				path.Flatten(new Matrix(), flatness);
			}

			P3PaintContext.Tesselator.Tesselate(path, this);
		}

		/// <summary>
		/// Implements <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor.TessBegin">
		/// TesselationVisitor.TessBegin</see>.
		/// </summary>
		public virtual void TessBegin(P3Util.GlPrimitiveType which) {
			PrimitiveType type = P3Util.GetD3DPrimitiveType(which);
			this.renderListTypes.Add(new PrimitiveTypeInfo(renderList.Count, renderList.Count-1, currentTesselationColor, type));
		}

		/// <summary>
		/// Implements <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor.TessVertex">
		/// TesselationVisitor.TessVertex</see>.
		/// </summary>
		public virtual void TessVertex(double[] vertex) {
			renderList.Add(new PointF((float)vertex[0], (float)vertex[1]));
			PrimitiveTypeInfo pti = (PrimitiveTypeInfo)renderListTypes[renderListTypes.Count-1];
			pti.End++;
			renderListTypes[renderListTypes.Count-1] = pti;
		}

		/// <summary>
		/// Implements <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor.TessEnd">
		/// TesselationVisitor.TessEnd</see>.
		/// </summary>
		public virtual void TessEnd() {
		}

		/// <summary>
		/// Implements <see cref="UMD.HCIL.PiccoloDirect3D.Util.TesselationVisitor.TessError">
		/// TesselationVisitor.TessError</see>.
		/// </summary>
		public virtual void TessError(P3Util.GlTessError error) {
			System.Console.Error.WriteLine("Glu tesselation error: " + error);
		}
		#endregion

		#region Painting
		/// <summary>
		/// Overridden.  See <see cref="P3Node.Paint">P3Node.Paint</see>.
		/// </summary>
		protected override void Paint(PPaintContext paintContext) {
			if (path.PointCount > 0) {
				P3PaintContext p3PaintContext = paintContext as P3PaintContext;

				// Set the flatness
				flatness = tolerance/paintContext.Scale;

				// Fill the path
				if (Brush != null) {
					PaintFill(p3PaintContext);
				}

				// Draw the stroke
				if (pen != null) {
					PaintStroke(p3PaintContext);
				}
			}
		}

		/// <summary>
		/// Paints the fill using the brush associated with this node.
		/// </summary>
		/// <param name="paintContext">The paint context to use for painting this node.</param>
		protected virtual void PaintFill(P3PaintContext paintContext) {
			Device device = paintContext.Device;

			if (renderMode == PathRenderMode.Cached) {
				Render(device, GetValidVertexBuffer(device), 0, penStartIndex-1);
			} else {
				// Reset the path to flatten
				TEMP_PATH.Reset();
				TEMP_PATH.AddPath(path, false);
				TEMP_PATH.FillMode = FillMode;

				Tesselate(TEMP_PATH, (Brush as SolidBrush).Color.ToArgb(), false, true);
				Render(device, renderList);
			}
		}

		/// <summary>
		/// Paints the stroke using the pen associated with this path.
		/// </summary>
		/// <param name="paintContext">The paint context to use for painting this node.</param>
		protected virtual void PaintStroke(P3PaintContext paintContext) {
			Device device = paintContext.Device;

			// Reset the path to flatten
			TEMP_PATH.Reset();
			TEMP_PATH.AddPath(path, false);

			float absLineWidth = pen.Width * paintContext.Scale;
			bool renderFlattened = (pen.Width == 0 || absLineWidth < 3);

			if (renderMode == PathRenderMode.Cached) {
				if (renderFlattened) {
					Render(device, GetValidVertexBuffer(device), flattenedStartIndex, renderListTypes.Count-1);
				} else {
					Render(device, GetValidVertexBuffer(device), penStartIndex, flattenedStartIndex-1);
				}
			} else {
				if (renderFlattened) {
					TEMP_PATH.Flatten(new System.Drawing.Drawing2D.Matrix(), flatness);	
					Render(device, pen.Color.ToArgb(), TEMP_PATH);
				} else {
					Tesselate(TEMP_PATH, pen.Color.ToArgb(), true, true);
					Render(device, renderList);
				}
			}
		}

		/// <summary>
		/// Renders the flattened path to the device using the specified color. 
		/// </summary>
		/// <param name="device">The device to use for rendering the path.</param>
		/// <param name="color">The color to use for the vertices.</param>
		/// <param name="flattenedPath">The path to render.</param>
		protected virtual void Render(Device device, int color, GraphicsPath flattenedPath) {
			PointF[] points = flattenedPath.PathPoints;
			device.VertexFormat = CustomVertex.PositionColored.Format;

			bool isClosed;
			GraphicsPathIterator pi = new GraphicsPathIterator(flattenedPath);

			while(pi.NextSubpath(flattenedPath, out isClosed)!=0) {
				byte type;
				int start, end;

				while(pi.NextPathType(out type, out start, out end)!=0) {
					int numDistinctPoints = end-start+1;
					int totNumPoints = numDistinctPoints;
					if (isClosed) totNumPoints++;

					CustomVertex.PositionColored[] colVerts = new CustomVertex.PositionColored[totNumPoints];
					P3Util.CreateColoredVertexList(colVerts, points, start, 0, numDistinctPoints, color);

					if (isClosed){
						colVerts[numDistinctPoints] = colVerts[0];
					}

					device.DrawUserPrimitives(PrimitiveType.LineStrip, totNumPoints-1, colVerts);
				}
			}
		}

		/// <summary>
		/// Renders the list of primitives to the device.
		/// </summary>
		/// <param name="device">The device to use for rendering the primitives.</param>
		/// <param name="renderList">The list of primitives to render.</param>
		protected virtual void Render(Device device, ArrayList renderList) {
			PointF[] points = (PointF[])renderList.ToArray(typeof(PointF));
			device.VertexFormat = CustomVertex.PositionColored.Format;

			for (int i = 0; i < renderListTypes.Count; i++) {
				PrimitiveTypeInfo pti = (PrimitiveTypeInfo)renderListTypes[i];
				int numVerts = (pti.End - pti.Start + 1);

				CustomVertex.PositionColored[] colVerts = new CustomVertex.PositionColored[numVerts];
				P3Util.CreateColoredVertexList(colVerts, points, pti.Start, 0, numVerts, pti.Color);

				switch (pti.Type) {
					case PrimitiveType.TriangleFan:
						device.DrawUserPrimitives(PrimitiveType.TriangleFan, colVerts.Length - 2, colVerts);
						break;
					case PrimitiveType.TriangleList:
						device.DrawUserPrimitives(PrimitiveType.TriangleList, colVerts.Length / 3, colVerts);
						break;
					case PrimitiveType.TriangleStrip:
						device.DrawUserPrimitives(PrimitiveType.TriangleStrip, colVerts.Length - 2, colVerts);
						break;
				}
			}
		}

		/// <summary>
		/// Renders a portion the vertex buffer to the device.
		/// </summary>
		/// <param name="device">The device to use for rendering the primitives.</param>
		/// <param name="vb">The vertex buffer to render.</param>
		/// <param name="start">The start index of the vertices to render.</param>
		/// <param name="end">The end index of the vertices to render.</param>
		protected virtual void Render(Device device, VertexBuffer vb, int start, int end) {
			device.SetStreamSource(0, vb, 0);
			device.VertexFormat = CustomVertex.PositionColored.Format;

			for (int i = start; i <= end; i++) {
				PrimitiveTypeInfo pti = (PrimitiveTypeInfo)renderListTypes[i];
				int numVerts = (pti.End - pti.Start + 1);

				switch (pti.Type) {
					case PrimitiveType.LineStrip:
						device.DrawPrimitives(PrimitiveType.LineStrip, pti.Start, numVerts-1);
						break;
					case PrimitiveType.TriangleFan:
						device.DrawPrimitives(PrimitiveType.TriangleFan, pti.Start, numVerts-2);
						break;
					case PrimitiveType.TriangleList:
						device.DrawPrimitives(PrimitiveType.TriangleList, pti.Start, numVerts/3);
						break;
					case PrimitiveType.TriangleStrip:
						device.DrawPrimitives(PrimitiveType.TriangleStrip, pti.Start, numVerts-2);
						break;
				}
			}
		}
		#endregion
	}
}