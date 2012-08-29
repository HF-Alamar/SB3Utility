using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SB3Utility
{
	public abstract class Expr
	{
		public ExprType Type;
	}

	public class Literal : Expr
	{
		public string Value;

		public Literal(ExprType type, string value)
		{
			this.Type = type;
			this.Value = value;
		}
	}

	public class Command : Expr
	{
		public List<Expr> Args = new List<Expr>();

		public Command(ExprType type)
		{
			this.Type = type;
		}
	}

	public enum ExprType
	{
		Root,

		// literals
		String,
		Bytes,
		Name,
		Bool,
		Null,
		Number,

		DotInstanceDefault,
		DotInstance,
		DotDefault,
		DotProperty,
		Function,
		Array,

		DotInstanceDefaultChain,
		DotInstanceChain,
		DotPropertyChain,

		Indexed,

		Assign,
		Negative,

		Mod,
		Div,
		Mul,
		Sub,
		Add
	}

	public class ScriptParser
	{
		public enum TokenType
		{
			Name,
			String,
			Number,
			HexOrIndex,
			Dot,
			Comma,
			LP,
			RP,
			LCB,
			RCB,
			Mod,
			Div,
			Mul,
			Minus,
			Plus,
			Equals
		}

		public class Token
		{
			public string Value;
			public TokenType Type;
			public string Path;
			public int Line;
			public int Column;

			public Token(string value, TokenType type, string path, int line, int column)
			{
				this.Value = value;
				this.Type = type;

				this.Path = path;
				this.Line = line;
				this.Column = column - value.Length;
			}

			public string Error(int offset)
			{
				return (" (" + System.IO.Path.GetFileName(this.Path) + ", line " + this.Line + ", column " + (this.Column + offset) + ")");
			}

			public string Error()
			{
				return Error(0);
			}
		}

		class StreamReaderCursor : StreamReader
		{
			private char lastChar = Char.MinValue;

			public StreamReaderCursor(string path)
				: base(path)
			{
			}

			public StreamReaderCursor(Stream stream)
				: base(stream)
			{
			}

			public int Read(ref int line, ref int column)
			{
				int i = base.Read();
				if (i != -1)
				{
					char c = (char)i;
					if (c == '\r')
					{
						line++;
						column = 1;
					}
					else if (c == '\n')
					{
						column = 1;
						if (lastChar != '\r')
						{
							line++;
						}
					}
					else
					{
						column++;
					}
					this.lastChar = c;
				}
				return i;
			}
		}

		List<List<Token>> Scanner(StreamReaderCursor reader, string scriptName)
		{
			var tokenList = new List<List<Token>>();
			var tokens = new List<Token>();
			tokenList.Add(tokens);
			int line = 1;
			int column = 1;

			while (!reader.EndOfStream)
			{
				char c = (char)reader.Read(ref line, ref column);
				if (Char.IsWhiteSpace(c))
				{
					if ((c == '\r') || (c == '\n'))
					{
						if (tokens.Count > 0)
						{
							tokens = new List<Token>();
							tokenList.Add(tokens);
						}
					}
				}
				else if (c == ';')
				{
					while (!reader.EndOfStream && (reader.Peek() != -1))
					{
						c = (char)reader.Peek();
						if ((c == '\r') || (c == '\n'))
						{
							break;
						}
						reader.Read(ref line, ref column);
					}
				}
				else if (c == '/')
				{
					if (reader.Peek() != -1)
					{
						c = (char)reader.Peek();
						if (c == '/')
						{
							reader.Read(ref line, ref column);
							while (!reader.EndOfStream && (reader.Peek() != -1))
							{
								c = (char)reader.Peek();
								if ((c == '\r') || (c == '\n'))
								{
									break;
								}
								reader.Read(ref line, ref column);
							}
						}
						else if (c == '*')
						{
							reader.Read(ref line, ref column);
							while (!reader.EndOfStream)
							{
								c = (char)reader.Read(ref line, ref column);
								if ((c == '*') && (reader.Peek() != -1) && ((char)reader.Peek() == '/'))
								{
									reader.Read(ref line, ref column);
									break;
								}
							}
						}
						else
						{
							tokens.Add(new Token(c.ToString(), TokenType.Div, scriptName, line, column));
						}
					}
					else
					{
						tokens.Add(new Token(c.ToString(), TokenType.Div, scriptName, line, column));
					}
				}
				else if (c == '\"')
				{
					var sb = new StringBuilder();
					while (!reader.EndOfStream)
					{
						c = (char)reader.Read(ref line, ref column);
						if ((c == '\\') && (reader.Peek() != -1) && ((char)reader.Peek() == '\"'))
						{
							c = (char)reader.Read(ref line, ref column);
							sb.Append(c);
						}
						else
						{
							if (c != '\"')
							{
								sb.Append(c);
							}
							else
							{
								break;
							}
						}
					}
					tokens.Add(new Token(sb.ToString(), TokenType.String, scriptName, line, column));
				}
				else if (c == '[')
				{
					var sb = new StringBuilder();
					while (!reader.EndOfStream)
					{
						c = (char)reader.Read(ref line, ref column);
						if (c != ']')
						{
							sb.Append(c);
						}
						else
						{
							break;
						}
					}
					tokens.Add(new Token(sb.ToString(), TokenType.HexOrIndex, scriptName, line, column));
				}
				else if (Char.IsLetter(c) || c == '_')
				{
					var sb = new StringBuilder();
					sb.Append(c);
					while (!reader.EndOfStream && (reader.Peek() != -1))
					{
						c = (char)reader.Peek();
						if (Char.IsLetterOrDigit(c) || (c == '_'))
						{
							sb.Append(c);
							reader.Read(ref line, ref column);
						}
						else
						{
							break;
						}
					}
					tokens.Add(new Token(sb.ToString(), TokenType.Name, scriptName, line, column));
				}
				else if (Char.IsDigit(c))
				{
					var sb = new StringBuilder();
					sb.Append(c);
					while (!reader.EndOfStream && (reader.Peek() != -1))
					{
						c = (char)reader.Peek();
						if (Char.IsDigit(c))
						{
							sb.Append(c);
							reader.Read(ref line, ref column);
						}
						else
						{
							break;
						}
					}
					tokens.Add(new Token(sb.ToString(), TokenType.Number, scriptName, line, column));
				}
				else if (c == '.')
				{
					var sb = new StringBuilder();
					sb.Append(c);
					if (!reader.EndOfStream && (reader.Peek() != -1) && Char.IsDigit((char)reader.Peek()))
					{
						while (!reader.EndOfStream && (reader.Peek() != -1))
						{
							c = (char)reader.Peek();
							if (Char.IsDigit(c))
							{
								sb.Append(c);
								reader.Read(ref line, ref column);
							}
							else
							{
								break;
							}
						}
						tokens.Add(new Token(sb.ToString(), TokenType.Number, scriptName, line, column));
					}
					else
					{
						tokens.Add(new Token(sb.ToString(), TokenType.Dot, scriptName, line, column));
					}
				}
				else if (c == '(')
				{
					tokens.Add(new Token(c.ToString(), TokenType.LP, scriptName, line, column));
				}
				else if (c == ')')
				{
					tokens.Add(new Token(c.ToString(), TokenType.RP, scriptName, line, column));
				}
				else if (c == '{')
				{
					tokens.Add(new Token(c.ToString(), TokenType.LCB, scriptName, line, column));
				}
				else if (c == '}')
				{
					tokens.Add(new Token(c.ToString(), TokenType.RCB, scriptName, line, column));
				}
				else if (c == ',')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Comma, scriptName, line, column));
				}
				else if (c == '%')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Mod, scriptName, line, column));
				}
				else if (c == '*')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Mul, scriptName, line, column));
				}
				else if (c == '+')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Plus, scriptName, line, column));
				}
				else if (c == '-')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Minus, scriptName, line, column));
				}
				else if (c == '=')
				{
					tokens.Add(new Token(c.ToString(), TokenType.Equals, scriptName, line, column));
				}
				else
				{
					throw new Exception("Unexpected char: " + c + " (" + Path.GetFileName(scriptName) + ", line " + line + ", column " + column);
				}
			}

			if (tokens.Count <= 0)
			{
				tokenList.RemoveAt(tokenList.Count - 1);
			}
			return tokenList;
		}

		Expr GetExpr(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((result = GetBytes(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetArray(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetNull(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetBool(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetGroup(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetArith(tokens, ref tokenIdx)) != null) { }
			return result;
		}

		Expr GetDotInstanceDefault(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Dot) &&
				((tokenIdx + 2) < tokens.Count) && (tokens[tokenIdx + 2].Type == TokenType.Dot))
			{
				Token token = tokens[tokenIdx];
				tokenIdx += 3;
				Command cmd = new Command(ExprType.DotInstanceDefault);
				result = cmd;
				cmd.Args.Add(new Literal(ExprType.Name, token.Value));
				Expr expr = GetFunction(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a function" + token.Error(2));
				}
				cmd.Args.Add(expr);
			}
			return result;
		}

		Expr GetDotInstance(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Dot) &&
				((tokenIdx + 2) < tokens.Count) && (tokens[tokenIdx + 2].Type == TokenType.Name) &&
				((tokenIdx + 3) < tokens.Count) && (tokens[tokenIdx + 3].Type == TokenType.LP))
			{
				Token token = tokens[tokenIdx];
				tokenIdx += 2;
				Command cmd = new Command(ExprType.DotInstance);
				result = cmd;
				cmd.Args.Add(new Literal(ExprType.Name, token.Value));
				Expr expr = GetFunction(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a function" + token.Error(1));
				}
				cmd.Args.Add(expr);
			}
			return result;
		}

		Expr GetDotDefault(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Dot))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				Command cmd = new Command(ExprType.DotDefault);
				result = cmd;
				Expr expr = GetFunction(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a function" + token.Error(1));
				}
				cmd.Args.Add(expr);
			}
			return result;
		}

		Expr GetDotProperty(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Dot) &&
				((tokenIdx + 2) < tokens.Count) && (tokens[tokenIdx + 2].Type == TokenType.Name))
			{
				Token token = tokens[tokenIdx];
				Token property = tokens[tokenIdx + 2];
				tokenIdx += 3;

				Command cmd = new Command(ExprType.DotProperty);
				cmd.Args.Add(new Literal(ExprType.Name, token.Value));
				cmd.Args.Add(new Literal(ExprType.Name, property.Value));
				result = cmd;

				result = GetIndexed(result, tokens, ref tokenIdx);
				result = GetChain(result, tokens, ref tokenIdx);
			}
			return result;
		}

		Expr GetDotPropertyChain(Expr result, List<Token> tokens, ref int tokenIdx)
		{
			Expr chain = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Dot) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Name))
			{
				Token property = tokens[tokenIdx + 1];
				tokenIdx += 2;

				Command cmd = new Command(ExprType.DotPropertyChain);
				cmd.Args.Add(result);
				cmd.Args.Add(new Literal(ExprType.Name, property.Value));
				chain = cmd;

				chain = GetIndexed(chain, tokens, ref tokenIdx);
				chain = GetChain(chain, tokens, ref tokenIdx);
			}
			return chain;
		}

		Expr GetDotInstanceChain(Expr result, List<Token> tokens, ref int tokenIdx)
		{
			Expr chain = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Dot) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Name) &&
				((tokenIdx + 2) < tokens.Count) && (tokens[tokenIdx + 2].Type == TokenType.LP))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				Command cmd = new Command(ExprType.DotInstanceChain);
				chain = cmd;
				cmd.Args.Add(result);
				Expr expr = GetFunction(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a function" + token.Error(1));
				}
				cmd.Args.Add(expr);
			}
			return chain;
		}

		Expr GetDotInstanceDefaultChain(Expr result, List<Token> tokens, ref int tokenIdx)
		{
			Expr chain = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Dot) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Dot))
			{
				Token token = tokens[tokenIdx];
				tokenIdx += 2;
				Command cmd = new Command(ExprType.DotInstanceDefaultChain);
				chain = cmd;
				cmd.Args.Add(result);
				Expr expr = GetFunction(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a function" + token.Error(2));
				}
				cmd.Args.Add(expr);
			}
			return chain;
		}

		Expr GetChain(Expr result, List<Token> tokens, ref int tokenIdx)
		{
			Expr chain = null;
			if ((chain = GetDotInstanceDefaultChain(result, tokens, ref tokenIdx)) != null)
			{
				return chain;
			}
			else if ((chain = GetDotInstanceChain(result, tokens, ref tokenIdx)) != null)
			{
				return chain;
			}
			else if ((chain = GetDotPropertyChain(result, tokens, ref tokenIdx)) != null)
			{
				return chain;
			}
			return result;
		}

		Expr GetFunction(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.LP))
			{
				Token token = tokens[tokenIdx];
				tokenIdx += 2;
				Command cmd = new Command(ExprType.Function);
				result = cmd;
				cmd.Args.Add(new Literal(ExprType.Name, token.Value));
				GetExprList(cmd, token, TokenType.RP, ')', tokens, ref tokenIdx);

				if ((token.Value.ToLowerInvariant() == ScriptExecutor.LoadPluginName) && (cmd.Args.Count != 2))
				{
					throw new Exception("Invalid path for LoadPlugin()" + token.Error());
				}
				else if ((token.Value.ToLowerInvariant() == ScriptExecutor.ImportName) && (cmd.Args.Count != 2))
				{
					throw new Exception("Invalid path for Import()" + token.Error());
				}

				result = GetIndexed(result, tokens, ref tokenIdx);
				result = GetChain(result, tokens, ref tokenIdx);
			}
			return result;
		}

		Expr GetFunctionVar(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((result = GetDotInstanceDefault(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetDotInstance(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetDotDefault(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetDotProperty(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetFunction(tokens, ref tokenIdx)) != null) { }
			return result;
		}

		Expr GetArray(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.LCB))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				Command cmd = new Command(ExprType.Array);
				result = cmd;
				GetExprList(cmd, token, TokenType.RCB, '}', tokens, ref tokenIdx);
			}
			return result;
		}

		void GetExprList(Command cmd, Token startToken, TokenType endToken, char endTokenChar, List<Token> tokens, ref int tokenIdx)
		{
			Token token = startToken;
			while ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type != endToken))
			{
				token = tokens[tokenIdx];
				Expr expr;
				if ((expr = GetAssign(tokens, ref tokenIdx)) != null) { }
				else if ((expr = GetExpr(tokens, ref tokenIdx)) != null) { }
				else
				{
					throw new Exception("Expected an expression" + token.Error());
				}
				cmd.Args.Add(expr);
				if (tokenIdx < tokens.Count)
				{
					token = tokens[tokenIdx];
					if (token.Type != endToken)
					{
						if (token.Type == TokenType.Comma)
						{
							tokenIdx++;
						}
						else
						{
							throw new Exception("Expected a comma" + token.Error());
						}
					}
				}
			}
			if ((tokenIdx >= tokens.Count) || ((token = tokens[tokenIdx]).Type != endToken))
			{
				throw new Exception("Missing '" + endTokenChar + "'" + token.Error());
			}
			tokenIdx++;
		}

		Expr GetGroup(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.LP))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				result = GetArith(tokens, ref tokenIdx);
				if ((tokenIdx >= tokens.Count) || ((token = tokens[tokenIdx]).Type != TokenType.RP))
				{
					throw new Exception("Missing ')'" + token.Error());
				}
				tokenIdx++;
			}
			return result;
		}

		Expr GetBytes(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.HexOrIndex))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				result = new Literal(ExprType.Bytes, token.Value);
			}
			return result;
		}

		Expr GetNull(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name))
			{
				Token token = tokens[tokenIdx];
				if (token.Value.ToLowerInvariant() == "null")
				{
					tokenIdx++;
					result = new Literal(ExprType.Null, token.Value);
				}
			}
			return result;
		}

		Expr GetBool(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name))
			{
				Token token = tokens[tokenIdx];
				if ((token.Value.ToLowerInvariant() == "true") || (token.Value.ToLowerInvariant() == "false"))
				{
					tokenIdx++;
					result = new Literal(ExprType.Bool, token.Value);
				}
			}
			return result;
		}

		Expr GetName(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				result = new Literal(ExprType.Name, token.Value);

				result = GetIndexed(result, tokens, ref tokenIdx);
				result = GetChain(result, tokens, ref tokenIdx);
			}
			return result;
		}

		Expr GetNumber(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Number))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;

				if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Number))
				{
					Token fp = tokens[tokenIdx];
					tokenIdx++;
					result = new Literal(ExprType.Number, token.Value + fp.Value);
				}
				else
				{
					result = new Literal(ExprType.Number, token.Value);
				}
			}
			return result;
		}

		Expr GetString(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.String))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				result = new Literal(ExprType.String, token.Value);
			}
			return result;
		}

		Expr GetNegative(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Minus))
			{
				Token token = tokens[tokenIdx];
				tokenIdx++;
				Command cmd = new Command(ExprType.Negative);
				result = cmd;
				Expr expr = GetNegVar(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected a numeric expression" + token.Error());
				}
				cmd.Args.Add(expr);
			}
			return result;
		}

		Expr GetNegVar(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((result = GetFunctionVar(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetGroup(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetName(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetNumber(tokens, ref tokenIdx)) != null) { }
			return result;
		}

		Expr GetArith(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if (tokenIdx < tokens.Count)
			{
				if ((result = GetNegative(tokens, ref tokenIdx)) != null) { }
				else if ((result = GetNegVar(tokens, ref tokenIdx)) != null) { }
				else if ((result = GetString(tokens, ref tokenIdx)) != null) { }

				if (tokenIdx < tokens.Count)
				{
					if (tokens[tokenIdx].Type == TokenType.Mod)
					{
						result = GetArithHelper(result, ExprType.Mod, tokens, ref tokenIdx);
					}
					else if (tokens[tokenIdx].Type == TokenType.Div)
					{
						result = GetArithHelper(result, ExprType.Div, tokens, ref tokenIdx);
					}
					else if (tokens[tokenIdx].Type == TokenType.Mul)
					{
						result = GetArithHelper(result, ExprType.Mul, tokens, ref tokenIdx);
					}
					else if (tokens[tokenIdx].Type == TokenType.Minus)
					{
						result = GetArithHelper(result, ExprType.Sub, tokens, ref tokenIdx);
					}
					else if (tokens[tokenIdx].Type == TokenType.Plus)
					{
						result = GetArithHelper(result, ExprType.Add, tokens, ref tokenIdx);
					}
				}
			}
			return result;
		}

		Expr GetArithHelper(Expr result, ExprType exprType, List<Token> tokens, ref int tokenIdx)
		{
			Token token = tokens[tokenIdx];
			tokenIdx++;
			Command cmd = new Command(exprType);
			cmd.Args.Add(result);
			Expr expr = GetArith(tokens, ref tokenIdx);
			if (expr == null)
			{
				throw new Exception("Expected a numeric expression" + token.Error());
			}
			cmd.Args.Add(expr);
			return cmd;
		}

		Expr GetAssign(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.Name) &&
				((tokenIdx + 1) < tokens.Count) && (tokens[tokenIdx + 1].Type == TokenType.Equals))
			{
				Token token = tokens[tokenIdx];
				tokenIdx += 2;
				Command cmd = new Command(ExprType.Assign);
				result = cmd;
				cmd.Args.Add(new Literal(ExprType.Name, token.Value));

				Expr expr = GetExpr(tokens, ref tokenIdx);
				if (expr == null)
				{
					throw new Exception("Expected an expression" + token.Error());
				}
				cmd.Args.Add(expr);
			}
			return result;
		}

		Expr GetIndexed(Expr result, List<Token> tokens, ref int tokenIdx)
		{
			if ((tokenIdx < tokens.Count) && (tokens[tokenIdx].Type == TokenType.HexOrIndex))
			{
				Command cmd = new Command(ExprType.Indexed);
				cmd.Args.Add(result);
				cmd.Args.Add(new Literal(ExprType.Number, tokens[tokenIdx].Value));
				tokenIdx++;

				result = cmd;
			}
			return result;
		}

		Expr ParseStmt(List<Token> tokens, ref int tokenIdx)
		{
			Expr result = null;
			if ((result = GetAssign(tokens, ref tokenIdx)) != null) { }
			else if ((result = GetExpr(tokens, ref tokenIdx)) != null) { }

			if (tokenIdx < tokens.Count)
			{
				Token token = tokens[tokenIdx];
				throw new Exception("Expected newline" + token.Error());
			}
			return result;
		}

		public Command CommandRoot = null;
		public string CWD = Environment.CurrentDirectory;

		public ScriptParser(string path)
		{
			using (var reader = new StreamReaderCursor(path))
			{
				FileInfo file = new FileInfo(path);
				string prevCWD = CWD;
				CWD = file.DirectoryName;
				Environment.CurrentDirectory = CWD;
				Parse(reader, path);
				Environment.CurrentDirectory = prevCWD;
			}
		}

		public ScriptParser(Stream stream, string scriptName)
		{
			using (var reader = new StreamReaderCursor(stream))
			{
				Parse(reader, scriptName);
			}
		}

		void Parse(StreamReaderCursor reader, string scriptName)
		{
			var tokenList = Scanner(reader, scriptName);
			this.CommandRoot = new Command(ExprType.Root);
			foreach (var tokens in tokenList)
			{
				int tokenIdx = 0;
				while (tokenIdx < tokens.Count)
				{
					Expr expr = ParseStmt(tokens, ref tokenIdx);
					if (expr != null)
					{
						this.CommandRoot.Args.Add(expr);
					}
				}
			}
		}
	}
}
