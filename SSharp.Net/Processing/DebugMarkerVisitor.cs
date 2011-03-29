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
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Text;

namespace Scripting.SSharp.Processing {

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

    public class SourcePosition {
        public int Position { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public int Length { get; private set; }

        public SourcePosition(int position, int line, int column, int length) {
            Position = position;
            Line = line;
            Column = column;
            Length = length;
        }
    }

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

    /// <summary>
    /// Represents default debugger process
    /// </summary>
    internal class DebugerProcess {

        Process process = null;
        NamedPipeClientStream pipeClient;
        StreamString stringWriter;

        public void Start() {
            process = Process.Start(new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "Scripting.Debug.Debugger.exe"
            });

            pipeClient = new NamedPipeClientStream(".", "ssharp_dbg", PipeDirection.InOut, PipeOptions.None);
            pipeClient.Connect(30 * 1000);

            stringWriter = new StreamString(pipeClient);
            if (stringWriter.ReadString() == "S# Debugger") {

            } else {
                throw new NotSupportedException("Debugger validation token failed");
            }
        }

        public void Stop() {
            pipeClient.Close();

            if (!process.HasExited)
                process.Kill();
            process.WaitForExit();
        }

        public bool ProcessStep(string code, string text) {

            try {
                stringWriter.WriteString(code);
                stringWriter.WriteString(text);

                string reply = stringWriter.ReadString();

                if (reply == "Next")
                    return true;
            }
            catch {
                return false;
            }

            return false;
        }

        public class StreamString {
            private Stream ioStream;
            private UnicodeEncoding streamEncoding;

            public StreamString(Stream ioStream) {
                this.ioStream = ioStream;
                streamEncoding = new UnicodeEncoding();
            }

            public string ReadString() {
                int len;
                len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                return streamEncoding.GetString(inBuffer);
            }

            public int WriteString(string outString) {
                byte[] outBuffer = streamEncoding.GetBytes(outString);
                int len = outBuffer.Length;
                if (len > UInt16.MaxValue) {
                    len = (int)UInt16.MaxValue;
                }
                ioStream.WriteByte((byte)(len / 256));
                ioStream.WriteByte((byte)(len & 255));
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();

                return outBuffer.Length + 2;
            }
        }

    }

    public class DebugBreakPointEventArgs : EventArgs {
        public Script Script { get; private set; }
        public DebugLocation Location { get; private set; }

        public DebugBreakPointEventArgs(Script script, DebugLocation location) {
            Script = script;
            Location = location;
        }
    }

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