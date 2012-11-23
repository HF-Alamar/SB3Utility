#include <fbxsdk.h>
#include <fbxfilesdk/kfbxio/kfbxiosettings.h>
#include "SB3UtilityFBX.h"

namespace SB3Utility
{
	void Fbx::Exporter::Export(String^ path, IImported^ imported, int startKeyframe, int endKeyframe, bool linear, bool EulerFilter, float filterPrecision, String^ exportFormat, bool allFrames, bool skins)
	{
		FileInfo^ file = gcnew FileInfo(path);
		DirectoryInfo^ dir = file->Directory;
		if (!dir->Exists)
		{
			dir->Create();
		}
		String^ currentDir = Directory::GetCurrentDirectory();
		Directory::SetCurrentDirectory(dir->FullName);

		Exporter^ exporter = gcnew Exporter(path, imported, exportFormat, allFrames, skins);
		exporter->ExportAnimations(startKeyframe, endKeyframe, linear, EulerFilter, filterPrecision);
		exporter->pExporter->Export(exporter->pScene);

		Directory::SetCurrentDirectory(currentDir);
	}

	Fbx::Exporter::Exporter(String^ path, IImported^ imported, String^ exportFormat, bool allFrames, bool skins)
	{
		this->imported = imported;
		exportSkins = skins;

		frameNames = nullptr;
		if (!allFrames)
		{
			frameNames = SearchHierarchy();
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

		pMaterials = new KArrayTemplate<KFbxSurfacePhong*>();
		pTextures = new KArrayTemplate<KFbxFileTexture*>();
		pMaterials->Reserve(imported->MaterialList->Count);
		pTextures->Reserve(imported->TextureList->Count);

		pMeshNodes = new KArrayTemplate<KFbxNode*>();
		ExportFrame(pScene->GetRootNode(), imported->FrameList[0]);

		SetJointsFromImportedMeshes();

		for (int i = 0; i < imported->MeshList->Count; i++)
		{
			ExportMesh(pMeshNodes->GetAt(i), imported->MeshList[i]);
		}
	}

	HashSet<String^>^ Fbx::Exporter::SearchHierarchy()
	{
		HashSet<String^>^ exportFrames = gcnew HashSet<String^>();
		SearchHierarchy(imported->FrameList[0], exportFrames);
		return exportFrames;
	}

	void Fbx::Exporter::SearchHierarchy(ImportedFrame^ frame, HashSet<String^>^ exportFrames)
	{
		ImportedMesh^ meshListSome = ImportedHelpers::FindMesh(frame->Name, imported);
		if (meshListSome != nullptr)
		{
			ImportedFrame^ parent = frame;
			while (parent != nullptr)
			{
				exportFrames->Add(parent->Name);
				parent = (ImportedFrame^)parent->Parent;
			}

			List<ImportedBone^>^ boneList = meshListSome->BoneList;
			for (int i = 0; i < boneList->Count; i++)
			{
				if (!exportFrames->Contains(boneList[i]->Name))
				{
					ImportedFrame^ boneParent = ImportedHelpers::FindFrame(boneList[i]->Name, imported->FrameList[0]);
					while (boneParent != nullptr)
					{
						exportFrames->Add(boneParent->Name);
						boneParent = (ImportedFrame^)boneParent->Parent;
					}
				}
			}
		}

		for (int i = 0; i < frame->Count; i++)
		{
			SearchHierarchy(frame[i], exportFrames);
		}
	}

	void Fbx::Exporter::SetJointsFromImportedMeshes()
	{
		HashSet<String^>^ boneNames = gcnew HashSet<String^>();
		for (int i = 0; i < imported->MeshList->Count; i++)
		{
			ImportedMesh^ meshList = imported->MeshList[i];
			List<ImportedBone^>^ boneList = meshList->BoneList;
			for (int j = 0; j < boneList->Count; j++)
			{
				ImportedBone^ bone = boneList[j];
				boneNames->Add(bone->Name);
			}
		}

		SetJointsNode(pScene->GetRootNode()->GetChild(0), boneNames);
	}

	void Fbx::Exporter::ExportFrame(KFbxNode* pParentNode, ImportedFrame^ frame)
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

			if (ImportedHelpers::FindMesh(frameName, imported) != nullptr)
			{
				pMeshNodes->Add(pFrameNode);
			}

			for (int i = 0; i < frame->Count; i++)
			{
				ExportFrame(pFrameNode, frame[i]);
			}
		}
	}

	void Fbx::Exporter::ExportMesh(KFbxNode* pFrameNode, ImportedMesh^ meshList)
	{
		String^ frameName = meshList->Name;
		List<ImportedBone^>^ boneList = meshList->BoneList;
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
					ImportedBone^ bone = boneList[i];
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

					ImportedSubmesh^ meshObj = meshList->SubmeshList[i];
					List<ImportedFace^>^ faceList = meshObj->FaceList;
					List<ImportedVertex^>^ vertexList = meshObj->VertexList;

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

					ImportedMaterial^ mat = ImportedHelpers::FindMaterial(meshObj->Material, imported);
					if (mat != nullptr)
					{
						KFbxLayerElementMaterial* pMaterialLayer = KFbxLayerElementMaterial::Create(pMesh, "");
						pMaterialLayer->SetMappingMode(KFbxLayerElement::eALL_SAME);
						pMaterialLayer->SetReferenceMode(KFbxLayerElement::eINDEX_TO_DIRECT);
						pMaterialLayer->GetIndexArray().Add(0);
						pLayer->SetMaterials(pMaterialLayer);

						char* pMatName = NULL;
						try
						{
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
							KFbxFileTexture* pTextureDiffuse = ExportTexture(ImportedHelpers::FindTexture(mat->Textures[0], imported), pTextureLayerDiffuse, pMesh);
							if (pTextureDiffuse != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eDIFFUSE_TEXTURES, pTextureLayerDiffuse);
								pMat->Diffuse.ConnectSrcObject(pTextureDiffuse);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerAmbient = NULL;
							KFbxFileTexture* pTextureAmbient = ExportTexture(ImportedHelpers::FindTexture(mat->Textures[1], imported), pTextureLayerAmbient, pMesh);
							if (pTextureAmbient != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eAMBIENT_TEXTURES, pTextureLayerAmbient);
								pMat->Ambient.ConnectSrcObject(pTextureAmbient);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerEmissive = NULL;
							KFbxFileTexture* pTextureEmissive = ExportTexture(ImportedHelpers::FindTexture(mat->Textures[2], imported), pTextureLayerEmissive, pMesh);
							if (pTextureEmissive != NULL)
							{
								pLayer->SetTextures(KFbxLayerElement::eEMISSIVE_TEXTURES, pTextureLayerEmissive);
								pMat->Emissive.ConnectSrcObject(pTextureEmissive);
								hasTexture = true;
							}

							KFbxLayerElementTexture* pTextureLayerSpecular = NULL;
							KFbxFileTexture* pTextureSpecular = ExportTexture(ImportedHelpers::FindTexture(mat->Textures[3], imported), pTextureLayerSpecular, pMesh);
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
						ImportedVertex^ vertex = vertexList[j];
						Vector3 coords = vertex->Position;
						pControlPoints[j] = KFbxVector4(coords.X, coords.Y, coords.Z);
						Vector3 normal = vertex->Normal;
						pLayerElementNormal->GetDirectArray().Add(KFbxVector4(normal.X, normal.Y, normal.Z));
						array<float>^ uv = vertex->UV;
						pUVLayer->GetDirectArray().Add(KFbxVector2(uv[0], -uv[1]));

						if (hasBones)
						{
							array<unsigned char>^ boneIndices = vertex->BoneIndices;
							array<float>^ weights4 = vertex->Weights;
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
						ImportedFace^ face = faceList[j];
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

	KFbxFileTexture* Fbx::Exporter::ExportTexture(ImportedTexture^ matTex, KFbxLayerElementTexture*& pTextureLayer, KFbxMesh* pMesh)
	{
		KFbxFileTexture* pTex = NULL;

		if (matTex != nullptr)
		{
			String^ matTexName = matTex->Name;

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

					String^ path = Path::GetDirectoryName(gcnew String(pExporter->GetFileName().Buffer())) + Path::DirectorySeparatorChar + Path::GetFileName(matTex->Name);
					FileInfo^ file = gcnew FileInfo(path);
					DirectoryInfo^ dir = file->Directory;
					if (!dir->Exists)
					{
						dir->Create();
					}
					{
						BinaryWriter^ writer = gcnew BinaryWriter(file->Create());
						writer->Write(matTex->Data);
						writer->Close();
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

	void Fbx::Exporter::ExportAnimations(int startKeyframe, int endKeyframe, bool linear, bool EulerFilter, float filterPrecision)
	{
		List<ImportedAnimation^>^ importedAnimationList = imported->AnimationList;
		if (importedAnimationList == nullptr)
		{
			return;
		}

		List<String^>^ pNotFound = gcnew List<String^>();

		KFbxTypedProperty<fbxDouble3> scale = KFbxProperty::Create(pScene, DTDouble3, InterpolationHelper::pScaleName);
		KFbxTypedProperty<fbxDouble3> rotate = KFbxProperty::Create(pScene, DTDouble3, InterpolationHelper::pRotateName);
		KFbxTypedProperty<fbxDouble3> translate = KFbxProperty::Create(pScene, DTDouble3, InterpolationHelper::pTranslateName);

		KFbxAnimCurveFilterUnroll* lFilter;
		if (EulerFilter)
		{
			lFilter = new KFbxAnimCurveFilterUnroll();
		}

		for (int i = 0; i < importedAnimationList->Count; i++)
		{
			ImportedAnimation^ parser = importedAnimationList[i];
			List<ImportedAnimationTrack^>^ pAnimationList = parser->TrackList;

			KString kTakeName = KString("Take") + KString(i);
			char* lTakeName = kTakeName.Buffer();

			KTime lTime;
			KFbxAnimStack* lAnimStack = KFbxAnimStack::Create(pScene, lTakeName);
			KFbxAnimLayer* lAnimLayer = KFbxAnimLayer::Create(pScene, "Base Layer");
			lAnimStack->AddMember(lAnimLayer);
			InterpolationHelper^ interpolationHelper;
			int resampleCount = 0;
			if (startKeyframe >= 0)
			{
				interpolationHelper = gcnew InterpolationHelper(pScene, lAnimLayer, linear ? KFbxAnimCurveDef::eINTERPOLATION_LINEAR : KFbxAnimCurveDef::eINTERPOLATION_CUBIC, &scale, &rotate, &translate);
				for each (ImportedAnimationTrack^ track in pAnimationList)
				{
					if (track->Keyframes->Length > resampleCount)
					{
						resampleCount = track->Keyframes->Length;
					}
				}
			}

			for (int j = 0; j < pAnimationList->Count; j++)
			{
				ImportedAnimationTrack^ keyframeList = pAnimationList[j];
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

					array<ImportedAnimationKeyframe^>^ keyframes = keyframeList->Keyframes;

					double fps = 1.0 / 24;
					int startAt, endAt;
					if (startKeyframe >= 0)
					{
						bool resample = false;
						if (keyframes->Length < resampleCount)
						{
							resample = true;
						}
						else
						{
							for (int k = 0; k < resampleCount; k++)
							{
								if (keyframes[k] == nullptr)
								{
									resample = true;
									break;
								}
							}
						}
						if (resample)
						{
							keyframes = interpolationHelper->InterpolateTrack(keyframes, resampleCount);
						}

						startAt = startKeyframe;
						endAt = endKeyframe < resampleCount ? endKeyframe : resampleCount - 1;
					}
					else
					{
						startAt = 0;
						endAt = keyframes->Length - 1;
					}

					for (int k = startAt, keySetIndex = 0; k <= endAt; k++)
					{
						if (keyframes[k] == nullptr)
							continue;

						lTime.SetSecondDouble(fps * (k - startAt));

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
						lCurveSX->KeySet(keySetIndex, lTime, keyframes[k]->Scaling.X);
						lCurveSY->KeySet(keySetIndex, lTime, keyframes[k]->Scaling.Y);
						lCurveSZ->KeySet(keySetIndex, lTime, keyframes[k]->Scaling.Z);
						lCurveRX->KeySet(keySetIndex, lTime, rotation.X);
						lCurveRY->KeySet(keySetIndex, lTime, rotation.Y);
						lCurveRZ->KeySet(keySetIndex, lTime, rotation.Z);
						lCurveTX->KeySet(keySetIndex, lTime, keyframes[k]->Translation.X);
						lCurveTY->KeySet(keySetIndex, lTime, keyframes[k]->Translation.Y);
						lCurveTZ->KeySet(keySetIndex, lTime, keyframes[k]->Translation.Z);
						keySetIndex++;
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

					if (EulerFilter)
					{
						KFbxAnimCurve* lCurve [3];
						lCurve[0] = lCurveRX;
						lCurve[1] = lCurveRY;
						lCurve[2] = lCurveRZ;
						lFilter->Reset();
						lFilter->SetTestForPath(true);
						lFilter->SetQualityTolerance(filterPrecision);
						lFilter->Apply((KFbxAnimCurve**)lCurve, 3);
					}
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
}
