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

using System.Diagnostics;
using Scripting.SSharp.Runtime.Operators;
using Scripting.SSharp.Runtime;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Assignment Expression
  /// </summary>
  internal class ScriptAssignExpr : ScriptExpr
  {
    private readonly ScriptQualifiedName _nameExpr;
    private readonly ScriptExpr _rightExpr;
    private readonly string _oper;
    private readonly AssignmentFunction _assignOperation;

    public ScriptQualifiedName LeftExpression
    {
      get { return _nameExpr; }
    }

    public ScriptExpr RightExpression
    {
      get { return _rightExpr; }
    }

    public string Symbol
    {
      get { return _oper; }
    }

    private delegate object AssignmentFunction(IScriptContext context);

    public ScriptAssignExpr(AstNodeArgs args)
        : base(args)
    {
      var varExpr = args.ChildNodes[0] as ScriptVarExpr;
      if (varExpr != null)
      {
        var id = varExpr.ChildNodes[1] as TokenAst;
        _nameExpr = new ScriptQualifiedName(new AstNodeArgs(args.Term, args.Span,
           new AstNodeList { id, new AstNode(new AstNodeArgs(args.Term, new SourceSpan(new SourceLocation(0, 0, 0), 0), new AstNodeList())) })) { IsVar = true };
      }
      else
      {
        _nameExpr = (ScriptQualifiedName)args.ChildNodes[0];
      }

      _oper = ((TokenAst)args.ChildNodes[1]).Text;
      if (args.ChildNodes.Count == 3)
        _rightExpr = (ScriptExpr)args.ChildNodes[2];

      Debug.Assert(_oper == "=" || _oper == ":=" || _oper == "+=" || _oper == "-=" || _oper == "++" || _oper == "--" || _oper == ":=");
            
      switch (_oper)
      {
        case "=":
          _assignOperation = Assign;
          break;
        case ":=":
          _assignOperation = AssignEx;
          break;
        case "++":
          _assignOperation = PlusPlus;
          break;
        case "--":
          _assignOperation = MinusMinus;
          break;
        case "+=":
          _assignOperation = PlusEqual;
          break;
        case "-=":
          _assignOperation = MinusEqual;
          break;
        default:
          throw new System.InvalidOperationException(string.Format(Strings.AssignmentOperatorNotSupported, _oper));
      }

      _minus = RuntimeHost.GetBinaryOperator(OperatorCodes.Sub);
      _plus = RuntimeHost.GetBinaryOperator(OperatorCodes.Add);

      if (_plus == null || _minus == null)
        throw new ScriptRuntimeException(string.Format(Strings.MissingOperatorError, "+,-"));
    }

    public override void Evaluate(IScriptContext context)
    {
      if (_rightExpr != null)
      {
        _rightExpr.Evaluate(context);
      }
 
      context.Result = _assignOperation(context);
    }

    #region Operators
    private readonly IOperator _minus;
    private readonly IOperator _plus;
    #endregion

    #region Assignments
    private object MinusEqual(IScriptContext context)
    {
      object rez = context.Result;
      _nameExpr.Evaluate(context);

      var rezName = context.Result;

      var handling = OnHandleOperator(this, context, "-=", rezName, rez);
      rez = handling.Cancel ? handling.Result : _minus.Evaluate(rezName, rez);

      //if (!(rezName is EventInfo))
      //{
      //  rez = RuntimeHost.GetBinaryOperator("-").Evaluate(rezName, rez);
      //}
      //else
      //{
      //  rez = new RemoveDelegate((IInvokable)rez);
      //}
      _nameExpr.Assign(rez, context);
      return rez;
    }

    private object PlusEqual(IScriptContext context)
    {
      var rez = context.Result;

      _nameExpr.Evaluate(context);
      var rezName = context.Result;

      var handling = OnHandleOperator(this, context, "+=", rezName, rez);
      rez = handling.Cancel ? handling.Result : _plus.Evaluate(rezName, rez);

      //TODO: Events!
      //if (!(rezName is EventInfo))
      //{
      //  rez = RuntimeHost.GetBinaryOperator("+").Evaluate(rezName, rez);
      //}

      _nameExpr.Assign(rez, context);
      return rez;
    }

    private object MinusMinus(IScriptContext context)
    {
      _nameExpr.Evaluate(context);
      var rez = _minus.Evaluate(context.Result, 1);

      _nameExpr.Assign(rez, context);
      return rez;
    }

    private object PlusPlus(IScriptContext context)
    {
      _nameExpr.Evaluate(context);
      var rez = _plus.Evaluate(context.Result, 1);
                  
      _nameExpr.Assign(rez, context);
      return rez;
    }

    private object AssignEx(IScriptContext context)
    {
      var rez = context.Result;

      _nameExpr.Evaluate(context);

      var handling = OnHandleOperator(this, context, ":=", context.Result, rez);
      if (handling.Cancel)
        rez = handling.Result;
      else
      {
        ((ISupportAssign)rez).AssignTo(context.Result);
        rez = context.Result;
      }

      return rez;
    }

    private object Assign(IScriptContext context)
    {
      var rez = context.Result;
      _nameExpr.Assign(rez, context);
      return rez;
    }
    #endregion
  }
}
