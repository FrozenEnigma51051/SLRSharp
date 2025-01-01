using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLR_parser
{
    public class InputParsing
    {

        public List<String> ParsingStack = new List<string>();
        public List<String> InputTape = new List<string>();
        public List<List<String>> Table = new List<List<string>>();
        public List<List<String>> StackTable = new List<List<string>>();

        public IDictionary<int, List<List<String>>> numbered_rules = new Dictionary<int, List<List<String>>>();

        public InputParsing(List<List<String>> Table, IDictionary<int, List<List<String>>> numbered_rules)
        {
            if (Table == null || numbered_rules == null)
                throw new ArgumentNullException("Table and rules cannot be null");

            // Create a deep copy of the table to avoid modifying the original
            this.Table = Table.Select(row => new List<String>(row)).ToList();

            // Remove first column from each row
            foreach (var row in this.Table)
            {
                if (row.Count > 0)
                    row.RemoveAt(0);
            }

            this.numbered_rules = numbered_rules;
        }

        public List<List<String>> parse(String Input, List<String> cols)
        {
            if (string.IsNullOrEmpty(Input) || cols == null)
                throw new ArgumentNullException("Input and cols cannot be null");

            // Clear previous data
            InputTape.Clear();
            ParsingStack.Clear();
            StackTable.Clear();

            // Initialize input tape
            foreach (var a in Input.ToCharArray())
            {
                if (a != ' ')
                    InputTape.Add(a.ToString());
            }
            InputTape.Add("$");

            // Initialize parsing stack
            ParsingStack.Add("$_0");

            String Action = "";
            List<String> l1 = new List<string>();

            while (InputTape.Count > 0)
            {  // Changed from >= 0 to > 0
                try
                {
                    l1.Clear();
                    l1.Add(String.Join(" ", ParsingStack));
                    l1.Add(String.Join(" ", InputTape));
                    l1.Add(Action);

                    String stackTop = ParsingStack[ParsingStack.Count - 1];
                    String inputTop = InputTape[0];

                    int stateIndex = int.Parse(stackTop.Split('_')[1]);
                    int colIndex = cols.IndexOf(inputTop);

                    if (stateIndex >= Table.Count || colIndex < 0 || colIndex >= Table[stateIndex].Count)
                    {
                        Action = "Error";
                        break;
                    }

                    String TableLookup = Table[stateIndex][colIndex];

                    if (string.IsNullOrEmpty(TableLookup))
                    {
                        Action = "Error";
                        break;
                    }

                    if (TableLookup.Contains("S"))
                    {
                        InputTape.RemoveAt(0);
                        int shiftState = int.Parse(Regex.Match(TableLookup, @"\d+").Value);
                        ParsingStack.Add(inputTop + "_" + shiftState);
                        Action = "SHIFT-" + shiftState;
                    }
                    else if (TableLookup.Contains("R"))
                    {
                        int ruleNumber = int.Parse(Regex.Match(TableLookup, @"\d+").Value);

                        if (!numbered_rules.ContainsKey(ruleNumber))
                        {
                            Action = "Error: Invalid rule number";
                            break;
                        }

                        var rule = numbered_rules[ruleNumber];
                        int pops = rule[1].Count;

                        if (ParsingStack.Count < pops)
                        {
                            Action = "Error: Stack underflow";
                            break;
                        }

                        // Perform reduction
                        for (int pop = 0; pop < pops; pop++)
                        {
                            ParsingStack.RemoveAt(ParsingStack.Count - 1);
                        }

                        int currentState = int.Parse(ParsingStack[ParsingStack.Count - 1].Split('_')[1]);
                        int symbolIndex = cols.IndexOf(rule[0][0]);

                        if (symbolIndex < 0 || currentState >= Table.Count ||
                            symbolIndex >= Table[currentState].Count)
                        {
                            Action = "Error: Invalid state transition";
                            break;
                        }

                        String newState = Table[currentState][symbolIndex];
                        ParsingStack.Add(rule[0][0] + "_" + newState);
                        Action = "REDUCE " + rule[0][0] + " -> " + String.Join(" ", rule[1]);
                    }
                    else if (TableLookup.Contains("Accept"))
                    {
                        Action = "Accept";
                        break;
                    }
                    else
                    {
                        Action = "Error";
                        break;
                    }

                    StackTable.Add(new List<String>(l1));
                }
                catch (Exception ex)
                {
                    Action = "Error: " + ex.Message;
                    break;
                }
            }

            // Add final state
            l1.Clear();
            l1.Add(String.Join(" ", ParsingStack));
            l1.Add(String.Join(" ", InputTape));
            l1.Add(Action);
            StackTable.Add(new List<String>(l1));

            return StackTable;
        }
    }
}