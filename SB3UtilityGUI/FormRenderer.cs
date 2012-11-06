using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using WeifenLuo.WinFormsUI.Docking;

namespace SB3Utility
{
	public partial class FormRenderer : DockContent
	{
		const int sensitivityScale = 10000;

		public AnimationRenderer Renderer { get; protected set; }

		public FormRenderer()
		{
			InitializeComponent();
			Renderer = new AnimationRenderer(panel1);

			numericSensitivity.Value = (decimal)Renderer.Sensitivity * sensitivityScale;
			numericSensitivity.ValueChanged += new EventHandler(numericSensitivity_ValueChanged);

			bonesToolStripMenuItem.Checked = Renderer.ShowBones;
			bonesToolStripMenuItem.CheckedChanged += new EventHandler(bonesToolStripMenuItem_CheckedChanged);

			normalsToolStripMenuItem.Checked = Renderer.ShowNormals;
			normalsToolStripMenuItem.CheckedChanged += new EventHandler(normalsToolStripMenuItem_CheckedChanged);

			wireframeToolStripMenuItem.Checked = Renderer.Wireframe;
			wireframeToolStripMenuItem.CheckedChanged += new EventHandler(wireframeToolStripMenuItem_CheckChanged);

			cullingToolStripMenuItem.Checked = Renderer.Culling;
			cullingToolStripMenuItem.CheckedChanged += new EventHandler(cullingToolStripMenuItem_CheckedChanged);

			centerViewAutomaticallyToolStripMenuItem.Checked = (bool)Gui.Config["AutoCenterView"];
			centerViewAutomaticallyToolStripMenuItem.CheckedChanged += centerViewAutomaticallyToolStripMenuItem_CheckChanged;
		}

		void CustomDispose()
		{
			Renderer.Dispose();
		}

		private void buttonCenterView_Click(object sender, EventArgs e)
		{
			Renderer.CenterView();
		}

		private void buttonResetPose_Click(object sender, EventArgs e)
		{
			Renderer.ResetPose();
		}

		private void numericSensitivity_ValueChanged(object sender, EventArgs e)
		{
			Renderer.Sensitivity = Decimal.ToSingle(numericSensitivity.Value / sensitivityScale);
		}

		private void normalsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Renderer.ShowNormals = normalsToolStripMenuItem.Checked;
			Renderer.Render();
		}

		private void bonesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Renderer.ShowBones = bonesToolStripMenuItem.Checked;
			Renderer.Render();
		}

		private void wireframeToolStripMenuItem_CheckChanged(object sender, EventArgs e)
		{
			Renderer.Wireframe = wireframeToolStripMenuItem.Checked;
			Renderer.Render();
		}

		private void cullingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Renderer.Culling = cullingToolStripMenuItem.Checked;
			Renderer.Render();
		}

		private void diffuseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = Renderer.Diffuse;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				Renderer.Diffuse = colorDialog1.Color;
				Renderer.Render();
			}
		}

		private void ambientToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = Renderer.Ambient;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				Renderer.Ambient = colorDialog1.Color;
				Renderer.Render();
			}
		}

		private void specularToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = Renderer.Specular;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				Renderer.Specular = colorDialog1.Color;
				Renderer.Render();
			}
		}

		private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = Renderer.Background;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				Renderer.Background = colorDialog1.Color;
				Renderer.Render();
			}
		}

		private void centerViewAutomaticallyToolStripMenuItem_CheckChanged(object sender, EventArgs e)
		{
			Gui.Config["AutoCenterView"] = centerViewAutomaticallyToolStripMenuItem.Checked;
		}
	}
}
