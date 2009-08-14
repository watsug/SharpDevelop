// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Description of PythonParserHelper.
	/// </summary>
	public class PythonParserHelper
	{
		/// <summary>
		/// Parses the code and returns the first statement as an assignment statement.
		/// </summary>
		public static AssignmentStatement GetAssignmentStatement(string code)
		{
			return GetFirstStatement(code) as AssignmentStatement;
		}
	
		/// <summary>
		/// Parses the code and returns the first statement's expression as call expression.
		/// </summary>		
		public static CallExpression GetCallExpression(string code)
		{
			ExpressionStatement expressionStatement = GetFirstStatement(code) as ExpressionStatement;
			return expressionStatement.Expression as CallExpression;
		}
		
		static Statement GetFirstStatement(string code)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"snippet.py", new StringTextBuffer(code));
			SuiteStatement suiteStatement = (SuiteStatement)ast.Body;
			return suiteStatement.Statements[0];
		}
	}
}
