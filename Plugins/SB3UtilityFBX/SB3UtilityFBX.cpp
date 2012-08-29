#include <fbxsdk.h>
#include <fbxfilesdk/kfbxio/kfbxiosettings.h>
#include "SB3UtilityFBX.h"

namespace SB3Utility
{
	char* Fbx::StringToCharArray(String^ s)
	{
		return (char*)(void*)Marshal::StringToHGlobalAnsi(s);
	}

	void Fbx::Init(KFbxSdkManager** pSdkManager, KFbxScene** pScene)
	{
		*pSdkManager = KFbxSdkManager::Create();
		if (!pSdkManager)
		{
			throw gcnew Exception(gcnew String("Unable to create the FBX SDK manager"));
		}

		KFbxIOSettings* ios = KFbxIOSettings::Create(*pSdkManager, IOSROOT);
		(*pSdkManager)->SetIOSettings(ios);

		KString lPath = KFbxGetApplicationDirectory();
#if defined(KARCH_ENV_WIN)
		KString lExtension = "dll";
#elif defined(KARCH_ENV_MACOSX)
		KString lExtension = "dylib";
#elif defined(KARCH_ENV_LINUX)
		KString lExtension = "so";
#endif
		(*pSdkManager)->LoadPluginsDirectory(lPath.Buffer(), lExtension.Buffer());

		*pScene = KFbxScene::Create(*pSdkManager, "");
	}

	Vector3 Fbx::QuaternionToEuler(Quaternion q)
	{
		KFbxXMatrix lMatrixRot;
		lMatrixRot.SetQ(KFbxQuaternion(q.X, q.Y, q.Z, q.W));
		KFbxVector4 lEuler = lMatrixRot.GetR();
		return Vector3((float)lEuler[0], (float)lEuler[1], (float)lEuler[2]);
	}

	Quaternion Fbx::EulerToQuaternion(Vector3 v)
	{
		KFbxXMatrix lMatrixRot;
		lMatrixRot.SetR(KFbxVector4(v.X, v.Y, v.Z));
		KFbxQuaternion lQuaternion = lMatrixRot.GetQ();
		return Quaternion((float)lQuaternion[0], (float)lQuaternion[1], (float)lQuaternion[2], (float)lQuaternion[3]);
	}
}
