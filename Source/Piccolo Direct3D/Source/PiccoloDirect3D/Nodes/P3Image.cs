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
using System.IO;
using System.Net;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloDirect3D.Util;

namespace UMD.HCIL.PiccoloDirect3D.Nodes {
	/// <summary>
	/// <b>P3Image</b> is a Direct3D wrapper around a <see cref="System.Drawing.Image">
	/// System.Drawing.Image</see>.
	/// </summary>
	public class P3Image : P3Node {
		#region Fields
		/// <summary>
		/// The default number of vertices in the vertex buffer.
		/// </summary>
		protected const int VERTEX_COUNT = 6;

		/// <summary>
		/// The underlying image object.
		/// </summary>
		protected Image image;

		/// <summary>
		/// The texture that will be applied to the vertices.
		/// </summary>
		protected Texture texture;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3Image with a <c>null</c> image.
		/// </summary>
		public P3Image() {}

		/// <summary>
		/// Constructs a new P3Image wrapping the given <see cref="System.Drawing.Image">
		/// System.Drawing.Image</see>.
		/// </summary>
		/// <param name="newImage">The image to wrap.</param>
		public P3Image(Image newImage) {
			Image = newImage;
		}
		
		/// <summary>
		/// Constructs a new P3Image by loading the given file.
		/// </summary>
		/// <param name="fileName">The filename of the image to load.</param>
		public P3Image(string fileName) : this(new Bitmap(fileName)) {}

		/// <summary>
		/// Constructs a new PImage by loading the given URI and wrapping the
		/// resulting <see cref="System.Drawing.Image">System.Drawing.Image</see>.
		/// If the URI is <c>null</c>, create an empty PImage; this behavior is
		/// useful when fetching resources that may be missing.
		/// </summary>
		/// <param name="requestURI">The URI of the image to load.</param>
		public P3Image(Uri requestURI) {
			if (requestURI != null) {
				WebClient myWebClient = new WebClient();
				Stream myStream = myWebClient.OpenRead(requestURI.AbsoluteUri);
				Image = new Bitmap(myStream);
				myStream.Close();
			}
		}
		#endregion

		#region Basic
		//****************************************************************
		// Basic - Methods for manipulating the underlying image.
		//****************************************************************

		/// <summary>
		/// Gets or sets the image shown by this node.
		/// </summary>
		/// <value>The image shown by this node.</value>
		public virtual Image Image {
			get { return image; }
			set { 
				DisposeTexture();  // Release the old texture here.

				image = value;
				if (image == null) {
					SetBounds(0, 0, 0, 0);
				} else {
					SetBounds(0, 0, image.Width, image.Height);
				}

				InvalidateVertices();
				InvalidatePaint();
			}
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Overridden.  Disposes the Direct3D texture.
		/// </summary>
		public override void Dispose() {
			base.Dispose();
			DisposeTexture();
		}

		/// <summary>
		/// Dispose the Direct3D texture.
		/// </summary>
		protected virtual void DisposeTexture() {
			if (texture != null) {
				texture.Dispose();
				texture = null;
			}
		}
		#endregion

		#region Vertex Buffer
		/// <summary>
		/// Overridden.  See <see cref="P3Node.CreateVertexBuffer">P3Node.CreateVertexBuffer</see>.
		/// </summary>
		protected override VertexBuffer CreateVertexBuffer(Device device) {
			return new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), VERTEX_COUNT, device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Managed);
		}

		/// <summary>
		/// Overridden.  See <see cref="P3Node.FillVertexBuffer">P3Node.FillVertexBuffer</see>.
		/// </summary>
		protected override void FillVertexBuffer(VertexBuffer vb) {
			GraphicsStream stm = vb.Lock(0, 0, 0);
			CustomVertex.PositionNormalTextured[] texVerts = new CustomVertex.PositionNormalTextured[VERTEX_COUNT];
			P3Util.CreateTexturedRectangle(texVerts, 0, Bounds);
			stm.Write(texVerts);
			vb.Unlock();
		}
		#endregion

		#region Texture Validation Management
		/// <summary>
		/// Overridden.  See <see cref="P3Node.ValidateVertices">P3Node.ValidateVertices</see>.
		/// </summary>
		public override void ValidateVertices(Device device) {
			base.ValidateVertices(device);
			if (image != null && texture == null) {
				texture = new Texture(device, new Bitmap(image), 0, Pool.Managed);
			}
		}
		#endregion

		#region Painting
		/// <summary>
		/// Overridden.  See <see cref="UMD.HCIL.Piccolo.PNode.Paint">PNode.Paint</see>.
		/// </summary>
		protected override void Paint(PPaintContext paintContext) {
			Device device = (paintContext as P3PaintContext).Device;
			VertexBuffer vb = GetValidVertexBuffer(device);

			device.SetTexture(0,texture);
			device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
			device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;

			device.SetStreamSource(0, vb, 0);
			device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
			device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			device.SetTexture(0, null);
		}
		#endregion
	}
}