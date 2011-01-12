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

using System;

namespace Scripting.SSharp.Parser.Ast
{
  using Runtime.Operators;
  using Runtime;
  
  /// <summary>
  /// 
  /// </summary>
  internal class ScriptTypeConvertExpr : ScriptExpr
  {
    private readonly ScriptExpr _expr;
    private readonly ScriptExpr _typeExpr;
    private readonly IOperator _operator;

    public ScriptExpr Expression
    {
      get
      {
        return _expr;
      }
    }

    public ScriptExpr TypeExpression
    {
      get
      {
        return _typeExpr;
      }
    }

    protected internal override bool IsConst {
        get {
            if (TypeExpression == null) {
                if (Expression != null) return Expression.IsConst;
                return false;
            }

            return false;
        }
    }

    public ScriptTypeConvertExpr(AstNodeArgs args)
        : base(args)
    {
      if (ChildNodes.Count == 2)
      {
        if (args.ChildNodes[0] is ScriptExpr &&
            !(args.ChildNodes[1] is ScriptExpr))
        {
          // ( Expr )
          _expr = args.ChildNodes[0] as ScriptExpr;
          _typeExpr = null;
        }
        else
        {
          //(Type) Expr
          _typeExpr = args.ChildNodes[0] as ScriptExpr;
          _expr = args.ChildNodes[1] as ScriptExpr;         
        }
      }
      else
      {
        throw new ScriptSyntaxErrorException(Strings.GrammarErrorExceptionMessage);
      }

      _operator = RuntimeHost.GetBinaryOperator("+");
      if (_operator == null)
        throw new ScriptRuntimeException(string.Format(Strings.MissingOperatorError, "+"));
    }

    public override void Evaluate(IScriptContext context)
    {
      // ( Expr )
      if (_typeExpr == null)
      {
        _expr.Evaluate(context);
      }
      // (Type) Expr
      else
      {
        _typeExpr.Evaluate(context);

        var type = context.Result as Type;
        if (type == null)
        {
          //NOTE: Handling special case of unary minus operator:
          //      (3+2)-2;
          var unary = _expr as ScriptUnaryExpr;

          if (unary == null || unary.OperationSymbol != "-")
            throw new ScriptSyntaxErrorException(Strings.TypeExpressionIsNotSyntacticallyCorrect);

          //NOTE: expr + (unary expr)
          var left = context.Result;
          unary.Evaluate(context);
          context.Result = _operator.Evaluate(left, context.Result);
          return;
        }

        _expr.Evaluate(context);
        context.Result = RuntimeHost.Binder.ConvertTo(context.Result, type);
      }

    }
  }
}