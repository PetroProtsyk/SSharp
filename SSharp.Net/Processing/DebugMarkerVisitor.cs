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
#if !SILVERLIGHT

using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Parser.FastGrammar;
using Scripting.SSharp.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Scripting.SSharp.Processing
{
  //Script s = Script.Compile(@"1+1; 2+2;", new ScriptNET.Processing.IPostProcessing[] { new ScriptNET.Processing.DebugMarkerVisitor() }, false);
  //Console.Write(s.SyntaxTree);
  internal class DebugMarkerVisitor : IPostProcessing
  {
    #region IPostProcessing Members

    public void BeginProcessing(Script script)
    {
      
    }

    public void EndProcessing(Script script)
    {
      DebugManager.Add(script);
      script.Disposing += (s,e) => DebugManager.Remove((Script)s);
    }

    #endregion

    #region IAstVisitor Members

    public void BeginVisit(AstNode node)
    {
      var elements = node as ScriptElements;
      if (elements == null) return;

      var modified = new List<AstNode>();
      foreach (var child in elements.ChildNodes)
      {
        ScriptAst scriptNode = child as ScriptAst;
        if (child != null)
        {
          modified.Add(new DebugNode(scriptNode));
        }
        modified.Add(child);
      }

      elements.ChildNodes.Clear();
      elements.ChildNodes.AddRange(modified);
    }

    public void EndVisit(AstNode node)
    {
      
    }

    #endregion
  }

  public class SourcePosition
  {
    public int Position { get; private set; }

    public int Line { get; private set; }

    public int Column { get; private set; }

    public int Length { get; private set; }

    public SourcePosition(int position, int line, int column, int length)
    {
      Position = position;
      Line = line;
      Column = column;
      Length = length;
    }
  }
  
  /// <summary>
  /// Public debug information about AstNode
  /// </summary>
  public class DebugLocation
  {
    private ScriptAst node;
    private IScriptContext context;
    private string code = null;
    private SourcePosition position = null;

    internal DebugLocation(ScriptAst node, IScriptContext context)
    {
      this.node = node;
      this.context = context;
    }

    public string Code
    {
      get
      {
        if (code == null)
          code = node.Code(context);

        return code;
      }
    }

    public SourcePosition Position
    {
      get
      {
        if (position == null)
          position = new SourcePosition(node.Location.Position,node.Location.Line,node.Location.Column,node.Span.Length);

        return position;
      }
    }
  }

  /// <summary>
  /// Manages information about scripts compiled in debug mode
  /// </summary>
  public static class DebugManager
  {
    private static Dictionary<Script, DebugInfo> scriptCache = new Dictionary<Script, DebugInfo>();

    internal class DebugInfo
    {
      public bool isBreak = true;
      public object syncRoot = new object();
    }

    internal static void Add(Script script)
    {
      scriptCache.Add(script, new DebugInfo());
    }

    internal static void Remove(Script script)
    {
      scriptCache.Remove(script);
    }

    internal static DebugInfo Get(Script script)
    {
      return scriptCache[script];
    }

    /// <summary>
    /// Occurs when script reaches break point. Event will be raise from debug thread.
    ///
    /// Note: All UI updates should be synchronized
    /// </summary>
    public static EventHandler<DebugBreakPointEventArgs> BreakPoint;

    internal static void OnBreakPoint(Script script, ScriptAst location)
    {
      var handler = BreakPoint;
      if (handler != null)
        handler.Invoke(script, new DebugBreakPointEventArgs(script, new DebugLocation(location, script.Context)));
    }
  }

  public class DebugBreakPointEventArgs : EventArgs
  {
    public Script Script { get; private set; }
    public DebugLocation Location { get; private set; }

    public DebugBreakPointEventArgs(Script script, DebugLocation location)
    {
      Script = script;
      Location = location;
    }
  }

  internal class DebugNode : ScriptAst
  {
    ScriptAst boundNode;

    public DebugNode(ScriptAst boundItems)
      : base(new AstNodeArgs(new Terminal("DEBUG"), new SourceSpan(), null))
    {
      this.boundNode = boundItems;
    }

    public override void Evaluate(IScriptContext context)
    {
      var debugInfo = DebugManager.Get(context.Owner);

      lock (debugInfo.syncRoot)
      {
        if (debugInfo.isBreak)
        {
          Task.Factory.StartNew(() => DebugMonitor(context, debugInfo));

          // Stop execution until DebugMonitor thread will release lock
          Monitor.Wait(debugInfo.syncRoot);
        }
      }

      base.Evaluate(context);
    }

    public void DebugMonitor(IScriptContext context, DebugManager.DebugInfo debugInfo)
    {
      DebugManager.OnBreakPoint(context.Owner, boundNode);

      lock (debugInfo.syncRoot)
      {
        Monitor.Pulse(debugInfo.syncRoot);
      }
    }
  }
}
#endif