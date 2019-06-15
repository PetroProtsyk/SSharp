using System;

namespace UnitTests
{
    internal class ExpectedExceptionAttribute : Attribute
    {
        public ExpectedExceptionAttribute(Type t) { }
    }
}