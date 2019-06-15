using System;
using Xunit;
using Scripting.SSharp.Runtime;

namespace UnitTests
{
  public class BaseAssemblyManagerTest
  {    
    [Fact]
    public void AddTypeRequiresAlias()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().AddType(null, typeof(string)));
    }

    [Fact]
    public void AddTypeRequiresType()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().AddType("type", null));
    }
        
    [Fact]
    public void HasNamespaceRequiresName()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().HasNamespace(null));
    }

    [Fact]
    public void InitializeRequiresConfiguration()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().Initialize(null));
    }

    [Fact]
    public void GetTypeRequiresName()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().GetType(null));
    }

    [Fact]
    public void HasTypeRequiresName()
    {
     Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().HasType(null));
    }

    [Fact]
    public void RemoveAssemblyRequiresAssembly()
    {
      Assert.Throws<ArgumentNullException>(() => new BaseAssemblyManager().RemoveAssembly(null));
    }
  }
}
