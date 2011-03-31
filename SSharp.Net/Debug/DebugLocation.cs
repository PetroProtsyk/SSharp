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
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Debug {
    /// <summary>
    /// Public debug information about AstNode
    /// </summary>
    public class DebugLocation {
        private ScriptAst node;
        private IScriptContext context;
        private string code = null;
        private SourcePosition position = null;

        internal DebugLocation(ScriptAst node, IScriptContext context) {
            this.node = node;
            this.context = context;
        }

        public string Code {
            get {
                if (code == null)
                    code = node.Code(context);

                return code;
            }
        }

        public SourcePosition Position {
            get {
                if (position == null)
                    position = new SourcePosition(node.Location.Position, node.Location.Line, node.Location.Column, node.Span.Length);

                return position;
            }
        }
    }
}
#endif