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
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.Piccolo.Util;
using UMD.HCIL.PiccoloDirect3D.Util;

namespace UMD.HCIL.PiccoloDirect3D {
	/// <summary>
	/// A Direct3D canvas.  All nodes that implment D3DNode, should be added to a P3Canvas.
	/// </summary>
	public class P3Canvas : PCanvas {
		#region Fields
		private Device device = null;
		private PresentParameters presentParams = new PresentParameters();

		private static int MIN_USER_CLIPPLANES = 4; // PiccoloDirect3D requires at least 4 clipplanes.

		/// <summary>
		/// This event gets fired whenever the Direct3D device is reset (which happens on resize, 
		/// display device properties change, wake from sleep, etc.).  PiccoloDirect3D takes care
		/// of reallocating its D3D resources.  But, an application may want to listen to this event
		/// if it keeps its own D3D resources.  Note Vertex and Index buffers will be reallocated
		/// automatically, but you will need to refill them after they are created.  See P3Node for
		/// an example.
		/// </summary>
		public event EventHandler DeviceReset;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new P3Canvas.
		/// </summary>
		public P3Canvas() : base() {
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.Opaque, true);

			RegionManagement = false;
		}
		#endregion

		#region Scene Graph
		/// <summary>
		/// Overridden.  Creates a PiccoloDirect3D scene graph.
		/// </summary>
		/// <returns>The main camera node in the new scene graph.</returns>
		protected override PCamera CreateBasicScenegraph() {
			return P3Util.CreateBasicScenegraph ();
		}
		#endregion

		#region Painting
		/// <summary>
		/// Overridden.  Returns a P3PaintContext.
		/// </summary>
		protected override PPaintContext CreatePaintContext(PaintEventArgs pe) {
			return new P3PaintContext(pe.Graphics, Device, this);
		}

		/// <summary>
		/// Overridden. Begins the Direct3D scene, paints Piccolo and ends the Direct3D scene.
		/// </summary>
		/// <param name="paintContext">The paint context to use for painting piccolo.</param>
		protected override void PaintPiccolo(PPaintContext paintContext) {
			if (device != null) {
				device.Clear(ClearFlags.Target, BackColor, 1.0f, 0);
				device.BeginScene();   // Begin the scene.

				base.PaintPiccolo (paintContext);

				device.EndScene();  // End the scene.
				device.Present();
			}
		}
		#endregion

		#region Device
		/// <summary>
		/// Gets or sets the device associates with this canvas.
		/// </summary>
		/// <value>The device assoicated with this canvas.</value>
		public virtual Device Device {
			get {
				if (device == null) {
					device = CreateDevice();
				}
				return device;
			}
		}

		/// <summary>
		/// Override this method to modify how the device is created.
		/// </summary>
		/// <returns>The new device.</returns>
		protected virtual Device CreateDevice() {
			try {
				Device device;
				presentParams.Windowed=true; // We don't want to run fullscreen				
				presentParams.SwapEffect = SwapEffect.Discard; // Discard the frames 

				// Uncomment to enable depth buffer for 3D rendering.
				// Leave these lines commented out if you are rendering in 2D as
				// defining a depth buffer will incur a performance hit.
				//presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
				//presentParams.AutoDepthStencilFormat = DepthFormat.D16; // And the stencil format

				// Store the default adapter
				int adapterOrdinal = Manager.Adapters.Default.Adapter;
				CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

				// Get the capabilities of the adapter
				Caps caps = Manager.GetDeviceCaps(adapterOrdinal, DeviceType.Hardware);

				// If the device supports our minimum required user clip planes we will try
				// to do hardware vertex processing.
				if (caps.MaxUserClipPlanes >= MIN_USER_CLIPPLANES) {
					// Do we support hardware vertex processing?
					if (caps.DeviceCaps.SupportsHardwareTransformAndLight) {
						flags = CreateFlags.HardwareVertexProcessing;
					}

					// Do we support a pure device?
					if (caps.DeviceCaps.SupportsPureDevice) {
						flags |= CreateFlags.PureDevice;
					}
				}

				device = new Device(0, DeviceType.Hardware, this, flags, presentParams);
				device.DeviceReset += new System.EventHandler(this.OnResetDevice);
				this.OnResetDevice(device, null);

				return device;
			}
			catch (DirectXException e) {
				// Catch any errors and return a failure
				Console.WriteLine("Unable to initialize Direct3D");
				Console.WriteLine(e);
				return null;
			}
		}

		/// <summary>
		/// Resets the render state of the device and notifies piccolo that the device
		/// has been reset.
		/// </summary>
		/// <param name="sender">The source of the DeviceReset event.</param>
		/// <param name="e">An EventArgs that stores the DeviceReset event data.</param>
		public virtual void OnResetDevice(object sender, EventArgs e) {
			device = (Device)sender;

			SetupMatrices();       // Setup the world, view, and projection matrices
			device.RenderState.CullMode = Cull.None;
			device.RenderState.Lighting = false;
			device.RenderState.ZBufferEnable = false;

			Caps caps = device.DeviceCaps;
			if (caps.SourceBlendCaps.SupportsSourceAlpha && 
				caps.DestinationBlendCaps.SupportsInverseSourceAlpha) {

				device.RenderState.AlphaBlendEnable = true;
				device.RenderState.SourceBlend = Blend.SourceAlpha;
				device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			}

			// Give nodes a chance to respond to a reset
			FullResetDevice(Root);

			// Then fire an event to let the app know about it
			if (DeviceReset != null) {
				DeviceReset(this, e);
			}
		}

		/// <summary>
		/// Sets up the view and projection matrices.
		/// </summary>
		protected virtual void SetupMatrices() {
			// Set up our view matrix. A view matrix can be defined given an eye point,
			// a point to lookat, and a direction for which way is up. Here, we set the
			// eye five units back along the z-axis and up three units, look at the
			// origin, and define "up" to be in the y-direction.
			device.Transform.View = Matrix.LookAtRH(new Vector3(ClientSize.Width/2, ClientSize.Height/2, -5.0f), new Vector3(ClientSize.Width/2, ClientSize.Height/2, 0.0f), new Vector3(0.0f, -1.0f, 0.0f));

			// For the projection matrix, we set up a perspective transform (which
			// transforms geometry from 3D view space to 2D viewport space, with
			// a perspective divide making objects smaller in the distance). To build
			// a perpsective transform, we need the field of view (1/4 pi is common),
			// the aspect ratio, and the near and far clipping planes (which define at
			// what distances geometry should be no longer be rendered).
			device.Transform.Projection = Matrix.OrthoRH(ClientSize.Width, ClientSize.Height, -10, 10);
		}

		/// <summary>
		/// Notifies all Direct3D nodes that the device has been reset.
		/// </summary>
		/// <param name="node">The node to notify.</param>
		protected virtual void FullResetDevice(PNode node) {
			if (node is D3DNode) {
				(node as D3DNode).OnResetDevice();
			}
			foreach (PNode child in node) {
				FullResetDevice(child);
			}
		}

		/// <summary>
		/// Overridden.  See <see cref="Control.OnResize">PCanvas.OnResize</see>.
		/// </summary>
		protected override void OnResize(EventArgs e) {
			// This hack is necessary due to a bug in Managed DirectX.  When there is no
			// visible client area, DirectX throws an exception and the device is lost.
			// So we need to restore it here.

			// If the window handle exists, the device is null and the client area is greater
			// than zero, we will recreate the device.
			if (IsHandleCreated && device == null && ClientSize.Width > 0 && ClientSize.Height > 0) {
				device = this.CreateDevice();

				// Invalidate all D3D nodes so that the resources will be recreated
				// with the new device.
				FullInvalidateVertices(this.Root);
			}

			try {
				base.OnResize (e);
			} catch(DriverInternalErrorException) {
				// If the exception occurred dispose the invalid device and set it to null.
				device.Dispose();
				device = null;
			}
		}

		private void FullInvalidateVertices(PNode node) {
			if (node is D3DNode) {
				// Dispose each node and invalidate it's vertices.
				(node as D3DNode).Dispose();
				(node as D3DNode).InvalidateVertices();
			}
			foreach (PNode child in node) {
				FullInvalidateVertices(child);
			}
		}
		#endregion
	}
}