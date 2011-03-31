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
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Processing;

namespace Scripting.SSharp.Debug {
    /// <summary>
    ///<code>
    /// try {
    ///    DebugManager.SetDefaultDebugger(true);
    ///
    ///    using (Script s = Script.CompileForDebug(@"1+1; 2+2; b=true; for (i=1; i<3; i++) { b=false; }")) {
    ///        Console.Write(s.SyntaxTree);
    ///        s.Execute();
    ///    }
    /// }
    /// finally {
    ///    DebugManager.SetDefaultDebugger(false);
    /// }
    ///</code>
    /// </summary>
    internal class DebugMarkerVisitor : IPostProcessing {
        #region IPostProcessing Members

        public void BeginProcessing(Script script) {

        }

        public void EndProcessing(Script script) {
            DebugManager.Add(script);
            script.Disposing += (s, e) => DebugManager.Remove((Script)s);
        }

        #endregion

        #region IAstVisitor Members

        public bool BeginVisit(AstNode node) {
            DebugNode dbgNode = node as DebugNode;
            if (dbgNode != null) return false;

            List<AstNode> nodes = new List<AstNode>();

            foreach (AstNode child in node.ChildNodes) {

                ScriptExpr exprNode = child as ScriptExpr;
                if (exprNode != null) {
                    nodes.Add(new DebugNode(exprNode));
                    continue;
                }

                nodes.Add(child);
            }

            node.ReplaceChildNodes(nodes);
            return true;
        }

        public void EndVisit(AstNode node) {

        }

        #endregion
    }
}

#endif
