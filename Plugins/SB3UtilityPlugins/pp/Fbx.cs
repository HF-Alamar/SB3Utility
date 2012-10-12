using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX;

namespace SB3Utility
{
	public static partial class Plugins
	{
		[Plugin]
		[PluginOpensFile(".fbx")]
		public static void WorkspaceFbx(string path, string variable)
		{
			string importVar = Gui.Scripting.GetNextVariable("importFbx");
			var importer = (Fbx.Importer)Gui.Scripting.RunScript(importVar + " = ImportFbx(\"" + path + "\")");

			string editorVar = Gui.Scripting.GetNextVariable("importedEditor");
			var editor = (ImportedEditor)Gui.Scripting.RunScript(editorVar + " = ImportedEditor(" + importVar + ")");

			new FormWorkspace(path, importer, editorVar, editor);
		}

		[Plugin]
		public static void ExportFbx([DefaultVar]xxParser xxParser, object[] meshNames, object[] xaParsers, int startKeyframe, int endKeyframe, string path, string exportFormat, bool allFrames, bool skins)
		{
			List<xaParser> xaParserList = null;
			if (xaParsers != null)
			{
				xaParserList = new List<xaParser>(Utility.Convert<xaParser>(xaParsers));
			}

			List<xxFrame> meshFrames = xx.FindMeshFrames(xxParser.Frame, new List<string>(Utility.Convert<string>(meshNames)));
			Fbx.Exporter.Export(path, xxParser, meshFrames, xaParserList, startKeyframe, endKeyframe, exportFormat, allFrames, skins);
		}

		[Plugin]
		public static void ExportMorphFbx([DefaultVar]xxParser xxparser, string path, xxFrame meshFrame, xaParser xaparser, xaMorphClip morphClip, string exportFormat)
		{
			Fbx.Exporter.ExportMorph(path, xxparser, meshFrame, morphClip, xaparser, exportFormat);
		}

		[Plugin]
		public static Fbx.Importer ImportFbx([DefaultVar]string path)
		{
			return new Fbx.Importer(path);
		}
	}

	public static class FbxUtility
	{
		public static Vector3 QuaternionToEuler(Quaternion q)
		{
			return Fbx.QuaternionToEuler(q);
		}

		public static Quaternion EulerToQuaternion(Vector3 v)
		{
			return Fbx.EulerToQuaternion(v);
		}

		public static Matrix SRTToMatrix(Vector3 scale, Vector3 euler, Vector3 translate)
		{
			return Matrix.Scaling(scale) * Matrix.RotationQuaternion(EulerToQuaternion(euler)) * Matrix.Translation(translate);
		}

		public static Vector3[] MatrixToSRT(Matrix m)
		{
			Quaternion q;
			Vector3[] srt = new Vector3[3];
			m.Decompose(out srt[0], out q, out srt[2]);
			srt[1] = QuaternionToEuler(q);
			return srt;
		}
	}
}
