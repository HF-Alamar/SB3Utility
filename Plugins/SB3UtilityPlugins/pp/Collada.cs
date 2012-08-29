using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using COLLADA;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		public static void ExportDae([DefaultVar]string path, xxParser xxparser, List<xxFrame> meshFrames, List<xaParser> xaparsers, bool allFrames)
		{
			Collada.Exporter.Export(path, xxparser, meshFrames, xaparsers, allFrames);
		}

		[Plugin]
		public static Collada.Importer ImportDae([DefaultVar]string path)
		{
			return new Collada.Importer(path);
		}
	}

	public class Collada
	{
		private const float FPS = 24;

		public class Exporter
		{
			private const string uri = @"http://www.collada.org/2005/11/COLLADASchema";
			private XmlDocument doc = new XmlDocument();
			private XmlElement[] libraries = new XmlElement[7];

			private HashSet<string> frameNames = new HashSet<string>();
			private HashSet<string> meshNames = new HashSet<string>();

			private enum LibraryIdx
			{
				Images, Materials, Effects, Geometries, Controllers, Animations, Visual_Scenes
			}

			public static void Export(string path, xxParser xxParser, List<xxFrame> meshParents, List<xaParser> xaSubfileList, bool allFrames)
			{
				Exporter exporter = new Exporter(xxParser, meshParents, allFrames);
				exporter.Export(path, xxParser, xaSubfileList);
			}

			private Exporter(xxParser xxParser, List<xxFrame> meshParents, bool allFrames)
			{
				try
				{
					if (xxParser != null)
					{
						foreach (var frame in meshParents)
						{
							meshNames.Add(frame.Name);
						}

						if (!allFrames)
						{
							frameNames = xx.SearchHierarchy(xxParser.Frame, meshNames);
						}
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error initializing Collada exporter: " + e.Message);
				}
			}

			private void Export(string path, xxParser xxParser, List<xaParser> xaSubfileList)
			{
				try
				{
					XmlElement collada = doc.CreateElement("COLLADA", uri);
					collada.SetAttribute("version", "1.4.1");
					doc.AppendChild(collada);

					XmlElement asset = doc.CreateElement("asset", uri);
					collada.AppendChild(asset);

					DateTime creationTime = DateTime.Now.ToUniversalTime();
					string creationTimeString = creationTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
					asset.AppendChild(doc.CreateElement("created", uri)).InnerText = creationTimeString;
					asset.AppendChild(doc.CreateElement("modified", uri)).InnerText = creationTimeString;
					asset.AppendChild(doc.CreateElement("up_axis", uri)).InnerText = "Y_UP";

					if (xaSubfileList.Count > 0)
					{
						libraries[(int)LibraryIdx.Animations] = doc.CreateElement("library_animations", uri);
						Dictionary<string, int> animationNameDic = new Dictionary<string, int>();
						foreach (var subfile in xaSubfileList)
						{
							xaParser parser = subfile;
							var animationNodeList = parser.AnimationSection.TrackList;

							int animationNameIdx;
							if (animationNameDic.TryGetValue(subfile.Name, out animationNameIdx))
							{
								animationNameIdx++;
								animationNameDic[subfile.Name] = animationNameIdx;
								ExportAnimation(animationNodeList, subfile.Name + animationNameIdx);
							}
							else
							{
								animationNameDic.Add(subfile.Name, 1);
								ExportAnimation(animationNodeList, subfile.Name);
							}
						}
					}

					if (xxParser != null)
					{
						libraries[(int)LibraryIdx.Images] = doc.CreateElement("library_images", uri);
						libraries[(int)LibraryIdx.Materials] = doc.CreateElement("library_materials", uri);
						libraries[(int)LibraryIdx.Effects] = doc.CreateElement("library_effects", uri);
						libraries[(int)LibraryIdx.Geometries] = doc.CreateElement("library_geometries", uri);
						libraries[(int)LibraryIdx.Controllers] = doc.CreateElement("library_controllers", uri);
						libraries[(int)LibraryIdx.Visual_Scenes] = doc.CreateElement("library_visual_scenes", uri);

						XmlElement visualScene = doc.CreateElement("visual_scene", uri);
						visualScene.SetAttribute("id", "RootNode");
						visualScene.SetAttribute("name", "RootNode");
						libraries[(int)LibraryIdx.Visual_Scenes].AppendChild(visualScene);

						List<xxFrame> meshFrames = new List<xxFrame>();
						List<XmlElement> meshNodes = new List<XmlElement>();
						ExportFrame(visualScene, xxParser.Frame, meshFrames, meshNodes);

						SetJoints(xxParser, visualScene);

						List<xxMaterial> usedMaterials = new List<xxMaterial>(xxParser.MaterialList.Count);
						for (int i = 0; i < meshFrames.Count; i++)
						{
							ExportMesh(meshFrames[i], meshNodes[i], xxParser, usedMaterials);
						}
						List<xxTexture> usedTextures = new List<xxTexture>(xxParser.TextureList.Count);
						for (int i = 0; i < usedMaterials.Count; i++)
						{
							ExportMaterial(usedMaterials[i], xxParser, usedTextures);
						}
						foreach (xxTexture tex in usedTextures)
						{
							ExportTexture(tex, xxParser);
							xx.ExportTexture(tex, Path.GetDirectoryName(path) + @"\" + tex.Name);
						}
					}
					for (int i = 0; i < libraries.Length; i++)
					{
						if ((libraries[i] != null) && (libraries[i].ChildNodes.Count > 0))
						{
							collada.AppendChild(libraries[i]);
						}
					}
					if (xxParser != null)
					{
						XmlElement scene = doc.CreateElement("scene", uri);
						collada.AppendChild(scene);
						XmlElement sceneInstance = doc.CreateElement("instance_visual_scene", uri);
						sceneInstance.SetAttribute("url", "#RootNode");
						scene.AppendChild(sceneInstance);
					}

					DirectoryInfo destDir = new DirectoryInfo(Path.GetDirectoryName(path));
					if (!destDir.Exists)
					{
						destDir.Create();
					}
					using (XmlTextWriter xmlWriter = new XmlTextWriter(path, Encoding.UTF8))
					{
						xmlWriter.Formatting = Formatting.Indented;
						doc.Save(xmlWriter);
					}
					Report.ReportLog("Finished exporting to " + path);
				}
				catch (Exception e)
				{
					Report.ReportLog("Error exporting to Collada: " + e.Message);
				}
			}

			private void SetJoints(xxParser parser, XmlElement visualScene)
			{
				HashSet<string> boneNames = new HashSet<string>();
				List<xxFrame> meshFrames = xx.FindMeshFrames(parser.Frame);
				foreach (xxFrame frame in meshFrames)
				{
					xxMesh meshList = frame.Mesh;
					List<xxBone> boneList = meshList.BoneList;
					for (int j = 0; j < boneList.Count; j++)
					{
						xxBone bone = boneList[j];
						string boneName = EncodeName(bone.Name);
						boneNames.Add(boneName);
					}
				}

				SetJointsNode(visualScene, boneNames);
			}

			private void SetJointsNode(XmlElement node, HashSet<string> boneNames)
			{
				string nodeName = node.GetAttribute("name");
				if (boneNames.Contains(nodeName))
				{
					SetJointsNodeRecursive(node);
				}
				else
				{
					foreach (XmlNode child in node.ChildNodes)
					{
						if (child is XmlElement)
						{
							SetJointsNode((XmlElement)child, boneNames);
						}
					}
				}
			}

			private void SetJointsNodeRecursive(XmlElement node)
			{
				node.SetAttribute("type", "JOINT");

				foreach (XmlNode child in node.ChildNodes)
				{
					if (child is XmlElement)
					{
						SetJointsNodeRecursive((XmlElement)child);
					}
				}
			}

			private static string EncodeName(string name)
			{
				string encoded = HttpUtility.UrlEncode(name, Encoding.UTF8);
				encoded = encoded.Replace('.', '_');
				return encoded;
			}

			private void ExportFrame(XmlElement parentNode, xxFrame frame, List<xxFrame> meshFrames, List<XmlElement> meshNodes)
			{
				string frameName = frame.Name;
				if ((frameNames == null) || frameNames.Contains(frameName))
				{
					string frameNameCleaned = EncodeName(frameName);
					XmlElement node = doc.CreateElement("node", uri);
					node.SetAttribute("id", frameNameCleaned);
					node.SetAttribute("name", frameNameCleaned);
					parentNode.AppendChild(node);

					Vector3 scalePart;
					Quaternion rotatePart;
					Vector3 translatePart;
					if (!frame.Matrix.Decompose(out scalePart, out rotatePart, out translatePart))
					{
						throw new Exception("Failed to decompose matrix");

					}
					Vector3 rotateVector = FbxUtility.QuaternionToEuler(rotatePart);

					XmlElement translate = doc.CreateElement("translate", uri);
					translate.SetAttribute("sid", "translate");
					translate.InnerText = translatePart.X.ToFloatString() + " " + translatePart.Y.ToFloatString() + " " + translatePart.Z.ToFloatString();
					node.AppendChild(translate);
					XmlElement rotateZ = doc.CreateElement("rotate", uri);
					rotateZ.SetAttribute("sid", "rotateZ");
					rotateZ.InnerText = "0 0 1 " + rotateVector.X.ToFloatString();
					node.AppendChild(rotateZ);
					XmlElement rotateY = doc.CreateElement("rotate", uri);
					rotateY.SetAttribute("sid", "rotateY");
					rotateY.InnerText = "0 1 0 " + rotateVector.Y.ToFloatString();
					node.AppendChild(rotateY);
					XmlElement rotateX = doc.CreateElement("rotate", uri);
					rotateX.SetAttribute("sid", "rotateX");
					rotateX.InnerText = "1 0 0 " + rotateVector.Z.ToFloatString();
					node.AppendChild(rotateX);
					XmlElement scale = doc.CreateElement("scale", uri);
					scale.SetAttribute("sid", "scale");
					scale.InnerText = scalePart.X.ToFloatString() + " " + scalePart.Y.ToFloatString() + " " + scalePart.Z.ToFloatString();
					node.AppendChild(scale);

					if (meshNames.Contains(frameName) && (frame.Mesh != null))
					{
						meshFrames.Add(frame);
						meshNodes.Add(node);
					}

					for (int i = 0; i < frame.Count; i++)
					{
						ExportFrame(node, frame[i], meshFrames, meshNodes);
					}
				}
			}

			private void ExportMesh(xxFrame frame, XmlElement parent, xxParser parser, List<xxMaterial> usedMaterials)
			{
				string frameName = EncodeName(frame.Name);
				xxMesh meshList = frame.Mesh;
				List<xxBone> boneList = meshList.BoneList;

				for (int i = 0; i < meshList.SubmeshList.Count; i++)
				{
					string meshName = frameName + "_" + i;
					xxSubmesh meshObj = meshList.SubmeshList[i];
					List<xxFace> faceList = meshObj.FaceList;
					List<xxVertex> vertexList = meshObj.VertexList;

					XmlElement geometry = doc.CreateElement("geometry", uri);
					geometry.SetAttribute("id", meshName + "-lib");
					geometry.SetAttribute("name", meshName + "Mesh");
					libraries[(int)LibraryIdx.Geometries].AppendChild(geometry);

					XmlElement mesh = doc.CreateElement("mesh", uri);
					geometry.AppendChild(mesh);

					// positions
					XmlElement positions = doc.CreateElement("source", uri);
					positions.SetAttribute("id", meshName + "-lib-Position");
					mesh.AppendChild(positions);

					XmlElement positionsArray = doc.CreateElement("float_array", uri);
					positionsArray.SetAttribute("id", meshName + "-lib-Position-array");
					positionsArray.SetAttribute("count", (vertexList.Count * 3).ToString());
					positions.AppendChild(positionsArray);

					XmlElement positionsTechnique = doc.CreateElement("technique_common", uri);
					positions.AppendChild(positionsTechnique);
					XmlElement positionsAccessor = doc.CreateElement("accessor", uri);
					positionsAccessor.SetAttribute("source", "#" + meshName + "-lib-Position-array");
					positionsAccessor.SetAttribute("count", vertexList.Count.ToString());
					positionsAccessor.SetAttribute("stride", "3");
					positionsTechnique.AppendChild(positionsAccessor);
					XmlElement positionsAccessorParamX = doc.CreateElement("param", uri);
					positionsAccessorParamX.SetAttribute("name", "X");
					positionsAccessorParamX.SetAttribute("type", "float");
					positionsAccessor.AppendChild(positionsAccessorParamX);
					XmlElement positionsAccessorParamY = doc.CreateElement("param", uri);
					positionsAccessorParamY.SetAttribute("name", "Y");
					positionsAccessorParamY.SetAttribute("type", "float");
					positionsAccessor.AppendChild(positionsAccessorParamY);
					XmlElement positionsAccessorParamZ = doc.CreateElement("param", uri);
					positionsAccessorParamZ.SetAttribute("name", "Z");
					positionsAccessorParamZ.SetAttribute("type", "float");
					positionsAccessor.AppendChild(positionsAccessorParamZ);

					// normals
					XmlElement normals = doc.CreateElement("source", uri);
					normals.SetAttribute("id", meshName + "-lib-Normal0");
					mesh.AppendChild(normals);

					XmlElement normalsArray = doc.CreateElement("float_array", uri);
					normalsArray.SetAttribute("id", meshName + "-lib-Normal0-array");
					normalsArray.SetAttribute("count", (vertexList.Count * 3).ToString());
					normals.AppendChild(normalsArray);

					XmlElement normalsTechnique = doc.CreateElement("technique_common", uri);
					normals.AppendChild(normalsTechnique);
					XmlElement normalsAccessor = doc.CreateElement("accessor", uri);
					normalsAccessor.SetAttribute("source", "#" + meshName + "-lib-Normal0-array");
					normalsAccessor.SetAttribute("count", vertexList.Count.ToString());
					normalsAccessor.SetAttribute("stride", "3");
					normalsTechnique.AppendChild(normalsAccessor);
					XmlElement normalsAccessorParamX = doc.CreateElement("param", uri);
					normalsAccessorParamX.SetAttribute("name", "X");
					normalsAccessorParamX.SetAttribute("type", "float");
					normalsAccessor.AppendChild(normalsAccessorParamX);
					XmlElement normalsAccessorParamY = doc.CreateElement("param", uri);
					normalsAccessorParamY.SetAttribute("name", "Y");
					normalsAccessorParamY.SetAttribute("type", "float");
					normalsAccessor.AppendChild(normalsAccessorParamY);
					XmlElement normalsAccessorParamZ = doc.CreateElement("param", uri);
					normalsAccessorParamZ.SetAttribute("name", "Z");
					normalsAccessorParamZ.SetAttribute("type", "float");
					normalsAccessor.AppendChild(normalsAccessorParamZ);

					// uvs
					XmlElement uvs = doc.CreateElement("source", uri);
					uvs.SetAttribute("id", meshName + "-lib-UV0");
					mesh.AppendChild(uvs);

					XmlElement uvsArray = doc.CreateElement("float_array", uri);
					uvsArray.SetAttribute("id", meshName + "-lib-UV0-array");
					uvsArray.SetAttribute("count", (vertexList.Count * 2).ToString());
					uvs.AppendChild(uvsArray);

					XmlElement uvsTechnique = doc.CreateElement("technique_common", uri);
					uvs.AppendChild(uvsTechnique);
					XmlElement uvsAccessor = doc.CreateElement("accessor", uri);
					uvsAccessor.SetAttribute("source", "#" + meshName + "-lib-UV0-array");
					uvsAccessor.SetAttribute("count", vertexList.Count.ToString());
					uvsAccessor.SetAttribute("stride", "2");
					uvsTechnique.AppendChild(uvsAccessor);
					XmlElement uvsAccessorParamS = doc.CreateElement("param", uri);
					uvsAccessorParamS.SetAttribute("name", "S");
					uvsAccessorParamS.SetAttribute("type", "float");
					uvsAccessor.AppendChild(uvsAccessorParamS);
					XmlElement uvsAccessorParamT = doc.CreateElement("param", uri);
					uvsAccessorParamT.SetAttribute("name", "T");
					uvsAccessorParamT.SetAttribute("type", "float");
					uvsAccessor.AppendChild(uvsAccessorParamT);

					// faces
					XmlElement vertices = doc.CreateElement("vertices", uri);
					vertices.SetAttribute("id", meshName + "-lib-Vertex");
					mesh.AppendChild(vertices);

					XmlElement verticesInput = doc.CreateElement("input", uri);
					verticesInput.SetAttribute("semantic", "POSITION");
					verticesInput.SetAttribute("source", "#" + meshName + "-lib-Position");
					vertices.AppendChild(verticesInput);

					XmlElement polygons = doc.CreateElement("polygons", uri);
					mesh.AppendChild(polygons);
					XmlElement polygonsVertex = doc.CreateElement("input", uri);
					polygonsVertex.SetAttribute("semantic", "VERTEX");
					polygonsVertex.SetAttribute("offset", "0");
					polygonsVertex.SetAttribute("source", "#" + meshName + "-lib-Vertex");
					polygons.AppendChild(polygonsVertex);
					XmlElement polygonsNormal = doc.CreateElement("input", uri);
					polygonsNormal.SetAttribute("semantic", "NORMAL");
					polygonsNormal.SetAttribute("offset", "1");
					polygonsNormal.SetAttribute("source", "#" + meshName + "-lib-Normal0");
					polygons.AppendChild(polygonsNormal);
					XmlElement polygonsUV = doc.CreateElement("input", uri);
					polygonsUV.SetAttribute("semantic", "TEXCOORD");
					polygonsUV.SetAttribute("offset", "2");
					polygonsUV.SetAttribute("set", "0");
					polygonsUV.SetAttribute("source", "#" + meshName + "-lib-UV0");
					polygons.AppendChild(polygonsUV);

					// text
					StringBuilder positionsString = new StringBuilder(12 * 3 * vertexList.Count);
					StringBuilder normalsString = new StringBuilder(12 * 3 * vertexList.Count);
					StringBuilder uvsString = new StringBuilder(12 * 2 * vertexList.Count);
					positionsString.AppendLine();
					normalsString.AppendLine();
					uvsString.AppendLine();
					for (int j = 0; j < vertexList.Count; j++)
					{
						xxVertex vert = vertexList[j];
						positionsString.Append(vert.Position[0].ToFloatString() + " ");
						positionsString.Append(vert.Position[1].ToFloatString() + " ");
						positionsString.AppendLine(vert.Position[2].ToFloatString());
						normalsString.Append(vert.Normal[0].ToFloatString() + " ");
						normalsString.Append(vert.Normal[1].ToFloatString() + " ");
						normalsString.AppendLine(vert.Normal[2].ToFloatString());
						uvsString.Append(vert.UV[0].ToFloatString() + " ");
						uvsString.AppendLine((1f - vert.UV[1]).ToFloatString());
					}
					positionsArray.InnerText = positionsString.ToString();
					normalsArray.InnerText = normalsString.ToString();
					uvsArray.InnerText = uvsString.ToString();

					for (int j = 0; j < faceList.Count; j++)
					{
						XmlElement polygonsP = doc.CreateElement("p", uri);
						polygons.AppendChild(polygonsP);

						xxFace face = faceList[j];
						StringBuilder polygonsPText = new StringBuilder(12 * 3 * 3);
						for (int m = 0; m < face.VertexIndices.Length; m++)
						{
							polygonsPText.Append(face.VertexIndices[m] + " " + face.VertexIndices[m] + " " + face.VertexIndices[m] + " ");
						}
						polygonsP.InnerText = polygonsPText.ToString(0, polygonsPText.Length - 1);
					}

					// mesh binding
					XmlElement meshNode = doc.CreateElement("node", uri);
					meshNode.SetAttribute("id", meshName);
					meshNode.SetAttribute("name", meshName);
					parent.AppendChild(meshNode);

					XmlElement nodeGeometry;
					bool skinned = (boneList.Count > 0);
					if (skinned)
					{
						nodeGeometry = doc.CreateElement("instance_controller", uri);
						nodeGeometry.SetAttribute("url", "#" + meshName + "Controller");
						meshNode.AppendChild(nodeGeometry);

						XmlElement controller = doc.CreateElement("controller", uri);
						controller.SetAttribute("id", meshName + "Controller");
						libraries[(int)LibraryIdx.Controllers].AppendChild(controller);

						XmlElement skin = doc.CreateElement("skin", uri);
						skin.SetAttribute("source", "#" + meshName + "-lib");
						controller.AppendChild(skin);

						Matrix combined = Matrix.Identity;
						/*ObjInfo parentFrame = frame;
						while (parentFrame is xxFrame)
						{
							combined = ((xxFrame)parentFrame).matrix.ToMatrix4() * combined;
							parentFrame = parentFrame.parent;
						}*/
						XmlElement bindMatrix = doc.CreateElement("bind_shape_matrix", uri);
						string bindMatrixStr = String.Empty;
						for (int j = 0; j < 4; j++)
						{
							for (int k = 0; k < 4; k++)
							{
								bindMatrixStr += combined[j, k].ToFloatString() + " ";
							}
						}
						bindMatrix.InnerText = bindMatrixStr.Substring(0, bindMatrixStr.Length - 1);
						skin.AppendChild(bindMatrix);

						XmlElement joints = doc.CreateElement("source", uri);
						joints.SetAttribute("id", meshName + "Controller-Joints");
						skin.AppendChild(joints);
						XmlElement jointsArray = doc.CreateElement("Name_array", uri);
						jointsArray.SetAttribute("id", meshName + "Controller-Joints-array");
						joints.AppendChild(jointsArray);
						XmlElement jointsTechnique = doc.CreateElement("technique_common", uri);
						joints.AppendChild(jointsTechnique);
						XmlElement jointsAccessor = doc.CreateElement("accessor", uri);
						jointsAccessor.SetAttribute("source", "#" + meshName + "Controller-Joints-array");
						jointsTechnique.AppendChild(jointsAccessor);
						XmlElement jointsParam = doc.CreateElement("param", uri);
						jointsParam.SetAttribute("type", "name");
						jointsAccessor.AppendChild(jointsParam);

						XmlElement bindPoses = doc.CreateElement("source", uri);
						bindPoses.SetAttribute("id", meshName + "Controller-Matrices");
						skin.AppendChild(bindPoses);
						XmlElement bindPosesArray = doc.CreateElement("float_array", uri);
						bindPosesArray.SetAttribute("id", meshName + "Controller-Matrices-array");
						bindPoses.AppendChild(bindPosesArray);
						XmlElement bindPosesTechnique = doc.CreateElement("technique_common", uri);
						bindPoses.AppendChild(bindPosesTechnique);
						XmlElement bindPosesAccessor = doc.CreateElement("accessor", uri);
						bindPosesAccessor.SetAttribute("source", "#" + meshName + "Controller-Matrices-array");
						bindPosesTechnique.AppendChild(bindPosesAccessor);
						XmlElement bindPosesParam = doc.CreateElement("param", uri);
						bindPosesParam.SetAttribute("type", "float4x4");
						bindPosesAccessor.AppendChild(bindPosesParam);

						XmlElement weights = doc.CreateElement("source", uri);
						weights.SetAttribute("id", meshName + "Controller-Weights");
						skin.AppendChild(weights);
						XmlElement weightsArray = doc.CreateElement("float_array", uri);
						weightsArray.SetAttribute("id", meshName + "Controller-Weights-array");
						weights.AppendChild(weightsArray);
						XmlElement weightsTechnique = doc.CreateElement("technique_common", uri);
						weights.AppendChild(weightsTechnique);
						XmlElement weightsAccessor = doc.CreateElement("accessor", uri);
						weightsAccessor.SetAttribute("source", "#" + meshName + "Controller-Weights-array");
						weightsTechnique.AppendChild(weightsAccessor);
						XmlElement weightsParam = doc.CreateElement("param", uri);
						weightsParam.SetAttribute("type", "float");
						weightsAccessor.AppendChild(weightsParam);

						XmlElement jointSemantic = doc.CreateElement("joints", uri);
						skin.AppendChild(jointSemantic);
						XmlElement jointSemanticJoint = doc.CreateElement("input", uri);
						jointSemanticJoint.SetAttribute("semantic", "JOINT");
						jointSemanticJoint.SetAttribute("source", "#" + meshName + "Controller-Joints");
						jointSemantic.AppendChild(jointSemanticJoint);
						XmlElement jointSemanticMatrix = doc.CreateElement("input", uri);
						jointSemanticMatrix.SetAttribute("semantic", "INV_BIND_MATRIX");
						jointSemanticMatrix.SetAttribute("source", "#" + meshName + "Controller-Matrices");
						jointSemantic.AppendChild(jointSemanticMatrix);

						XmlElement verticesArray = doc.CreateElement("vertex_weights", uri);
						verticesArray.SetAttribute("count", vertexList.Count.ToString());
						skin.AppendChild(verticesArray);
						XmlElement verticesJoint = doc.CreateElement("input", uri);
						verticesJoint.SetAttribute("semantic", "JOINT");
						verticesJoint.SetAttribute("offset", "0");
						verticesJoint.SetAttribute("source", "#" + meshName + "Controller-Joints");
						verticesArray.AppendChild(verticesJoint);
						XmlElement verticesWeight = doc.CreateElement("input", uri);
						verticesWeight.SetAttribute("semantic", "WEIGHT");
						verticesWeight.SetAttribute("offset", "1");
						verticesWeight.SetAttribute("source", "#" + meshName + "Controller-Weights");
						verticesArray.AppendChild(verticesWeight);
						XmlElement verticesVCount = doc.CreateElement("vcount", uri);
						verticesArray.AppendChild(verticesVCount);
						XmlElement verticesV = doc.CreateElement("v", uri);
						verticesArray.AppendChild(verticesV);

						StringBuilder weightsArrayText = new StringBuilder(12 * 4 * vertexList.Count);
						StringBuilder vCountText = new StringBuilder(12 * 4 * vertexList.Count);
						weightsArrayText.AppendLine();
						int[] numVertexWeights = new int[vertexList.Count];
						int totalWeights = 0;
						for (int j = 0; j < vertexList.Count; j++)
						{
							xxVertex vert = vertexList[j];
							byte[] indices = vert.BoneIndices;
							float[] weights4 = vert.Weights4(skinned);
							for (int k = 0; k < indices.Length; k++)
							{
								if ((indices[k] < boneList.Count) && (weights4[k] > 0))
								{
									weightsArrayText.Append(weights4[k].ToFloatString() + " ");
									numVertexWeights[j]++;
								}
							}
							if (numVertexWeights[j] > 0)
							{
								weightsArrayText.Remove(weightsArrayText.Length - 1, 1);
								weightsArrayText.AppendLine();
							}
							vCountText.Append(numVertexWeights[j] + " ");
							totalWeights += numVertexWeights[j];
						}
						weightsArray.SetAttribute("count", totalWeights.ToString());
						weightsArray.InnerText = weightsArrayText.ToString();
						weightsAccessor.SetAttribute("count", totalWeights.ToString());
						verticesVCount.InnerText = vCountText.ToString(0, vCountText.Length - 1);

						StringBuilder jointsArrayText = new StringBuilder(32 * boneList.Count);
						StringBuilder bindPosesArrayText = new StringBuilder(12 * 16 * boneList.Count);
						jointsArrayText.AppendLine();
						bindPosesArrayText.AppendLine();
						int totalBones = 0;
						int[] boneIdx = new int[boneList.Count];
						List<string> usedBoneNames = new List<string>(boneList.Count);
						for (int j = 0; j < boneList.Count; j++)
						{
							xxBone bone = boneList[j];
							string boneName = EncodeName(bone.Name);
							Matrix boneMatrix = bone.Matrix;
							for (int m = 0; m < 4; m++)
							{
								for (int n = 0; n < 4; n++)
								{
									bindPosesArrayText.Append(boneMatrix[m, n].ToFloatString() + " ");
								}
							}
							usedBoneNames.Add(boneName);
							bindPosesArrayText.Remove(bindPosesArrayText.Length - 1, 1);
							bindPosesArrayText.AppendLine();
							jointsArrayText.Append(boneName + " ");
							boneIdx[j] = totalBones;
							totalBones++;
						}

						jointsArray.SetAttribute("count", totalBones.ToString());
						if (jointsArrayText.Length > 0)
						{
							jointsArrayText.Remove(jointsArrayText.Length - 1, 1);
							jointsArrayText.AppendLine();
						}
						jointsArray.InnerText = jointsArrayText.ToString();
						jointsAccessor.SetAttribute("count", totalBones.ToString());
						bindPosesArray.SetAttribute("count", (totalBones * 16).ToString());
						bindPosesArray.InnerText = bindPosesArrayText.ToString();
						bindPosesAccessor.SetAttribute("count", totalBones.ToString());
						bindPosesAccessor.SetAttribute("stride", "16");

						StringBuilder vText = new StringBuilder(12 * 2 * vertexList.Count);
						int weightsIdx = 0;
						for (int j = 0; j < vertexList.Count; j++)
						{
							xxVertex vert = vertexList[j];
							byte[] indices = vert.BoneIndices;
							float[] weights4 = vert.Weights4(skinned);
							for (int k = 0; k < indices.Length; k++)
							{
								if ((indices[k] < boneList.Count) && (weights4[k] > 0))
								{
									vText.Append(boneIdx[indices[k]] + " " + weightsIdx + " ");
									weightsIdx++;
								}
							}
						}
						verticesV.InnerText = vText.ToString(0, vText.Length - 1);
					}
					else
					{
						nodeGeometry = doc.CreateElement("instance_geometry", uri);
						nodeGeometry.SetAttribute("url", "#" + meshName + "-lib");
						meshNode.AppendChild(nodeGeometry);
					}

					int matIdx = meshObj.MaterialIndex;
					if ((matIdx >= 0) && (matIdx < parser.MaterialList.Count))
					{
						xxMaterial mat = parser.MaterialList[matIdx];
						if (!usedMaterials.Contains(mat))
						{
							usedMaterials.Add(mat);
						}

						string matName = EncodeName(mat.Name);
						polygons.SetAttribute("material", matName);

						XmlElement nodeMaterial = doc.CreateElement("bind_material", uri);
						nodeGeometry.AppendChild(nodeMaterial);

						XmlElement nodeMaterialTechnique = doc.CreateElement("technique_common", uri);
						nodeMaterial.AppendChild(nodeMaterialTechnique);

						XmlElement nodeMaterialInstance = doc.CreateElement("instance_material", uri);
						nodeMaterialInstance.SetAttribute("symbol", matName);
						nodeMaterialInstance.SetAttribute("target", "#" + matName);
						nodeMaterialTechnique.AppendChild(nodeMaterialInstance);
					}

					polygons.SetAttribute("count", faceList.Count.ToString());
				}
			}

			private void ExportMaterial(xxMaterial mat, xxParser parser, List<xxTexture> usedTextures)
			{
				string matName = EncodeName(mat.Name);

				// material
				XmlElement material = doc.CreateElement("material", uri);
				material.SetAttribute("id", matName);
				material.SetAttribute("name", matName);
				libraries[(int)LibraryIdx.Materials].AppendChild(material);

				XmlElement materialEffect = doc.CreateElement("instance_effect", uri);
				materialEffect.SetAttribute("url", "#" + matName + "-fx");
				material.AppendChild(materialEffect);

				// effect
				XmlElement effect = doc.CreateElement("effect", uri);
				effect.SetAttribute("id", matName + "-fx");
				effect.SetAttribute("name", matName);
				libraries[(int)LibraryIdx.Effects].AppendChild(effect);

				XmlElement effectProfile = doc.CreateElement("profile_COMMON", uri);
				effect.AppendChild(effectProfile);

				XmlElement effectTechnique = doc.CreateElement("technique", uri);
				effectTechnique.SetAttribute("sid", "standard");
				// added at the end

				XmlElement effectPhong = doc.CreateElement("phong", uri);
				effectTechnique.AppendChild(effectPhong);

				XmlElement effectEmission = doc.CreateElement("emission", uri);
				effectPhong.AppendChild(effectEmission);
				XmlElement effectEmissionColor = doc.CreateElement("color", uri);
				effectEmissionColor.SetAttribute("sid", "emission");
				effectEmissionColor.InnerText = mat.Emissive.Red.ToFloatString() + " " + mat.Emissive.Green.ToFloatString() + " " + mat.Emissive.Blue.ToFloatString() + " " + mat.Emissive.Alpha.ToFloatString();
				effectEmission.AppendChild(effectEmissionColor);

				XmlElement effectAmbient = doc.CreateElement("ambient", uri);
				effectPhong.AppendChild(effectAmbient);
				XmlElement effectAmbientColor = doc.CreateElement("color", uri);
				effectAmbientColor.SetAttribute("sid", "ambient");
				effectAmbientColor.InnerText = mat.Ambient.Red.ToFloatString() + " " + mat.Ambient.Green.ToFloatString() + " " + mat.Ambient.Blue.ToFloatString() + " " + mat.Ambient.Alpha.ToFloatString();
				effectAmbient.AppendChild(effectAmbientColor);

				XmlElement effectDiffuse = doc.CreateElement("diffuse", uri);
				effectPhong.AppendChild(effectDiffuse);

				XmlElement effectSpecular = doc.CreateElement("specular", uri);
				effectPhong.AppendChild(effectSpecular);
				XmlElement effectSpecularColor = doc.CreateElement("color", uri);
				effectSpecularColor.SetAttribute("sid", "ambient");
				effectSpecularColor.InnerText = mat.Specular.Red.ToFloatString() + " " + mat.Specular.Green.ToFloatString() + " " + mat.Specular.Blue.ToFloatString() + " " + mat.Specular.Alpha.ToFloatString();
				effectSpecular.AppendChild(effectSpecularColor);

				XmlElement effectShininess = doc.CreateElement("shininess", uri);
				effectPhong.AppendChild(effectShininess);
				XmlElement effectShininessFloat = doc.CreateElement("float", uri);
				effectShininessFloat.SetAttribute("sid", "shininess");
				effectShininessFloat.InnerText = mat.Power.ToFloatString();
				effectShininess.AppendChild(effectShininessFloat);

				XmlElement effectReflective = doc.CreateElement("reflective", uri);
				effectPhong.AppendChild(effectReflective);
				XmlElement effectReflectiveColor = doc.CreateElement("color", uri);
				effectReflectiveColor.SetAttribute("sid", "reflective");
				effectReflectiveColor.InnerText = "0 0 0 1";
				effectReflective.AppendChild(effectReflectiveColor);

				XmlElement effectReflectivity = doc.CreateElement("reflectivity", uri);
				effectPhong.AppendChild(effectReflectivity);
				XmlElement effectReflectivityFloat = doc.CreateElement("float", uri);
				effectReflectivityFloat.SetAttribute("sid", "reflectivity");
				effectReflectivityFloat.InnerText = "0.5";
				effectReflectivity.AppendChild(effectReflectivityFloat);

				XmlElement effectTransparent = doc.CreateElement("transparent", uri);
				effectPhong.AppendChild(effectTransparent);
				XmlElement effectTransparentColor = doc.CreateElement("color", uri);
				effectTransparentColor.SetAttribute("sid", "transparent");
				effectTransparentColor.InnerText = "0 0 0 1";
				effectTransparent.AppendChild(effectTransparentColor);

				XmlElement effectTransparency = doc.CreateElement("transparency", uri);
				effectPhong.AppendChild(effectTransparency);
				XmlElement effectTransparencyFloat = doc.CreateElement("float", uri);
				effectTransparencyFloat.SetAttribute("sid", "transparency");
				effectTransparencyFloat.InnerText = "1";
				effectTransparency.AppendChild(effectTransparencyFloat);

				xxMaterialTexture matTex = mat.Textures[0];
				string matTexName = matTex.Name;
				if ((matTexName == null) || (matTexName == String.Empty))
				{
					XmlElement effectDiffuseColor = doc.CreateElement("color", uri);
					effectDiffuseColor.InnerText = mat.Diffuse.Red.ToFloatString() + " " + mat.Diffuse.Green.ToFloatString() + " " + mat.Diffuse.Blue.ToFloatString() + " " + mat.Diffuse.Alpha.ToFloatString();
					effectDiffuse.AppendChild(effectDiffuseColor);
				}
				else
				{
					xxTexture usedTex = null;
					foreach (xxTexture tex in parser.TextureList)
					{
						if (matTexName == tex.Name)
						{
							usedTex = tex;
							break;
						}
					}

					if (usedTex == null)
					{
						XmlElement effectDiffuseColor = doc.CreateElement("color", uri);
						effectDiffuseColor.InnerText = mat.Diffuse.Red.ToFloatString() + " " + mat.Diffuse.Green.ToFloatString() + " " + mat.Diffuse.Blue.ToFloatString() + " " + mat.Diffuse.Alpha.ToFloatString();
						effectDiffuse.AppendChild(effectDiffuseColor);
					}
					else
					{
						if (!usedTextures.Contains(usedTex))
						{
							usedTextures.Add(usedTex);
						}
						string texName = EncodeName(usedTex.Name);

						XmlElement effectDiffuseTexture = doc.CreateElement("texture", uri);
						effectDiffuseTexture.SetAttribute("texture", texName + "-image");
						effectDiffuseTexture.SetAttribute("texcoord", "CHANNEL0");
						effectDiffuse.AppendChild(effectDiffuseTexture);

						XmlElement effectDiffuseTextureExtra = doc.CreateElement("extra", uri);
						effectDiffuseTexture.AppendChild(effectDiffuseTextureExtra);

						XmlElement effectDiffuseTextureTechnique = doc.CreateElement("technique", uri);
						effectDiffuseTextureTechnique.SetAttribute("profile", "MAYA");
						effectDiffuseTextureExtra.AppendChild(effectDiffuseTextureTechnique);

						XmlElement effectDiffuseTextureWrapU = doc.CreateElement("wrapU", uri);
						effectDiffuseTextureWrapU.SetAttribute("sid", "wrapU0");
						effectDiffuseTextureWrapU.InnerText = "TRUE";
						effectDiffuseTextureTechnique.AppendChild(effectDiffuseTextureWrapU);

						XmlElement effectDiffuseTextureWrapV = doc.CreateElement("wrapV", uri);
						effectDiffuseTextureWrapV.SetAttribute("sid", "wrapV0");
						effectDiffuseTextureWrapV.InnerText = "TRUE";
						effectDiffuseTextureTechnique.AppendChild(effectDiffuseTextureWrapV);

						XmlElement effectDiffuseTextureBlend = doc.CreateElement("blend_mode", uri);
						effectDiffuseTextureBlend.InnerText = "NONE";
						effectDiffuseTextureTechnique.AppendChild(effectDiffuseTextureBlend);
					}
				}

				effectProfile.AppendChild(effectTechnique);
			}

			private void ExportTexture(xxTexture texture, xxParser parser)
			{
				string texName = EncodeName(texture.Name);

				// image
				XmlElement image = doc.CreateElement("image", uri);
				image.SetAttribute("id", texName + "-image");
				image.SetAttribute("name", texName);
				libraries[(int)LibraryIdx.Images].AppendChild(image);

				XmlElement imageInit = doc.CreateElement("init_from", uri);
				imageInit.InnerText = "file://" + texture.Name;
				image.AppendChild(imageInit);
			}

			private void ExportAnimation(List<xaAnimationTrack> animationList, string baseName)
			{
				XmlElement animationName = doc.CreateElement("animation", uri);
				animationName.SetAttribute("name", baseName);
				animationName.SetAttribute("id", baseName);
				libraries[(int)LibraryIdx.Animations].AppendChild(animationName);

				for (int i = 0; i < animationList.Count; i++)
				{
					xaAnimationTrack keyframeList = animationList[i];
					string usedFrame = EncodeName(keyframeList.Name);
					List<xaAnimationKeyframe> keyframes = keyframeList.KeyframeList;

					XmlElement translateXArray;
					XmlElement translateYArray;
					XmlElement translateZArray;
					XmlElement rotateXArray;
					XmlElement rotateYArray;
					XmlElement rotateZArray;
					XmlElement scaleXArray;
					XmlElement scaleYArray;
					XmlElement scaleZArray;
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "translate", "X", keyframes.Count, keyframes.Count, new string[] { "X" }, new string[] { "float" }, out translateXArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "translate", "Y", keyframes.Count, keyframes.Count, new string[] { "Y" }, new string[] { "float" }, out translateYArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "translate", "Z", keyframes.Count, keyframes.Count, new string[] { "Z" }, new string[] { "float" }, out translateZArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "rotateX", "ANGLE", keyframes.Count, keyframes.Count, new string[] { "ANGLE" }, new string[] { "float" }, out rotateXArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "rotateY", "ANGLE", keyframes.Count, keyframes.Count, new string[] { "ANGLE" }, new string[] { "float" }, out rotateYArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "rotateZ", "ANGLE", keyframes.Count, keyframes.Count, new string[] { "ANGLE" }, new string[] { "float" }, out rotateZArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "scale", "X", keyframes.Count, keyframes.Count, new string[] { "X" }, new string[] { "float" }, out scaleXArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "scale", "Y", keyframes.Count, keyframes.Count, new string[] { "Y" }, new string[] { "float" }, out scaleYArray));
					animationName.AppendChild(XmlAnimationTransform(usedFrame, "animation", "scale", "Z", keyframes.Count, keyframes.Count, new string[] { "Z" }, new string[] { "float" }, out scaleZArray));

					StringBuilder translateXString = new StringBuilder(12 * keyframes.Count);
					StringBuilder translateYString = new StringBuilder(12 * keyframes.Count);
					StringBuilder translateZString = new StringBuilder(12 * keyframes.Count);
					StringBuilder rotateXString = new StringBuilder(12 * keyframes.Count);
					StringBuilder rotateYString = new StringBuilder(12 * keyframes.Count);
					StringBuilder rotateZString = new StringBuilder(12 * keyframes.Count);
					StringBuilder scaleXString = new StringBuilder(12 * keyframes.Count);
					StringBuilder scaleYString = new StringBuilder(12 * keyframes.Count);
					StringBuilder scaleZString = new StringBuilder(12 * keyframes.Count);
					for (int j = 0; j < keyframes.Count; j++)
					{
						translateXString.Append(keyframes[j].Translation.X.ToFloatString() + " ");
						translateYString.Append(keyframes[j].Translation.Y.ToFloatString() + " ");
						translateZString.Append(keyframes[j].Translation.Z.ToFloatString() + " ");

						Vector3 rotation = FbxUtility.QuaternionToEuler(keyframes[j].Rotation);
						rotateXString.Append(rotation.X.ToFloatString() + " ");
						rotateYString.Append(rotation.Y.ToFloatString() + " ");
						rotateZString.Append(rotation.Z.ToFloatString() + " ");

						scaleXString.Append(keyframes[j].Scaling.X.ToFloatString() + " ");
						scaleYString.Append(keyframes[j].Scaling.Y.ToFloatString() + " ");
						scaleZString.Append(keyframes[j].Scaling.Z.ToFloatString() + " ");
					}
					translateXArray.InnerText = translateXString.ToString();
					translateYArray.InnerText = translateYString.ToString();
					translateZArray.InnerText = translateZString.ToString();
					rotateXArray.InnerText = rotateXString.ToString();
					rotateYArray.InnerText = rotateYString.ToString();
					rotateZArray.InnerText = rotateZString.ToString();
					scaleXArray.InnerText = scaleXString.ToString();
					scaleYArray.InnerText = scaleYString.ToString();
					scaleZArray.InnerText = scaleZString.ToString();
				}
			}

			private XmlElement XmlTechnique(string source, int count, string[] names, string[] types)
			{
				XmlElement technique = doc.CreateElement("technique_common", uri);
				XmlElement accessor = doc.CreateElement("accessor", uri);
				accessor.SetAttribute("source", "#" + source);
				accessor.SetAttribute("count", count.ToString());
				accessor.SetAttribute("stride", names.Length.ToString());
				technique.AppendChild(accessor);
				for (int i = 0; i < names.Length; i++)
				{
					XmlElement param = doc.CreateElement("param", uri);
					param.SetAttribute("name", names[i]);
					param.SetAttribute("type", types[i]);
					accessor.AppendChild(param);
				}
				return technique;
			}

			private XmlElement XmlAnimationTransform(string name, string midName, string transform, string transformExt, int count, int outCount, string[] outNames, string[] outTypes, out XmlElement outputArray)
			{
				string animationId = name + "-" + transform + "-" + midName;

				XmlElement animation = doc.CreateElement("animation", uri);
				animation.SetAttribute("id", name + "-" + transform + transformExt);

				XmlElement animationInput = doc.CreateElement("source", uri);
				animationInput.SetAttribute("id", animationId + "-input" + transformExt);
				animation.AppendChild(animationInput);
				XmlElement animationInputArray = doc.CreateElement("float_array", uri);
				animationInputArray.SetAttribute("id", animationId + "-input" + transformExt + "-array");
				animationInputArray.SetAttribute("count", count.ToString());
				animationInput.AppendChild(animationInputArray);
				animationInput.AppendChild(XmlTechnique(animationId + "-input" + transformExt + "-array", count, new string[] { "TIME" }, new string[] { "float" }));

				XmlElement animationOutput = doc.CreateElement("source", uri);
				animationOutput.SetAttribute("id", animationId + "-output" + transformExt);
				animation.AppendChild(animationOutput);
				XmlElement animationOutputArray = doc.CreateElement("float_array", uri);
				animationOutputArray.SetAttribute("id", animationId + "-output" + transformExt + "-array");
				animationOutputArray.SetAttribute("count", outCount.ToString());
				animationOutput.AppendChild(animationOutputArray);
				animationOutput.AppendChild(XmlTechnique(animationId + "-output" + transformExt + "-array", count, outNames, outTypes));

				XmlElement animationInterpolation = doc.CreateElement("source", uri);
				animationInterpolation.SetAttribute("id", animationId + "-interpolation" + transformExt);
				animation.AppendChild(animationInterpolation);
				XmlElement animationInterpolationArray = doc.CreateElement("Name_array", uri);
				animationInterpolationArray.SetAttribute("id", animationId + "-interpolation" + transformExt + "-array");
				animationInterpolationArray.SetAttribute("count", count.ToString());
				animationInterpolation.AppendChild(animationInterpolationArray);
				animationInterpolation.AppendChild(XmlTechnique(animationId + "-interpolation" + transformExt + "-array", count, new string[] { "INTERPOLATION" }, new string[] { "Name" }));

				XmlElement animationSampler = doc.CreateElement("sampler", uri);
				animationSampler.SetAttribute("id", animationId + transformExt);
				animation.AppendChild(animationSampler);
				XmlElement animationSamplerInput = doc.CreateElement("input", uri);
				animationSamplerInput.SetAttribute("semantic", "INPUT");
				animationSamplerInput.SetAttribute("source", "#" + animationId + "-input" + transformExt);
				animationSampler.AppendChild(animationSamplerInput);
				XmlElement animationSamplerOutput = doc.CreateElement("input", uri);
				animationSamplerOutput.SetAttribute("semantic", "OUTPUT");
				animationSamplerOutput.SetAttribute("source", "#" + animationId + "-output" + transformExt);
				animationSampler.AppendChild(animationSamplerOutput);
				XmlElement animationSamplerInterpolation = doc.CreateElement("input", uri);
				animationSamplerInterpolation.SetAttribute("semantic", "INTERPOLATION");
				animationSamplerInterpolation.SetAttribute("source", "#" + animationId + "-interpolation" + transformExt);
				animationSampler.AppendChild(animationSamplerInterpolation);

				XmlElement animationChannel = doc.CreateElement("channel", uri);
				animationChannel.SetAttribute("source", "#" + animationId + transformExt);
				animationChannel.SetAttribute("target", name + "/" + transform + "." + transformExt);
				animation.AppendChild(animationChannel);

				StringBuilder inputString = new StringBuilder(12 * count);
				StringBuilder interpolationString = new StringBuilder(7 * count);
				for (int i = 0; i < count; i++)
				{
					inputString.Append((i / FPS).ToFloatString() + " ");
					interpolationString.Append("LINEAR ");
				}
				animationInputArray.InnerText = inputString.ToString(0, inputString.Length - 1);
				animationInterpolationArray.InnerText = interpolationString.ToString(0, interpolationString.Length - 1);

				outputArray = animationOutputArray;
				return animation;
			}
		}

		public class Importer : IImported
		{
			public List<ImportedFrame> FrameList { get; protected set; }
			public List<ImportedMesh> MeshList { get; protected set; }
			public List<ImportedMaterial> MaterialList { get; protected set; }
			public List<ImportedTexture> TextureList { get; protected set; }
			public List<ImportedAnimation> AnimationList { get; protected set; }
			public List<ImportedMorph> MorphList { get; protected set; }

			private Document colladaDoc = null;
			private Dictionary<string, ImportedTexture> textureDic = new Dictionary<string, ImportedTexture>();
			private Dictionary<string, ImportedMaterial> materialDic = new Dictionary<string, ImportedMaterial>();
			private bool IsBlender = false;
			private bool Z_UP = false;
			private Matrix ZUpToYUpMatrix = new Matrix();

			public Importer(string path)
			{
				try
				{
					FrameList = new List<ImportedFrame>();
					MeshList = new List<ImportedMesh>();
					MaterialList = new List<ImportedMaterial>();
					TextureList = new List<ImportedTexture>();
					AnimationList = new List<ImportedAnimation>();
					MorphList = new List<ImportedMorph>();

					colladaDoc = new Document(path);
					if (colladaDoc.asset != null)
					{
						if (colladaDoc.asset.up_axis == "Z_UP")
						{
							Z_UP = true;
							ZUpToYUpMatrix[0, 0] = 1;
							ZUpToYUpMatrix[1, 2] = 1;
							ZUpToYUpMatrix[2, 1] = -1;
						}
						if ((colladaDoc.asset.contributors != null) && colladaDoc.asset.contributors[0].authoring_tool.Contains("Blender"))
						{
							IsBlender = true;
						}
					}

					Dictionary<string, ImportedTexture> textureIdDic = new Dictionary<string, ImportedTexture>();
					if (colladaDoc.images != null)
					{
						for (int i = 0; i < colladaDoc.images.Count; i++)
						{
							ImportedTexture tex = ImportImage(colladaDoc.images[i]);
							if (tex != null)
							{
								ImportedTexture prevTex;
								if (textureDic.TryGetValue(tex.Name, out prevTex))
								{
									tex = prevTex;
								}
								else
								{
									textureDic.Add(tex.Name, tex);
									TextureList.Add(tex);
								}

								textureIdDic.Add(colladaDoc.images[i].id, tex);
							}
						}
					}

					if (colladaDoc.materials != null)
					{
						for (int i = 0; i < colladaDoc.materials.Count; i++)
						{
							ImportedMaterial mat = ImportMaterial(colladaDoc.materials[i], textureIdDic);
							if ((mat != null) && !materialDic.ContainsKey(mat.Name))
							{
								materialDic.Add(mat.Name, mat);
								MaterialList.Add(mat);
							}
						}
					}

					if (colladaDoc.instanceVisualScene != null)
					{
						Document.VisualScene scene = (Document.VisualScene)colladaDoc.dic[colladaDoc.instanceVisualScene.url.Fragment];
						if (scene != null)
						{
							Conditioner.ConvexTriangulator(colladaDoc);
							for (int i = 0; i < scene.nodes.Count; i++)
							{
								ImportedFrame frame = ImportNode(scene.nodes[i]);
								if (frame != null)
								{
									FrameList.Add(frame);
								}
							}
						}
					}

					ImportedAnimation wsAnimation = new ImportedAnimation();
					wsAnimation.TrackList = new List<ImportedAnimationTrack>();
					ImportAnimationSet(colladaDoc.animations, wsAnimation);
					if (wsAnimation.TrackList.Count > 0)
					{
						AnimationList.Add(wsAnimation);
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing Collada: " + e.Message);
				}
			}

			private static string DecodeName(string name)
			{
				string decoded = HttpUtility.UrlDecode(name, Encoding.UTF8);
				/*if (decoded.StartsWith("_") && !Char.IsLetter(decoded, 1))
				{
					decoded = decoded.Substring(1);
				}
				if (decoded.EndsWith("_") && Char.IsNumber(decoded, decoded.Length - 2))
				{
					decoded = decoded.Substring(0, decoded.Length - 1);
				}*/
				return decoded;
			}

			private void ImportAnimationSet(List<Document.Animation> animations, ImportedAnimation wsAnimation)
			{
				try
				{
					if (animations == null)
					{
						return;
					}

					Document.Animation animation = animations[0];
					if (animation.channel == null)
					{
						foreach (Document.Animation animationSet in animations)
						{
							ImportAnimationSet(animationSet.children, wsAnimation);
						}
					}
					else
					{
						ImportAnimation(animations, wsAnimation);
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing animations: " + e.Message);
				}
			}

			private void ImportAnimation(List<Document.Animation> animations, ImportedAnimation wsAnimation)
			{
				Dictionary<string, List<KeyValuePair<string, Document.Source>>> trackList = new Dictionary<string, List<KeyValuePair<string, Document.Source>>>();
				for (int i = 0; i < animations.Count; i++)
				{
					int typeIdx = animations[i].channel.target.IndexOf('/');
					if (typeIdx < 0)
					{
						throw new Exception("Couldn't find transform type for ANIMATION " + animations[i].id);
					}
					string frameName = DecodeName(animations[i].channel.target.Substring(0, typeIdx));
					typeIdx++;
					string type = animations[i].channel.target.Substring(typeIdx, animations[i].channel.target.Length - typeIdx);

					for (int j = 0; j < animations[i].sampler.inputs.Count; j++)
					{
						if (animations[i].sampler.inputs[j].semantic == "OUTPUT")
						{
							Document.Source source = (Document.Source)animations[i].sampler.inputs[j].source;
							List<KeyValuePair<string, Document.Source>> track;
							if (!trackList.TryGetValue(frameName, out track))
							{
								track = new List<KeyValuePair<string, Document.Source>>();
								trackList.Add(frameName, track);
							}
							track.Add(new KeyValuePair<string, Document.Source>(type, source));
							break;
						}
					}
				}

				foreach (KeyValuePair<string, List<KeyValuePair<string, Document.Source>>> track in trackList)
				{
					int numFrames = 0;
					for (int i = 0; i < track.Value.Count; i++)
					{
						int count = track.Value[i].Value.accessor.count;
						if (count > numFrames)
						{
							numFrames = count;
						}
					}

					ImportedAnimationKeyframe[] keyframes = new ImportedAnimationKeyframe[numFrames];
					for (int i = 0; i < numFrames; i++)
					{
						Vector3 translate = new Vector3(0, 0, 0);
						Vector3 scale = new Vector3(1, 1, 1);
						float rotX = 0;
						float rotY = 0;
						float rotZ = 0;

						foreach (KeyValuePair<string, Document.Source> transform in track.Value)
						{
							Document.Source source = transform.Value;
							Document.Array<float> array = (Document.Array<float>)source.array;
							int arrayIdx = source.accessor.offset + (source.accessor.stride * i);
							if (arrayIdx < array.Count)
							{
								float val = array[arrayIdx];

								switch (transform.Key)
								{
									case "translate.X":
										translate[0] = val;
										break;
									case "translate.Y":
										if (Z_UP)
										{
											translate[2] = -val;
										}
										else
										{
											translate[1] = val;
										}
										break;
									case "translate.Z":
										if (Z_UP)
										{
											translate[1] = val;
										}
										else
										{
											translate[2] = val;
										}
										break;
									case "rotateX.ANGLE":
										rotX = val;
										break;
									case "rotateY.ANGLE":
										rotY = val;
										break;
									case "rotateZ.ANGLE":
										rotZ = val;
										break;
									case "scale.X":
										scale[0] = val;
										break;
									case "scale.Y":
										if (Z_UP)
										{
											scale[2] = val;
										}
										else
										{
											scale[1] = val;
										}
										break;
									case "scale.Z":
										if (Z_UP)
										{
											scale[1] = val;
										}
										else
										{
											scale[2] = val;
										}
										break;
									default:
										throw new Exception("Unknown transform type " + transform.Key + " for ANIMATION " + animations[i].id);
								}
							}
						}

						Matrix rotMatrix = Matrix.Identity;
						Matrix rotXMatrix = Matrix.RotationAxis(new Vector3(1, 0, 0), (float)(rotX * Math.PI / 180));
						Matrix rotYMatrix = Matrix.RotationAxis(new Vector3(0, 1, 0), (float)(rotY * Math.PI / 180));
						Matrix rotZMatrix = Matrix.RotationAxis(new Vector3(0, 0, 1), (float)(rotZ * Math.PI / 180));
						rotMatrix = rotMatrix * rotZMatrix;
						rotMatrix = rotMatrix * rotYMatrix;
						rotMatrix = rotMatrix * rotXMatrix;
						if (Z_UP)
						{
							rotMatrix = ZUpToYUpMatrix * rotMatrix;
						}

						Vector3 dummyScale;
						Quaternion rotation;
						Vector3 dummyTranslate;
						if (!rotMatrix.Decompose(out dummyScale, out rotation, out dummyTranslate))
						{
							throw new Exception("Failed to decompose matrix");
						}

						keyframes[i] = new ImportedAnimationKeyframe();
						keyframes[i].Rotation = rotation;
						keyframes[i].Scaling = scale;
						keyframes[i].Translation = translate;
					}

					ImportedAnimationTrack importedTrack = new ImportedAnimationTrack();
					importedTrack.Name = track.Key;
					importedTrack.Keyframes = keyframes;
					wsAnimation.TrackList.Add(importedTrack);
				}
			}

			private ImportedTexture ImportImage(Document.Image img)
			{
				ImportedTexture tex = null;
				try
				{
					string texPath = Path.GetDirectoryName(img.init_from.Uri.LocalPath) + Path.DirectorySeparatorChar + Path.GetFileName(img.init_from.Uri.LocalPath);
					if (texPath.StartsWith("./") || texPath.StartsWith("/") || texPath.StartsWith(".\\") || texPath.StartsWith("\\"))
					{
						texPath = Path.GetDirectoryName(colladaDoc.baseURI.LocalPath) + Path.DirectorySeparatorChar + texPath.TrimStart(new char[] { '.', '/', '\\' });
					}
					tex = new ImportedTexture(texPath);
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing texture " + img.id + ": " + e.Message);
				}
				return tex;
			}

			private ImportedMaterial ImportMaterial(Document.Material material, Dictionary<string, ImportedTexture> textureIdDic)
			{
				ImportedMaterial mat = null;
				try
				{
					Document.Effect effect = (Document.Effect)colladaDoc.dic[material.instanceEffect.Fragment];
					if (effect == null)
					{
						throw new Exception("Couldn't find effect " + material.instanceEffect.Fragment);
					}

					Document.ProfileCOMMON profile = null;
					for (int i = 0; i < effect.profiles.Count; i++)
					{
						if (effect.profiles[i] is Document.ProfileCOMMON)
						{
							profile = (Document.ProfileCOMMON)effect.profiles[i];
							break;
						}
					}
					if (profile == null)
					{
						throw new Exception("Couldn't find profile_COMMON for " + effect.id);
					}
					if (!(profile.technique.shader is Document.Phong))
					{
						throw new Exception(effect.id + " isn't phong shading");
					}

					ImportedMaterial matInfo = new ImportedMaterial();
					matInfo.Name = DecodeName(material.id);
					matInfo.Textures = new string[4] { String.Empty, String.Empty, String.Empty, String.Empty };

					Document.Phong phong = (Document.Phong)profile.technique.shader;
					if (phong.diffuse is Document.Texture)
					{
						matInfo.Diffuse = new Color4(1.0f, 1.0f, 1.0f, 1.0f);

						Document.Texture docTex = (Document.Texture)phong.diffuse;
						ImportedTexture diffuseTex;
						if (textureIdDic.TryGetValue(docTex.texture, out diffuseTex))
						{
							matInfo.Textures[0] = diffuseTex.Name;
						}
					}
					else
					{
						matInfo.Diffuse = ((Document.Color)phong.diffuse).floats.ToColor4();
					}

					if (phong.reflective is Document.Texture)
					{
						Document.Texture docTex = (Document.Texture)phong.reflective;
						ImportedTexture reflectiveTex;
						if (textureIdDic.TryGetValue(docTex.texture, out reflectiveTex))
						{
							matInfo.Textures[1] = reflectiveTex.Name;
						}
					}

					matInfo.Ambient = ((Document.Color)phong.ambient).floats.ToColor4();
					matInfo.Emissive = ((Document.Color)phong.emission).floats.ToColor4();
					matInfo.Specular = ((Document.Color)phong.specular).floats.ToColor4();
					matInfo.Power = ((Document.Float)phong.shininess).theFloat;
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing material " + material.id + ": " + e.Message);
				}
				return mat;
			}

			private void ImportJoint(Document.Node node, Dictionary<string, string> jointTable)
			{
				if (node.sid != null)
				{
					jointTable.Add(node.sid, node.id);
				}

				if (node.children != null)
				{
					for (int i = 0; i < node.children.Count; i++)
					{
						ImportJoint(node.children[i], jointTable);
					}
				}
			}

			private ImportedFrame ImportNode(Document.Node node)
			{
				ImportedFrame frame = new ImportedFrame();
				try
				{
					frame.Name = DecodeName(node.id);
					frame.Matrix = ProcessNodeMatrix(node);

					ImportedMesh meshList = new ImportedMesh();
					meshList.Name = frame.Name;
					meshList.SubmeshList = new List<ImportedSubmesh>();

					List<string> boneNames = new List<string>(255);
					List<Matrix> boneMatrices = new List<Matrix>(255);
					List<Document.InstanceGeometry> geometries = new List<Document.InstanceGeometry>();
					List<Document.InstanceController> controllers = new List<Document.InstanceController>();
					List<Document.InstanceNode> instanceNodes;
					ProcessNodeInstances(node, geometries, controllers, out instanceNodes);

					for (int i = 0; i < instanceNodes.Count; i++)
					{
						List<Document.InstanceNode> dummy;
						Document.Node instanceNode = (Document.Node)colladaDoc.dic[instanceNodes[i].url.Fragment];
						if (ProcessNodeInstances(instanceNode, geometries, controllers, out dummy))
						{
							ImportedFrame child = ImportNode(instanceNode);
							if (child != null)
							{
								frame.AddChild(child);
							}
						}
						for (int j = 0; j < dummy.Count; j++)
						{
							Report.ReportLog("Warning: instance node " + dummy[j].name + " wasn't processed");
						}
					}

					if (node.children != null)
					{
						for (int i = 0; i < node.children.Count; i++)
						{
							List<Document.InstanceNode> dummy;
							if (ProcessNodeInstances(node.children[i], geometries, controllers, out dummy))
							{
								ImportedFrame child = ImportNode(node.children[i]);
								if (child != null)
								{
									frame.AddChild(child);
								}
							}
							for (int j = 0; j < dummy.Count; j++)
							{
								Report.ReportLog("Warning: instance node " + dummy[j].name + " wasn't processed");
							}
						}
					}

					int vertInfoIdx = 0;
					for (int i = 0; i < controllers.Count; i++)
					{
						ImportedSubmesh submesh = ImportController((Document.Controller)colladaDoc.dic[controllers[i].url.Fragment], boneNames, boneMatrices, ref vertInfoIdx);
						if (submesh != null)
						{
							SetMaterial(controllers[i], submesh);
							submesh.Index = meshList.SubmeshList.Count;
							meshList.SubmeshList.Add(submesh);
						}
					}
					for (int i = 0; i < geometries.Count; i++)
					{
						ImportedSubmesh submesh = ImportGeometry((Document.Geometry)colladaDoc.dic[geometries[i].url.Fragment], ref vertInfoIdx);
						if (submesh != null)
						{
							SetMaterial(geometries[i], submesh);
							submesh.Index = meshList.SubmeshList.Count;
							meshList.SubmeshList.Add(submesh);

							foreach (ImportedVertex vert in submesh.VertexList)
							{
								if (boneNames.Count > 0)
								{
									vert.BoneIndices = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
									vert.Weights = new float[] { 1, 0, 0, 0 };
								}
								else
								{
									vert.BoneIndices = new byte[4];
									vert.Weights = new float[4];
								}
							}
						}
					}

					if (meshList.SubmeshList.Count > 0)
					{
						meshList.BoneList = new List<ImportedBone>(boneNames.Count);
						for (int i = 0; i < boneNames.Count; i++)
						{
							string name = boneNames[i];
							ImportedBone bone = new ImportedBone();
							bone.Name = DecodeName(name);
							bone.Matrix = boneMatrices[i];
							meshList.BoneList.Add(bone);
						}
						MeshList.Add(meshList);
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing node " + node.id + ": " + e.Message);
					frame = null;
				}
				return frame;
			}

			private Matrix ProcessNodeMatrix(Document.Node node)
			{
				Matrix transform = Matrix.Identity;
				if (node.transforms != null)
				{
					Matrix matrix = Matrix.Identity;
					Matrix t = Matrix.Identity;
					Matrix r = Matrix.Identity;
					Matrix s = Matrix.Identity;
					for (int i = 0; i < node.transforms.Count; i++)
					{
						if (node.transforms[i] is Document.Translate)
						{
							Matrix m = Matrix.Translation(node.transforms[i][0], node.transforms[i][1], node.transforms[i][2]);
							t = t * m;
						}
						else if (node.transforms[i] is Document.Rotate)
						{
							Matrix m = Matrix.RotationAxis(new Vector3(node.transforms[i][0], node.transforms[i][1], node.transforms[i][2]), (float)(node.transforms[i][3] * Math.PI / 180));
							r = r * m;
						}
						else if (node.transforms[i] is Document.Scale)
						{
							Matrix m = Matrix.Scaling(node.transforms[i][0], node.transforms[i][1], node.transforms[i][2]);
							s = s * m;
						}
						else if (node.transforms[i] is Document.Matrix)
						{
							Document.Matrix docMatrix = (Document.Matrix)node.transforms[i];
							Matrix m = new Matrix();
							for (int j = 0; j < 4; j++)
							{
								for (int k = 0; k < 4; k++)
								{
									m[j, k] = docMatrix[j, k];
								}
							}
							matrix = matrix * m;
						}
						else
						{
							throw new Exception("Unknown transform for node " + node.id);
						}
					}
					transform = matrix * t * r * s;
					if (Z_UP)
					{
						transform = ZUpToYUpMatrix * transform;
					}
				}
				return transform;
			}

			private bool ProcessNodeInstances(Document.Node node, List<Document.InstanceGeometry> geometries, List<Document.InstanceController> controllers, out List<Document.InstanceNode> instanceNodes)
			{
				bool makeChildNode = true;
				instanceNodes = new List<Document.InstanceNode>();
				if (node.instances != null)
				{
					for (int i = 0; i < node.instances.Count; i++)
					{
						if (node.instances[i] is Document.InstanceGeometry)
						{
							geometries.Add((Document.InstanceGeometry)node.instances[i]);
							makeChildNode = false;
						}
						else if (node.instances[i] is Document.InstanceController)
						{
							controllers.Add((Document.InstanceController)node.instances[i]);
							makeChildNode = false;
						}
						else if (node.instances[i] is Document.InstanceNode)
						{
							instanceNodes.Add((Document.InstanceNode)node.instances[i]);
						}
					}
				}
				return makeChildNode;
			}

			private void SetMaterial(Document.Instance instance, ImportedSubmesh submesh)
			{
				if (instance is Document.InstanceWithMaterialBind)
				{
					Document.BindMaterial bindMat = ((Document.InstanceWithMaterialBind)instance).bindMaterial;
					if ((bindMat.instanceMaterials != null) && (bindMat.instanceMaterials.Count > 0))
					{
						foreach (DictionaryEntry de in (IDictionary)(bindMat.instanceMaterials))
						{
							Document.InstanceMaterial instanceMat = (Document.InstanceMaterial)de.Value;
							submesh.Material = DecodeName(instanceMat.target.Fragment);
							break;
						}
					}
				}
			}

			private ImportedSubmesh ImportController(Document.Controller controller, List<string> boneNames, List<Matrix> boneMatrices, ref int vertInfoIdx)
			{
				ImportedSubmesh submesh = null;
				try
				{
					if (controller.controller is Document.Skin)
					{
						Document.Skin skin = (Document.Skin)controller.controller;
						submesh = ImportGeometry((Document.Geometry)colladaDoc.dic[skin.source.Fragment], ref vertInfoIdx);
						Document.Array<string> nameArray = null;
						Document.Array<float> transformArray = null;
						for (int i = 0; i < skin.joint.inputs.Count; i++)
						{
							Document.Source source = (Document.Source)skin.joint.inputs[i].source;
							switch (skin.joint.inputs[i].semantic)
							{
								case "JOINT":
									nameArray = (Document.Array<string>)source.array;
									break;
								case "INV_BIND_MATRIX":
									transformArray = (Document.Array<float>)source.array;
									break;
							}
						}
						if (nameArray == null)
						{
							throw new Exception("Couldn't find JOINT array");
						}
						if (transformArray == null)
						{
							throw new Exception("Couldn't find INV_BIND_MATRIX array");
						}

						Matrix bindShape = Matrix.Identity;
						if (skin.bindShapeMatrix != null)
						{
							bindShape[0, 0] = skin.bindShapeMatrix[0];
							bindShape[1, 0] = skin.bindShapeMatrix[1];
							bindShape[2, 0] = skin.bindShapeMatrix[2];
							bindShape[3, 0] = skin.bindShapeMatrix[3];
							bindShape[0, 1] = skin.bindShapeMatrix[4];
							bindShape[1, 1] = skin.bindShapeMatrix[5];
							bindShape[2, 1] = skin.bindShapeMatrix[6];
							bindShape[3, 1] = skin.bindShapeMatrix[7];
							bindShape[0, 2] = skin.bindShapeMatrix[8];
							bindShape[1, 2] = skin.bindShapeMatrix[9];
							bindShape[2, 2] = skin.bindShapeMatrix[10];
							bindShape[3, 2] = skin.bindShapeMatrix[11];
							bindShape[0, 3] = skin.bindShapeMatrix[12];
							bindShape[1, 3] = skin.bindShapeMatrix[13];
							bindShape[2, 3] = skin.bindShapeMatrix[14];
							bindShape[3, 3] = skin.bindShapeMatrix[15];
						}
						for (int i = 0; i < nameArray.Count; i++)
						{
							Matrix boneMatrix = Matrix.Identity;
							int offset = i * 16;
							boneMatrix[0, 0] = transformArray[offset];
							boneMatrix[1, 0] = transformArray[offset + 1];
							boneMatrix[2, 0] = transformArray[offset + 2];
							boneMatrix[3, 0] = transformArray[offset + 3];
							boneMatrix[0, 1] = transformArray[offset + 4];
							boneMatrix[1, 1] = transformArray[offset + 5];
							boneMatrix[2, 1] = transformArray[offset + 6];
							boneMatrix[3, 1] = transformArray[offset + 7];
							boneMatrix[0, 2] = transformArray[offset + 8];
							boneMatrix[1, 2] = transformArray[offset + 9];
							boneMatrix[2, 2] = transformArray[offset + 10];
							boneMatrix[3, 2] = transformArray[offset + 11];
							boneMatrix[0, 3] = transformArray[offset + 12];
							boneMatrix[1, 3] = transformArray[offset + 13];
							boneMatrix[2, 3] = transformArray[offset + 14];
							boneMatrix[3, 3] = transformArray[offset + 15];
							boneMatrix = boneMatrix * bindShape;
							boneMatrix = Matrix.Transpose(boneMatrix);
							if (Z_UP)
							{
								boneMatrix = ZUpToYUpMatrix * boneMatrix;
							}

							int idx = boneNames.IndexOf(nameArray[i]);
							if (idx < 0)
							{
								idx = boneNames.Count;
								boneNames.Add(nameArray[i]);
								boneMatrices.Add(boneMatrix);
							}
							else if (boneMatrices[idx] != boneMatrix)
							{
								Report.ReportLog("Warning: CONTROLLER " + controller.id + " has a different INV_BIND_MATRIX for JOINT " + nameArray[i] + ". The original bind matrix will be used");
							}
						}

						Document.Input weightInput = null;
						Document.Input jointInput = null;
						for (int i = 0; i < skin.vertexWeights.inputs.Count; i++)
						{
							switch (skin.vertexWeights.inputs[i].semantic)
							{
								case "WEIGHT":
									weightInput = skin.vertexWeights.inputs[i];
									break;
								case "JOINT":
									jointInput = skin.vertexWeights.inputs[i];
									break;
							}
						}
						if (weightInput == null)
						{
							throw new Exception("Couldn't find WEIGHT input");
						}
						if (jointInput == null)
						{
							throw new Exception("Couldn't find JOINT input");
						}

						bool reportedNumWeightsExceeded = false;
						Document.Array<float> weightArray = (Document.Array<float>)((Document.Source)weightInput.source).array;
						Document.Array<string> jointArray = (Document.Array<string>)((Document.Source)jointInput.source).array;
						int vOffset = 0;
						for (int i = 0; i < skin.vertexWeights.count; i++)
						{
							int numJoints = (int)skin.vertexWeights.vcount[i];
							if ((numJoints > 4) && !reportedNumWeightsExceeded)
							{
								Report.ReportLog("Warning: CONTROLLER " + controller.id + " has more than 4 weights for one or more vertices. Only the first 4 weights will be used");
								reportedNumWeightsExceeded = true;
							}

							float[] vertexWeights = new float[4];
							byte[] boneIdxs = new byte[4];
							for (int j = 0; (j < numJoints) && (j < 4); j++)
							{
								int offset = vOffset + (j * skin.vertexWeights.inputs.Count);
								vertexWeights[j] = weightArray[skin.vertexWeights.v[offset + weightInput.offset]];
								boneIdxs[j] = (byte)boneNames.IndexOf(jointArray[skin.vertexWeights.v[offset + jointInput.offset]]);
							}
							for (int j = numJoints; j < 4; j++)
							{
								boneIdxs[j] = 0xFF;
							}

							ImportedVertex vert = submesh.VertexList[i];
							vert.Weights = vertexWeights;
							vert.BoneIndices = boneIdxs;

							vOffset += skin.vertexWeights.inputs.Count * numJoints;
						}
					}
					else if (controller.controller is Document.Morph)
					{
						throw new Exception("Importing morphs isn't implemented");
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing controller " + controller.id + ": " + e.Message);
				}
				return submesh;
			}

			private ImportedSubmesh ImportGeometry(Document.Geometry geo, ref int vertInfoIdx)
			{
				ImportedSubmesh submesh = null;
				try
				{
					bool hasNormalInput = false;
					Document.Input positionInput = null;
					Document.Input normalInput = null;
					Document.Input texcoordInput = null;
					for (int i = 0; i < geo.mesh.vertices.inputs.Count; i++)
					{
						switch (geo.mesh.vertices.inputs[i].semantic)
						{
							case "POSITION":
								positionInput = geo.mesh.vertices.inputs[i];
								break;
							case "NORMAL":
								normalInput = geo.mesh.vertices.inputs[i];
								break;
							case "TEXCOORD":
								texcoordInput = geo.mesh.vertices.inputs[i];
								break;
						}
					}
					if (positionInput == null)
					{
						throw new Exception("No POSITION vertex input in " + geo.id);
					}

					int numVerts = ((Document.Source)positionInput.source).accessor.count;
					List<ImportedVertex> vertList = new List<ImportedVertex>(numVerts);
					for (int i = 0; i < numVerts; i++)
					{
						ImportedVertex vert = new ImportedVertex();
						vertList.Add(vert);
						vert.Normal = new Vector3(0, 0, 0);
						vert.UV = new float[] { 0, 0 };
						vert.BoneIndices = new byte[] { 0, 0, 0, 0 };
						vert.Weights = new float[] { 0, 0, 0, 0 };
						if (Z_UP)
						{
							vert.Position = new Vector3(
								GetSourceValue(positionInput, 0, i),
								GetSourceValue(positionInput, 2, i),
								-GetSourceValue(positionInput, 1, i));
						}
						else
						{
							vert.Position = new Vector3(
								GetSourceValue(positionInput, 0, i),
								GetSourceValue(positionInput, 1, i),
								GetSourceValue(positionInput, 2, i));
						}
						if (normalInput != null)
						{
							hasNormalInput = true;
							if (Z_UP)
							{
								vert.Normal = new Vector3(
									GetSourceValue(normalInput, 0, i),
									GetSourceValue(normalInput, 2, i),
									-GetSourceValue(normalInput, 1, i));
							}
							else
							{
								vert.Normal = new Vector3(
									GetSourceValue(normalInput, 0, i),
									GetSourceValue(normalInput, 1, i),
									GetSourceValue(normalInput, 2, i));
							}
						}
						if (texcoordInput == null)
						{
							vert.UV = new float[2];
						}
						else
						{
							if (IsBlender)
							{
								vert.UV = new float[] {
									GetSourceValue(texcoordInput, 0, i),
									-GetSourceValue(texcoordInput, 1, i) };
							}
							else
							{
								vert.UV = new float[] {
									GetSourceValue(texcoordInput, 0, i),
									1.0f - GetSourceValue(texcoordInput, 1, i) };
							}
						}
						vertInfoIdx++;
					}

					List<ImportedFace> faceList = new List<ImportedFace>();
					foreach (Document.Primitive primitive in geo.mesh.primitives)
					{
						if (primitive is Document.Triangle)
						{
							Document.Input vertexInput = null;
							List<Document.Input> normalInputs = new List<Document.Input>();
							List<Document.Input> textureInputs = new List<Document.Input>();
							foreach (Document.Input input in primitive.Inputs)
							{
								switch (input.semantic)
								{
									case "VERTEX":
										vertexInput = input;
										break;
									case "NORMAL":
										hasNormalInput = true;
										normalInputs.Add(input);
										break;
									case "TEXCOORD":
										textureInputs.Add(input);
										break;
								}
							}

							if (vertexInput != null)
							{
								for (int faceIdx = 0; faceIdx < primitive.count; faceIdx++)
								{
									ushort[] faceVerts = new ushort[3];
									for (int i = 0; i < 3; i++)
									{
										int pIdx = (faceIdx * 3) + i;
										int vertIdx = GetPValue(vertexInput, primitive, pIdx);
										faceVerts[i] = (ushort)vertIdx;

										ImportedVertex vert = vertList[vertIdx];
										for (int j = 0; j < normalInputs.Count; j++)
										{
											int p = GetPValue(normalInputs[j], primitive, pIdx);
											if (Z_UP)
											{
												vert.Normal = new Vector3(
													GetSourceValue(normalInputs[j], 0, p),
													GetSourceValue(normalInputs[j], 2, p),
													-GetSourceValue(normalInputs[j], 1, p));
											}
											else
											{
												vert.Normal = new Vector3(
													GetSourceValue(normalInputs[j], 0, p),
													GetSourceValue(normalInputs[j], 1, p),
													GetSourceValue(normalInputs[j], 2, p));
											}
										}
										for (int j = 0; j < textureInputs.Count; j++)
										{
											int p = GetPValue(textureInputs[j], primitive, pIdx);
											if (IsBlender)
											{
												vert.UV = new float[] {
													GetSourceValue(textureInputs[j], 0, p),
													-GetSourceValue(textureInputs[j], 1, p) };
											}
											else
											{
												vert.UV = new float[] {
													GetSourceValue(textureInputs[j], 0, p),
													1.0f - GetSourceValue(textureInputs[j], 1, p) };
											}
										}
									}
									ImportedFace face = new ImportedFace();
									faceList.Add(face);
									face.VertexIndices = new int[3] { faceVerts[0], faceVerts[1], faceVerts[2] };
								}
							}
						}
					}

					submesh = new ImportedSubmesh();
					submesh.VertexList = vertList;
					submesh.FaceList = faceList;

					if (!hasNormalInput)
					{
						for (int i = 0; i < submesh.VertexList.Count; i++)
						{
							submesh.VertexList[i].Normal = new Vector3();
						}
					}
				}
				catch (Exception e)
				{
					Report.ReportLog("Error importing " + geo.id + ": " + e.Message);
				}
				return submesh;
			}

			public static float GetSourceValue(Document.Input input, int channel, int index)
			{
				Document.Source source = (Document.Source)input.source;
				Document.Array<float> array = (Document.Array<float>)source.array;
				return array[source.accessor.offset + source.accessor.parameters[channel].index + (source.accessor.stride * index)];
			}

			public static int GetPValue(Document.Input input, Document.Primitive primitive, int index)
			{
				return primitive.p[input.offset + (primitive.stride * index)];
			}
		}
	}
}
