using System;
using System.Collections.Generic;
using System.Text;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Processing;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp
{
  /// <summary>
  /// Compiled script
  /// </summary>
  public class Script : IDisposable
  {
    #region Properties
    /// <summary>
    /// Ast of the given Source Code
    /// </summary>
    public ScriptAst Ast { get; set; }

    /// <summary>
    /// Code of the script
    /// </summary>
    public string SourceCode { get; set; }

    /// <summary>
    /// Execution context
    /// </summary>
    public IScriptContext Context { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    protected Script()
    {
        Context = new ScriptContext();
        Runtime.RuntimeHost.InitializeScript(Context);
    }

    /// <summary>
    /// Script Constructor
    /// </summary>
    /// <param name="Context">Context in which script will execute</param>
    protected Script(IScriptContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      this.Context = context;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Executes current script returning result
    /// </summary>
    /// <returns>result of execution</returns>
    public object Execute()
    {
      object rez = Ast.Execute(Context);
      EventBroker.ClearAllEventsIfNeeded();
      return rez;
    }

    /// <summary>
    /// Returns source code for given node
    /// </summary>
    /// <param name="node">Node</param>
    /// <returns>source code</returns>
    public string Code(ScriptAst node)
    {
      return SourceCode.Substring(node.Span.Start.Position, node.Span.Length);
    }

    /// <summary>
    /// String representing syntax tree
    /// </summary>
    public string SyntaxTree
    {
      get
      {
        return Ast.ConcreteSyntaxTree();
      }
    }
    #endregion

    #region Service Methods
    /// <summary>
    /// Compiles code and performs post processing
    /// </summary>
    /// <param name="code"></param>
    /// <param name="postProcessings">instance of vistor</param>
    /// <returns></returns>
    public static Script Compile(string code, IEnumerable<IPostProcessing> postProcessings, bool isExpression)
    {
      Script script = new Script();
      script.SourceCode = code;

      RuntimeHost.Lock();
      try
      {
        script.Ast = (ScriptAst)Parse(code, isExpression);

        if (postProcessings != null)
        {
          foreach (var postProcessing in postProcessings)
          {
            postProcessing.BeginProcessing(script);
             script.Ast.AcceptVisitor(postProcessing);
            postProcessing.EndProcessing(script);
          }
        }
      }
      finally
      {
        RuntimeHost.UnLock();
      }

      return script;
    }

    /// <summary>
    /// Compiles Script.NET code into AST representation
    /// </summary>
    /// <param name="code">Code string</param>
    /// <returns>Compiled Script. Throws Script Exception on Syntax Errors</returns>
    public static Script Compile(string code)
    {
      return Compile(code, new IPostProcessing[] { new FunctionDeclarationVisitor() }, false);
    }

    public static Script CompileExpression(string code)
    {
      return Compile(code, null, true);
    }

    /// <summary>
    /// Executes script code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static object RunCode(string code)
    {
      return RunCode(code, null, false);
    }

    public static object RunCode(string code, bool isExpression)
    {
      return RunCode(code, null, isExpression);
    }

    public static object RunCode(string code, IScriptContext context)
    {
      return RunCode(code, context, false);
    }

    public static object RunCode(string code, IScriptContext context, bool isExpression)
    {
      Script script = isExpression ? CompileExpression(code) : Compile(code);
      if (context != null)
        script.Context = context;

      return script.Execute();
    }

    /// <summary>
    /// Parses code
    /// </summary>
    /// <param name="code">Script code</param>
    /// <returns>AstNode or throws:ArgumentException, ScriptSyntaxErrorException</returns>
    protected internal static AstNode Parse(string code, bool isExpression)
    {
      if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

      AstNode result = null;
      if (isExpression)
      {
        result = RuntimeHost.Parser.Parse("return " + code + ";");
      }
      else
      {
        result = RuntimeHost.Parser.Parse(code);
      }
      
      if (result == null)
      {
        StringBuilder errorMessage = new StringBuilder("Parsing error:");
        foreach (SyntaxError error in RuntimeHost.Parser.Context.Errors)
        {
          errorMessage.AppendLine(error.Message);
          errorMessage.AppendLine(string.Format("at line:{0} position:{1} in code:",
             error.Location.Line,
             error.Location.Position));
          errorMessage.AppendLine(code.Substring(error.Location.Position, Math.Min(50, code.Length - error.Location.Position)));
        }

        throw new ScriptSyntaxErrorException(errorMessage.ToString());
      }

      return result;
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      EventBroker.ClearMapping(this);
    }

    #endregion
  }

}
