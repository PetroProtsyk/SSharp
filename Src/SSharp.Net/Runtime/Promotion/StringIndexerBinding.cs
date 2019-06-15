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
