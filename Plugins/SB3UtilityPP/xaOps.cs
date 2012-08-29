using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SlimDX;

namespace SB3Utility
{
	public static class xa
	{
		public static xaMorphIndexSet FindMorphIndexSet(string name, xaMorphSection section)
		{
			for (int i = 0; i < section.IndexSetList.Count; i++)
			{
				if (section.IndexSetList[i].Name == name)
				{
					return section.IndexSetList[i];
				}
			}
			return null;
		}

		public static xaMorphKeyframe FindMorphKeyFrame(string name, xaMorphSection section)
		{
			for (int i = 0; i < section.KeyframeList.Count; i++)
			{
				if (section.KeyframeList[i].Name == name)
				{
					return section.KeyframeList[i];
				}
			}
			return null;
		}

		public static int MorphMeshObjIdx(ushort[] meshIndices, xxMesh mesh)
		{
			int meshObjIdx = -1;
			if (mesh.SubmeshList.Count > 0)
			{
				if (mesh.SubmeshList.Count == 1)
				{
					if (ValidIndices(meshIndices, mesh.SubmeshList[0].VertexList))
					{
						meshObjIdx = 0;
					}
				}
				else
				{
					float maxModified = 0;
					for (int i = 0; i < mesh.SubmeshList.Count; i++)
					{
						if (ValidIndices(meshIndices, mesh.SubmeshList[i].VertexList))
						{
							float modified = (float)meshIndices.Length / mesh.SubmeshList[i].VertexList.Count;
							if (modified > maxModified)
							{
								maxModified = modified;
								meshObjIdx = i;
							}
						}
					}
				}
			}
			return meshObjIdx;
		}

		static bool ValidIndices(ushort[] meshIndices, List<xxVertex> vertList)
		{
			bool valid = true;
			foreach (ushort index in meshIndices)
			{
				if (index >= vertList.Count)
				{
					valid = false;
					break;
				}
			}
			return valid;
		}
	}
}
