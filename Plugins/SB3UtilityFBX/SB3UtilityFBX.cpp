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

#define ADD_KEY_VECTOR3(curveX, curveY, curveZ, time, vec, interpolationMethod) \
	{ \
		int keyIndex = curveX->KeyAdd(time); \
		curveX->KeySet(keyIndex, time, vec.X, interpolationMethod); \
		keyIndex = curveY->KeyAdd(time); \
		curveY->KeySet(keyIndex, time, vec.Y, interpolationMethod); \
		keyIndex = curveZ->KeyAdd(time); \
		curveZ->KeySet(keyIndex, time, vec.Z, interpolationMethod); \
	}

#define ASSIGN_CHANNEL_VALUES(prop, time, vec) \
	{ \
		KFbxAnimCurveNode& val = pAnimEvaluator->GetPropertyValue(prop, time); \
		vec.X = (float)val.GetChannelValue<double>(0U, 0.0); \
		vec.Y = (float)val.GetChannelValue<double>(1U, 0.0); \
		vec.Z = (float)val.GetChannelValue<double>(2U, 0.0); \
	}

	void Fbx::InterpolateKeyframes(List<Tuple<ImportedAnimationTrack^, array<xaAnimationKeyframe^>^>^>^ extendedTrackList, int resampleCount)
	{
		KFbxSdkManager* pSdkManager = NULL;
		KFbxScene* pScene = NULL;
		pin_ptr<KFbxSdkManager*> pSdkManagerPin = &pSdkManager;
		pin_ptr<KFbxScene*> pScenePin = &pScene;
		Init(pSdkManagerPin, pScenePin);

		KFbxAnimStack* pAnimStack = KFbxAnimStack::Create(pScene, NULL);
		KFbxAnimLayer* pAnimLayer = KFbxAnimLayer::Create(pScene, NULL);
		pAnimStack->AddMember(pAnimLayer);

		KFbxAnimEvaluator* pAnimEvaluator = pScene->GetEvaluator();

		const KFbxAnimCurveDef::EInterpolationType interpolationMethod = KFbxAnimCurveDef::eINTERPOLATION_LINEAR; // eINTERPOLATION_CUBIC ?

		// S
		const char* pScaleName = "Scale";
		KFbxTypedProperty<fbxDouble3> scale = KFbxProperty::Create(pScene, DTDouble3, pScaleName);
		scale.ModifyFlag(KFbxProperty::eANIMATABLE, true);
		KFbxAnimCurveNode* pScaleCurveNode = scale.GetCurveNode(pAnimLayer, true);
		pAnimLayer->AddMember(pScaleCurveNode);
		scale.ConnectSrcObject(pScaleCurveNode);
		KFbxAnimCurve* pScaleCurveX = pScaleCurveNode->CreateCurve(pScaleName, 0U);
		KFbxAnimCurve* pScaleCurveY = pScaleCurveNode->CreateCurve(pScaleName, 1U);
		KFbxAnimCurve* pScaleCurveZ = pScaleCurveNode->CreateCurve(pScaleName, 2U);

		// R
		const char* pRotateName = "Rotate";
		KFbxTypedProperty<fbxDouble3> rotate = KFbxProperty::Create(pScene, DTDouble3, pRotateName);
		rotate.ModifyFlag(KFbxProperty::eANIMATABLE, true);
		KFbxAnimCurveNode* pRotateCurveNode = rotate.GetCurveNode(pAnimLayer, true);
		pAnimLayer->AddMember(pRotateCurveNode);
		rotate.ConnectSrcObject(pRotateCurveNode);
		KFbxAnimCurve* pRotateCurveX = pRotateCurveNode->CreateCurve(pRotateName, 0U);
		KFbxAnimCurve* pRotateCurveY = pRotateCurveNode->CreateCurve(pRotateName, 1U);
		KFbxAnimCurve* pRotateCurveZ = pRotateCurveNode->CreateCurve(pRotateName, 2U);

		// T
		const char* pTranslateName = "Translate";
		KFbxTypedProperty<fbxDouble3> translate = KFbxProperty::Create(pScene, DTDouble3, pTranslateName);
		translate.ModifyFlag(KFbxProperty::eANIMATABLE, true);
		KFbxAnimCurveNode* pTranslateCurveNode = translate.GetCurveNode(pAnimLayer, true);
		pAnimLayer->AddMember(pTranslateCurveNode);
		translate.ConnectSrcObject(pTranslateCurveNode);
		KFbxAnimCurve* pTranslateCurveX = pTranslateCurveNode->CreateCurve(pTranslateName, 0U);
		KFbxAnimCurve* pTranslateCurveY = pTranslateCurveNode->CreateCurve(pTranslateName, 1U);
		KFbxAnimCurve* pTranslateCurveZ = pTranslateCurveNode->CreateCurve(pTranslateName, 2U);

		KFbxAnimCurve* AllCurves[9] = { pScaleCurveX, pScaleCurveY, pScaleCurveZ, pRotateCurveX, pRotateCurveY, pRotateCurveZ, pTranslateCurveX, pTranslateCurveY, pTranslateCurveZ };

		KTime time;
		float animationLen = (float)(resampleCount - 1);
		for each (Tuple<ImportedAnimationTrack^, array<xaAnimationKeyframe^>^>^ tuple in extendedTrackList)
		{
			ImportedAnimationTrack^ wsTrack = tuple->Item1;
			for (int i = 0; i < wsTrack->Keyframes->Length; i++)
			{
				ImportedAnimationKeyframe^ wsKeyframe = wsTrack->Keyframes[i];
				float timePos = i * animationLen / (wsTrack->Keyframes->Length - 1);
				time.SetSecondDouble(timePos);

				Vector3 s = wsKeyframe->Scaling;
				ADD_KEY_VECTOR3(pScaleCurveX, pScaleCurveY, pScaleCurveZ, time, s, interpolationMethod);
				Vector3 r = QuaternionToEuler(wsKeyframe->Rotation);
				ADD_KEY_VECTOR3(pRotateCurveX, pRotateCurveY, pRotateCurveZ, time, r, interpolationMethod);
				Vector3 t = wsKeyframe->Translation;
				ADD_KEY_VECTOR3(pTranslateCurveX, pTranslateCurveY, pTranslateCurveZ, time, t, interpolationMethod);
			}

			array<xaAnimationKeyframe^>^ newKeyframes = tuple->Item2;
			for (int i = 0; i < newKeyframes->Length; i++)
			{
				newKeyframes[i] = gcnew xaAnimationKeyframe();
				newKeyframes[i]->Index = i;
				xa::CreateUnknowns(newKeyframes[i]);

				time.SetSecondDouble(i);
				Vector3 s, r, t;
				ASSIGN_CHANNEL_VALUES(scale, time, s);
				newKeyframes[i]->Scaling = s;
				ASSIGN_CHANNEL_VALUES(rotate, time, r);
				newKeyframes[i]->Rotation = EulerToQuaternion(r);
				ASSIGN_CHANNEL_VALUES(translate, time, t);
				newKeyframes[i]->Translation = t;
			}

			for each (KFbxAnimCurve* pCurve in AllCurves)
			{
				pCurve->KeyClear();
			}
		}

		if (pScene != NULL)
		{
			pScene->Destroy();
		}
		if (pSdkManager != NULL)
		{
			pSdkManager->Destroy();
		}
	}
}
