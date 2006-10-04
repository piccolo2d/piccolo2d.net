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
using UMD.HCIL.PiccoloDirect3D.Util;

namespace UMD.HCIL.PiccoloDirect3D {
	/// <summary>
	/// <b>P3Node</b> is the central abstraction in PiccoloDirect3D. All objects that are
	/// visible on the screen are instances of the P3Node class. All nodes may have 
	/// other "child" nodes added to them. 
	/// </summary>
	public class P3Node : PNode, D3DNode {
		#region Fields
		/// <summary>
		/// Indicates whether this node's cached vertices are invalid. 
		/// </summary>
		protected bool verticesInvalid;

		/// <summary>
		/// The vertex buffer that stores the vertices for this node.
		/// </summary>
		protected VertexBuffer vb;

		private static int VERTEX_COUNT = 6;
		private static int VERTEX_INDEX = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3Node.
		/// </summary>
		public P3Node() {
			Brush = new SolidBrush(Color.White);
		}
		#endregion

		#region Basics
		/// <summary>
		/// Overridden.  See <see cref="PNode.Brush">PNode.Brush</see>.
		/// </summary>
		public override Brush Brush {
			set {
				base.Brush = value;
				InvalidateVertices();
			}
		}

		/// <summary>
		/// Overridden.  See <see cref="PNode.SetBounds">PNode.SetBounds</see>.
		/// </summary>
		public override bool SetBounds(float x, float y, float width, float height) {
			InvalidateVertices();
			return base.SetBounds (x, y, width, height);
		}
		#endregion

		#region Vertex Buffer
		/// <summary>
		/// Returns the vertex buffer, validating it if necessary.
		/// </summary>
		/// <remarks>
		/// The vertex buffer is a cache of the node's vertices.  At any given time, the
		/// cache could be invalid.  The vertex buffer should be accessed through this
		/// method to ensure that it's contents are up to date.
		/// </remarks>
		/// <param name="device">The device associated with this node.</param>
		/// <returns>A valid vertex buffer.</returns>
		public VertexBuffer GetValidVertexBuffer(Device device) {
			if (verticesInvalid) {
				ValidateVertices(device);
			}
			return vb;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this node's cached vertices are invalid.
		/// </summary>
		/// <value>True if this node node's cached vertices are invalid; else false;</value>
		/// <remarks>
		/// If this property is set to true, the node's cached vertices will be validated
		/// at a later point. Note, this property is most often set internally.
		/// </remarks>
		public bool VerticesInvalid {
			get { return verticesInvalid; }
			set { verticesInvalid = value; }
		}

		/// <summary>
		/// Disposes the vertex buffer associated with this node.
		/// </summary>
		/// <remarks>
		/// DirectX resources will be disposed automatically, but in a non-deterministic
		/// fashion. Call this method to dispose a node's vertex buffer immediately.
		/// </remarks>
		public virtual void Dispose() {
			if (vb != null) {
				vb.Dispose();
				vb = null;
			}
		}
		#endregion

		#region Device
		/// <summary>
		/// Override this method to get notified when the device associated with this node
		/// has been reset.
		/// </summary>
		public virtual void OnResetDevice() {
		}
		#endregion

		#region Vertex Validation Management
		/// <summary>
		/// Invalidate the node's cached vertices.
		/// </summary>
		public virtual void InvalidateVertices() {
			VerticesInvalid = true;
		}

		/// <summary>
		/// Update this node's cached vertices.
		/// </summary>
		/// <remarks>
		/// This method will get called before painting a node, if it's Direct3D vertices need
		/// to be updated.  By default, this method will fill the vertex buffer.  To create a
		/// node that does not use P3Node's default vertex buffer override this method and do not
		/// call the base implmentation.  Instead you can update your custom vertices.
		/// </remarks>
		/// <param name="device">The device associated with this node.</param>
		public virtual void ValidateVertices(Device device) {
			if (vb == null) {
				vb = CreateVertexBuffer(device);
				vb.Created += new EventHandler(OnVertexBufferCreated);
			}
			FillVertexBuffer(vb);
			verticesInvalid = false;
		}

		/// <summary>
		/// Calls <see cref="P3Node.FillVertexBuffer">P3Node.FillVertexBuffer</see> whenever the
		/// vertex buffer is recreated.
		/// </summary>
		/// <param name="sender">The source of the Created event.</param>
		/// <param name="e">An EventArgs that contains the event data.</param>
		private void OnVertexBufferCreated(object sender, EventArgs e) {
			FillVertexBuffer((VertexBuffer)sender);
		}

		/// <summary>
		/// Override this method to create a custom vertex buffer.  Most nodes that extend P3Node
		/// will need to override this method since their number and type of vertices will vary.
		/// </summary>
		/// <param name="device"></param>
		/// <remarks>
		/// It is not safe to call GetValidVertexBuffer() here, since that method may call
		/// CreateVertexBuffer().
		/// </remarks>
		/// <returns>The new vertex buffer.</returns>
		protected virtual VertexBuffer CreateVertexBuffer(Device device) {
			VertexBuffer vb = new VertexBuffer(typeof(CustomVertex.PositionColored), VERTEX_COUNT, device, Usage.WriteOnly,
				CustomVertex.PositionColored.Format, Pool.Managed);
			return vb;
		}

		/// <summary>
		/// Override this method to fill the vertex buffer with the appropriate data.  Most nodes
		/// that extend P3Node will need to override this method since their number and type of
		/// vertices will vary.
		/// </summary>
		/// <remarks>
		/// It is not safe to call GetValidVertexBuffer() here, since that method may call
		/// FillVertexBuffer().
		/// </remarks>
		/// <param name="vb">The vertex buffer to fill.</param>
		protected virtual void FillVertexBuffer(VertexBuffer vb) {
			// Only fill the vertex buffer if the brush is not null.
			if (Brush != null) {
				GraphicsStream stm = vb.Lock(0, 0, 0);
				CustomVertex.PositionColored[] colVerts = new CustomVertex.PositionColored[VERTEX_COUNT];
				P3Util.CreateColoredRectangle(colVerts, 0, Bounds, (Brush as SolidBrush).Color.ToArgb());
				stm.Write(colVerts);
				vb.Unlock();
			}
		}
		#endregion

		#region Painting
		/// <summary>
		/// Overridden.  See <see cref="PNode.Paint">PNode.Paint</see>.
		/// </summary>
		protected override void Paint(PPaintContext paintContext) {
			// Only paint the node if the brush is not null.
			if (Brush != null) {
				Device device = (paintContext as P3PaintContext).Device;
				if (device != null) {
					device.SetStreamSource(0, GetValidVertexBuffer(device), 0);
					device.VertexFormat = CustomVertex.PositionColored.Format;
					device.DrawPrimitives(PrimitiveType.TriangleList, VERTEX_INDEX, 2);
				}
			}
		}
		#endregion
	}
}