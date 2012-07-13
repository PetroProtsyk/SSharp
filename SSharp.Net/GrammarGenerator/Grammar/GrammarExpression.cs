using System;
using System.Collections.Generic;

namespace Scripting.SSharp.Parser
{
  internal class GrammarExpression : GrammarTerm
  {
    #region Constructor
    public GrammarExpression(GrammarTerm element)
      : base(null)
    {
      Data = new BnfExpressionData();
      Data.Add(new BnfTermList() { element });
    }
    #endregion

    #region Properties
    public BnfExpressionData Data { get; private set; }
    #endregion
  }
}
