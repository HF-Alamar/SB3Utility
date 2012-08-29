using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public class xaParser : IWriteFile
	{
		public byte[] Header { get; set; }
		public xaMaterialSection MaterialSection { get; set; }
		public xaSection2 Section2 { get; set; }
		public xaMorphSection MorphSection { get; set; }
		public xaSection4 Section4 { get; set; }
		public xaAnimationSection AnimationSection { get; set; }
		public byte[] Footer { get; set; }

		public int Format { get; protected set; }
		public string Name { get; set; }

		protected BinaryReader reader;

		public xaParser(Stream stream, string name)
			: this(stream)
		{
			this.Name = name;
		}

		public xaParser(Stream stream)
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				this.reader = reader;

				byte type = reader.ReadByte();
				if (type == 0x00)
				{
					Format = -1;
					Section2 = ParseSection2();
					MorphSection = ParseMorphSection();
					AnimationSection = ParseAnimationSection();
				}
				else if ((type == 0x02) || (type == 0x03))
				{
					Header = new byte[5];
					Header[0] = type;
					reader.ReadBytes(4).CopyTo(Header, 1);
					ParseSB3Format();
				}
				else if (type == 0x01)
				{
					byte[] testBuf = reader.ReadBytes(4);
					if ((testBuf[0] | testBuf[1] | testBuf[2]) == 0)
					{
						Header = new byte[5];
						Header[0] = type;
						testBuf.CopyTo(Header, 1);
						ParseSB3Format();
					}
					else
					{
						Format = -1;
						MaterialSection = ParseMaterialSection(BitConverter.ToInt32(testBuf, 0));
						Section2 = ParseSection2();
						MorphSection = ParseMorphSection();
						AnimationSection = ParseAnimationSection();
					}
				}
				else
				{
					throw new Exception("Unable to determine .xa format");
				}

				Footer = reader.ReadToEnd();

				this.reader = null;
			}
		}

		public void WriteTo(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			if (Header != null)
			{
				writer.Write(Header);
			}

			WriteSection(stream, MaterialSection);
			WriteSection(stream, Section2);
			WriteSection(stream, MorphSection);
			if (Format >= 0x02)
			{
				WriteSection(stream, Section4);
			}
			WriteSection(stream, AnimationSection);

			writer.Write(Footer);
		}

		protected void WriteSection(Stream stream, IObjInfo section)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			if (section == null)
			{
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)1);
				section.WriteTo(stream);
			}
		}

		protected void ParseSB3Format()
		{
			Format = BitConverter.ToInt32(Header, 0);
			MaterialSection = ParseMaterialSection();
			Section2 = ParseSection2();
			MorphSection = ParseMorphSection();
			if (Format >= 0x02)
			{
				Section4 = ParseSection4();
			}
			AnimationSection = ParseAnimationSection();
		}

		protected xaMaterialSection ParseMaterialSection()
		{
			xaMaterialSection section = null;
			if (reader.ReadByte() == 1)
			{
				section = ParseMaterialSection(reader.ReadInt32());
			}
			return section;
		}

		protected xaMaterialSection ParseMaterialSection(int numMaterials)
		{
			xaMaterialSection section = new xaMaterialSection();
			section.MaterialList = new List<xaMaterial>(numMaterials);
			for (int i = 0; i < numMaterials; i++)
			{
				xaMaterial mat = new xaMaterial();
				section.MaterialList.Add(mat);
				mat.Name = reader.ReadName();

				int numColors = reader.ReadInt32();
				mat.ColorList = new List<xaMaterialColor>(numColors);
				for (int j = 0; j < numColors; j++)
				{
					xaMaterialColor color = new xaMaterialColor();
					mat.ColorList.Add(color);

					color.Diffuse = reader.ReadColor4();
					color.Ambient = reader.ReadColor4();
					color.Specular = reader.ReadColor4();
					color.Emissive = reader.ReadColor4();
					color.Power = reader.ReadSingle();
					color.Unknown1 = reader.ReadBytes(4);
				}
			}
			return section;
		}

		protected xaSection2 ParseSection2()
		{
			if (reader.ReadByte() == 0)
			{
				return null;
			}

			xaSection2 section = new xaSection2();

			int numItems = reader.ReadInt32();
			section.ItemList = new List<xaSection2Item>(numItems);
			for (int i = 0; i < numItems; i++)
			{
				xaSection2Item item = new xaSection2Item();
				section.ItemList.Add(item);

				item.Unknown1 = reader.ReadBytes(4);
				item.Name = reader.ReadName();
				item.Unknown2 = reader.ReadBytes(4);
				item.Unknown3 = reader.ReadBytes(4);

				int numItemBlocks = reader.ReadInt32();
				item.Unknown4 = reader.ReadBytes(4);
				item.ItemBlockList = new List<xaSection2ItemBlock>(numItemBlocks);
				for (int j = 0; j < numItemBlocks; j++)
				{
					xaSection2ItemBlock itemBlock = new xaSection2ItemBlock();
					item.ItemBlockList.Add(itemBlock);

					itemBlock.Unknown1 = reader.ReadBytes(1);
					itemBlock.Unknown2 = reader.ReadBytes(4);
				}

				item.Unknown5 = reader.ReadBytes(1);
			}

			return section;
		}

		protected xaMorphSection ParseMorphSection()
		{
			if (reader.ReadByte() == 0)
			{
				return null;
			}

			xaMorphSection section = new xaMorphSection();

			int numIndexSets = reader.ReadInt32();
			section.IndexSetList = new List<xaMorphIndexSet>(numIndexSets);
			for (int i = 0; i < numIndexSets; i++)
			{
				xaMorphIndexSet indexSet = new xaMorphIndexSet();
				section.IndexSetList.Add(indexSet);

				indexSet.Unknown1 = reader.ReadBytes(1);

				int numVertices = reader.ReadInt32();
				indexSet.MeshIndices = reader.ReadUInt16Array(numVertices);
				indexSet.MorphIndices = reader.ReadUInt16Array(numVertices);

				indexSet.Name = reader.ReadName();
			}

			int numKeyframes = reader.ReadInt32();
			section.KeyframeList = new List<xaMorphKeyframe>(numKeyframes);
			for (int i = 0; i < numKeyframes; i++)
			{
				xaMorphKeyframe keyframe = new xaMorphKeyframe();
				section.KeyframeList.Add(keyframe);

				int numVertices = reader.ReadInt32();
				keyframe.PositionList = new List<Vector3>(numVertices);
				keyframe.NormalList = new List<Vector3>(numVertices);
				for (int j = 0; j < numVertices; j++)
				{
					keyframe.PositionList.Add(reader.ReadVector3());
				}
				for (int j = 0; j < numVertices; j++)
				{
					keyframe.NormalList.Add(reader.ReadVector3());
				}

				keyframe.Name = reader.ReadName();
			}

			int numClips = reader.ReadInt32();
			section.ClipList = new List<xaMorphClip>(numClips);
			for (int i = 0; i < numClips; i++)
			{
				xaMorphClip clip = new xaMorphClip();
				section.ClipList.Add(clip);

				clip.MeshName = reader.ReadName();
				clip.Name = reader.ReadName();

				int numKeyframeRefs = reader.ReadInt32();
				clip.KeyframeRefList = new List<xaMorphKeyframeRef>(numKeyframeRefs);
				for (int j = 0; j < numKeyframeRefs; j++)
				{
					xaMorphKeyframeRef keyframeRef = new xaMorphKeyframeRef();
					clip.KeyframeRefList.Add(keyframeRef);

					keyframeRef.Unknown1 = reader.ReadBytes(1);
					keyframeRef.Index = reader.ReadInt32();
					keyframeRef.Unknown2 = reader.ReadBytes(1);
					keyframeRef.Name = reader.ReadName();
				}

				clip.Unknown1 = reader.ReadBytes(4);
			}

			return section;
		}

		protected xaSection4 ParseSection4()
		{
			if (reader.ReadByte() == 0)
			{
				return null;
			}

			xaSection4 section = new xaSection4();

			int numItemLists = reader.ReadInt32();
			section.ItemListList = new List<List<xaSection4Item>>(numItemLists);
			for (int i = 0; i < numItemLists; i++)
			{
				int numItems = reader.ReadInt32();
				List<xaSection4Item> itemList = new List<xaSection4Item>(numItems);
				section.ItemListList.Add(itemList);

				for (int j = 0; j < numItems; j++)
				{
					xaSection4Item item = new xaSection4Item();
					item.Unknown1 = reader.ReadBytes(104);
					item.Unknown2 = reader.ReadBytes(4);
					item.Unknown3 = reader.ReadBytes(64);
				}
			}

			return section;
		}

		protected xaAnimationSection ParseAnimationSection()
		{
			if (reader.ReadByte() == 0)
			{
				return null;
			}

			xaAnimationSection section = new xaAnimationSection();

			int numClips;
			if (Format == 0x03)
			{
				numClips = 1024;
			}
			else
			{
				numClips = 512;
			}

			section.ClipList = new List<xaAnimationClip>(numClips);
			for (int i = 0; i < numClips; i++)
			{
				xaAnimationClip clip = new xaAnimationClip();
				section.ClipList.Add(clip);

				clip.Name = reader.ReadName(64);
				clip.Speed = reader.ReadSingle();
				clip.Unknown1 = reader.ReadBytes(4);
				clip.Start = reader.ReadSingle();
				clip.End = reader.ReadSingle();
				clip.Unknown2 = reader.ReadBytes(1);
				clip.Unknown3 = reader.ReadBytes(1);
				clip.Unknown4 = reader.ReadBytes(1);
				clip.Next = reader.ReadInt32();
				clip.Unknown5 = reader.ReadBytes(1);
				clip.Unknown6 = reader.ReadBytes(4);
				clip.Unknown7 = reader.ReadBytes(16);
			}

			int numTracks = reader.ReadInt32();
			section.TrackList = new List<xaAnimationTrack>(numTracks);
			for (int i = 0; i < numTracks; i++)
			{
				xaAnimationTrack track = new xaAnimationTrack();
				section.TrackList.Add(track);

				track.Name = reader.ReadName();
				int numKeyframes = reader.ReadInt32();
				track.Unknown1 = reader.ReadBytes(4);

				track.KeyframeList = new List<xaAnimationKeyframe>(numKeyframes);
				for (int j = 0; j < numKeyframes; j++)
				{
					xaAnimationKeyframe keyframe = new xaAnimationKeyframe();
					track.KeyframeList.Add(keyframe);

					keyframe.Index = reader.ReadInt32();
					keyframe.Rotation = reader.ReadQuaternion();
					keyframe.Unknown1 = reader.ReadBytes(8);
					keyframe.Translation = reader.ReadVector3();
					keyframe.Scaling = reader.ReadVector3();
				}
			}

			return section;
		}
	}
}
