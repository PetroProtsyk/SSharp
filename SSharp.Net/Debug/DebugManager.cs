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
using System;
using System.Collections.Generic;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Debug {
    /// <summary>
    /// Manages information about scripts compiled in debug mode
    /// </summary>
    public static class DebugManager {
        private static Dictionary<Script, DebugInfo> scriptCache = new Dictionary<Script, DebugInfo>();

        internal class DebugInfo {
            public bool isBreak = true;
            public object syncRoot = new object();
        }

        internal static void Add(Script script) {
            scriptCache.Add(script, new DebugInfo());
        }

        internal static void Remove(Script script) {
            scriptCache.Remove(script);
        }

        internal static DebugInfo Get(Script script) {
            return scriptCache[script];
        }

        /// <summary>
        /// Occurs when script reaches break point. Event will be raise from debug thread.
        ///
        /// Note: All UI updates should be synchronized
        /// </summary>
        public static EventHandler<DebugBreakPointEventArgs> BreakPoint;

        internal static void OnBreakPoint(Script script, ScriptAst location) {
            var handler = BreakPoint;
            if (handler != null)
                handler.Invoke(script, new DebugBreakPointEventArgs(script, new DebugLocation(location, script.Context)));
        }

        #region Default Debugger
        static DebugerProcess debugger;

        public static void SetDefaultDebugger(bool enable) {
            if (enable)
                BreakPoint += HandleBreakPoint;
            else {
                if (debugger != null)
                    debugger.Stop();

                BreakPoint -= HandleBreakPoint;
            }
        }

        public static void HandleBreakPoint(object sender, DebugBreakPointEventArgs e) {
            if (debugger == null) {
                debugger = new DebugerProcess();
                debugger.Start();
            }

            if (!debugger.ProcessStep(e.Script.SourceCode,
                string.Format("{0}:{1} Result:[{2}] Code:[{3}]",
                    e.Location.Position.Line, e.Location.Position.Column,
                    e.Script.Context.Result == null ? "null" : e.Script.Context.Result.ToString(),
                    e.Location.Code))) {

                debugger.Stop();
                debugger = null;

                Get((Script)sender).isBreak = false;
            }
        }
        #endregion
    }
}
#endif