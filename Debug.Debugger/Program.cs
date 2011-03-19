using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO.Pipes;
using System.IO;

namespace Scripting.Debug.Debugger {
    class Program {
        static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("S# Debugger " + Assembly.GetEntryAssembly().FullName);
            Console.WriteLine("(c) 2011");
            Console.ResetColor();

            NamedPipeServerStream pipeServer =
                        new NamedPipeServerStream("ssharp_dbg", PipeDirection.InOut, 1);
            pipeServer.WaitForConnection();

            string old_code=null;

            try {
                StreamString stringProtocol = new StreamString(pipeServer);
                stringProtocol.WriteString("S# Debugger");

                while (true) {

                    string code = stringProtocol.ReadString();
                    if (!string.Equals(code,old_code)) {
                        old_code=code;

                        Console.ForegroundColor = ConsoleColor.Yellow;

                        string[] lines = code.Split(new char[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries);
                        int line = 0;
                        foreach (string s in lines) {
                            Console.WriteLine(string.Format("{0}: {1}",line++,s));
                        }
                        Console.ResetColor();
                    }

                    string text = stringProtocol.ReadString();

                    Console.Write('\t');
                    Console.WriteLine(text);
                    Console.ReadKey(true);

                    stringProtocol.WriteString("Next");
                }
            }
            catch (IOException e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.ResetColor();
            }
            catch {
            }
            finally {
                pipeServer.Close();
            }
        }
    }

    public class StreamString {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream) {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString() {
            int len = 0;

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
