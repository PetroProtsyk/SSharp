using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Runtime.Modules {
    /// <summary>
    /// Provides content of named script snippets
    /// </summary>
    public interface IScriptModuleManager {

        void RegisterModule(string moduleName, string snippet);

        string GetModule(string moduleName);

    }
}
