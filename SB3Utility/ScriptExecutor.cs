using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace SB3Utility
{
	public class ScriptFunction
	{
		public string Name;
		public ScriptArg[] Args;
		public object Return;
	}

	public class ScriptArg
	{
		public string Name;
		public object Value;
		public string Variable;
	}

	public class ScriptExecutor
	{
		public static string LoadPluginName = "loadplugin";
		public static string ImportName = "import";
		public static string PluginDirectoryName = "plugindirectory";

		public Dictionary<string, object> Variables { get; protected set; }

		object defaultVar = null;

		public ScriptExecutor()
		{
			string exeDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Variables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
			Variables.Add(PluginDirectoryName, exeDirPath + Path.DirectorySeparatorChar + "plugins" + Path.DirectorySeparatorChar);
		}

		public object RunScript(ScriptParser parser)
		{
			object result = null;
			string prevCWD = Environment.CurrentDirectory;
			Environment.CurrentDirectory = parser.CWD;
			result = ExecuteExpr(parser.CommandRoot);
			Environment.CurrentDirectory = prevCWD;
			return result;
		}

		public object ExecuteExpr(Expr expr)
		{
			object result = null;
			if (expr is Literal)
			{
				Literal literal = (Literal)expr;
				if (literal.Type == ExprType.String)
				{
					result = literal.Value;
				}
				else if (literal.Type == ExprType.Bytes)
				{
					result = Utility.StringToBytes(literal.Value);
				}
				else if (literal.Type == ExprType.Name)
				{
					result = Variables[literal.Value.ToLowerInvariant()];
				}
				else if (literal.Type == ExprType.Bool)
				{
					result = Boolean.Parse(literal.Value);
				}
				else if (literal.Type == ExprType.Number)
				{
					result = Double.Parse(literal.Value);
				}
				else if (literal.Type == ExprType.Null)
				{
				}
				else
				{
					throw new Exception("Unexpected literal: " + literal.Type);
				}
			}
			else if (expr is Command)
			{
				Command cmd = (Command)expr;
				if (cmd.Type == ExprType.Root)
				{
					foreach (var arg in cmd.Args)
					{
						if (arg.Type == ExprType.Assign)
						{
							result = ExecuteExpr(arg);
						}
						else
						{
							result = ExecuteExpr(arg);
							defaultVar = result;
						}
					}
				}
				else if ((cmd.Type == ExprType.DotInstanceDefault) || (cmd.Type == ExprType.DotInstanceDefaultChain))
				{
					result = ExecuteFunction((Command)cmd.Args[1], ExecuteExpr(cmd.Args[0]), true);
				}
				else if ((cmd.Type == ExprType.DotInstance) || (cmd.Type == ExprType.DotInstanceChain))
				{
					result = ExecuteFunction((Command)cmd.Args[1], ExecuteExpr(cmd.Args[0]), false);
				}
				else if (cmd.Type == ExprType.DotDefault)
				{
					result = ExecuteFunction((Command)cmd.Args[0], null, true);
				}
				else if (cmd.Type == ExprType.Function)
				{
					result = ExecuteFunction(cmd, null, false);
				}
				else if ((cmd.Type == ExprType.DotProperty) || (cmd.Type == ExprType.DotPropertyChain))
				{
					object obj = ExecuteExpr(cmd.Args[0]);
					string propertyName = ((Literal)cmd.Args[1]).Value;
					result = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
				}
				else if (cmd.Type == ExprType.Indexed)
				{
					object obj = ExecuteExpr(cmd.Args[0]);
					int index = Int32.Parse(((Literal)cmd.Args[1]).Value);

					var type = obj.GetType();
					if (type.IsArray)
					{
						result = ((Array)obj).GetValue(index);
					}
					else
					{
						var attributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
						if (attributes.Length > 0)
						{
							var indexerName = ((DefaultMemberAttribute)attributes[0]).MemberName;
							result = type.GetProperty(indexerName).GetValue(obj, new object[] { index });
						}
						else
						{
							throw new Exception(obj.ToString() + " can't be indexed.");
						}
					}
				}
				else if (cmd.Type == ExprType.Array)
				{
					object[] array = new object[cmd.Args.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = ExecuteExpr(cmd.Args[i]);
					}
					result = array;
				}
				else if (cmd.Type == ExprType.Assign)
				{
					string name = ((Literal)cmd.Args[0]).Value.ToLowerInvariant();
					result = ExecuteExpr(cmd.Args[1]);
					if (Variables.ContainsKey(name))
					{
						Variables[name] = result;
					}
					else
					{
						Variables.Add(name, result);
					}
				}
				else if (cmd.Type == ExprType.Negative)
				{
					result = ((double)ExecuteExpr(cmd.Args[0])) * -1;
				}
				else if (cmd.Type == ExprType.Mod)
				{
					result = ((double)ExecuteExpr(cmd.Args[0])) % ((double)ExecuteExpr(cmd.Args[1]));
				}
				else if (cmd.Type == ExprType.Div)
				{
					result = ((double)ExecuteExpr(cmd.Args[0])) / ((double)ExecuteExpr(cmd.Args[1]));
				}
				else if (cmd.Type == ExprType.Mul)
				{
					result = ((double)ExecuteExpr(cmd.Args[0])) * ((double)ExecuteExpr(cmd.Args[1]));
				}
				else if (cmd.Type == ExprType.Sub)
				{
					result = ((double)ExecuteExpr(cmd.Args[0])) - ((double)ExecuteExpr(cmd.Args[1]));
				}
				else if (cmd.Type == ExprType.Add)
				{
					object left = ExecuteExpr(cmd.Args[0]);
					object right = ExecuteExpr(cmd.Args[1]);
					if ((left is String) && (right is String))
					{
						result = ((string)left) + ((string)right);
					}
					else
					{
						result = ((double)left) + ((double)right);
					}
				}
				else
				{
					throw new Exception("Unexpected command: " + cmd.Type);
				}
			}
			return result;
		}

		object ExecuteFunction(Command cmd, object instance, bool useDefaultVar)
		{
			object result = null;
			string origName = ((Literal)cmd.Args[0]).Value;

			string assemblyName = null;
			string className = null;
			string methodName = null;
			string[] splitName = origName.ToLowerInvariant().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
			if (splitName.Length == 1)
			{
				methodName = splitName[0];
			}
			else if (splitName.Length == 2)
			{
				className = splitName[0];
				methodName = splitName[1];
			}
			else if (splitName.Length == 3)
			{
				assemblyName = splitName[0];
				className = splitName[1];
				methodName = splitName[2];
			}
			else
			{
				throw new Exception("Invalid function name: " + origName + "()");
			}

			if (methodName == LoadPluginName)
			{
				string path;
				if (useDefaultVar)
				{
					path = (string)defaultVar;
				}
				else
				{
					path = (string)ExecuteExpr(cmd.Args[1]);
				}
				PluginManager.LoadPlugin(path);
			}
			else if (methodName == ImportName)
			{
				string path;
				if (useDefaultVar)
				{
					path = (string)defaultVar;
				}
				else
				{
					path = (string)ExecuteExpr(cmd.Args[1]);
				}
				ScriptParser parser = new ScriptParser(path);
				string prevCWD = Environment.CurrentDirectory;
				Environment.CurrentDirectory = parser.CWD;
				ExecuteExpr(parser.CommandRoot);
				Environment.CurrentDirectory = prevCWD;
			}
			else
			{
				List<FunctionBase> plugins;
				if (!PluginManager.Functions.TryGetValue(methodName, out plugins) || (plugins.Count <= 0))
				{
					throw new Exception("Couldn't find plugin for " + origName + "()");
				}

				if (assemblyName != null)
				{
					for (int i = 0; i < plugins.Count; )
					{
						string pluginAssemblyName = Path.GetFileNameWithoutExtension(plugins[i].Type.Assembly.CodeBase).ToLowerInvariant();
						if (assemblyName == pluginAssemblyName)
						{
							i++;
						}
						else
						{
							plugins.RemoveAt(i);
						}
					}
				}
				if (className != null)
				{
					for (int i = 0; i < plugins.Count; )
					{
						string pluginClassName = plugins[i].Type.Name.ToLowerInvariant();
						if (className == pluginClassName)
						{
							i++;
						}
						else
						{
							plugins.RemoveAt(i);
						}
					}
				}
				if (plugins.Count <= 0)
				{
					throw new Exception("Couldn't find plugin for " + origName + "()");
				}

				ScriptArg[] scriptArgs;
				int argIdx = 0;
				if (useDefaultVar)
				{
					scriptArgs = new ScriptArg[cmd.Args.Count];

					ScriptArg scriptArg = new ScriptArg();
					scriptArg.Value = defaultVar;
					scriptArgs[argIdx] = scriptArg;
					argIdx++;
				}
				else
				{
					scriptArgs = new ScriptArg[cmd.Args.Count - 1];
				}
				for (int cmdIdx = 1; cmdIdx < cmd.Args.Count; cmdIdx++)
				{
					ScriptArg scriptArg = new ScriptArg();
					Expr cmdArg = cmd.Args[cmdIdx];
					if (cmdArg.Type == ExprType.Assign)
					{
						Command assign = (Command)cmdArg;
						scriptArg.Name = ((Literal)assign.Args[0]).Value;
						cmdArg = assign.Args[1];
					}
					if (cmdArg.Type == ExprType.Name)
					{
						scriptArg.Variable = ((Literal)cmdArg).Value;
					}
					scriptArg.Value = ExecuteExpr(cmdArg);
					scriptArgs[argIdx] = scriptArg;
					argIdx++;
				}

				bool instanceExactMatch = false;
				int minConversions = Int32.MaxValue;
				FunctionArg[] pluginArgs = null;
				FunctionBase plugin = null;
				for (int i = 0; i < plugins.Count; i++)
				{
					int numConversions;
					FunctionArg[] matchArgs = plugins[i].Match(scriptArgs, useDefaultVar, out numConversions);
					if ((matchArgs != null) && (numConversions < minConversions))
					{
						if (instance == null)
						{
							pluginArgs = matchArgs;
							plugin = plugins[i];
							minConversions = numConversions;
						}
						else
						{
							Type instanceType = instance.GetType();
							if (plugins[i].Type.Equals(instanceType))
							{
								pluginArgs = matchArgs;
								plugin = plugins[i];
								minConversions = numConversions;
								instanceExactMatch = true;
							}
							else if (!instanceExactMatch && plugins[i].Type.IsAssignableFrom(instanceType))
							{
								pluginArgs = matchArgs;
								plugin = plugins[i];
								minConversions = numConversions;
							}
						}
					}
				}
				if (pluginArgs == null)
				{
					throw new Exception("Couldn't match args for " + origName + "()");
				}

				List<FunctionArg> changedVars;
				result = plugin.RunPlugin(instance, pluginArgs, out changedVars);
				for (int i = 0; i < changedVars.Count; i++)
				{
					FunctionArg pluginArg = changedVars[i];
					if (pluginArg.DefaultVar)
					{
						defaultVar = pluginArg.Value;
					}
					else
					{
						Variables[pluginArg.Variable] = pluginArg.Value;
					}
				}
			}
			return result;
		}

		public static void PrintTree(Command cmd, string space)
		{
			Console.WriteLine(space + cmd.Type);
			foreach (var arg in cmd.Args)
			{
				if (arg is Literal)
				{
					Literal literal = (Literal)arg;
					Console.WriteLine(space + "  " + literal.Value + " [" + literal.Type + "]");
				}
				else
				{
					Command child = (Command)arg;
					PrintTree(child, space + "  ");
				}
			}
		}
	}
}
