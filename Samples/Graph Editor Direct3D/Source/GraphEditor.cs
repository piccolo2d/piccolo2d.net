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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.Piccolo.Event;
using UMD.HCIL.PiccoloDirect3D;
using UMD.HCIL.PiccoloDirect3D.Nodes;

namespace GraphEditorDirect3D {
	/// <summary>
	/// Creates a simple graph with some random nodes and connected edges.  An event
	/// handler allows users to drag nodes around, keeping the edges connected.
	/// </summary>
	public class GraphEditor : P3Form {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GraphEditor() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public override void Initialize() {
			int numNodes = 50;
			int numEdges = 50;

			// Initialize, and create a layer for the edges (always underneath the nodes)
			PLayer nodeLayer = Canvas.Layer;
			PLayer edgeLayer = new PLayer();
			Canvas.Root.AddChild(edgeLayer);
			Canvas.Camera.AddLayer(0, edgeLayer);
			Random rnd = new Random();

			// Create some random nodes
			// Each node's Tag has an ArrayList used to store associated edges
			for (int i=0; i<numNodes; i++) {
				float x = (float)(ClientSize.Width * rnd.NextDouble());
				float y = (float)(ClientSize.Height * rnd.NextDouble());
				P3Path node = P3Path.CreateEllipse(x, y, 20, 20);

				node.Tag = new ArrayList();
				nodeLayer.AddChild(node);
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
				P3Path edge = P3Path.CreateLine((node1.Bounds.Left + node1.Bounds.Right)/2, (node1.Bounds.Top + node1.Bounds.Bottom)/2,
					(node2.Bounds.Left + node2.Bounds.Right)/2, (node2.Bounds.Top + node2.Bounds.Bottom)/2);

				((ArrayList)node1.Tag).Add(edge);
				((ArrayList)node2.Tag).Add(edge);
				edge.Tag = new ArrayList();
				((ArrayList)edge.Tag).Add(node1);
				((ArrayList)edge.Tag).Add(node2);

				edgeLayer.AddChild(edge);
			}

			// Create event handler to move nodes and update edges
			nodeLayer.AddInputEventListener(new NodeDragHandler());
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			// 
			// GraphEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 516);
			this.Name = "GraphEditor";
			this.Text = "GraphEditor";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new GraphEditor());
		 }
	}

	class NodeDragHandler : PDragSequenceEventHandler {
		public override void OnMouseEnter(object sender, PInputEventArgs e) {
			base.OnMouseEnter (sender, e);
			if (e.Button == MouseButtons.None) {
				e.PickedNode.Brush = Brushes.Red;
			}
		}

		public override void OnMouseDown(object sender, PInputEventArgs e) {
			base.OnMouseDown (sender, e);
			e.Handled = true;
		}

		public override void OnMouseLeave(object sender, PInputEventArgs e) {
			base.OnMouseLeave (sender, e);
			if (e.Button == MouseButtons.None) {
				e.PickedNode.Brush = Brushes.White;
			}
		}

		protected override void OnDrag(object sender, PInputEventArgs e) {
			base.OnDrag (sender, e);
			e.PickedNode.TranslateBy(e.Delta.Width, e.Delta.Height);

			ArrayList edges = (ArrayList)e.PickedNode.Tag;
			foreach (P3Path edge in edges) {
				ArrayList nodes = (ArrayList)edge.Tag;
				PNode node1 = (PNode)nodes[0];
				PNode node2 = (PNode)nodes[1];
				// Note that the node's "FullBounds" must be used (instead of just the "Bound") 
				// because the nodes have non-identity transforms which must be included when
				// determining their position.
				edge.Reset();
				edge.AddLine((node1.FullBounds.Left + node1.FullBounds.Right) / 2, (node1.FullBounds.Top + node1.FullBounds.Bottom) / 2,
					(node2.FullBounds.Left + node2.FullBounds.Right) / 2, (node2.FullBounds.Top + node2.FullBounds.Bottom) / 2);
			}
		}
	}
}