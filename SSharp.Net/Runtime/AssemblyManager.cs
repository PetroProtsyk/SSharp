using System;
using Scripting.SSharp.Runtime.Configuration;
using System.Collections.Generic;
using Scripting.SSharp.Runtime.Promotion;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Scripting.SSharp.Runtime
{
    public class AssemblyManager : BaseAssemblyManager {
        #region Fields
        private Dictionary<Guid, Dictionary<string, List<MethodInfo>>> extensionMethods = new Dictionary<Guid, Dictionary<string, List<MethodInfo>>>();
        #endregion

        #region Initialization
        [Promote(false)]
        public override void Initialize(ScriptConfiguration configuration) {
            base.Initialize(configuration);

            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainAssemblyLoad;
        }
        #endregion

        #region Overrides
        protected override void LoadAssemblies() {
            //Load assemblies referenced in configuration
            base.LoadAssemblies();

            WorkingAssemblies.Clear();
            WorkingAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        }

        private void CurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args) {
            AddAssembly(args.LoadedAssembly);
        }

        protected override void RegisterType(Type type) {
            base.RegisterType(type);
            if (!(type.IsSealed && !type.IsGenericType && !type.IsNested)) return;

            IEnumerable<MethodInfo> members =
                type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false));

            foreach (MethodInfo extensionMethod in members) {
                Dictionary<string, List<MethodInfo>> methodCache;
                Type exType = extensionMethod.GetParameters().First().ParameterType;

                if (!extensionMethods.TryGetValue(exType.GUID, out methodCache)) {
                    methodCache = new Dictionary<string, List<MethodInfo>>();
                    extensionMethods.Add(exType.GUID, methodCache);
                }

                List<MethodInfo> methodList = new List<MethodInfo>();
                if (!methodCache.TryGetValue(extensionMethod.Name, out methodList)) {
                    methodList = new List<MethodInfo>();
                    methodCache.Add(extensionMethod.Name, methodList);
                }

                methodList.Add(extensionMethod);
            }
        }

        public override IEnumerable<MethodInfo> GetExtensionMethods(Type type) {
            Dictionary<string, List<MethodInfo>> methodCache;
            if (!extensionMethods.TryGetValue(type.GUID, out methodCache)) {
                return Enumerable.Empty<MethodInfo>();
            }

            return methodCache.Values.SelectMany(v=>v);
        }

        public override IEnumerable<MethodInfo> GetExtensionMethods(Type type, string methodName) {
            Dictionary<string, List<MethodInfo>> methodCache;
            List<MethodInfo> result;

            while (type != null) {                
                if (extensionMethods.TryGetValue(type.GUID, out methodCache) &&
                    methodCache.TryGetValue(methodName, out result)) {

                    return result;
                }

                type = type.BaseType;
            }

            return Enumerable.Empty<MethodInfo>();
        }
        #endregion

        #region IDisposable Members
        [Promote(false)]
        public override void Dispose() {
            AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomainAssemblyLoad;
            base.Dispose();
        }

        #endregion
    }
}
