using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Parser
{
  /// <summary>
  /// System.IO.File.WriteAllText("d:\\code.cs", Scripting.SSharp.Parser.FastParserGenerator.Build());
  /// </summary>
  public static class FastParserGenerator
  {
    public static string Build()
    {
      ParserData g = new GrammarDataBuilder(Grammar.CreateScriptGrammar(false)).Build();
      if (2 != g.Errors.Count)
      {
        Console.WriteLine("Grammar changed!");
        return null;
      }

      StringBuilder code = new StringBuilder();

      string template = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scripting.SSharp.Parser.Ast;

namespace Scripting.SSharp.Parser.FastGrammar
{
  public partial class LRParser
  {
    public LRParser()
    {
    
//@@@CodeHere@@@      
    }
  }
}
";
      Dictionary<long, int> stateIndex = new Dictionary<long, int>();

      int startIndex = -1, endIndex = -1, index = 0;

      foreach (var state in g.States)
      {
        if (state == g.InitialState) startIndex = index;
        if (state == g.FinalState) endIndex = index;

        code.AppendLine("     ParserState state_" + index + " = new ParserState(" + state.ID + ");");
        stateIndex.Add(state.ID, index);
        index++;
      }


      Dictionary<int, GrammarTerm> Terms = new Dictionary<int, GrammarTerm>();

      foreach (var state in g.States)
      {
        foreach (var ak in state.Actions)
        {
          if (ak.Value.NonTerminal != null && !Terms.ContainsKey(ak.Value.NonTerminal.id))
          {
            string ncode = string.Format("new NonTerminal(\"{0}\", typeof({1}),\"{2}\",{3},{4})",
              NormalizeName(ak.Value.NonTerminal.Name),
              ak.Value.NonTerminal.NodeType == null ? "AstNode" : ak.Value.NonTerminal.NodeType.FullName,
              NormalizeName(ak.Value.NonTerminal.Key),
              GetOptions(ak.Value.NonTerminal.Options),
              ak.Value.NonTerminal.id);

            Terms.Add(ak.Value.NonTerminal.id, ak.Value.NonTerminal);

            code.AppendLine(string.Format("NonTerminal Terms_{0} = {1};", ak.Value.NonTerminal.id, ncode));

            //code.AppendLine(string.Format("Terms.Add({0}, {1});", ak.Value.NonTerminal.id, ncode));
          }
        }
      }

      index = 0;
      code.AppendLine("ActionsRecord fd;");

      Dictionary<string, int> aRecords = new Dictionary<string, int>();
      int aIndex = 0;

      foreach (var state in g.States)
      {
        int sIndex = stateIndex[state.ID];
        string stateName = string.Format("state_{0}", sIndex);
        code.AppendLine("fd = " +stateName + ".Actions;");

        foreach (var ak in state.Actions)
        {
          string ncode = ak.Value.NonTerminal == null ? "null" :
          string.Format("Terms_{0}", ak.Value.NonTerminal.id);

          //string.Format("Terms[{0}]", ak.Value.NonTerminal.id);

          string acode = string.Format("new ActionRecord({0},{1},{2},{3})",
          "ParserActionType." + ak.Value.ActionType.ToString(),
          ak.Value.NewState == null ? "null" : "state_" + stateIndex[ak.Value.NewState.ID] + "",
          ncode,
          ak.Value.PopCount);

          string aName = "ar_";

          if (aRecords.ContainsKey(acode))
          {
            aName += aRecords[acode];
          }
          else
          {
            aRecords.Add(acode, aIndex);
            code.AppendLine("ActionRecord ar_" + aIndex + "=" + acode + ";");
            aName = "ar_" + aIndex;
            aIndex++;
          }

          code.AppendLine(string.Format("fd.Add(\"{0}\", {1});", NormalizeName(ak.Key), aName));
        }

        //code.AppendLine("fd = new ActionsRecord(");
        //code.AppendLine("new KeyValuePair<int, KeyValuePair<string, ActionRecord>[]>[]{");

        //int cLength = -1;
        //foreach (var ak in state.Actions.OrderBy(f=>f.Key.Length))
        //{
        //  if (cLength != ak.Key.Length)
        //  {
        //    if (cLength > 0) code.AppendLine("}),");
        //    cLength = ak.Key.Length;
        //    code.AppendLine("new KeyValuePair<int, KeyValuePair<string, ActionRecord>[]>(" + cLength + ", new KeyValuePair<string, ActionRecord>[]{ ");
        //  }

        //  string ncode = ak.Value.NonTerminal == null ? "null" :
        //  string.Format("Terms_{0}", ak.Value.NonTerminal.id);

        //  string acode = string.Format("new ActionRecord({0},{1},{2},{3})",
        //  "ParserActionType." + ak.Value.ActionType.ToString(),
        //  ak.Value.NewState == null ? "null" : "States[" + stateIndex[ak.Value.NewState.ID] + "]",
        //  ncode,
        //  ak.Value.PopCount);

        //  code.AppendLine(string.Format("new KeyValuePair<string, ActionRecord>(\"{0}\", {1}),", NormalizeName(ak.Key), acode));
        //}

        //code.AppendLine("})      });");
        //code.AppendLine("ts.Actions = fd;");

        index++;
      }

      code.AppendLine("InitialState = state_" + startIndex + ";");
      code.AppendLine("FinalState = state_" + endIndex + ";");

      code.AppendLine("TerminalList tl;");
      code.AppendLine("NumberLiteral n = new NumberLiteral();");
      code.AppendLine("IdentifierTerminal v = new IdentifierTerminal();");
      code.AppendLine("ITerminal s = new StringLiteral();");
      code.AppendLine("ITerminal Comment = new CommentTerminal(\"Comment\", \"/*\", \"*/\");");
      code.AppendLine("ITerminal LineComment = new CommentTerminal(\"LineComment\", \"//\", \"\\n\");");
      code.AppendLine();
      foreach (var t in g.TerminalsLookup)
      {
        code.Append("tl = new TerminalList(){");
        foreach (var tm in t.Value)
        {
          if (tm is NumberLiteral) code.Append("n,");
          else
            if (tm is IdentifierTerminal) code.Append("v,");
            else
              if (tm is StringLiteral) code.Append("s,");
              else
                if (tm is CommentTerminal)
                {
                  CommentTerminal cm = (CommentTerminal)tm;
                  if (cm.Name == "Comment") code.Append("Comment,");
                  else
                    code.Append("LineComment,");
                }
                else
                  code.Append("SymbolTerminal.GetSymbol(\"" + ((SymbolTerminal)tm).Symbol + "\"),");
        }
        code.AppendLine("};");
        code.AppendLine("Scanner.TerminalsLookup.Add((char)" + (int)t.Key + ",tl);");
        //tl.Sort(Terminal.ByPriorityReverse)
      }

      code.AppendLine("SymbolTerminal.RegisterPunctuation(\"(\", \")\", \"[\", \"]\", \"{\", \"}\", \",\", \";\");");

      code.AppendLine("SymbolTerminal.RegisterOperators(1, \"=\", \"+=\", \"-=\", \":=\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(2, \"|\", \"||\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(3, \"&\", \"&&\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(4, \"==\", \"!=\", \">\", \"<\", \">=\", \"<=\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(5, \"is\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(6, \"+\", \"-\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(7, \"*\", \"/\", \"%\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(8, Associativity.Right, \"^\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(9, \"~\", \"!\", \"$\", \"++\", \"--\");");
      code.AppendLine("SymbolTerminal.RegisterOperators(10, \".\");");

      code.AppendLine("SymbolTerminal.ClearSymbols();");

      return template.Replace("//@@@CodeHere@@@", code.ToString());
    }

    private static string GetOptions(TermOptions termOptions)
    {
      return "TermOptions." + Enum.Format(typeof(TermOptions), termOptions, "g");
    }

    public static string NormalizeName(string name)
    {
      return name.Replace("\\", "\\\\").Replace("\b", "\\b");
    }
  }
}
