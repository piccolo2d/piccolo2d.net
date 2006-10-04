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
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.PiccoloX;
using UMD.HCIL.Piccolo.Util;

namespace UMD.HCIL.GraphEditor {
	/// <summary>
	/// A simple graph control with some random nodes and connected edges.  An event
	/// handler allows users to drag nodes around, keeping the edges connected.
	/// </summary>
	public class GraphEditor : PCanvas {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private static int DEFAULT_WIDTH = 500;
		private static int DEFAULT_HEIGHT = 500;

		/// <summary>
		/// Empty Constructor is necessary so that this control can be used as an applet.
		/// </summary>
		public GraphEditor() : this(DEFAULT_WIDTH, DEFAULT_HEIGHT) {}

		public GraphEditor(int width, int height) {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.Size = new Size(width, height);
			int numNodes = 50;
			int numEdges = 50;

			// Initialize, and create a layer for the edges (always underneath the nodes)
			PLayer nodeLayer = this.Layer;
			PLayer edgeLayer = new PLayer();
			Root.AddChild(edgeLayer);
			this.Camera.AddLayer(0, edgeLayer);
			Random rnd = new Random();

			// Create some random nodes
			// Each node's Tag has an ArrayList used to store associated edges
			for (int i=0; i<numNodes; i++) {
				float x = (float)(this.ClientSize.Width * rnd.NextDouble());
				float y = (float)(this.ClientSize.Height * rnd.NextDouble());
				PPath path = PPath.CreateEllipse(x, y, 20, 20);
				path.Tag = new ArrayList();
				nodeLayer.AddChild(path);
			}

			// Create some random edges
			// Each edge's Tag has an ArrayList used to store associated nodes
			for (int i=0; i<numEdges; i++) {
				int n1 = rnd.Next(numNodes);
				int n2 = n1;
				while (n2 == n1) {
					n2 = rnd.Next(numNodes);  // Make sure we have two distinct nodes.
				}

				PNode node1 = nodeLayer[n1];
				PNode node2 = nodeLayer[n2];
				PPath edge = new PPath();
				((ArrayList)node1.Tag).Add(edge);
				((ArrayList)node2.Tag).Add(edge);
				edge.Tag = new ArrayList();
				((ArrayList)edge.Tag).Add(node1);
				((ArrayList)edge.Tag).Add(node2);
				edgeLayer.AddChild(edge);
				UpdateEdge(edge);
			}

			// Create event handler to move nodes and update edges
			nodeLayer.AddInputEventListener(new NodeDragHandler());
		}

		public static void UpdateEdge(PPath edge) {
			// Note that the node's "FullBounds" must be used (instead of just the "Bound") 
			// because the nodes have non-identity transforms which must be included when
			// determining their position.

			ArrayList nodes = (ArrayList)edge.Tag;
			PNode node1 = (PNode)nodes[0];
			PNode node2 = (PNode)nodes[1];
			PointF start = PUtil.CenterOfRectangle(node1.FullBounds);
			PointF end = PUtil.CenterOfRectangle(node2.FullBounds);
			edge.Reset();
			edge.AddLine(start.X, start.Y, end.X, end.Y);
		}

		/// <summary>
		/// Simple event handler which applies the following actions to every node it is called on:
		///   * Turn node red when the mouse goes over the node
		///   * Turn node white when the mouse exits the node
		///   * Drag the node, and associated edges on mousedrag
		/// It assumes that the node's Tag references an ArrayList with a list of associated
		/// edges where each edge is a PPath which each have a Tag that references an ArrayList
		/// with a list of associated nodes.
		/// </summary>
		class NodeDragHandler : PDragEventHandler {
			public override bool DoesAcceptEvent(PInputEventArgs e) {
				return e.IsMouseEvent && (e.Button != MouseButtons.None || e.IsMouseEnterOrMouseLeave);
			}

			public override void OnMouseEnter(object sender, PInputEventArgs e) {
				base.OnMouseEnter (sender, e);
				if (e.Button == MouseButtons.None) {
					e.PickedNode.Brush = Brushes.Red;
				}
			}

			public override void OnMouseLeave(object sender, PInputEventArgs e) {
				base.OnMouseLeave (sender, e);
				if (e.Button == MouseButtons.None) {
					e.PickedNode.Brush = Brushes.White;
				}
			}

			protected override void OnStartDrag(object sender, PInputEventArgs e) {
				base.OnStartDrag(sender, e);
				e.Handled = true;
				e.PickedNode.MoveToFront();
			}

			protected override void OnDrag(object sender, PInputEventArgs e) {
				base.OnDrag (sender, e);

				ArrayList edges = (ArrayList)e.PickedNode.Tag;
				foreach (PPath edge in edges) {
					GraphEditor.UpdateEdge(edge);
				}
			}
		}

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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion

		// Draw a border for when this control is used as an applet.
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
			e.Graphics.DrawRectangle(Pens.Black, 0, 0, ClientSize.Width-1, ClientSize.Height-1);
		}
	}
}