using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Runtime.Promotion {
    /// <summary>
    /// For some reason it is not possible to bind to get_Item or Get indexer for string via reflection
    /// This specific binding should do the work
    /// </summary>
    internal class StringIndexerBinding : IBinding {

        private char Value { get; set; }

        public StringIndexerBinding(string value, int index) {
            Value = value[index];
        }
        
        public bool CanInvoke() {
            return true;
        }

        public object Invoke(IScriptContext context, object[] args) {
            return Value;
        }
    }
}
