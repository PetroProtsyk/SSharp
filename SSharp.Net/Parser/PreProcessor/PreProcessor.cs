using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.PreProcessor {
    internal static class PreProcessor {

        static Regex regex = new Regex(@"^\s*#include\s*<(?<moduleName>[0-9A-z_.]+)>\s*$", RegexOptions.Multiline);

        public static string Process(string code) {
            return regex.Replace(code, MatchEvaluator);
        }

        private static string MatchEvaluator(Match match) {
            return RuntimeHost.ModuleManager.GetModule(match.Groups["moduleName"].Value);
        }
    }
}
