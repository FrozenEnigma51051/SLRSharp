using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLR_parser
{
    public class SemanticAnalyzer
    {
        private readonly IDictionary<String, List<List<String>>> grammarRules;
        private readonly List<string> terminals;
        private readonly List<string> nonTerminals;
        private IDictionary<String, List<String>> semanticRules;

        public SemanticAnalyzer(Preprocessor preprocessor)
        {
            this.grammarRules = preprocessor.Rules;
            this.terminals = preprocessor.terminals;
            this.nonTerminals = preprocessor.nonterminals;
            this.semanticRules = new Dictionary<String, List<String>>();
        }

        public IDictionary<String, List<String>> GenerateSemanticRules()
        {
            int ruleCounter = 1;

            foreach (var rule in grammarRules)
            {
                String nonTerminal = rule.Key;
                List<List<String>> productions = rule.Value;

                foreach (var production in productions)
                {
                    List<String> semantics = new List<String>();

                    if (IsArithmeticProduction(production))
                    {
                        semantics.Add(GenerateArithmeticRule(nonTerminal, production, ruleCounter));
                    }
                    else if (IsAssignmentProduction(production))
                    {
                        semantics.Add(GenerateAssignmentRule(nonTerminal, production, ruleCounter));
                    }
                    else if (IsTerminalProduction(production))
                    {
                        semantics.Add(GenerateTerminalRule(nonTerminal, production, ruleCounter));
                    }
                    else
                    {
                        semantics.Add(GenerateDefaultRule(nonTerminal, production, ruleCounter));
                    }

                    if (semanticRules.ContainsKey($"Rule{ruleCounter}"))
                    {
                        semanticRules[$"Rule{ruleCounter}"].AddRange(semantics);
                    }
                    else
                    {
                        semanticRules.Add($"Rule{ruleCounter}", semantics);
                    }

                    ruleCounter++;
                }
            }

            return semanticRules;
        }

        private bool IsArithmeticProduction(List<String> production)
        {
            return production.Any(symbol => symbol == "+" || symbol == "-" || symbol == "*" || symbol == "/");
        }

        private bool IsAssignmentProduction(List<String> production)
        {
            return production.Any(symbol => symbol == "=");
        }

        private bool IsTerminalProduction(List<String> production)
        {
            return production.All(symbol => terminals.Contains(symbol));
        }

        private String GenerateArithmeticRule(String nonTerminal, List<String> production, int ruleNumber)
        {
            StringBuilder rule = new StringBuilder();
            rule.Append($"{nonTerminal}.val = ");

            if (production.Count >= 3)
            {
                String operator_ = production.FirstOrDefault(p => p == "+" || p == "-" || p == "*" || p == "/");
                int opIndex = production.IndexOf(operator_);

                if (opIndex > 0 && opIndex < production.Count - 1)
                {
                    String leftSide = production[opIndex - 1];
                    String rightSide = production[opIndex + 1];

                    rule.Append($"{leftSide}.val {operator_} {rightSide}.val");
                }
            }

            return rule.ToString();
        }

        private String GenerateAssignmentRule(String nonTerminal, List<String> production, int ruleNumber)
        {
            return $"{nonTerminal}.val = {production.Last()}.val";
        }

        private String GenerateTerminalRule(String nonTerminal, List<String> production, int ruleNumber)
        {
            if (production.Count == 1)
            {
                return $"{nonTerminal}.val = {production[0]}.lexval";
            }
            return $"{nonTerminal}.val = {String.Join(" ", production)}.lexval";
        }

        private String GenerateDefaultRule(String nonTerminal, List<String> production, int ruleNumber)
        {
            if (production.Count == 1)
            {
                return $"{nonTerminal}.val = {production[0]}.val";
            }
            return $"{nonTerminal}.val = {String.Join(".val + ", production)}.val";
        }

        public string GetSemanticRulesText()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("Generated Semantic Rules:");
            output.AppendLine("------------------------");

            foreach (var rule in semanticRules)
            {
                output.AppendLine($"Rule {rule.Key}:");
                foreach (var semantic in rule.Value)
                {
                    output.AppendLine($"  {semantic}");
                }
                output.AppendLine();
            }

            return output.ToString();
        }

        // Keep this for console output if needed
        public void PrintSemanticRules()
        {
            Console.Write(GetSemanticRulesText());
        }
    }
}