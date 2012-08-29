#include <fbxsdk.h>
#include <fbxfilesdk/kfbxio/kfbxiosettings.h>
#include "SB3UtilityFBX.h"

namespace SB3Utility
{
	void Fbx::Exporter::Export(String^ path, xxParser^ xxParser, List<xxFrame^>^ meshParents, List<xaParser^>^ xaSubfileList, String^ exportFormat, bool allFrames, bool skins)
	{
		FileInfo^ file = gcnew FileInfo(path);
		DirectoryInfo^ dir = file->Directory;
		if (!dir->Exists)
		{
			dir->Create();
		}
		String^ currentDir = Directory::GetCurrentDirectory();
		Directory::SetCurrentDirectory(dir->FullName);

		Exporter^ exporter = gcnew Exporter(path, xxParser, meshParents, exportFormat, allFrames, skins);
		exporter->ExportAnimations(xaSubfileList);
		exporter->pExporter->Export(exporter->pScene);

		Directory::SetCurrentDirectory(currentDir);
	}

	void Fbx::Exporter::ExportMorph(String^ path, xxParser^ xxParser, xxFrame^ meshFrame, xaMorphClip^ morphClip, xaParser^ xaparser, String^ exportFormat)
	{
		FileInfo^ file = gcnew FileInfo(path);
		DirectoryInfo^ dir = file->Directory;
		if (!dir->Exists)
		{
			dir->Create();
		}
		String^ currentDir = Directory::GetCurrentDirectory();
		Directory::SetCurrentDirectory(dir->FullName);

		List<xxFrame^>^ meshParents = gcnew List<xxFrame^>(1);
		meshParents->Add(meshFrame);
		Exporter^ exporter = gcnew Exporter(path, xxParser, meshParents, exportFormat, false, false);
		exporter->ExportMorphs(meshFrame, morphClip, xaparser);
		exporter->pExporter->Export(exporter->pScene);

		Directory::SetCurrentDirectory(currentDir);
	}

	Fbx::Exporter::Exporter(String^ path, xxParser^ xxparser, List<xxFrame^>^ meshParents, String^ exportFormat, bool allFrames, bool skins)
	{
		this->xxparser = xxparser;
		exportSkins = skins;
		meshNames = gcnew HashSet<String^>();
		for (int i = 0; i < meshParents->Count; i++)
		{
			meshNames->Add(meshParents[i]->Name);
		}

		frameNames = nullptr;
		if (!allFrames)
		{
			frameNames = xx::SearchHierarchy(xxparser->Frame, meshNames);
		}

		cDest = NULL;
		cFormat = NULL;
		pSdkManager = NULL;
		pScene = NULL;
		pExporter = NULL;
		pMaterials = NULL;
		pTextures = NULL;
		pMeshNodes = NULL;

		pin_ptr<KFbxSdkManager*> pSdkManagerPin = &pSdkManager;
		pin_ptr<KFbxScene*> pScenePin = &pScene;
		Init(pSdkManagerPin, pScenePin);

		cDest = Fbx::StringToCharArray(path);
		cFormat = Fbx::StringToCharArray(exportFormat);
		pExporter = KFbxExporter::Create(pSdkManager, "");
		int lFormatIndex, lFormatCount = pSdkManager->GetIOPluginRegistry()->GetWriterFormatCount();
		for (lFormatIndex = 0; lFormatIndex < lFormatCount; lFormatIndex++)
		{
			KString lDesc = KString(pSdkManager->GetIOPluginRegistry()->GetWriterFormatDescription(lFormatIndex));
			if (lDesc.Find(cFormat) >= 0)
			{
				if (pSdkManager->GetIOPluginRegistry()->WriterIsFBX(lFormatIndex))
				{
					if (lDesc.Find("binary") >= 0)
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
		}

		IOS_REF.SetBoolProp(EXP_FBX_MATERIAL, true);
		IOS_REF.SetBoolProp(EXP_FBX_TEXTURE, true);
		IOS_REF.SetBoolProp(EXP_FBX_EMBEDDED, false);
		IOS_REF.SetBoolProp(EXP_FBX_SHAPE, true);
		IOS_REF.SetBoolProp(EXP_FBX_GOBO, true);
		IOS_REF.SetBoolProp(EXP_FBX_ANIMATION, true);
		IOS_REF.SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);

		if (!pExporter->Initialize(cDest, lFormatIndex, pSdkManager->GetIOSettings()))
		{
			throw gcnew Exception(gcnew String("Failed to initialize KFbxExporter: ") + gcnew String(pExporter->GetLastErrorString()));
		}

		if (xxparser != nullptr)
		{
			pMaterials = new KArrayTemplate<KFbxSurfacePhong*>();
			pTextures = new KArrayTemplate<KFbxFileTexture*>();
			pMaterials->Reserve(xxparser->MaterialList->Count);
			pTextures->Reserve(xxparser->TextureList->Count);

			meshFrames = gcnew List<xxFrame^>();
			pMeshNodes = new KArrayTemplate<KFbxNode*>();
			ExportFrame(pScene->GetRootNode(), xxparser->Frame);

			SetJoints();

			for (int i = 0; i < meshFrames->Count; i++)
			{
				ExportMesh(pMeshNodes->GetAt(i), meshFrames[i]);
			}
		}
	}

	Fbx::Exporter::~Exporter()
	{
		this->!Exporter();
	}

	Fbx::Exporter::!Exporter()
	{
		if (pMeshNodes != NULL)
		{
			delete pMeshNodes;
		}
		if (pMaterials != NULL)
		{
			delete pMaterials;
		}
		if (pTextures != NULL)
		{
			delete pTextures;
		}
		if (pExporter != NULL)
		{
			pExporter->Destroy();
		}
		if (pScene != NULL)
		{
			pScene->Destroy();
		}
		if (pSdkManager != NULL)
		{
			pSdkManager->Destroy();
		}
		if (cFormat != NULL)
		{
			Marshal::FreeHGlobal((IntPtr)cFormat);
		}
		if (cDest != NULL)
		{
			Marshal::FreeHGlobal((IntPtr)cDest);
		}
	}

	void Fbx::Exporter::SetJoints()
	{
		List<xxFrame^>^ meshes = xx::FindMeshFrames(xxparser->Frame);
		HashSet<String^>^ boneNames = gcnew HashSet<String^>();
		for (int i = 0; i < meshes->Count; i++)
		{
			xxMesh^ meshList = meshes[i]->Mesh;
			List<xxBone^>^ boneList = meshList->BoneList;
			for (int j = 0; j < boneList->Count; j++)
			{
				xxBone^ bone = boneList[j];
				boneNames->Add(bone->Name);
			}
		}

		SetJointsNode(pScene->GetRootNode()->GetChild(0), boneNames);
	}

	void Fbx::Exporter::SetJointsNode(KFbxNode* pNode, HashSet<String^>^ boneNames)
	{
		String^ nodeName = gcnew String(pNode->GetName());
		if (boneNames->Contains(nodeName))
		{
			KFbxSkeleton* pJoint = KFbxSkeleton::Create(pSdkManager, "");
			pJoint->SetSkeletonType(KFbxSkeleton::eLIMB_NODE);
			pNode->SetNodeAttribute(pJoint);
		}
		else
		{
			KFbxNull* pNull = KFbxNull::Create(pSdkManager, "");
			if (pNode->GetChildCount() > 0)
			{
				pNull->Look.Set(KFbxNull::eNONE);
			}

			pNode->SetNodeAttribute(pNull);
		}

		for (int i = 0; i < pNode->GetChildCount(); i++)
		{
			SetJointsNode(pNode->GetChild(i), boneNames);
		}
	}

	void Fbx::Exporter::ExportFrame(KFbxNode* pParentNode, xxFrame^ frame)
	{
		String^ frameName = frame->Name;
		if ((frameNames == nullptr) || frameNames->Contains(frameName))
		{
			KFbxNode* pFrameNode = NULL;
			char* pName = NULL;
			try
			{
				pName = StringToCharArray(frameName);
				pFrameNode = KFbxNode::Create(pSdkManager, pName);
			}
			finally
			{
				Marshal::FreeHGlobal((IntPtr)pName);
			}

			Vector3 scale, translate;
			Quaternion rotate;
			frame->Matrix.Decompose(scale, rotate, translate);
			Vector3 rotateVector = Fbx::QuaternionToEuler(rotate);

			pFrameNode->LclScaling.Set(KFbxVector4(scale.X , scale.Y, scale.Z));
			pFrameNode->LclRotation.Set(KFbxVector4(fbxDouble3(rotateVector.X, rotateVector.Y, rotateVector.Z)));
			pFrameNode->LclTranslation.Set(KFbxVector4(translate.X, translate.Y, translate.Z));
			pParentNode->AddChild(pFrameNode);

			if (meshNames->Contains(frameName) && (frame->Mesh != nullptr))
			{
				meshFrames->Add(frame);
				pMeshNodes->Add(pFrameNode);
			}

			for (int i = 0; i < frame->Count; i++)
			{
				ExportFrame(pFrameNode, frame[i]);
			}
		}
	}

	void Fbx::Exporter::ExportMesh(KFbxNode* pFrameNode, xxFrame^ frame)
	{
		xxMesh^ meshList = frame->Mesh;
		String^ frameName = frame->Name;
		List<xxBone^>^ boneList = meshList->BoneList;
		bool hasBones;
		if (exportSkins)
		{
			hasBones = boneList->Count > 0;
		}
		else
		{
			hasBones = false;
		}

		KArrayTemplate<KFbxNode*>* pBoneNodeList = NULL;
		try
		{
			if (hasBones)
			{
				pBoneNodeList = new KArrayTemplate<KFbxNode*>();
				pBoneNodeList->Reserve(boneList->Count);
				for (int i = 0; i < boneList->Count; i++)
				{
					xxBone^ bone = boneList[i];
					String^ boneName = bone->Name;
					char* pBoneName = NULL;
					try
					{
						pBoneName = StringToCharArray(boneName);
						KFbxNode* foundNode = pScene->GetRootNode()->FindChild(pBoneName);
						if (foundNode == NULL)
						{
							throw gcnew Exception(gcnew String("Couldn't find frame ") + boneName + gcnew String(" used by the bone"));
						}
						pBoneNodeList->Add(foundNode);
					}
					finally
					{
						Marshal::FreeHGlobal((IntPtr)pBoneName);
					}
				}
			}

			for (int i = 0; i < meshList->SubmeshList->Count; i++)
			{
				char* pName = NULL;
				KArrayTemplate<KFbxCluster*>* pClusterArray = NULL;
				try
				{
					pName = StringToCharArray(frameName + "_" + i);
					KFbxMesh* pMesh = KFbxMesh::Create(pSdkManager, "");

					if (hasBones)
					{
						pClusterArray = new KArrayTemplate<KFbxCluster*>();
						pClusterArray->Reserve(boneList->Count);

						for (int i = 0; i < boneList->Count; i++)
						{
							KFbxNode* pNode = pBoneNodeList->GetAt(i);
							KString lClusterName = pNode->GetNameOnly() + KString("Cluster");
							KFbxCluster* pCluster = KFbxCluster::Create(pSdkManager, lClusterName.Buffer());
							pCluster->SetLink(pNode);
							pCluster->SetLinkMode(KFbxCluster::eTOTAL1);
							pClusterArray->Add(pCluster);
						}
					}

					xxSubmesh^ meshObj = meshList->SubmeshList[i];
					List<xxFace^>^ faceList = meshObj->FaceList;
					List<xxVertex^>^ vertexList = meshObj->VertexList;

					KFbxLayer* pLayer = pMesh->GetLayer(0);
					if (pLayer == NULL)
					{
						pMesh->CreateLayer();
						pLayer = pMesh->GetLayer(0);
					}

					pMesh->InitControlPoints(vertexList->Count);
					KFbxVector4* pControlPoints = pMesh->GetControlPoints();

					KFbxLayerElementNormal* pLayerElementNormal = KFbxLayerElementNormal::Create(pMesh, "");
					pLayerElementNormal->SetMappingMode(KFbxLayerElement::eBY_CONTROL_POINT);
					pLayerElementNormal->SetReferenceMode(KFbxLayerElement::eDIRECT);
					pLayer->SetNormals(pLayerElementNormal);

					KFbxLayerElementUV* pUVLayer = KFbxLayerElementUV::Create(pMesh, "");
					pUVLayer->SetMappingMode(KFbxLayerElement::eBY_CONTROL_POINT);
					pUVLayer->SetReferenceMode(KFbxLayerElement::eDIRECT);
					pLayer->SetUVs(pUVLayer, KFbxLayerElement::eDIFFUSE_TEXTURES);

					KFbxNode* pMeshNode = KFbxNode::Create(pSdkManager, pName);
					pMeshNode->SetNodeAttribute(pMesh);
					pFrameNode->AddChild(pMeshNode);

					List<xxMaterial^>^ pMatSection = xxparser->MaterialList;
					int matIdx = meshObj->MaterialIndex;
					if ((matIdx >= 0) && (matIdx < pMatSection->Count))
					{
						KFbxLayerElementMaterial* pMaterialLayer = KFbxLayerElementMaterial::Create(pMesh, "");
						pMaterialLayer->SetMappingMode(KFbxLayerElement::eALL_SAME);
						pMaterialLayer->SetReferenceMode(KFbxLayerElement::eINDEX_TO_DIRECT);
						pMaterialLayer->GetIndexArray().Add(0);
						pLayer->SetMaterials(pMaterialLayer);

						char* pMatName = NULL;
						try
						{
							xxMaterial^ mat = pMatSection[matIdx];
							pMatName = StringToCharArray(mat->Name);
							int foundMat = -1;
							for (int j = 0; j < pMaterials->GetCount(); j++)
							{
								KFbxSurfacePhong* pMatTemp = pMaterials->GetAt(j);
								if (strcmp(pMatTemp->GetName(), pMatName) == 0)
								{
									foundMat = j;
									break;
								}
							}

							KFbxSurfacePhong* pMat;
							if (foundMat >= 0)
							{
								pMat = pMaterials->GetAt(foundMat);
							}
							else
							{
								KString lShadingName  = "Phong";
								Color4 diffuse = mat->Diffuse;
								Color4 ambient = mat->Ambient;
								Color4 emissive = mat->Emissive;
								Color4 specular = mat->Specular;
								float specularPower = mat->Power;
								pMat = KFbxSurfacePhong::Create(pSdkManager, pMatName);
								pMat->Diffuse.Set(fbxDouble3(diffuse.Red, diffuse.Green, diffuse.Blue));
								pMat->DiffuseFactor.Set(fbxDouble1(diffuse.Alpha));
								pMat->Ambient.Set(fbxDouble3(ambient.Red, ambient.Green, ambient.Blue));
								pMat->AmbientFactor.Set(fbxDouble1(ambient.Alpha));
								pMat->Emissive.Set(fbxDouble3(emissive.Red, emissive.Green, emissive.Blue));
								pMat->EmissiveFactor.Set(fbxDouble1(emissive.Alpha));
								pMat->Specular.Set(fbxDouble3(specular.Red, specular.Green, specular.Blue));
								pMat->SpecularFactor.Set(fbxDouble1(specular.Alpha));
								pMat->Shininess.Set(specularPower);
								pMat->ShadingModel.Set(lShadingName);

								foundMat = pMaterials->GetCount();
								pMaterials->Add(pMat);
							}
							pMeshNode->AddMaterial(pMat);

							bool hasTexture = false;
							KFbxLayerElementTexture* pTextureLayerDiffuse = NULL;
							KFbxFileTexture* pTextureDiffuse = ExportTexture(mat->Textures[0], pTextureLayerDiffuse, pMesh);
							if (pTextureDiffuse != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eDIFFUSE_TEXTURES, pTextureLayerDiffuse);
								pMat->Diffuse.ConnectSrcObject(pTextureDiffuse);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerAmbient = NULL;
							KFbxFileTexture* pTextureAmbient = ExportTexture(mat->Textures[1], pTextureLayerAmbient, pMesh);
							if (pTextureAmbient != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eAMBIENT_TEXTURES, pTextureLayerAmbient);
								pMat->Ambient.ConnectSrcObject(pTextureAmbient);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerEmissive = NULL;
							KFbxFileTexture* pTextureEmissive = ExportTexture(mat->Textures[2], pTextureLayerEmissive, pMesh);
							if (pTextureEmissive != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eEMISSIVE_TEXTURES, pTextureLayerEmissive);
								pMat->Emissive.ConnectSrcObject(pTextureEmissive);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerSpecular = NULL;
							KFbxFileTexture* pTextureSpecular = ExportTexture(mat->Textures[3], pTextureLayerSpecular, pMesh);
							if (pTextureSpecular != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eSPECULAR_TEXTURES, pTextureLayerSpecular);
								pMat->Specular.ConnectSrcObject(pTextureSpecular);
								hasTexture = true;
							}

							if (hasTexture)
							{
								pMeshNode->SetShadingMode(KFbxNode::eTEXTURE_SHADING);
							}
						}
						finally
						{
							Marshal::FreeHGlobal((IntPtr)pMatName);
						}
					}

					for (int j = 0; j < vertexList->Count; j++)
					{
						xxVertex^ vertex = vertexList[j];
						Vector3 coords = vertex->Position;
						pControlPoints[j] = KFbxVector4(coords.X, coords.Y, coords.Z);
						Vector3 normal = vertex->Normal;
						pLayerElementNormal->GetDirectArray().Add(KFbxVector4(normal.X, normal.Y, normal.Z));
						array<float>^ uv = vertex->UV;
						pUVLayer->GetDirectArray().Add(KFbxVector2(uv[0], -uv[1]));

						if (hasBones)
						{
							array<unsigned char>^ boneIndices = vertex->BoneIndices;
							array<float>^ weights4 = vertex->Weights4(hasBones);
							for (int k = 0; k < weights4->Length; k++)
							{
								if (boneIndices[k] < boneList->Count)
								{
									KFbxCluster* pCluster = pClusterArray->GetAt(boneIndices[k]);
									pCluster->AddControlPointIndex(j, weights4[k]);
								}
							}
						}
					}

					for (int j = 0; j < faceList->Count; j++)
					{
						xxFace^ face = faceList[j];
						unsigned short v1 = face->VertexIndices[0];
						unsigned short v2 = face->VertexIndices[1];
						unsigned short v3 = face->VertexIndices[2];
						pMesh->BeginPolygon(false);
						pMesh->AddPolygon(v1);
						pMesh->AddPolygon(v2);
						pMesh->AddPolygon(v3);
						pMesh->EndPolygon();
					}

					if (hasBones)
					{
						KFbxSkin* pSkin = KFbxSkin::Create(pSdkManager, "");
						for (int j = 0; j < boneList->Count; j++)
						{
							KFbxCluster* pCluster = pClusterArray->GetAt(j);
							if (pCluster->GetControlPointIndicesCount() > 0)
							{
								KFbxNode* pBoneNode = pBoneNodeList->GetAt(j);
								Matrix boneMatrix = boneList[j]->Matrix;
								KFbxXMatrix lBoneMatrix;
								for (int m = 0; m < 4; m++)
								{
									for (int n = 0; n < 4; n++)
									{
										lBoneMatrix.mData[m][n] = boneMatrix[m, n];
									}
								}

								KFbxXMatrix lMeshMatrix = pScene->GetEvaluator()->GetNodeGlobalTransform(pMeshNode);

								pCluster->SetTransformMatrix(lMeshMatrix);
								pCluster->SetTransformLinkMatrix(lMeshMatrix * lBoneMatrix.Inverse());

								pSkin->AddCluster(pCluster);
							}
						}

						if (pSkin->GetClusterCount() > 0)
						{
							pMesh->AddDeformer(pSkin);
						}
					}
				}
				finally
				{
					if (pClusterArray != NULL)
					{
						delete pClusterArray;
					}
					Marshal::FreeHGlobal((IntPtr)pName);
				}
			}
		}
		finally
		{
			if (pBoneNodeList != NULL)
			{
				delete pBoneNodeList;
			}
		}
	}

	KFbxFileTexture* Fbx::Exporter::ExportTexture(xxMaterialTexture^ matTex, KFbxLayerElementTexture*& pTextureLayer, KFbxMesh* pMesh)
	{
		KFbxFileTexture* pTex = NULL;

		String^ matTexName = matTex->Name;
		if (matTexName != String::Empty)
		{
			pTextureLayer = KFbxLayerElementTexture::Create(pMesh, "");
			pTextureLayer->SetMappingMode(KFbxLayerElement::eALL_SAME);
			pTextureLayer->SetReferenceMode(KFbxLayerElement::eDIRECT);

			char* pTexName = NULL;
			try
			{
				pTexName = StringToCharArray(matTexName);
				int foundTex = -1;
				for (int i = 0; i < pTextures->GetCount(); i++)
				{
					KFbxFileTexture* pTexTemp = pTextures->GetAt(i);
					if (strcmp(pTexTemp->GetName(), pTexName) == 0)
					{
						foundTex = i;
						break;
					}
				}

				if (foundTex >= 0)
				{
					pTex = pTextures->GetAt(foundTex);
				}
				else
				{
					pTex = KFbxFileTexture::Create(pSdkManager, pTexName);
					pTex->SetFileName(pTexName);
					pTex->SetTextureUse(KFbxTexture::eSTANDARD);
					pTex->SetMappingType(KFbxTexture::eUV);
					pTex->SetMaterialUse(KFbxFileTexture::eMODEL_MATERIAL);
					pTex->SetSwapUV(false);
					pTex->SetTranslation(0.0, 0.0);
					pTex->SetScale(1.0, 1.0);
					pTex->SetRotation(0.0, 0.0);
					pTextures->Add(pTex);

					List<xxTexture^>^ pTexSection = xxparser->TextureList;
					for (int j = 0; j < pTexSection->Count; j++)
					{
						xxTexture^ hTex = pTexSection[j];
						String^ hTexName = hTex->Name;
						if (matTexName == hTexName)
						{
							xx::ExportTexture(hTex, Path::GetDirectoryName(gcnew String(pExporter->GetFileName().Buffer())) + Path::DirectorySeparatorChar + Path::GetFileName(hTexName));
							break;
						}
					}
				}
				
				pTextureLayer->GetDirectArray().Add(pTex);
			}
			finally
			{
				Marshal::FreeHGlobal((IntPtr)pTexName);
			}
		}

		return pTex;
	}

	void Fbx::Exporter::ExportAnimations(List<xaParser^>^ xaSubfileList)
	{
		if (xaSubfileList == nullptr)
		{
			return;
		}

		List<String^>^ pNotFound = gcnew List<String^>();

		for (int i = 0; i < xaSubfileList->Count; i++)
		{
			xaParser^ parser = xaSubfileList[i];
			List<xaAnimationTrack^>^ pAnimationList = parser->AnimationSection->TrackList;

			KString kTakeName = KString("Take") + KString(i);
			char* lTakeName = kTakeName.Buffer();

			KTime lTime;
			KFbxAnimStack* lAnimStack = KFbxAnimStack::Create(pScene, lTakeName);
			KFbxAnimLayer* lAnimLayer = KFbxAnimLayer::Create(pScene, "Base Layer");
			lAnimStack->AddMember(lAnimLayer);

			for (int j = 0; j < pAnimationList->Count; j++)
			{
				xaAnimationTrack^ keyframeList = pAnimationList[j];
				String^ name = keyframeList->Name;
				KFbxNode* pNode = NULL;
				char* pName = NULL;
				try
				{
					pName = Fbx::StringToCharArray(name);
					pNode = pScene->GetRootNode()->FindChild(pName);
				}
				finally
				{
					Marshal::FreeHGlobal((IntPtr)pName);
				}

				if (pNode == NULL)
				{
					if (!pNotFound->Contains(name))
					{
						pNotFound->Add(name);
					}
				}
				else
				{
					KFbxAnimCurve* lCurveSX = pNode->LclScaling.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_S_X, true);
					KFbxAnimCurve* lCurveSY = pNode->LclScaling.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_S_Y, true);
					KFbxAnimCurve* lCurveSZ = pNode->LclScaling.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_S_Z, true);
					KFbxAnimCurve* lCurveRX = pNode->LclRotation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_R_X, true);
					KFbxAnimCurve* lCurveRY = pNode->LclRotation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_R_Y, true);
					KFbxAnimCurve* lCurveRZ = pNode->LclRotation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_R_Z, true);
					KFbxAnimCurve* lCurveTX = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_T_X, true);
					KFbxAnimCurve* lCurveTY = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_T_Y, true);
					KFbxAnimCurve* lCurveTZ = pNode->LclTranslation.GetCurve<KFbxAnimCurve>(lAnimLayer, KFCURVENODE_T_Z, true);

					lCurveSX->KeyModifyBegin();
					lCurveSY->KeyModifyBegin();
					lCurveSZ->KeyModifyBegin();
					lCurveRX->KeyModifyBegin();
					lCurveRY->KeyModifyBegin();
					lCurveRZ->KeyModifyBegin();
					lCurveTX->KeyModifyBegin();
					lCurveTY->KeyModifyBegin();
					lCurveTZ->KeyModifyBegin();

					List<xaAnimationKeyframe^>^ keyframes = keyframeList->KeyframeList;
					double fps = 1.0 / 24;
					for (int k = 0; k < keyframes->Count; k++)
					{
						lTime.SetSecondDouble(fps * k);

						lCurveSX->KeyAdd(lTime);
						lCurveSY->KeyAdd(lTime);
						lCurveSZ->KeyAdd(lTime);
						lCurveRX->KeyAdd(lTime);
						lCurveRY->KeyAdd(lTime);
						lCurveRZ->KeyAdd(lTime);
						lCurveTX->KeyAdd(lTime);
						lCurveTY->KeyAdd(lTime);
						lCurveTZ->KeyAdd(lTime);

						Vector3 rotation = Fbx::QuaternionToEuler(keyframes[k]->Rotation);
						lCurveSX->KeySet(k, lTime, keyframes[k]->Scaling.X);
						lCurveSY->KeySet(k, lTime, keyframes[k]->Scaling.Y);
						lCurveSZ->KeySet(k, lTime, keyframes[k]->Scaling.Z);
						lCurveRX->KeySet(k, lTime, rotation.X);
						lCurveRY->KeySet(k, lTime, rotation.Y);
						lCurveRZ->KeySet(k, lTime, rotation.Z);
						lCurveTX->KeySet(k, lTime, keyframes[k]->Translation.X);
						lCurveTY->KeySet(k, lTime, keyframes[k]->Translation.Y);
						lCurveTZ->KeySet(k, lTime, keyframes[k]->Translation.Z);
					}
					lCurveSX->KeyModifyEnd();
					lCurveSY->KeyModifyEnd();
					lCurveSZ->KeyModifyEnd();
					lCurveRX->KeyModifyEnd();
					lCurveRY->KeyModifyEnd();
					lCurveRZ->KeyModifyEnd();
					lCurveTX->KeyModifyEnd();
					lCurveTY->KeyModifyEnd();
					lCurveTZ->KeyModifyEnd();
				}
			}
		}

		if (pNotFound->Count > 0)
		{
			String^ pNotFoundString = gcnew String("Warning: Animations weren't exported for the following missing frames: ");
			for (int i = 0; i < pNotFound->Count; i++)
			{
				pNotFoundString += pNotFound[i] + ", ";
			}
			Report::ReportLog(pNotFoundString->Substring(0, pNotFoundString->Length - 2));
		}
	}

	void Fbx::Exporter::ExportMorphs(xxFrame^ baseFrame, xaMorphClip^ morphClip, xaParser^ xaparser)
	{
		KFbxNode* pBaseNode = pMeshNodes->GetAt(0);
		xaMorphSection^ morphSection = xaparser->MorphSection;

		array<unsigned short>^ meshIndices;
		array<unsigned short>^ morphIndices;
		xaMorphIndexSet^ indexSet = xa::FindMorphIndexSet(morphClip->Name, morphSection);
		meshIndices = indexSet->MeshIndices;
		morphIndices = indexSet->MorphIndices;

		xxMesh^ meshList = baseFrame->Mesh;
		int meshObjIdx = xa::MorphMeshObjIdx(meshIndices, meshList);
		if (meshObjIdx < 0)
		{
			throw gcnew Exception("No valid mesh object was found for the morph");
		}
		xxSubmesh^ meshObjBase = meshList->SubmeshList[meshObjIdx];
		List<xxVertex^>^ vertList = meshObjBase->VertexList;

		KFbxNode* pBaseMeshNode = pBaseNode->GetChild(meshObjIdx);
		KFbxMesh* pBaseMesh = pBaseMeshNode->GetMesh();
		char* pMorphClipName = NULL;
		try
		{
			String^ morphClipName = gcnew String(pBaseMeshNode->GetName()) + "_morph_" + morphClip->Name;
			pMorphClipName = StringToCharArray(morphClipName);
			pBaseMeshNode->SetName(pMorphClipName);
		}
		finally
		{
			Marshal::FreeHGlobal((IntPtr)pMorphClipName);
		}

		KFbxLayer* pBaseLayer = pBaseMesh->GetLayer(0);
		KFbxLayerElementVertexColor* pVertexColorLayer = KFbxLayerElementVertexColor::Create(pBaseMesh, "");
		pVertexColorLayer->SetMappingMode(KFbxLayerElement::eBY_CONTROL_POINT);
		pVertexColorLayer->SetReferenceMode(KFbxLayerElement::eDIRECT);
		pBaseLayer->SetVertexColors(pVertexColorLayer);
		for (int i = 0; i < vertList->Count; i++)
		{
			pVertexColorLayer->GetDirectArray().Add(KFbxColor(1, 1, 1));
		}
		for (int i = 0; i < meshIndices->Length; i++)
		{
			pVertexColorLayer->GetDirectArray().SetAt(meshIndices[i], KFbxColor(0, 0, 1));
		}

		List<xaMorphKeyframeRef^>^ refList = morphClip->KeyframeRefList;
		List<String^>^ morphNames = gcnew List<String^>(refList->Count);
		for (int i = 0; i < refList->Count; i++)
		{
			if (!morphNames->Contains(refList[i]->Name))
			{
				xaMorphKeyframe^ keyframe = xa::FindMorphKeyFrame(refList[i]->Name, morphSection);

				KFbxShape* pShape = KFbxShape::Create(pScene, "");
				pShape->InitControlPoints(vertList->Count);
				KFbxVector4* pControlPoints = pShape->GetControlPoints();

				KFbxLayer* pLayer = pShape->GetLayer(0);
				if (pLayer == NULL)
				{
					pShape->CreateLayer();
					pLayer = pShape->GetLayer(0);
				}

				KFbxLayerElementNormal* pLayerElementNormal = KFbxLayerElementNormal::Create(pShape, "");
				pLayerElementNormal->SetMappingMode(KFbxLayerElement::eBY_CONTROL_POINT);
				pLayerElementNormal->SetReferenceMode(KFbxLayerElement::eDIRECT);
				pLayer->SetNormals(pLayerElementNormal);

				for (int j = 0; j < vertList->Count; j++)
				{
					xxVertex^ vertex = vertList[j];
					Vector3 coords = vertex->Position;
					pControlPoints[j] = KFbxVector4(coords.X, coords.Y, coords.Z);
					Vector3 normal = vertex->Normal;
					pLayerElementNormal->GetDirectArray().Add(KFbxVector4(normal.X, normal.Y, normal.Z));
				}
				for (int j = 0; j < meshIndices->Length; j++)
				{
					Vector3 coords = keyframe->PositionList[morphIndices[j]];
					pControlPoints[meshIndices[j]] = KFbxVector4(coords.X, coords.Y, coords.Z);
					Vector3 normal = keyframe->NormalList[morphIndices[j]];
					pLayerElementNormal->GetDirectArray().SetAt(meshIndices[j], KFbxVector4(normal.X, normal.Y, normal.Z));
				}

				char* pMorphShapeName = NULL;
				try
				{
					pMorphShapeName = StringToCharArray(keyframe->Name);
					pBaseNode->GetChild(meshObjIdx)->GetMesh()->AddShape(pShape, pMorphShapeName);
				}
				finally
				{
					Marshal::FreeHGlobal((IntPtr)pMorphShapeName);
				}
			}
		}
	}
}
