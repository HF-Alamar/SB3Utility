#include <fbxsdk.h>
#include <fbxfilesdk/kfbxio/kfbxiosettings.h>
#include "SB3UtilityFBX.h"

namespace SB3Utility
{
	Fbx::Importer::Importer(String^ path)
	{
		String^ currentDir;

		try
		{
			currentDir = Directory::GetCurrentDirectory();
			Directory::SetCurrentDirectory(Path::GetDirectoryName(path));

			unnamedMeshCount = 0;
			FrameList = gcnew List<ImportedFrame^>();
			MeshList = gcnew List<ImportedMesh^>();
			MaterialList = gcnew List<ImportedMaterial^>();
			TextureList = gcnew List<ImportedTexture^>();
			AnimationList = gcnew List<ImportedAnimation^>();
			MorphList = gcnew List<ImportedMorph^>();

			cPath = NULL;
			pSdkManager = NULL;
			pScene = NULL;
			pImporter = NULL;
			pMaterials = NULL;
			pTextures = NULL;

			pin_ptr<KFbxSdkManager*> pSdkManagerPin = &pSdkManager;
			pin_ptr<KFbxScene*> pScenePin = &pScene;
			Init(pSdkManagerPin, pScenePin);

			cPath = Fbx::StringToCharArray(path);
			pImporter = KFbxImporter::Create(pSdkManager, "");

			IOS_REF.SetBoolProp(IMP_FBX_MATERIAL, true);
			IOS_REF.SetBoolProp(IMP_FBX_TEXTURE, true);
			IOS_REF.SetBoolProp(IMP_FBX_LINK, true);
			IOS_REF.SetBoolProp(IMP_FBX_SHAPE, true);
			IOS_REF.SetBoolProp(IMP_FBX_GOBO, true);
			IOS_REF.SetBoolProp(IMP_FBX_ANIMATION, true);
			IOS_REF.SetBoolProp(IMP_FBX_GLOBAL_SETTINGS, true);

			if (!pImporter->Initialize(cPath, -1, pSdkManager->GetIOSettings()))
			{
				throw gcnew Exception(gcnew String("Failed to initialize KFbxImporter: ") + gcnew String(pImporter->GetLastErrorString()));
			}

			pImporter->Import(pScene);
			pMaterials = new KArrayTemplate<KFbxSurfacePhong*>();
			pTextures = new KArrayTemplate<KFbxTexture*>();

			KFbxNode* pRootNode = pScene->GetRootNode();
			if (pRootNode != NULL)
			{
				ImportNode(nullptr, pRootNode);
			}

			ImportAnimation();
		}
		finally
		{
			if (pMaterials != NULL)
			{
				delete pMaterials;
			}
			if (pTextures != NULL)
			{
				delete pTextures;
			}
			if (pImporter != NULL)
			{
				pImporter->Destroy();
			}
			if (pScene != NULL)
			{
				pScene->Destroy();
			}
			if (pSdkManager != NULL)
			{
				pSdkManager->Destroy();
			}
			if (cPath != NULL)
			{
				Marshal::FreeHGlobal((IntPtr)cPath);
			}

			Directory::SetCurrentDirectory(currentDir);
		}
	}

	void Fbx::Importer::ImportNode(ImportedFrame^ parent, KFbxNode* pNode)
	{
		KArrayTemplate<KFbxNode*>* pMeshArray = NULL;
		try
		{
			pMeshArray = new KArrayTemplate<KFbxNode*>();
			bool hasShapes = false;

			for (int i = 0; i < pNode->GetChildCount(); i++)
			{
				KFbxNode* pNodeChild = pNode->GetChild(i);
				if (pNodeChild->GetNodeAttribute() == NULL)
				{
					ImportedFrame^ frame = ImportFrame(parent, pNodeChild);
					ImportNode(frame, pNodeChild);
				}
				else
				{
					KFbxNodeAttribute::EAttributeType lAttributeType = pNodeChild->GetNodeAttribute()->GetAttributeType();

					switch (lAttributeType)
					{
						case KFbxNodeAttribute::eNULL:
						case KFbxNodeAttribute::eSKELETON:
							{
								ImportedFrame^ frame = ImportFrame(parent, pNodeChild);
								ImportNode(frame, pNodeChild);
							}
							break;

						case KFbxNodeAttribute::eMESH:
							if (pNodeChild->GetMesh()->GetShapeCount() > 0)
							{
								hasShapes = true;
							}
							pMeshArray->Add(pNodeChild);
							break;

						default:
							KString str = KString(lAttributeType);
							Report::ReportLog(gcnew String("Warning: ") + gcnew String(pNodeChild->GetName()) + gcnew String(" has unsupported node attribute type ") + gcnew String(str.Buffer()));
							break;
					}
				}
			}

			if (hasShapes)
			{
				ImportMorph(pMeshArray);
			}
			else
			{
				ImportMesh(parent, pMeshArray);
			}
		}
		finally
		{
			if (pMeshArray != NULL)
			{
				delete pMeshArray;
			}
		}
	}

	ImportedFrame^ Fbx::Importer::ImportFrame(ImportedFrame^ parent, KFbxNode* pNode)
	{
		ImportedFrame^ frame = gcnew ImportedFrame();
		frame->InitChildren(pNode->GetChildCount());
		frame->Name = gcnew String(pNode->GetName());

		if (parent == nullptr)
		{
			FrameList->Add(frame);
		}
		else
		{
			parent->AddChild(frame);
		}

		KFbxXMatrix lNodeMatrix = pScene->GetEvaluator()->GetNodeLocalTransform(pNode);
		Matrix matrix;
		for (int m = 0; m < 4; m++)
		{
			for (int n = 0; n < 4; n++)
			{
				matrix[m, n] = (float)lNodeMatrix[m][n];
			}
		}
		frame->Matrix = matrix;

		return frame;
	}

	void Fbx::Importer::ImportMesh(ImportedFrame^ parent, KArrayTemplate<KFbxNode*>* pMeshArray)
	{
		if (pMeshArray->GetCount() > 0)
		{
			ImportedMesh^ meshList = gcnew ImportedMesh();
			meshList->SubmeshList = gcnew List<ImportedSubmesh^>();
			MeshList->Add(meshList);

			if (parent == nullptr)
			{
				meshList->Name = gcnew String("no_name") + unnamedMeshCount;
				unnamedMeshCount++;
			}
			else
			{
				meshList->Name = parent->Name;
			}

			bool skinned = false;
			for (int i = 0; i < pMeshArray->GetCount(); i++)
			{
				KFbxNode* pMeshNode = pMeshArray->GetAt(i);
				KFbxMesh* pMesh = pMeshNode->GetMesh();
				if (pMesh->GetDeformerCount(KFbxDeformer::eSKIN) > 0)
				{
					skinned = true;
					break;
				}
			}

			SortedDictionary<String^, int>^ boneDic = gcnew SortedDictionary<String^, int>();
			List<ImportedBone^>^ boneList = gcnew List<ImportedBone^>(255);
			for (int i = 0; i < pMeshArray->GetCount(); i++)
			{
				ImportedSubmesh^ submesh = gcnew ImportedSubmesh();
				meshList->SubmeshList->Add(submesh);
				submesh->Index = i;

				KFbxNode* pMeshNode = pMeshArray->GetAt(i);
				KFbxMesh* pMesh = pMeshNode->GetMesh();

				String^ submeshName = gcnew String(pMeshNode->GetName());
				int idx = submeshName->LastIndexOf('_');
				if (idx >= 0)
				{
					idx++;
					int submeshIdx;
					if (Int32::TryParse(submeshName->Substring(idx, submeshName->Length - idx), submeshIdx))
					{
						submesh->Index = submeshIdx;
					}
				}

				KFbxLayer* pLayerNormal = pMesh->GetLayer(0, KFbxLayerElement::eNORMAL);
				KFbxLayerElementNormal* pLayerElementNormal = NULL;
				if (pLayerNormal != NULL)
				{
					pLayerElementNormal = pLayerNormal->GetNormals();
				}

				KFbxLayer* pLayerUV = pMesh->GetLayer(0, KFbxLayerElement::eUV);
				KFbxLayerElementUV* pLayerElementUV = NULL;
				if (pLayerUV != NULL)
				{
					pLayerElementUV = pLayerUV->GetUVs();
				}

				int numVerts = pMesh->GetControlPointsCount();
				array<List<Vertex^>^>^ vertMap = gcnew array<List<Vertex^>^>(numVerts);
				for (int j = 0; j < numVerts; j++)
				{
					vertMap[j] = gcnew List<Vertex^>();
				}

				int vertCount = 0;
				int numFaces = pMesh->GetPolygonCount();
				array<array<Vertex^>^>^ faceMap = gcnew array<array<Vertex^>^>(numFaces);
				for (int j = 0; j < numFaces; j++)
				{
					faceMap[j] = gcnew array<Vertex^>(3);

					int polySize = pMesh->GetPolygonSize(j);
					if (polySize != 3)
					{
						throw gcnew Exception(gcnew String("Mesh ") + gcnew String(pMeshNode->GetName()) + " needs to be triangulated");
					}
					int polyVertIdxStart = pMesh->GetPolygonVertexIndex(j);
					for (int k = 0; k < polySize; k++)
					{
						int controlPointIdx = pMesh->GetPolygonVertices()[polyVertIdxStart + k];
						Vertex^ vert = gcnew Vertex();

						KFbxVector4 pos = pMesh->GetControlPointAt(controlPointIdx);
						vert->position = gcnew array<float>(3) { (float)pos[0], (float)pos[1], (float)pos[2] };

						if (pLayerElementNormal != NULL)
						{
							KFbxVector4 norm;
							GetVector(pLayerElementNormal, norm, controlPointIdx, vertCount);
							vert->normal = gcnew array<float>(3) { (float)norm[0], (float)norm[1], (float)norm[2] };
						}

						if (pLayerElementUV != NULL)
						{
							KFbxVector2 uv;
							GetVector(pLayerElementUV, uv, controlPointIdx, vertCount);
							vert->uv = gcnew array<float>(2) { (float)uv[0], -(float)uv[1] };
						}

						List<Vertex^>^ vertMapList = vertMap[controlPointIdx];
						Vertex^ foundVert = nullptr;
						for (int m = 0; m < vertMapList->Count; m++)
						{
							if (vertMapList[m]->Equals(vert))
							{
								foundVert = vertMapList[m];
								break;
							}
						}

						if (foundVert == nullptr)
						{
							vertMapList->Add(vert);
						}
						faceMap[j][k] = vertMapList[0];

						vertCount++;
					}
				}

				for (int j = 0; j < vertMap->Length; j++)
				{
					List<Vertex^>^ vertMapList = vertMap[j];
					int numNormals = vertMapList->Count;
					if (numNormals > 0)
					{
						Vertex^ vertNormal = vertMapList[0];
						while (vertMapList->Count > 1)
						{
							array<float>^ addNormal = vertMapList[1]->normal;
							vertNormal->normal[0] += addNormal[0];
							vertNormal->normal[1] += addNormal[1];
							vertNormal->normal[2] += addNormal[2];
							vertMapList->RemoveAt(1);
						}
						vertNormal->normal[0] /= numNormals;
						vertNormal->normal[1] /= numNormals;
						vertNormal->normal[2] /= numNormals;
					}
				}

				KFbxSkin* pSkin = (KFbxSkin*)pMesh->GetDeformer(0, KFbxDeformer::eSKIN);
				if (pSkin != NULL)
				{
					if (pMesh->GetDeformerCount(KFbxDeformer::eSKIN) > 1)
					{
						Report::ReportLog(gcnew String("Warning: Mesh ") + gcnew String(pMeshNode->GetName()) + " has more than 1 skin. Only the first will be used");
					}

					int numClusters = pSkin->GetClusterCount();
					for (int j = 0; j < numClusters; j++)
					{
						KFbxCluster* pCluster = pSkin->GetCluster(j);
						if (pCluster->GetLinkMode() == KFbxCluster::eADDITIVE)
						{
							throw gcnew Exception(gcnew String("Mesh ") + gcnew String(pMeshNode->GetName()) + " has additive weights and aren't supported");
						}

#if 1
						KFbxXMatrix lMatrix;
						pCluster->GetTransformLinkMatrix(lMatrix);
						lMatrix = lMatrix.Inverse();
#else
						KFbxXMatrix lMatrix, lMeshMatrix;
						pCluster->GetTransformMatrix(lMeshMatrix);
						/*KFbxXMatrix geomMatrix = pMeshNode->GetScene()->GetEvaluator()->GetNodeLocalTransform(pMeshNode);
						lMeshMatrix *= geomMatrix;*/
						pCluster->GetTransformLinkMatrix(lMatrix);
						lMatrix = (lMeshMatrix.Inverse() * lMatrix).Inverse();
#endif
						Matrix boneMatrix;
						for (int m = 0; m < 4; m++)
						{
							for (int n = 0; n < 4; n++)
							{
								boneMatrix[m, n] = (float)lMatrix.mData[m][n];
							}
						}

						KFbxNode* pLinkNode = pCluster->GetLink();
						String^ boneName = gcnew String(pLinkNode->GetName());
						int boneIdx;
						if (!boneDic->TryGetValue(boneName, boneIdx))
						{
							ImportedBone^ boneInfo = gcnew ImportedBone();
							boneList->Add(boneInfo);
							boneInfo->Name = boneName;
							boneInfo->Matrix = boneMatrix;

							boneIdx = boneDic->Count;
							boneDic->Add(boneName, boneIdx);
						}

						int* lIndices = pCluster->GetControlPointIndices();
						double* lWeights = pCluster->GetControlPointWeights();
						int numIndices = pCluster->GetControlPointIndicesCount();
						for (int k = 0; k < numIndices; k++)
						{
							List<Vertex^>^ vert = vertMap[lIndices[k]];
							for (int m = 0; m < vert->Count; m++)
							{
								vert[m]->boneIndices->Add(boneIdx);
								vert[m]->weights->Add((float)lWeights[k]);
							}
						}
					}
				}

				int vertIdx = 0;
				List<ImportedVertex^>^ vertList = gcnew List<ImportedVertex^>(vertMap->Length);
				submesh->VertexList = vertList;
				for (int j = 0; j < vertMap->Length; j++)
				{
					for (int k = 0; k < vertMap[j]->Count; k++)
					{
						Vertex^ vert = vertMap[j][k];
						vert->index = vertIdx;

						ImportedVertex^ vertInfo = gcnew ImportedVertex();
						vertList->Add(vertInfo);
						vertInfo->Position = Vector3(vert->position[0], vert->position[1], vert->position[2]);

						if (skinned)
						{
							int numBones = vert->boneIndices->Count;
							if (numBones > 4)
							{
								throw gcnew Exception(gcnew String("Mesh ") + gcnew String(pMeshNode->GetName()) + " has vertices with more than 4 weights");
							}

							array<Byte>^ boneIndices = gcnew array<Byte>(4);
							array<float>^ weights4 = gcnew array<float>(4);
							float weightSum = 0;
							for (int m = 0; m < numBones; m++)
							{
								boneIndices[m] = vert->boneIndices[m];
								weightSum += vert->weights[m];
							}
							for (int m = 0; m < numBones; m++)
							{
								weights4[m] = vert->weights[m] / weightSum;
							}

							for (int m = numBones; m < 4; m++)
							{
								boneIndices[m] = 0xFF;
							}

							vertInfo->BoneIndices = boneIndices;
							vertInfo->Weights = weights4;
						}
						else
						{
							vertInfo->BoneIndices = gcnew array<Byte>(4);
							vertInfo->Weights = gcnew array<float>(4);
						}

						vertInfo->Normal = Vector3(vert->normal[0], vert->normal[1], vert->normal[2]);
						vertInfo->UV = gcnew array<float>(2) { vert->uv[0], vert->uv[1] };

						vertIdx++;
					}
				}

				List<ImportedFace^>^ faceList = gcnew List<ImportedFace^>(numFaces);
				submesh->FaceList = faceList;
				for (int j = 0; j < numFaces; j++)
				{
					ImportedFace^ face = gcnew ImportedFace();
					faceList->Add(face);
					face->VertexIndices = gcnew array<int>(3);
					face->VertexIndices[0] = faceMap[j][0]->index;
					face->VertexIndices[1] = faceMap[j][1]->index;
					face->VertexIndices[2] = faceMap[j][2]->index;
				}

				ImportedMaterial^ matInfo = ImportMaterial(pMesh);
				if (matInfo != nullptr)
				{
					submesh->Material = matInfo->Name;
				}
			}

			boneList->TrimExcess();
			meshList->BoneList = boneList;
		}
	}

	ImportedMaterial^ Fbx::Importer::ImportMaterial(KFbxMesh* pMesh)
	{
		ImportedMaterial^ matInfo = nullptr;

		KFbxLayer* pLayerMaterial = pMesh->GetLayer(0, KFbxLayerElement::eMATERIAL);
		if (pLayerMaterial != NULL)
		{
			KFbxLayerElementMaterial* pLayerElementMaterial = pLayerMaterial->GetMaterials();
			if (pLayerElementMaterial != NULL)
			{
				KFbxSurfaceMaterial* pMaterial = NULL;
				switch (pLayerElementMaterial->GetReferenceMode())
				{
				case KFbxLayerElement::eDIRECT:
					pMaterial = pMesh->GetNode()->GetMaterial(0);
					break;

				case KFbxLayerElement::eINDEX_TO_DIRECT:
					pMaterial = pMesh->GetNode()->GetMaterial(pLayerElementMaterial->GetIndexArray().GetAt(0));
					break;

				default:
					{
						int mode = (int)pLayerElementMaterial->GetReferenceMode();
						Report::ReportLog(gcnew String("Warning: Material ") + gcnew String(pMaterial->GetName()) + " has unsupported reference mode " + mode + " and will be skipped");
					}
					break;
				}

				if (pMaterial != NULL)
				{
					if (pMaterial->GetClassId().Is(KFbxSurfacePhong::ClassId))
					{
						KFbxSurfacePhong* pPhong = (KFbxSurfacePhong*)pMaterial;
						int matIdx = pMaterials->Find(pPhong);
						if (matIdx >= 0)
						{
							matInfo = MaterialList[matIdx];
						}
						else
						{
							matInfo = gcnew ImportedMaterial();
							matInfo->Name = gcnew String(pPhong->GetName());
							
							fbxDouble3 lDiffuse = pPhong->Diffuse.Get();
							fbxDouble1 lDiffuseFactor = pPhong->DiffuseFactor.Get();
							matInfo->Diffuse = Color4((float)lDiffuseFactor, (float)lDiffuse[0], (float)lDiffuse[1], (float)lDiffuse[2]);

							fbxDouble3 lAmbient = pPhong->Ambient.Get();
							fbxDouble1 lAmbientFactor = pPhong->AmbientFactor.Get();
							matInfo->Ambient = Color4((float)lAmbientFactor, (float)lAmbient[0], (float)lAmbient[1], (float)lAmbient[2]);

							fbxDouble3 lEmissive = pPhong->Emissive.Get();
							fbxDouble1 lEmissiveFactor = pPhong->EmissiveFactor.Get();
							matInfo->Emissive = Color4((float)lEmissiveFactor, (float)lEmissive[0], (float)lEmissive[1], (float)lEmissive[2]);

							fbxDouble3 lSpecular = pPhong->Specular.Get();
							fbxDouble1 lSpecularFactor = pPhong->SpecularFactor.Get();
							matInfo->Specular = Color4((float)lSpecularFactor, (float)lSpecular[0], (float)lSpecular[1], (float)lSpecular[2]);
							matInfo->Power = (float)pPhong->Shininess.Get();

							array<String^>^ texNames = gcnew array<String^>(4);
							texNames[0] = ImportTexture((KFbxFileTexture*)pPhong->Diffuse.GetSrcObject(KFbxFileTexture::ClassId));
							texNames[1] = ImportTexture((KFbxFileTexture*)pPhong->Ambient.GetSrcObject(KFbxFileTexture::ClassId));
							texNames[2] = ImportTexture((KFbxFileTexture*)pPhong->Emissive.GetSrcObject(KFbxFileTexture::ClassId));
							texNames[3] = ImportTexture((KFbxFileTexture*)pPhong->Specular.GetSrcObject(KFbxFileTexture::ClassId));
							matInfo->Textures = texNames;

							pMaterials->Add(pPhong);
							MaterialList->Add(matInfo);
						}
					}
					else
					{
						Report::ReportLog(gcnew String("Warning: Material ") + gcnew String(pMaterial->GetName()) + " isn't a Phong material and will be skipped");
					}
				}
			}
		}

		return matInfo;
	}

	String^ Fbx::Importer::ImportTexture(KFbxFileTexture* pTexture)
	{
		using namespace System::IO;

		String^ texName = String::Empty;

		if (pTexture != NULL)
		{
			texName = Path::GetFileName(gcnew String(pTexture->GetName()));

			int pTexIdx = pTextures->Find(pTexture);
			if (pTexIdx < 0)
			{
				pTextures->Add(pTexture);

				String^ texPath = Path::GetDirectoryName(gcnew String(cPath)) + Path::DirectorySeparatorChar + texName;
				ImportedTexture^ tex = gcnew ImportedTexture(texPath);
				if (tex != nullptr)
				{
					TextureList->Add(tex);
				}
			}
		}

		return texName;
	}

	void Fbx::Importer::ImportAnimation()
	{
		for (int i = 0; i < pScene->GetSrcObjectCount(FBX_TYPE(KFbxAnimStack)); i++)
		{
			KFbxAnimStack* pAnimStack = KFbxCast<KFbxAnimStack>(pScene->GetSrcObject(FBX_TYPE(KFbxAnimStack), i));

			int numLayers = pAnimStack->GetMemberCount(FBX_TYPE(KFbxAnimLayer));
			if (numLayers > 1)
			{
				Report::ReportLog(gcnew String("Warning: Only the first layer of animation ") + gcnew String(pAnimStack->GetName()) + " will be imported");
			}
			if (numLayers > 0)
			{
				ImportedAnimation^ wsAnimation = gcnew ImportedAnimation();
				wsAnimation->TrackList = gcnew List<ImportedAnimationTrack^>(pScene->GetNodeCount());
				ImportAnimation(pAnimStack->GetMember(FBX_TYPE(KFbxAnimLayer), 0), pScene->GetRootNode(), wsAnimation);
				if (wsAnimation->TrackList->Count > 0)
				{
					AnimationList->Add(wsAnimation);
				}
			}
		}
	}

	void Fbx::Importer::ImportAnimation(KFbxAnimLayer* pAnimLayer, KFbxNode* pNode, ImportedAnimation^ wsAnimation)
	{
		KFbxAnimCurve* pAnimCurveTX = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_T_X);
		KFbxAnimCurve* pAnimCurveTY = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_T_Y);
		KFbxAnimCurve* pAnimCurveTZ = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_T_Z);
		KFbxAnimCurve* pAnimCurveRX = pNode->LclRotation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_R_X);
		KFbxAnimCurve* pAnimCurveRY = pNode->LclRotation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_R_Y);
		KFbxAnimCurve* pAnimCurveRZ = pNode->LclRotation.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_R_Z);
		KFbxAnimCurve* pAnimCurveSX = pNode->LclScaling.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_S_X);
		KFbxAnimCurve* pAnimCurveSY = pNode->LclScaling.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_S_Y);
		KFbxAnimCurve* pAnimCurveSZ = pNode->LclScaling.GetCurve<KFbxAnimCurve>(pAnimLayer, KFCURVENODE_S_Z);

		if ((pAnimCurveTX != NULL) && (pAnimCurveTY != NULL) && (pAnimCurveTZ != NULL) &&
			(pAnimCurveRX != NULL) && (pAnimCurveRY != NULL) && (pAnimCurveRZ != NULL) &&
			(pAnimCurveSX != NULL) && (pAnimCurveSY != NULL) && (pAnimCurveSZ != NULL))
		{
			array<int>^ keyCount = gcnew array<int>(9) {
				pAnimCurveSX->KeyGetCount(), pAnimCurveSY->KeyGetCount(), pAnimCurveSZ->KeyGetCount(),
				pAnimCurveRX->KeyGetCount(), pAnimCurveRY->KeyGetCount(), pAnimCurveRZ->KeyGetCount(),
				pAnimCurveTX->KeyGetCount(), pAnimCurveTY->KeyGetCount(), pAnimCurveTZ->KeyGetCount() };
			for (int i = 1; i < keyCount->Length; i++)
			{
				if (keyCount[0] != keyCount[i])
				{
					throw gcnew Exception(gcnew String(pNode->GetName()) + " doesn't have the same number of keys for each SRT track");
				}
			}

			array<ImportedAnimationKeyframe^>^ keyArray = gcnew array<ImportedAnimationKeyframe^>(keyCount[0]);
			for (int i = 0; i < keyCount[0]; i++)
			{
				keyArray[i] = gcnew ImportedAnimationKeyframe();
				keyArray[i]->Scaling = Vector3(pAnimCurveSX->KeyGetValue(i), pAnimCurveSY->KeyGetValue(i), pAnimCurveSZ->KeyGetValue(i));
				keyArray[i]->Rotation = Fbx::EulerToQuaternion(Vector3(pAnimCurveRX->KeyGetValue(i), pAnimCurveRY->KeyGetValue(i), pAnimCurveRZ->KeyGetValue(i)));
				keyArray[i]->Translation = Vector3(pAnimCurveTX->KeyGetValue(i), pAnimCurveTY->KeyGetValue(i), pAnimCurveTZ->KeyGetValue(i));
			}

			ImportedAnimationTrack^ track = gcnew ImportedAnimationTrack();
			wsAnimation->TrackList->Add(track);
			track->Name = gcnew String(pNode->GetName());
			track->Keyframes = keyArray;
		}

		for (int i = 0; i < pNode->GetChildCount(); i++)
		{
			ImportAnimation(pAnimLayer, pNode->GetChild(i), wsAnimation);
		}
	}

	Fbx::Importer::Vertex::Vertex()
	{
		position = gcnew array<float>(3);
		normal = gcnew array<float>(3);
		uv = gcnew array<float>(2);
		boneIndices = gcnew List<Byte>(4);
		weights = gcnew List<float>(4);
	}

	bool Fbx::Importer::Vertex::Equals(Vertex^ vertex)
	{
		bool equals = true;

		equals &= normal[0].Equals(vertex->normal[0]);
		equals &= normal[1].Equals(vertex->normal[1]);
		equals &= normal[2].Equals(vertex->normal[2]);

		equals &= uv[0].Equals(vertex->uv[0]);
		equals &= uv[1].Equals(vertex->uv[1]);

		return equals;
	}

	template <class T> void Fbx::Importer::GetVector(KFbxLayerElementTemplate<T>* pLayerElement, T& pVector, int controlPointIdx, int vertexIdx)
	{
		switch (pLayerElement->GetMappingMode())
		{
		case KFbxLayerElement::eBY_CONTROL_POINT:
			switch (pLayerElement->GetReferenceMode())
			{
			case KFbxLayerElement::eDIRECT:
				pVector = pLayerElement->GetDirectArray().GetAt(controlPointIdx);
				break;
			case KFbxLayerElement::eINDEX:
			case KFbxLayerElement::eINDEX_TO_DIRECT:
				{
					int idx = pLayerElement->GetIndexArray().GetAt(controlPointIdx);
					pVector = pLayerElement->GetDirectArray().GetAt(idx);
				}
				break;
			default:
				{
					int mode = (int)pLayerElement->GetReferenceMode();
					throw gcnew Exception(gcnew String("Unknown reference mode: ") + mode);
				}
				break;
			}
			break;

		case KFbxLayerElement::eBY_POLYGON_VERTEX:
			switch (pLayerElement->GetReferenceMode())
			{
			case KFbxLayerElement::eDIRECT:
				pVector = pLayerElement->GetDirectArray().GetAt(vertexIdx);
				break;
			case KFbxLayerElement::eINDEX:
			case KFbxLayerElement::eINDEX_TO_DIRECT:
				{
					int idx = pLayerElement->GetIndexArray().GetAt(vertexIdx);
					pVector = pLayerElement->GetDirectArray().GetAt(idx);
				}
				break;
			default:
				{
					int mode = (int)pLayerElement->GetReferenceMode();
					throw gcnew Exception(gcnew String("Unknown reference mode: ") + mode);
				}
				break;
			}
			break;

		default:
			{
				int mode = (int)pLayerElement->GetMappingMode();
				throw gcnew Exception(gcnew String("Unknown mapping mode: ") + mode);
			}
			break;
		}
	}

	void Fbx::Importer::ImportMorph(KArrayTemplate<KFbxNode*>* pMeshArray)
	{
		for (int i = 0; i < pMeshArray->GetCount(); i++)
		{
			KFbxNode* pNode = pMeshArray->GetAt(i);
			KFbxMesh* pMesh = pNode->GetMesh();
			int numShapes = pMesh->GetShapeCount();
			if (numShapes > 0)
			{
				ImportedMorph^ morphList = gcnew ImportedMorph();
				morphList->KeyframeList = gcnew List<ImportedMorphKeyframe^>(numShapes);
				MorphList->Add(morphList);

				String^ clipName = gcnew String(pNode->GetName());
				int clipNameStartIdx = clipName->LastIndexOf("_morph_");
				if (clipNameStartIdx >= 0)
				{
					clipNameStartIdx += 7;
					morphList->Name = clipName->Substring(clipNameStartIdx, clipName->Length - clipNameStartIdx);
				}

				for (int j = 0; j < numShapes; j++)
				{
					KFbxShape* pShape = pMesh->GetShape(j);
					ImportedMorphKeyframe^ morph = gcnew ImportedMorphKeyframe();
					morphList->KeyframeList->Add(morph);

					String^ shapeName = gcnew String(pMesh->GetShapeName(j));
					int shapeNameStartIdx = shapeName->LastIndexOf(".");
					if (shapeNameStartIdx >= 0)
					{
						shapeNameStartIdx += 1;
						shapeName = shapeName->Substring(shapeNameStartIdx, shapeName->Length - shapeNameStartIdx);
					}
					morph->Name = shapeName;
					
					KFbxLayer* pLayerNormal = pShape->GetLayer(0, KFbxLayerElement::eNORMAL);
					KFbxLayerElementNormal* pLayerElementNormal = NULL;
					if (pLayerNormal != NULL)
					{
						pLayerElementNormal = pLayerNormal->GetNormals();
					}

					int numVerts = pShape->GetControlPointsCount();
					List<ImportedVertex^>^ vertList = gcnew List<ImportedVertex^>(numVerts);
					morph->VertexList = vertList;
					for (int k = 0; k < numVerts; k++)
					{
						ImportedVertex^ vertInfo = gcnew ImportedVertex();
						vertList->Add(vertInfo);
						vertInfo->BoneIndices = gcnew array<Byte>(4);
						vertInfo->Weights = gcnew array<float>(4);
						vertInfo->UV = gcnew array<float>(2);

						KFbxVector4 lCoords = pShape->GetControlPointAt(k);
						vertInfo->Position = Vector3((float)lCoords[0], (float)lCoords[1], (float)lCoords[2]);

						if (pLayerElementNormal == NULL)
						{
							vertInfo->Normal = Vector3(0);
						}
						else
						{
							KFbxVector4 lNorm;
							GetVector(pLayerElementNormal, lNorm, k, k);
							vertInfo->Normal = Vector3((float)lNorm[0], (float)lNorm[1], (float)lNorm[2]);
						}
					}
				}
			}
		}
	}
}
