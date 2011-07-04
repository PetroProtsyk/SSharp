using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Runtime.Modules {

    public class ScriptModuleManager : IScriptModuleManager {

        private Dictionary<string, string> snippets = new Dictionary<string, string>();

        public void RegisterModule(string moduleName, string snippet) {
            snippets.Add(moduleName, snippet);
        }

        public string GetModule(string moduleName) {
            string result;
            if (!snippets.TryGetValue(moduleName, out result))
            {
                throw new ScriptRuntimeException(string.Format("Module '{0}' not found", moduleName));
            }

            return result;
        }
    }
}
