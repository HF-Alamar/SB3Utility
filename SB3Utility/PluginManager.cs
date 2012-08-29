using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace SB3Utility
{
	public class FunctionArg
	{
		public object Value;
		public string Variable;
		public bool DefaultVar = false;

		public FunctionArg(object value, string variable)
		{
			this.Value = value;
			this.Variable = variable;
		}
	}

	public abstract class FunctionBase
	{
		public ParameterInfo[] Parameters;
		public Dictionary<string, ParameterInfo> ParameterDic = new Dictionary<string, ParameterInfo>();
		public Type Type;
		public int DefaultVarPos = -1;
		public XmlComments Comments = null;
		public MethodBase Method;

		public string Return { get; protected set; }
		public string Name { get; protected set; }
		public string ParameterString { get; protected set; }

		public abstract object RunPlugin(object instance, FunctionArg[] args, out List<FunctionArg> changedVars);

		public FunctionBase(MethodBase method, XmlComments comments)
		{
			Comments = comments;

			Method = method;
			Type = method.DeclaringType;
			Parameters = method.GetParameters();

			string parameterString = String.Empty;
			if (Parameters.Length > 0)
			{
				for (int i = 0; i < Parameters.Length; i++)
				{
					ParameterInfo parameter = Parameters[i];
					ParameterDic.Add(parameter.Name.ToLowerInvariant(), parameter);
					if ((DefaultVarPos < 0) && parameter.GetCustomAttributes(typeof(DefaultVar), false).Length > 0)
					{
						DefaultVarPos = i;
						parameterString += "[DefaultVar] ";
					}
					parameterString += parameter.ParameterType.GenericName() + " " + parameter.Name + ", ";
				}
				parameterString = parameterString.Substring(0, parameterString.Length - 2);
			}
			ParameterString = parameterString;
		}

		protected object[] GetObjArgs(FunctionArg[] pluginArgs)
		{
			object[] objArgs = new object[pluginArgs.Length];
			for (int i = 0; i < pluginArgs.Length; i++)
			{
				objArgs[i] = pluginArgs[i].Value;
			}
			return objArgs;
		}

		protected List<FunctionArg> GetChangedVars(FunctionArg[] pluginArgs, object[] objArgs)
		{
			List<FunctionArg> changedVars = new List<FunctionArg>(pluginArgs.Length);
			for (int i = 0; i < pluginArgs.Length; i++)
			{
				if (pluginArgs[i].DefaultVar || (pluginArgs[i].Variable != null))
				{
					pluginArgs[i].Value = objArgs[i];
					changedVars.Add(pluginArgs[i]);
				}
			}
			return changedVars;
		}

		private bool MatchParameter(FunctionArg[] pluginArgs, ScriptArg scriptArg, Type parameterType, int parameterPos, ref int numConversions)
		{
			FunctionArg pluginArg = new FunctionArg(scriptArg.Value, scriptArg.Variable);
			if (parameterType.IsByRef)
			{
				parameterType = parameterType.GetElementType();
			}
			else
			{
				pluginArg.Variable = null;
			}

			if (pluginArg.Value == null)
			{
				if (parameterType.IsValueType)
				{
					if (Nullable.GetUnderlyingType(parameterType) == null)
					{
						//not nullable
						return false;
					}
					else
					{
						//nullable
						pluginArgs[parameterPos] = pluginArg;
					}
				}
				else
				{
					//nullable
					pluginArgs[parameterPos] = pluginArg;
				}
			}
			else
			{
				if (parameterType.IsAssignableFrom(pluginArg.Value.GetType()))
				{
					pluginArgs[parameterPos] = pluginArg;
				}
				else
				{
					if (!(pluginArg.Value is IConvertible))
					{
						return false;
					}

					try
					{
						pluginArg.Value = Convert.ChangeType(pluginArg.Value, parameterType);
						pluginArgs[parameterPos] = pluginArg;
						numConversions++;
					}
					catch
					{
						return false;
					}
				}
			}
			return true;
		}

		public FunctionArg[] Match(ScriptArg[] args, bool useDefaultVar, out int numConversions)
		{
			FunctionArg[] result = null;
			numConversions = 0;

			if (args.Length == Parameters.Length)
			{
				FunctionArg[] objArgs = new FunctionArg[args.Length];

				List<int> unfilledPos = new List<int>(args.Length);
				for (int i = 0; i < args.Length; i++)
				{
					unfilledPos.Add(i);
				}

				List<ScriptArg> nondefault = new List<ScriptArg>(args);
				if (useDefaultVar && (DefaultVarPos >= 0))
				{
					Type paramType = Parameters[DefaultVarPos].ParameterType;
					if (MatchParameter(objArgs, args[0], paramType, DefaultVarPos, ref numConversions))
					{
						objArgs[DefaultVarPos].DefaultVar = paramType.IsByRef;
						unfilledPos.Remove(DefaultVarPos);
						nondefault.RemoveAt(0);
					}
					else
					{
						return null;
					}
				}

				List<ScriptArg> unnamed = new List<ScriptArg>(nondefault.Count);
				for (int i = 0; i < nondefault.Count; i++)
				{
					ScriptArg arg = nondefault[i];
					if (arg.Name == null)
					{
						unnamed.Add(arg);
					}
					else
					{
						ParameterInfo info;
						if (ParameterDic.TryGetValue(arg.Name.ToLowerInvariant(), out info))
						{
							if (MatchParameter(objArgs, arg, info.ParameterType, info.Position, ref numConversions))
							{
								unfilledPos.Remove(info.Position);
							}
							else
							{
								return null;
							}
						}
						else
						{
							return null;
						}
					}
				}

				for (int i = 0; i < unnamed.Count; i++)
				{
					int pos = unfilledPos[i];
					if (!MatchParameter(objArgs, unnamed[i], Parameters[pos].ParameterType, pos, ref numConversions))
					{
						return null;
					}
				}

				result = objArgs;
			}

			return result;
		}
	}

	public class FunctionMethod : FunctionBase
	{
		public FunctionMethod(MethodInfo methodInfo, XmlComments comments)
			: base(methodInfo, comments)
		{
			Return = (methodInfo.ReturnType == typeof(void)) ? String.Empty : methodInfo.ReturnType.GenericName();
			Name = methodInfo.Name;
		}

		public override object RunPlugin(object instance, FunctionArg[] pluginArgs, out List<FunctionArg> changedVars)
		{
			object[] objArgs = GetObjArgs(pluginArgs);
			object result = ((MethodInfo)Method).Invoke(instance, objArgs);
			changedVars = GetChangedVars(pluginArgs, objArgs);
			return result;
		}
	}

	public class FunctionClass : FunctionBase
	{
		public FunctionClass(ConstructorInfo constructorInfo, XmlComments comments)
			: base(constructorInfo, comments)
		{
			Return = Type.Name;
			Name = Type.Name;
		}

		public override object RunPlugin(object instance, FunctionArg[] pluginArgs, out List<FunctionArg> changedVars)
		{
			object[] objArgs = GetObjArgs(pluginArgs);
			object result = Activator.CreateInstance(Type, objArgs);
			changedVars = GetChangedVars(pluginArgs, objArgs);
			return result;
		}
	}

	public static class PluginManager
	{
		public static Dictionary<string, Tuple<string, List<FunctionBase>>> Plugins = new Dictionary<string, Tuple<string, List<FunctionBase>>>();
		public static Dictionary<string, List<FunctionBase>> Functions = new Dictionary<string, List<FunctionBase>>();
		public static Dictionary<string, List<string>> OpensFile = new Dictionary<string, List<string>>();
		public static List<string[]> Tools = new List<string[]>();
		public static SortedSet<string> DoNotLoad = new SortedSet<string>();

		public static void LoadPlugin(string path)
		{
			try
			{
				FileInfo file = new FileInfo(path);
				if (!file.Exists)
				{
					Report.ReportLog("File doesn't exist: " + path);
					return;
				}

				if (DoNotLoad.Contains(file.Name.ToLowerInvariant()))
				{
					return;
				}

				var assembly = Assembly.LoadFrom(file.FullName);
				RegisterFunctions(assembly);
			}
			catch (Exception ex)
			{
				Report.ReportLog("Failed to load plugin " + path);
				Utility.ReportException(ex);
			}
		}

		public static void RegisterFunctions(Assembly assembly)
		{
			string assemblyName = Path.GetFileName(assembly.Location);
			string assemblyNameLower = assemblyName.ToLowerInvariant();
			if (Plugins.ContainsKey(assemblyNameLower))
			{
				return;
			}

			Jolt.XmlDocCommentReader xmlReader = null;
			string xmlPath = Path.GetDirectoryName(assembly.Location) + @"\" + Path.GetFileNameWithoutExtension(assembly.Location) + ".xml";
			if (File.Exists(xmlPath))
			{
				xmlReader = new Jolt.XmlDocCommentReader(xmlPath);
			}

			List<FunctionBase> assemblyFunctions = new List<FunctionBase>();
			Plugins.Add(assemblyNameLower, new Tuple<string, List<FunctionBase>>(assemblyName, assemblyFunctions));
			foreach (var type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(Plugin), false).Length > 0)
				{
					string name = type.Name.ToLowerInvariant();
					List<FunctionBase> pluginClasses;
					if (!Functions.TryGetValue(name, out pluginClasses))
					{
						pluginClasses = new List<FunctionBase>();
						Functions.Add(name, pluginClasses);
					}
					foreach (var constructor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
					{
						FunctionClass function;
						XElement element = null;
						if ((xmlReader != null) && ((element = xmlReader.GetComments(constructor)) != null))
						{
							function = new FunctionClass(constructor, new XmlComments(element));
						}
						else
						{
							function = new FunctionClass(constructor, null);
						}
						pluginClasses.Add(function);
						assemblyFunctions.Add(function);
					}

					var opensFileAttribs = type.GetCustomAttributes(typeof(PluginOpensFile), false);
					for (int i = 0; i < opensFileAttribs.Length; i++)
					{
						var attrib = (PluginOpensFile)opensFileAttribs[i];
						string ext = attrib.FileExtension.ToLowerInvariant();

						List<string> opensFileList;
						if (!OpensFile.TryGetValue(ext, out opensFileList))
						{
							opensFileList = new List<string>();
							OpensFile.Add(ext, opensFileList);
						}
						opensFileList.Add(type.Name);
					}

					var toolAttribs = type.GetCustomAttributes(typeof(PluginTool), false);
					if (toolAttribs.Length > 0)
					{
						var attrib = (PluginTool)toolAttribs[0];
						Tools.Add(new string[] { type.Name, attrib.Name });
					}
				}

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
				{
					if (method.GetCustomAttributes(typeof(Plugin), false).Length > 0)
					{
						string name = method.Name.ToLowerInvariant();
						List<FunctionBase> pluginMethods;
						if (!Functions.TryGetValue(name, out pluginMethods))
						{
							pluginMethods = new List<FunctionBase>();
							Functions.Add(name, pluginMethods);
						}

						FunctionMethod function;
						XElement element = null;
						if ((xmlReader != null) && ((element = xmlReader.GetComments(method)) != null))
						{
							function = new FunctionMethod(method, new XmlComments(element));
						}
						else
						{
							function = new FunctionMethod(method, null);
						}
						pluginMethods.Add(function);
						assemblyFunctions.Add(function);

						var opensFileAttribs = method.GetCustomAttributes(typeof(PluginOpensFile), false);
						for (int i = 0; i < opensFileAttribs.Length; i++)
						{
							var attrib = (PluginOpensFile)opensFileAttribs[i];
							string ext = attrib.FileExtension.ToLowerInvariant();

							List<string> opensFileList;
							if (!OpensFile.TryGetValue(ext, out opensFileList))
							{
								opensFileList = new List<string>();
								OpensFile.Add(ext, opensFileList);
							}
							opensFileList.Add(method.Name);
						}

						var toolAttribs = method.GetCustomAttributes(typeof(PluginTool), false);
						if (toolAttribs.Length > 0)
						{
							var attrib = (PluginTool)toolAttribs[0];
							Tools.Add(new string[] { method.Name, attrib.Name });
						}
					}
				}
			}
		}
	}
}
