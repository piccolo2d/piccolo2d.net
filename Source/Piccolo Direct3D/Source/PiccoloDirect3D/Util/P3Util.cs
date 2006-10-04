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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo;

namespace UMD.HCIL.PiccoloDirect3D.Util {
	/// <summary>
	/// A utility class for PiccoloDirect3D.
	/// </summary>
	public class P3Util {
		#region NULL DEVICE
		private static Device NULL_DEVICE;
		private static Form NULL_DEVICE_FORM;

		/// <summary>
		/// Returns a device that is connected to an invisible window.
		/// </summary>
		/// <remarks>
		/// This is useful for creating resources, when you do not have access to
		/// a Device.  See <see cref="UMD.HCIL.PiccoloDirect3D.Nodes.P3Text">
		/// UMD.HCIL.PiccoloDirect3D.Nodes.P3Text</see> for an example.
		/// </remarks>
		public static Device NullDevice {
			get {
				if (NULL_DEVICE == null) {
					NULL_DEVICE_FORM = new Form();
					PresentParameters pm = new PresentParameters();
					pm.Windowed=true;
					pm.SwapEffect = SwapEffect.Discard;
					NULL_DEVICE = new Device(0, DeviceType.NullReference, NULL_DEVICE_FORM, CreateFlags.SoftwareVertexProcessing, pm);

					Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
				}
				return NULL_DEVICE;
			}
		}

		/// <summary>
		/// Disposes the null device, if it exists.
		/// </summary>
		/// <param name="sender">The source of the ApplicationExit event.</param>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected static void Application_ApplicationExit(object sender, EventArgs e) {
			NULL_DEVICE.Dispose();
		}
		#endregion

		#region Geometric
		/// <summary>
		/// Creates a textured rectangle from a rectangle structure.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="index">The start index in the destination vertices.</param>
		/// <param name="rect">The source rectangle structure.</param>
		public static void CreateTexturedRectangle(CustomVertex.PositionNormalTextured[] verts, int index, RectangleF rect) {
			verts[index].Position = new Vector3(rect.X, rect.Y, 0);
			verts[index].Normal = new Vector3(0, 0, -1);
			verts[index].Tu = 0;
			verts[index].Tv = 0;

			verts[index+1].Position = new Vector3(rect.X, rect.Y + rect.Height, 0);
			verts[index+1].Normal = new Vector3(0, 0, -1);
			verts[index+1].Tu = 0;
			verts[index+1].Tv = 1;

			verts[index+2].Position = new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0);
			verts[index+2].Normal = new Vector3(0, 0, -1);
			verts[index+2].Tu = 1;
			verts[index+2].Tv = 1;

			verts[index+3].Position = new Vector3(rect.X, rect.Y, 0);
			verts[index+3].Normal = new Vector3(0, 0, -1);
			verts[index+3].Tu = 0;
			verts[index+3].Tv = 0;

			verts[index+4].Position = new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0);
			verts[index+4].Normal = new Vector3(0, 0, -1);
			verts[index+4].Tu = 1;
			verts[index+4].Tv = 1;

			verts[index+5].Position = new Vector3(rect.X + rect.Width, rect.Y, 0);
			verts[index+5].Normal = new Vector3(0, 0, -1);
			verts[index+5].Tu = 1;
			verts[index+5].Tv = 0;
		}

		/// <summary>
		/// Creates a colored rectangle from a rectangle structure.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="index">The start index in the destination vertices.</param>
		/// <param name="rect">The source rectangle structure.</param>
		/// <param name="color">The color of the vertices.</param>
		public static void CreateColoredRectangle(CustomVertex.PositionColored[] verts, int index, RectangleF rect, int color) {
			CreateColoredRectangle(verts, index, rect.X, rect.Y, rect.Width, rect.Height, color);
		}

		/// <summary>
		/// Creates a colored rectangle from a rectangle structure.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="index">The start index in the destination vertices.</param>
		/// <param name="x">The x-coordinate of the source rectangle.</param>
		/// <param name="y">The y-coordinate of the source rectangle.</param>
		/// <param name="width">The width of the source rectangle.</param>
		/// <param name="height">The height of the source rectangle.</param>
		/// <param name="color">The color of the vertices.</param>
		public static void CreateColoredRectangle(CustomVertex.PositionColored[] verts, int index, float x, float y, float width, float height, int color) {
			verts[index+0].Position = new Vector3(x, y, 0);
			verts[index+1].Position = new Vector3(x, y + height, 0);
			verts[index+2].Position = new Vector3(x + width, y + height, 0);
			verts[index+3].Position = new Vector3(x, y, 0);
			verts[index+4].Position = new Vector3(x + width, y + height, 0);
			verts[index+5].Position = new Vector3(x + width, y, 0);

			for (int i=index; i<index+6; i++) {
				verts[i].Color = color;
			}
		}

		/// <summary>
		///Creates a rectangle line strip from a rectangle structure.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="index">The start index in the destination vertices.</param>
		/// <param name="rect">The source rectangle structure.</param>
		/// <param name="color">The color of the vertices.</param>
		public static void CreateColoredRectangleOutline(CustomVertex.PositionColored[] verts, int index, RectangleF rect, int color) {
			verts[index+0].Position = new Vector3(rect.X, rect.Y, 0);
			verts[index+1].Position = new Vector3(rect.X, rect.Y + rect.Height, 0);
			verts[index+2].Position = new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0);
			verts[index+3].Position = new Vector3(rect.X + rect.Width, rect.Y, 0);
			verts[index+4].Position = new Vector3(rect.X, rect.Y, 0);

			for (int i=index; i<index+5; i++) {
				verts[i].Color = color;
			}
		}
		
		/// <summary>
		/// Creates a line from the given points.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="index">The start index in the destination vertices.</param>
		/// <param name="x1">The x-coordinate of the first point.</param>
		/// <param name="y1">The y-coordinate of the first point..</param>
		/// <param name="x2">The x-coordinate of the second point.</param>
		/// <param name="y2">The y-coordinate of the second point.</param>
		/// <param name="color">The color of the vertices.</param>
		static internal void CreateColoredLine(CustomVertex.PositionColored[] verts, int index, float x1, float y1, float x2, float y2, int color) {
			verts[index+0].Position = new Vector3(x1, y1, 0);
			verts[index+1].Position = new Vector3(x2, y2, 0);

			verts[index+0].Color = color;
			verts[index+1].Color = color;
		}


		/// <summary>
		/// Creates a colored vertex list from the points array.
		/// </summary>
		/// <param name="verts">The destination vertices.</param>
		/// <param name="points">The source points array.</param>
		/// <param name="srcIndex">The start index in the source array.</param>
		/// <param name="dstIndex">The start index in the destination vertices.</param>
		/// <param name="count">The number of vertices to copy.</param>
		/// <param name="color">The color of the vertices.</param>
		static internal void CreateColoredVertexList(CustomVertex.PositionColored[] verts, PointF[] points, int srcIndex, int dstIndex, int count, int color) {
			for (int i = 0; i < count; i++) {
				PointF point = points[i+srcIndex];
				verts[i+dstIndex].Position = new Vector3(point.X, point.Y, 0);
				verts[i+dstIndex].Color = color;
			}
		}
		#endregion

		#region Measure String
		/// <summary>
		/// Measures the rectangle dimensions of the specified text string when
		/// drawn with the specified <see cref="Microsoft.DirectX.Direct3D.Sprite">
		/// Microsoft.DirectX.Direct3D.Sprite</see> object.
		/// </summary>
		/// <param name="font">
		/// The Direct3D font for which to measure the specified text.
		/// </param>
		/// <param name="sprite">
		/// A <see cref="Microsoft.DirectX.Direct3D.Sprite">Sprite</see> object that
		/// contains the string.
		/// </param>
		/// <param name="text">String to measure.</param>
		/// <param name="width">Maximum width of the string.</param>
		/// <param name="color">Color of the text.</param>
		/// <returns>
		/// A Rectangle structure that contains the rectangle, in logical coordinates, that
		/// encompasses the formatted text string.
		/// </returns>
		public static Rectangle MeasureString(Direct3D.Font font, Sprite sprite, String text, int width, Color color) {
			return font.MeasureString(sprite, WrapString(sprite, font, text, width, color), DrawTextFormat.None, color);
		}

		/// <summary>
		/// Wraps the specified text string, given the specified width and
		/// <see cref="Microsoft.DirectX.Direct3D.Sprite">
		/// Microsoft.DirectX.Direct3D.Sprite</see> object.
		/// </summary>
		/// <param name="sprite">
		/// A <see cref="Microsoft.DirectX.Direct3D.Sprite">Sprite</see> object that
		/// contains the string.
		/// </param>
		/// <param name="font">
		/// The Direct3D font for which to measure the specified text.
		/// </param>
		/// <param name="text">String to wrap.</param>
		/// <param name="width">Maximum width of the string.</param>
		/// <param name="color">Color of the text.</param>
		/// <returns>
		/// A string that has been wrapped with the specified width.
		/// </returns>
		public static String WrapString(Sprite sprite, Direct3D.Font font, String text, int width, Color color) {
			char[] sepChar = new char[1];
			sepChar[0] = ' ';
			String[] tokens = text.Split(sepChar);

			String result = "";
			String currLine = "";

			bool nonEmptyToken = false;
			for (int i = 0; i < tokens.Length; i++) {
				String token = tokens[i];

				if (token != "") nonEmptyToken = true;

				if (i > 0 && token != "") {
					result = AddToken(font, sprite, width, result, ref currLine, " ", color);
				}

				if (token == "") token = " ";

				result = AddToken(font, sprite, width, result, ref currLine, token, color);
			}
			if (!nonEmptyToken) result = AddToken(font, sprite, width, result, ref currLine, " ", color);


			return result;
		}

		// Adds a token to text result.
		private static String AddToken(Direct3D.Font font, Sprite sprite, int width, String text, ref String currLine, String token, Color color) {
			Rectangle r = font.MeasureString(sprite, currLine + token, DrawTextFormat.None, color);
			if (r.Width <= width || currLine == "") {
				text += token;
				currLine += token;
			} 
			else {
				if (token == " ") token = "";
				text += ("\n" + token);
				currLine = token;
			}
			return text;
		}

		/// <summary>
		/// Converts a <see cref="System.Drawing.StringAlignment">
		/// System.Drawing.StringAlignment</see> to the Direct3D equivalent.
		/// </summary>
		/// <param name="alignment">
		/// A <see cref="System.Drawing.StringAlignment">StringAlignment</see>.
		/// </param>
		/// <returns>
		/// A <see cref="Microsoft.DirectX.Direct3D.DrawTextFormat">
		/// Microsoft.DirectX.Direct3D.DrawTextFormat</see> that represents the
		/// specified <see cref="System.Drawing.StringAlignment">StringAlignment</see>.
		/// </returns>
		public static DrawTextFormat GetD3DAlignment(StringAlignment alignment) {
			DrawTextFormat D3DAlignment = DrawTextFormat.Left;
			switch (alignment) {
				case StringAlignment.Center:
					D3DAlignment = DrawTextFormat.Center;
					break;
				case StringAlignment.Far:
					D3DAlignment = DrawTextFormat.Right;
					break;
				case StringAlignment.Near:
					D3DAlignment = DrawTextFormat.Left;
					break;
			}
			return D3DAlignment;
		}
		#endregion

		#region Scene Graph
		/// <summary>
		/// Creates a basic PiccoloDirect3D scene graph.
		/// </summary>
		/// <returns>The main camera node in the new scene graph.</returns>
		/// <remarks>
		/// The scene graph will consist of  root node with two children, a layer and a
		/// camera.  Additionally, The camera will be set to view the layer.  Typically,
		/// you will want to add new nodes to the layer.
		/// </remarks>
		public static P3Camera CreateBasicScenegraph() {
			PRoot r = new PRoot();
			PLayer l = new PLayer();
			P3Camera c = new P3Camera();
		
			r.AddChild(c); 
			r.AddChild(l); 
			c.AddLayer(l);
		
			return c;
		}
		#endregion

		#region GLU Tesselation Constants
		/// <summary>
		/// Represents properties stored in a tesselation object.
		/// </summary>
		public enum GlTessProperty {

			/// <summary>
			/// Specifies which parts of the polygon are on the interior.
			/// </summary>
			WindingRule = 100140,

			/// <summary>
			/// Specifies whether to return the contours of a polygon or a tesselation.
			/// </summary>
			BoundaryOnly = 100141,

			/// <summary>
			/// Specifies a tolerance for merging features to reduce the size of the output.
			/// </summary>
			Tolerance = 100142
		}

		/// <summary>
		/// Used to determine which parts of the polygon are on the interior.
		/// </summary>
		public enum GlTessWinding {

			/// <summary>
			/// Classifies a region as inside if its winding number	is odd.
			/// </summary>
			WindingOdd = 100130,

			/// <summary>
			/// Classifies a region as inside if its winding number	is non-zero. 
			/// </summary>
			WindingNonzero = 100131,

			/// <summary>
			/// Classifies a region as inside if its winding number	is positive.
			/// </summary>
			WindingPositive = 100132,

			/// <summary>
			/// Classifies a region as inside if its winding number	is negative.
			/// </summary>
			WindingNegative = 100133,

			/// <summary>
			/// Classifies a region as inside if its winding number	has an absolute
			/// value of at least 2.
			/// </summary>
			WindingAbsGeqTwo = 100134
		}

		/// <summary>
		/// Specifies an OpenGl primitive
		/// </summary>
		public enum GlPrimitiveType { 

			/// <summary>
			/// Treats each vertex as a single point.
			/// </summary>
			Points = 0x0000,
			
			/// <summary>
			/// Treats each pair of vertices as an independent line	segment.
			/// </summary>
			Lines = 0x0001,

			/// <summary>
			/// Draws a connected group of line segments from the first vertex to
			/// the last, then back to the first.
			/// </summary>
			LineLoop = 0x0002,

			/// <summary>
			/// Draws a connected group of line segments from the first vertex to
			/// the last.
			/// </summary>
			LineStrip = 0x0003,

			/// <summary>
			/// Treats each triplet of vertices as an independent triangle.
			/// </summary>
			Triangles = 0x0004,

			/// <summary>
			/// Draws a connected group of triangles where one triangle is defined
			/// for each vertex presented after the first two vertices.
			/// </summary>
			TriangleStrip = 0x0005,

			/// <summary>
			/// Draws a connected group of triangles where one triangle is defined
			/// for each vertex presented after the first two vertices.
			/// </summary>
			TriangleFan = 0x0006,

			/// <summary>
			/// Treats each group of four vertices as an independent quadrilateral. 
			/// </summary>
			Quads = 0x0007,

			/// <summary>
			/// Draws a connected group of quadrilaterals.
			/// </summary>
			QuadStrip = 0x0008,

			/// <summary>
			/// Draws a single, convex polygon.
			/// </summary>
			Polygon = 0x0009
		}

		/// <summary>
		/// Specifies a callback to be used by a tessellation object.
		/// </summary>
		public enum GlCallbackName { 

			/// <summary>
			/// Invoked to indicate the start of a (triangle) primitive.
			/// </summary>
			Begin = 100100,

			/// <summary>
			/// Invoked between the begin and end callbacks to define the vertices of the
			/// triangles created by the tessellation process.
			/// </summary>
			Vertex = 100101,

			/// <summary>
			/// Invoked to indicate the end of a primitive.
			/// </summary>
			End = 100102,

			/// <summary>
			/// Invoked when an error is encountered.
			/// </summary>
			Error = 100103,

			/// <summary>
			/// Invoked to indicate which edges lie on the polygon boundary.
			/// </summary>
			EdgeFlag = 100104,

			/// <summary>
			/// Invoked to create a new vertex when the tessellation detects an intersection,
			/// or wishes to merge features.
			/// </summary>
			Combine = 100105
		} 

		/// <summary>
		/// Specifies the types of errors that can occur during the tesselation process.
		/// </summary>
		public enum GlTessError {

			/// <summary>
			/// Indicates that Error 1 has occured.
			/// </summary>
			Error1 = 100151,

			/// <summary>
			/// Indicates that Error 2 has occured. 
			/// </summary>
			Error2 = 100152,

			/// <summary>
			/// Indicates that Error 3 has occured.  
			/// </summary>
			Error3 = 100153,

			/// <summary>
			///  Indicates that Error 4 has occured. 
			/// </summary>
			Error4 = 100154,

			/// <summary>
			///  Indicates that Error 5 has occured. 
			/// </summary>
			Error5 = 100155,

			/// <summary>
			///  Indicates that Error 6 has occured. 
			/// </summary>
			Error6 = 100156,

			/// <summary>
			///  Indicates that Error 7 has occured. 
			/// </summary>
			Error7 = 100157,

			/// <summary>
			///  Indicates that Error 8 has occured. 
			/// </summary>
			Error8 = 100158
		}
		#endregion

		#region GLU Tesselation Callbacks
		/// <summary>
		/// Invoked to indicate the start of a (triangle) primitive.
		/// </summary>
		public delegate void BeginCallback(GlPrimitiveType type);

		/// <summary>
		/// Invoked to indicate the end of a primitive.
		/// </summary>
		public delegate void EndCallback();

		/// <summary>
		/// Invoked between the begin and end callbacks to define the vertices of the
		/// triangles created by the tessellation process.
		/// </summary>
		public delegate void VertexCallback(IntPtr data);

		/// <summary>
		/// Invoked when an error is encountered.
		/// </summary>
		public delegate void ErrorCallback(GlTessError ErrorCode);

		/// <summary>
		/// Invoked to create a new vertex when the tessellation detects an intersection,
		/// or wishes to merge features.
		/// </summary>
		public delegate void CombineCallback([In] IntPtr coords, IntPtr data, IntPtr weight, out IntPtr outData);

		/// <summary>
		/// Sets the begin callback for the specified tesselation object.
		/// </summary>
		/// <param name="tesselationObject">The tesselation object.</param>
		/// <param name="which">The type of callback being invoked.</param>
		/// <param name="callback">The begin callback delegate.</param>
		/// <remarks>
		/// The begin callback is invoked to indicate the start of a (triangle) primitive.
		/// </remarks>
		[DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
		public extern static void GluTessBeginCallBack( 
			IntPtr tesselationObject, 
			GlCallbackName which, 
			BeginCallback callback); 

		/// <summary>
		/// Sets the end callback for the specified tesselation object.
		/// </summary>
		/// <param name="tesselationObject">The tesselation object.</param>
		/// <param name="which">The type of callback being invoked.</param>
		/// <param name="callback">The end callback delegate.</param>
		/// <remarks>
		/// The end callback is invoked to indicate the end of a primitive.
		/// </remarks>
		[DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
		public extern static void GluTessEndCallBack( 
			IntPtr tesselationObject, 
			GlCallbackName which, 
			EndCallback callback); 

		/// <summary>
		/// Sets the vertex callback for the specified tesselation object.
		/// </summary>
		/// <param name="tesselationObject">The tesselation object.</param>
		/// <param name="which">The type of callback being invoked.</param>
		/// <param name="callback">The vertex callback delegate.</param>
		/// <remarks>
		/// The	vertex callback	is invoked between the begin and end callbacks to define
		/// the vertices of the triangles created by the tessellation process.
		/// </remarks>
		[DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
		public extern static void GluTessVertexCallBack( 
			IntPtr tesselationObject, 
			GlCallbackName which, 
			VertexCallback callback); 

		/// <summary>
		/// Sets the error callback for the specified tesselation object.
		/// </summary>
		/// <param name="tesselationObject">The tesselation object.</param>
		/// <param name="which">The type of callback being invoked.</param>
		/// <param name="callback">The error callback delegate.</param>
		/// <remarks>
		/// The	error callback is invoked when an error is encountered.
		/// </remarks>
		[DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
		public extern static void GluTessErrorCallBack( 
			IntPtr tesselationObject, 
			GlCallbackName which, 
			ErrorCallback callback); 

		/// <summary>
		/// Sets the combine callback for the specified tesselation object.
		/// </summary>
		/// <param name="tesselationObject">The tesselation object.</param>
		/// <param name="which">The type of callback being invoked.</param>
		/// <param name="callback">The combine callback delegate.</param>
		/// <remarks>
		/// The	combine	callback is invoked to create a new vertex when the tessellation
		/// detects an intersection, or wishes to merge features.
		/// </remarks>
		[DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
		public extern static void GluTessCombineCallBack( 
			IntPtr tesselationObject, 
			GlCallbackName which, 
			CombineCallback callback);
		#endregion

		#region GLU Tesselation Functions
		/// <summary>
		/// Create a tesselation object.
		/// </summary>
		/// <returns>Returns a pointer to the new tessellation object.</returns>
		[DllImport("glu32.dll", EntryPoint="gluNewTess")]
		public extern static IntPtr GluNewTess();

		/// <summary>
		/// Specify a normal for a polygon.
		/// </summary>
		/// <param name="tess">Specifies the tessellation object.</param>
		/// <param name="x">Specifies the first component of the normal.</param>
		/// <param name="y">Specifies the second component of the normal.</param>
		/// <param name="z">Specifies the third component of the normal.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessNormal")]
		public extern static void GluTessNormal(IntPtr tess, double x, double y, double z);

		/// <summary>
		/// Set	a tessellation object property.
		/// </summary>
		/// <param name="tess">Specifies the tessellation object.</param>
		/// <param name="property">Specifies the property to be set.</param>
		/// <param name="value">Specifies the value of the indicated property.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessProperty")]
		public extern static void GluTessProperty(IntPtr tess, GlTessProperty property, double value);

		/// <summary>
		/// Sets the winding rule property of the given tesselation object.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		/// <param name="rule">Specifies the winding rule to use.</param>
		public static void SetWindingRule(IntPtr tess, GlTessWinding rule) {
			GluTessProperty(tess, GlTessProperty.WindingRule, (double)rule);
		}

		/// <summary>
		/// Begin a polygon description.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		/// <param name="data">Specifies a pointer to user polygon data.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessBeginPolygon")]
		public extern static void GluTessBeginPolygon(IntPtr tess, IntPtr data);

		/// <summary>
		/// Begin a contour description.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessBeginContour")]
		public extern static void GluTessBeginContour(IntPtr tess);

		/// <summary>
		/// Specify a vertex on a polygon.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		/// <param name="coords">Specifies the coordinates of the vertex.</param>
		/// <param name="data">
		/// Specifies an opaque pointer passed back to the program with the
		/// vertex callback.
		/// </param>
		[DllImport("glu32.dll", EntryPoint="gluTessVertex")]
		public extern static void GluTessVertex(IntPtr tess, double[] coords, IntPtr data);

		/// <summary>
		/// End a contour description.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessEndContour")]
		public extern static void GluTessEndContour(IntPtr tess);

		/// <summary>
		/// End a polygon description.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		[DllImport("glu32.dll", EntryPoint="gluTessEndPolygon")]
		public extern static void GluTessEndPolygon(IntPtr tess);

		/// <summary>
		/// Destroy a tessellation object.
		/// </summary>
		/// <param name="tess">Specifies the tesselation object.</param>
		[DllImport("glu32.dll", EntryPoint="gluDeleteTess")]
		public extern static void GluDeleteTess(IntPtr tess);

		/* Other utility methods */

		/// <summary>
		/// Converts an OpenGL primitive type to the Direct3D equivalent.
		/// </summary>
		/// <param name="type">An OpenGL primitive type.</param>
		/// <returns>
		/// A <see cref="PrimitiveType">PrimitiveType</see> that represents the
		/// specified <see cref="GlPrimitiveType">GlPrimitiveType</see>.
		/// </returns>
		public static PrimitiveType GetD3DPrimitiveType(GlPrimitiveType type) {
			PrimitiveType D3DPrimitiveType = PrimitiveType.TriangleList;
			switch (type) {
				case GlPrimitiveType.Triangles:
					D3DPrimitiveType = PrimitiveType.TriangleList;
					break;
				case GlPrimitiveType.TriangleFan:
					D3DPrimitiveType = PrimitiveType.TriangleFan;
					break;
				case GlPrimitiveType.TriangleStrip:
					D3DPrimitiveType = PrimitiveType.TriangleStrip;
					break;
			}
			return D3DPrimitiveType;
		}
		#endregion
	}
}