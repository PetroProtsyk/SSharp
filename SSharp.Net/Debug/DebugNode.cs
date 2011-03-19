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
using System.Threading;
using System.Threading.Tasks;
using Scripting.SSharp.Parser;
using Scripting.SSharp.Parser.Ast;
using Scripting.SSharp.Parser.FastGrammar;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Debug {
    internal class DebugNode : ScriptExpr {
        ScriptAst boundNode;

        public DebugNode(ScriptAst boundItems)
            : base(new AstNodeArgs(new Terminal("DEBUG"), new SourceSpan(), null)) {

            AddChild(boundItems);
            this.boundNode = boundItems;
        }

        public override void Evaluate(IScriptContext context) {
            var debugInfo = DebugManager.Get(context.Owner);

            lock (debugInfo.syncRoot) {
                if (debugInfo.isBreak) {
                    Task.Factory.StartNew(() => DebugMonitor(context, debugInfo));

                    // Stop execution until DebugMonitor thread will release lock
                    Monitor.Wait(debugInfo.syncRoot);
                }
            }

            boundNode.Evaluate(context);
        }

        public void DebugMonitor(IScriptContext context, DebugManager.DebugInfo debugInfo) {
            DebugManager.OnBreakPoint(context.Owner, boundNode);

            lock (debugInfo.syncRoot) {
                Monitor.Pulse(debugInfo.syncRoot);
            }
        }
    }
}
#endif