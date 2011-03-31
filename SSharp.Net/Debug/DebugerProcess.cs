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
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Scripting.SSharp.Debug {
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
                FileName = Path.Combine(
                    Path.GetDirectoryName( typeof(DebugerProcess).Assembly.Location),
                    "Scripting.Debugger.exe")
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
}
#endif