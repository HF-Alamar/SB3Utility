// SB3UtilityFBX.h

#pragma once

#ifdef IOS_REF
	#undef  IOS_REF
	#define IOS_REF (*(pSdkManager->GetIOSettings()))
#endif

using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace SlimDX;

namespace SB3Utility {

	public ref class Fbx
	{
	public:
		static Vector3 QuaternionToEuler(Quaternion q);
		static Quaternion EulerToQuaternion(Vector3 v);
		static void InterpolateKeyframes(List<Tuple<ImportedAnimationTrack^, array<xaAnimationKeyframe^>^>^>^ extendedTrackList, int resampleCount);

		ref class Importer : IImported
		{
		public:
			virtual property List<ImportedFrame^>^ FrameList;
			virtual property List<ImportedMesh^>^ MeshList;
			virtual property List<ImportedMaterial^>^ MaterialList;
			virtual property List<ImportedTexture^>^ TextureList;
			virtual property List<ImportedAnimation^>^ AnimationList;
			virtual property List<ImportedMorph^>^ MorphList;

			Importer(String^ path);

		private:
			KArrayTemplate<KFbxSurfacePhong*>* pMaterials;
			KArrayTemplate<KFbxTexture*>* pTextures;
			int unnamedMeshCount;

			char* cPath;
			KFbxSdkManager* pSdkManager;
			KFbxScene* pScene;
			KFbxImporter* pImporter;

			void ImportNode(ImportedFrame^ parent, KFbxNode* pNode);
			ImportedFrame^ ImportFrame(ImportedFrame^ parent, KFbxNode* pNode);
			void ImportMesh(ImportedFrame^ parent, KArrayTemplate<KFbxNode*>* pMeshArray);
			ImportedMaterial^ ImportMaterial(KFbxMesh* pMesh);
			String^ ImportTexture(KFbxFileTexture* pTexture);
			void ImportAnimation();
			void ImportAnimation(KFbxAnimLayer* pAnimLayer, KFbxNode* pNode, ImportedAnimation^ wsAnimation);
			template <class T> void GetVector(KFbxLayerElementTemplate<T>* pLayerElement, T& pVector, int controlPointIdx, int vertexIdx);
			void ImportMorph(KArrayTemplate<KFbxNode*>* pMeshArray);

			ref class Vertex
			{
			public:
				int index;
				array<float>^ position;
				array<float>^ normal;
				array<float>^ uv;
				List<Byte>^ boneIndices;
				List<float>^ weights;

				bool Equals(Vertex^ vertex);

				Vertex();
			};
		};

		ref class Exporter
		{
		public:
			static void Export(String^ path, xxParser^ xxParser, List<xxFrame^>^ meshParents, List<xaParser^>^ xaSubfileList, String^ exportFormat, bool allFrames, bool skins);
			static void ExportMorph(String^ path, xxParser^ xxParser, xxFrame^ meshFrame, xaMorphClip^ morphClip, xaParser^ xaparser, String^ exportFormat);

		private:
			HashSet<String^>^ frameNames;
			HashSet<String^>^ meshNames;
			List<xxFrame^>^ meshFrames;
			bool exportSkins;
			xxParser^ xxparser;

			char* cDest;
			char* cFormat;
			KFbxSdkManager* pSdkManager;
			KFbxScene* pScene;
			KFbxExporter* pExporter;
			KArrayTemplate<KFbxSurfacePhong*>* pMaterials;
			KArrayTemplate<KFbxFileTexture*>* pTextures;
			KArrayTemplate<KFbxNode*>* pMeshNodes;

			Exporter(String^ path, xxParser^ xxparser, List<xxFrame^>^ meshParents, String^ exportFormat, bool allFrames, bool skins);
			~Exporter();
			!Exporter();
			void ExportFrame(KFbxNode* pParentNode, xxFrame^ frame);
			void ExportMesh(KFbxNode* pFrameNode, xxFrame^ frame);
			KFbxFileTexture* ExportTexture(xxMaterialTexture^ matTex, KFbxLayerElementTexture*& pLayerTexture, KFbxMesh* pMesh);
			void ExportAnimations(List<xaParser^>^ xaSubfileList);
			void SetJoints();
			void SetJointsNode(KFbxNode* pNode, HashSet<String^>^ boneNames);
			void ExportMorphs(xxFrame^ baseFrame, xaMorphClip^ morphClip, xaParser^ xaparser);
		};

	private:
		static char* StringToCharArray(String^ s);
		static void Init(KFbxSdkManager** pSdkManager, KFbxScene** pScene);
	};
}
