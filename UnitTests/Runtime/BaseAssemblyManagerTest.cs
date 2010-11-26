using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.SSharp.Runtime;
using Scripting.SSharp.Runtime.Promotion;

namespace Scripting.SSharp.UnitTests.Runtime
{
  [TestClass]
  public class BaseAssemblyManagerTest
  {    
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddTypeRequiresAlias()
    {
      new BaseAssemblyManager().AddType(null, typeof(string));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddTypeRequiresType()
    {
      new BaseAssemblyManager().AddType("type", null);
    }
        
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void HasNamespaceRequiresName()
    {
      new BaseAssemblyManager().HasNamespace(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void InitializeRequiresConfiguration()
    {
      new BaseAssemblyManager().Initialize(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetTypeRequiresName()
    {
      new BaseAssemblyManager().GetType(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void HasTypeRequiresName()
    {
      new BaseAssemblyManager().HasType(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RemoveAssemblyRequiresAssembly()
    {
      new BaseAssemblyManager().RemoveAssembly(null);
    }
  }
}
