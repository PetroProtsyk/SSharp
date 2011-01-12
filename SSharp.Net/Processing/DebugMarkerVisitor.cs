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

using System.Collections.Generic;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Parser.FastGrammar;

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
        modified.Add(new DebugNode());
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

  internal class DebugNode : AstNode
  {
    public DebugNode()
      : base(new AstNodeArgs(new Terminal("DEBUG"), new SourceSpan(), null))
    {

    }
  }
}
