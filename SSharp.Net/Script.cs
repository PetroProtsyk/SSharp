/*
 * Copyright © 2011, Petro Protsyk, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Scripting.SSharp.Processing;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp
{
  /// <summary>
  /// Compiled script
  /// </summary>
  public class Script : IDisposable, IInvokable
  {
    #region Properties
    /// <summary>
    /// Ast of the given Source Code
    /// </summary>
    internal ScriptAst Ast { get; set; }

    /// <summary>
    /// Code of the script
    /// </summary>
    public string SourceCode { get; set; }

    /// <summary>
    /// Execution context
    /// </summary>
    private IScriptContext _context;
    public IScriptContext Context
    {
      get
      {
        return _context;
      }
      set
      {
        if (_context == value) return;
        SwitchContext(_context as ScriptContext, false);
        _context = value;
        SwitchContext(_context as ScriptContext, true);
      }
    }

    private readonly List<IValueReference> _referenceCache = new List<IValueReference>(64);
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    protected Script()
    {
      var ctxt = new ScriptContext();
      RuntimeHost.InitializeScript(ctxt);
      Context = ctxt;
    }

    /// <summary>
    /// Script Constructor
    /// </summary>
    /// <param name="context">Context in which script will execute</param>
    protected Script(IScriptContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      Context = context;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Executes current script returning result
    /// </summary>
    /// <returns>result of execution</returns>
    public object Execute()
    {
      return Ast.Execute(Context);
    }

    /// <summary>
    /// Returns source code for given node
    /// </summary>
    /// <param name="node">Node</param>
    /// <returns>source code</returns>
    internal string Code(ScriptAst node)
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
    /// <param name="isExpression"></param>
    /// <returns></returns>
    internal static Script Compile(string code, IEnumerable<IPostProcessing> postProcessings, bool isExpression)
    {
      var script = new Script { SourceCode = code };

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

#if !SILVERLIGHT
    public static Script CompileForDebug(string code)
    {
      return Compile(code, 
                    new IPostProcessing[] 
                    { 
                        new FunctionDeclarationVisitor(), 
                        new Scripting.SSharp.Debug.DebugMarkerVisitor()
                    }, 
                    false);
    }
#endif

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

      object result = script.Execute();
      script.Context = null;
      script.Dispose();

      return result;
    }

    /// <summary>
    /// Parses code
    /// </summary>
    /// <param name="code">Script code</param>
    /// <param name="isExpression"></param>
    /// <returns>AstNode or throws:ArgumentException, ScriptSyntaxErrorException</returns>
    internal static AstNode Parse(string code, bool isExpression)
    {
      if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

      var result = isExpression ? RuntimeHost.Parser.Parse("return " + code + ";") : RuntimeHost.Parser.Parse(code);

      if (result == null)
      {
        var errorMessage = new StringBuilder("Parsing error:");
        foreach (var error in RuntimeHost.Parser.Context.Errors)
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

    private void SwitchContext(ScriptContext ctxt, bool isAssigning)
    {
      if (ctxt == null) return;

      if (isAssigning)
      {
        ctxt.SetOwner(this);
      }
      else
      {
        var refs = _referenceCache.ToArray();
        foreach (var r in refs) r.Remove();

        //Verify all references were cleared
        Diagnostics.Assumes.IsTrue(_referenceCache.Count == 0);

        ctxt.SetOwner(null);
      }
    }

    internal void NotifyReferenceCreated(IValueReference reference)
    {
      if (_referenceCache.Contains(reference)) return;
      _referenceCache.Add(reference);
      reference.Removed += ReferenceRemovedHandler;
    }

    private void ReferenceRemovedHandler(object sender, EventArgs e)
    {
      var reference = (IValueReference)sender;
      reference.Removed -= ReferenceRemovedHandler;
      _referenceCache.Remove(reference);
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Notifies clients of script that it is about to dispose. Gives opportunity to
    /// perform necessary clean up
    /// </summary>
    public EventHandler Disposing;
    
    protected virtual void OnDisposing()
    {
      var handler=Disposing;
      if (handler!=null)
        handler.Invoke(this, EventArgs.Empty);
    }

    private bool _disposed;
    protected bool Disposed
    {
      get
      {
        lock (this)
        {
          return _disposed;
        }
      }
    }

    public void Dispose()
    {
      lock (this)
      {
        if (_disposed) return;
        OnDisposing();
        Cleanup();
        _disposed = true;
        GC.SuppressFinalize(this);
      }
    }

    protected virtual void Cleanup()
    {
      EventBroker.ClearMapping(this);

      if (_context != null)
      {
        //Clear context owner and everything related
        SwitchContext(_context as ScriptContext, false);
        _context.Dispose();
        _context = null;
      }

      Ast = null;
      SourceCode = null;
      Disposing = null;
    }

    ~Script()
    {
      Cleanup();
    }
    #endregion

    #region IInvokable Members

    public bool CanInvoke()
    {
      return Ast != null && !Disposed;
    }

    public object Invoke(IScriptContext context, object[] args)
    {
      if (!CanInvoke()) throw new InvalidOperationException("Script is empty");

      //Switch context
      var oldContext = Context;
      Context = context;

      var result = RuntimeHost.NullValue;
      try
      {
        result = Execute();
      }
      finally
      {
        Context = oldContext;
      }

      return result;
    }

    #endregion
  }

}
