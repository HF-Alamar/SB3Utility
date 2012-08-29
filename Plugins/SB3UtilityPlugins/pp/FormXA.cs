using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	[Plugin]
	[PluginOpensFile(".xa")]
	public partial class FormXA : DockContent
	{
		private FormPP formPP;
		public xaEditor Editor { get; protected set; }
		public string EditorVar { get; protected set; }
		public string ParserVar { get; protected set; }

		private TextBox[][] xaMaterialMatrixText = new TextBox[6][];
		private TreeNode[] prevMorphKeyframeNodes = null;
		private ListViewItem loadedAnimationClip = null;

		private int animationId;
		private KeyframedAnimationSet animationSet = null;
		
		private Timer renderTimer = new Timer();
		private DateTime startTime;
		private double trackPos = 0;
		private bool play = false;
		private bool trackEnabled = false;
		private bool userTrackBar = true;

		public float AnimationSpeed { get; set; }
		public bool FollowSequence { get; set; }

		public FormXA(string path, string variable)
		{
			InitializeComponent();

			this.ShowHint = DockState.Document;
			this.Text = Path.GetFileName(path);
			this.ToolTipText = path;

			ParserVar = Gui.Scripting.GetNextVariable("xaParser");
			string parserCommand = ParserVar + " = OpenXA(path=\"" + path + "\")";
			xxParser parser = (xxParser)Gui.Scripting.RunScript(parserCommand);

			EditorVar = Gui.Scripting.GetNextVariable("xaEditor");
			string editorCommand = EditorVar + " = xaEditor(parser=" + ParserVar + ")";
			Editor = (xaEditor)Gui.Scripting.RunScript(editorCommand);

			Init();
			LoadXA();
		}

		public FormXA(ppParser ppParser, string xaParserVar)
		{
			InitializeComponent();

			xaParser parser = (xaParser)Gui.Scripting.Variables[xaParserVar];

			this.ShowHint = DockState.Document;
			this.Text = parser.Name;
			this.ToolTipText = ppParser.FilePath + @"\" + parser.Name;

			ParserVar = xaParserVar;

			EditorVar = Gui.Scripting.GetNextVariable("xaEditor");
			Editor = (xaEditor)Gui.Scripting.RunScript(EditorVar + " = xaEditor(parser=" + ParserVar + ")");

			Init();
			LoadXA();
		}

		void CustomDispose()
		{
			try
			{
				if (Editor.Parser.AnimationSection != null)
				{
					if (animationSet != null)
					{
						Pause();
						Gui.Renderer.RemoveAnimationSet(animationId);
						animationSet.Dispose();
					}

					Gui.Renderer.RenderObjectAdded -= new EventHandler(Renderer_RenderObjectAdded);
				}
			}
			catch
			{
			}
		}

		private void Init()
		{
			for (int i = 0; i < 4; i++)
			{
				xaMaterialMatrixText[i] = new TextBox[4];
			}
			xaMaterialMatrixText[4] = new TextBox[1];
			xaMaterialMatrixText[5] = new TextBox[1];
			xaMaterialMatrixText[0][0] = xaMatDiffuseR;
			xaMaterialMatrixText[0][1] = xaMatDiffuseG;
			xaMaterialMatrixText[0][2] = xaMatDiffuseB;
			xaMaterialMatrixText[0][3] = xaMatDiffuseA;
			xaMaterialMatrixText[1][0] = xaMatAmbientR;
			xaMaterialMatrixText[1][1] = xaMatAmbientG;
			xaMaterialMatrixText[1][2] = xaMatAmbientB;
			xaMaterialMatrixText[1][3] = xaMatAmbientA;
			xaMaterialMatrixText[2][0] = xaMatSpecularR;
			xaMaterialMatrixText[2][1] = xaMatSpecularG;
			xaMaterialMatrixText[2][2] = xaMatSpecularB;
			xaMaterialMatrixText[2][3] = xaMatSpecularA;
			xaMaterialMatrixText[3][0] = xaMatEmissiveR;
			xaMaterialMatrixText[3][1] = xaMatEmissiveG;
			xaMaterialMatrixText[3][2] = xaMatEmissiveB;
			xaMaterialMatrixText[3][3] = xaMatEmissiveA;
			xaMaterialMatrixText[4][0] = xaMatSpecularPower;
			xaMaterialMatrixText[5][0] = xaMatUnknown;

			/*foreach (KeyValuePair<ppSubfile, xxView> pair in mainForm.xxViewDic)
			{
				ppSubfile subfile = pair.Key;
				StringTag morphItem = new StringTag(subfile.name + "  " + subfile.ppParser.ppPath, subfile);
				comboBoxMorphMesh.Items.Add(morphItem);
			}
			if (comboBoxMorphMesh.Items.Count > 0)
			{
				comboBoxMorphMesh.SelectedIndex = 0;
			}*/

			tabControlXA.TabPages.Remove(tabPageXAObjectView);

			AnimationSpeed = Decimal.ToSingle(numericAnimationClipSpeed.Value);
			FollowSequence = checkBoxAnimationClipLoadNextClip.Checked;

			Gui.Docking.ShowDockContent(this, Gui.Docking.DockEditors);
		}

		private void LoadXA()
		{
			/*for (int i = 0; i < Editor.Parser.header.children.Count; i++)
			{
				xaSection section = (xaSection)Editor.Parser.header.children[i];
				TreeNode sectionNode = new TreeNode("Section " + (i + 1));
				sectionNode.Tag = section;
				makeXAObjectTree(section, sectionNode);
				treeViewXA.Nodes.Add(sectionNode);
			}*/

			if (Editor.Parser.MaterialSection != null)
			{
				for (int i = 0; i < Editor.Parser.MaterialSection.MaterialList.Count; i++)
				{
					xaMaterial mat = Editor.Parser.MaterialSection.MaterialList[i];
					ListViewItem item = new ListViewItem(mat.Name);
					item.Tag = mat;
					listViewType1.Items.Add(item);
				}
				listViewType1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				tabPageMaterial.Text = "Material [" + Editor.Parser.MaterialSection.MaterialList.Count + "]";
			}

			if (Editor.Parser.MorphSection != null)
			{
				List<xaMorphKeyframe> morphKeyframeList = Editor.Parser.MorphSection.KeyframeList;
				for (int i = 0; i < morphKeyframeList.Count; i++)
				{
					xaMorphKeyframe morphKeyframe = morphKeyframeList[i];
					ListViewItem item = new ListViewItem(new string[] { morphKeyframe.Name, morphKeyframe.PositionList.Count.ToString() });
					item.Tag = morphKeyframe;
					//*section3bItem.viewItems.Add(item);
					listViewMorphKeyframe.Items.Add(item);
				}
				columnHeaderMorphKeyframeName.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

				List<xaMorphClip> morphClipList = Editor.Parser.MorphSection.ClipList;
				for (int i = 0; i < morphClipList.Count; i++)
				{
					xaMorphClip morphClip = morphClipList[i];
					TreeNode morphClipNode = new TreeNode(morphClip.Name + " [" + morphClip.MeshName + "]");
					morphClipNode.Checked = true;
					morphClipNode.Tag = morphClip;
					//*section3cItem.viewItems.Add(animationSetNode);
					treeViewMorphClip.Nodes.Add(morphClipNode);

					List<xaMorphKeyframeRef> morphKeyframeRefList = morphClip.KeyframeRefList;
					for (int j = 0; j < morphKeyframeRefList.Count; j++)
					{
						xaMorphKeyframeRef morphKeyframeRef = morphKeyframeRefList[j];
						TreeNode morphKeyframeRefNode = new TreeNode(morphKeyframeRef.Name);
						morphKeyframeRefNode.Tag = morphKeyframeRef;
						//*animation.viewItems.Add(animationNode);
						morphClipNode.Nodes.Add(morphKeyframeRefNode);
					}
				}
				prevMorphKeyframeNodes = new TreeNode[morphClipList.Count];
				tabPageMorph.Text = "Morph [" + morphClipList.Count + "]";

				tabControlXA.SelectedTab = tabPageMorph;
			}

			if (Editor.Parser.AnimationSection != null)
			{
				animationSet = CreateAnimationSet();
				if (animationSet != null)
				{
					animationId = Gui.Renderer.AddAnimationSet(animationSet);

					renderTimer.Interval = 10;
					renderTimer.Tick += new EventHandler(renderTimer_Tick);
					Play();
				}

				List<xaAnimationClip> animationClipList = Editor.Parser.AnimationSection.ClipList;
				createAnimationClipListView(animationClipList, listViewAnimationClip);
				tabPageAnimation.Text = "Animation [" + listViewAnimationClip.Items.Count + "]";

				List<xaAnimationTrack> animationTrackList = Editor.Parser.AnimationSection.TrackList;
				createAnimationTrackListView(animationTrackList);
				animationSetMaxKeyframes(animationTrackList);

				tabControlXA.SelectedTab = tabPageAnimation;
				Gui.Renderer.RenderObjectAdded += new EventHandler(Renderer_RenderObjectAdded);
			}
			else
			{
				animationSetMaxKeyframes(null);
			}
		}

		private void Renderer_RenderObjectAdded(object sender, EventArgs e)
		{
			if (trackEnabled)
			{
				EnableTrack();
			}
			SetTrackPosition(trackPos);
			AdvanceTime(0);
		}

		private void animationSetMaxKeyframes(List<xaAnimationTrack> animationTrackList)
		{
			int max = 0;
			if (animationTrackList != null)
			{
				foreach (xaAnimationTrack animationTrack in animationTrackList)
				{
					int numKeyframes = animationTrack.KeyframeList.Count - 1;
					if (numKeyframes > max)
					{
						max = numKeyframes;
					}
				}
			}

			labelSkeletalRender.Text = "/ " + max;
			numericAnimationClipKeyframe.Maximum = max;
			trackBarAnimationClipKeyframe.Maximum = max;
			numericAnimationKeyframeStart.Maximum = max;
			numericAnimationKeyframeEnd.Maximum = max;
		}

		private void createAnimationTrackListView(List<xaAnimationTrack> animationTrackList)
		{
			if (animationTrackList.Count > 0)
			{
				listViewAnimationTrack.BeginUpdate();
				for (int i = 0; i < animationTrackList.Count; i++)
				{
					xaAnimationTrack track = animationTrackList[i];
					ListViewItem item = new ListViewItem(new string[] { track.Name, track.KeyframeList.Count.ToString() });
					item.Tag = track;
					listViewAnimationTrack.Items.Add(item);
				}
				listViewAnimationTrack.EndUpdate();
			}
		}

		public static void createAnimationClipListView(List<xaAnimationClip> clipList, ListView clipListView)
		{
			int clipMax = -1;
			for (int i = 0; i < clipList.Count; i++)
			{
				xaAnimationClip clip = clipList[i];
				if ((clip.Name != String.Empty) || (clip.Start != 0) || (clip.End != 0) || (clip.Next != 0))
				{
					if (i > clipMax)
					{
						clipMax = i;
					}
				}
				if (clip.Next > clipMax)
				{
					clipMax = clip.Next;
				}
			}

			clipListView.BeginUpdate();
			for (int i = 0; i <= clipMax; i++)
			{
				xaAnimationClip clip = clipList[i];
				ListViewItem item = new ListViewItem(new string[] { i.ToString(), clip.Name, clip.Start.ToString(), clip.End.ToString(), clip.Next.ToString(), clip.Speed.ToString() });
				item.Tag = clip;
				clipListView.Items.Add(item);
			}
			clipListView.EndUpdate();
		}

		KeyframedAnimationSet CreateAnimationSet()
		{
			var trackList = Editor.Parser.AnimationSection.TrackList;
			if ((trackList == null) || (trackList.Count <= 0))
			{
				return null;
			}

			KeyframedAnimationSet set = new KeyframedAnimationSet("SetName", 1, PlaybackType.Once, trackList.Count, new CallbackKey[0]);
			for (int i = 0; i < trackList.Count; i++)
			{
				var track = trackList[i];
				var keyframes = track.KeyframeList;
				ScaleKey[] scaleKeys = new ScaleKey[keyframes.Count];
				RotationKey[] rotationKeys = new RotationKey[keyframes.Count];
				TranslationKey[] translationKeys = new TranslationKey[keyframes.Count];
				set.RegisterAnimationKeys(track.Name, scaleKeys, rotationKeys, translationKeys);
				for (int j = 0; j < keyframes.Count; j++)
				{
					float time = keyframes[j].Index;

					ScaleKey scale = new ScaleKey();
					scale.Time = time;
					scale.Value = keyframes[j].Scaling;
					//scaleKeys[j] = scale;
					set.SetScaleKey(i, j, scale);

					RotationKey rotation = new RotationKey();
					rotation.Time = time;
					rotation.Value = Quaternion.Invert(keyframes[j].Rotation);
					//rotationKeys[j] = rotation;
					set.SetRotationKey(i, j, rotation);

					TranslationKey translation = new TranslationKey();
					translation.Time = time;
					translation.Value = keyframes[j].Translation;
					//translationKeys[j] = translation;
					set.SetTranslationKey(i, j, translation);
				}
			}

			return set;
		}

		void SetTrackPosition(double position)
		{
			Gui.Renderer.SetTrackPosition(animationId, position);
			trackPos = position;
		}

		void AdvanceTime(double time)
		{
			Gui.Renderer.AdvanceTime(animationId, time, null);
			trackPos += time;
		}

		public void Play()
		{
			if (loadedAnimationClip != null)
			{
				var clip = (xaAnimationClip)loadedAnimationClip.Tag;
				if (trackPos < clip.Start)
				{
					SetTrackPosition(clip.Start);
					AdvanceTime(0);
				}
			}

			this.play = true;
			this.startTime = DateTime.Now;
			renderTimer.Start();
			buttonAnimationClipPlayPause.ImageIndex = 1;
		}

		public void Pause()
		{
			this.play = false;
			renderTimer.Stop();
			buttonAnimationClipPlayPause.ImageIndex = 0;
		}

		public void AnimationSetClip(int idx)
		{
			bool play = this.play;
			Pause();

			if (loadedAnimationClip != null)
			{
				listViewAnimationClip.ItemSelectionChanged -= new ListViewItemSelectionChangedEventHandler(listViewAnimationClip_ItemSelectionChanged);
				loadedAnimationClip.Selected = false;
				listViewAnimationClip.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(listViewAnimationClip_ItemSelectionChanged);
			}

			if (idx < 0)
			{
				loadedAnimationClip = null;
				DisableTrack();
			}
			else
			{
				loadedAnimationClip = listViewAnimationClip.Items[idx];
				var clip = (xaAnimationClip)loadedAnimationClip.Tag;
				EnableTrack();
				SetTrackPosition(clip.Start);
				AdvanceTime(0);

				listViewAnimationClip.ItemSelectionChanged -= new ListViewItemSelectionChangedEventHandler(listViewAnimationClip_ItemSelectionChanged);
				loadedAnimationClip.Selected = true;
				loadedAnimationClip.EnsureVisible();
				listViewAnimationClip.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(listViewAnimationClip_ItemSelectionChanged);

				SetKeyframeNum((int)clip.Start);
			}

			if (play)
			{
				Play();
			}
		}

		private void EnableTrack()
		{
			Gui.Renderer.EnableTrack(animationId);
			trackEnabled = true;
		}

		private void DisableTrack()
		{
			Gui.Renderer.DisableTrack(animationId);
			trackEnabled = false;
		}

		private void SetKeyframeNum(int num)
		{
			if ((num >= 0) && (num <= numericAnimationClipKeyframe.Maximum))
			{
				userTrackBar = false;
				numericAnimationClipKeyframe.Value = num;
				trackBarAnimationClipKeyframe.Value = num;
				userTrackBar = true;
			}
		}

		private void listViewAnimationClip_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.IsSelected)
			{
				AnimationSetClip(e.Item.Index);
			}
			else
			{
				if (loadedAnimationClip == e.Item)
				{
					AnimationSetClip(-1);
				}
			}
		}

		private void renderTimer_Tick(object sender, EventArgs e)
		{
			if (play && (loadedAnimationClip != null))
			{
				TimeSpan elapsedTime = DateTime.Now - this.startTime;
				if (elapsedTime.TotalSeconds > 0)
				{
					double advanceTime = elapsedTime.TotalSeconds * AnimationSpeed;
					xaAnimationClip clip = (xaAnimationClip)loadedAnimationClip.Tag;
					if ((trackPos + advanceTime) >= clip.End)
					{
						if (FollowSequence && (clip.Next != 0) && (clip.Next != loadedAnimationClip.Index))
						{
							AnimationSetClip(clip.Next);
						}
						else
						{
							SetTrackPosition(clip.Start);
							AdvanceTime(0);
						}
					}
					else
					{
						AdvanceTime(advanceTime);
					}

					SetKeyframeNum((int)trackPos);
					this.startTime = DateTime.Now;
				}
			}
		}

		private void checkBoxAnimationClipLoadNextClip_CheckedChanged(object sender, EventArgs e)
		{
			FollowSequence = checkBoxAnimationClipLoadNextClip.Checked;
		}

		private void numericAnimationClipSpeed_ValueChanged(object sender, EventArgs e)
		{
			AnimationSpeed = Decimal.ToSingle(numericAnimationClipSpeed.Value);
		}

		private void buttonAnimationClipPlayPause_Click(object sender, EventArgs e)
		{
			if (this.play)
			{
				Pause();
			}
			else
			{
				Play();
			}
		}

		private void trackBarAnimationClipKeyframe_ValueChanged(object sender, EventArgs e)
		{
			if (userTrackBar && (Editor.Parser.AnimationSection != null))
			{
				Pause();

				if (!trackEnabled)
				{
					EnableTrack();
				}
				SetTrackPosition(Decimal.ToDouble(trackBarAnimationClipKeyframe.Value));
				AdvanceTime(0);

				userTrackBar = false;
				numericAnimationClipKeyframe.Value = trackBarAnimationClipKeyframe.Value;
				userTrackBar = true;
			}
		}

		private void numericAnimationClipKeyframe_ValueChanged(object sender, EventArgs e)
		{
			if (userTrackBar && (Editor.Parser.AnimationSection != null))
			{
				Pause();

				if (!trackEnabled)
				{
					EnableTrack();
				}
				SetTrackPosition((double)numericAnimationClipKeyframe.Value);
				AdvanceTime(0);

				userTrackBar = false;
				trackBarAnimationClipKeyframe.Value = Decimal.ToInt32(numericAnimationClipKeyframe.Value);
				userTrackBar = true;
			}
		}
	}
}
