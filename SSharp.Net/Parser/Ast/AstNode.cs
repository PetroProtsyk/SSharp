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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Scripting.SSharp.Parser.Ast
{
  /// <summary>
  /// Base node for representing abstract syntax tree
  /// </summary>
  internal class AstNode
  {
    #region Constructor
    public AstNode(AstNodeArgs args)
    {
      Term = args.Term;
      Span = args.Span;
      _childNodesReadonly = new ReadOnlyAstNodeList(_childNodes);

      ReplaceChildNodes(args.ChildNodes);
    }
    #endregion

    #region Fields
    /// <summary>
    /// NOTE: This will should be only accessed by LRParser
    /// </summary>
    internal readonly AstNodeList _childNodes = new AstNodeList();
    private readonly ReadOnlyAstNodeList _childNodesReadonly;
    #endregion

    #region Properties
    public SourceLocation Location
    {
      get { return Span.Start; }
    }

    internal IGrammarTerm Term
    {
      get;
      private set;
    }

    public SourceSpan Span
    {
      get;
      private set;
    }

    public AstNode Parent
    {
      get;
      internal set;
    }

    public ReadOnlyAstNodeList ChildNodes
    {
      get { return _childNodesReadonly; }
    }

    #endregion

    #region Methods
    public void AddChild(AstNode child)
    {
      if (child == null) return;
      child.Parent = this;
      _childNodes.Add(child);
    }

    public void ReplaceChildNodes(IEnumerable<AstNode> nodes) {
        _childNodes.Clear();
        if (nodes == null) return;

        foreach (AstNode child in nodes) {
            if (child != null && !child.Term.IsSet(TermOptions.IsPunctuation))
                AddChild(child);
        }

        OnNodesReplaced();
    }

    protected virtual void OnNodesReplaced() {
    }
    #endregion

    #region Visitors, Iterators
    public virtual void AcceptVisitor(IAstVisitor visitor)
    {
        if (visitor.BeginVisit(this)) {
            foreach (AstNode node in ChildNodes)
                node.AcceptVisitor(visitor);
        }
        visitor.EndVisit(this);
    }
    #endregion
  }
}
