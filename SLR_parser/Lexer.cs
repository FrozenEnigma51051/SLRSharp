using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLR_parser
{
    class Lexer
    {
        public string test;
        public Dictionary<string, Regex> dict = new Dictionary<string, Regex>();
        public List<List<string>> tokens = new List<List<string>>();
        public List<string> parsedInput = new List<string>();
        public string input;
        public string grammar;

        public Lexer(string input, string grammar)
        {
            this.input = input;
            this.grammar = grammar;
        }

        public List<List<string>> LexAnalysis()
        {
            Debug.WriteLine("Input In lexer", input);
            Debug.WriteLine("Grammar In lexer", grammar);

            // Adding dictionary with identifier support
            dict.Add("identifier", new Regex(@"^[a-z]+$")); // Common identifier pattern
            dict.Add("integer", new Regex(@"^[0-9]+$"));
            dict.Add("operator", new Regex(@"^[\+\-\*]$"));
            dict.Add("parenthesis", new Regex(@"^[\(\)]$"));

            // Splitting the input
            parsedInput = input.Split(' ').ToList<string>();

            // Looping the parsedInput to match the tokens with dictionary
            foreach (string parsedChar in parsedInput)
            {
                Debug.WriteLine(parsedChar + "\n");

                // Check for identifier first (since it's the most specific pattern)
                if (dict["identifier"].IsMatch(parsedChar))
                {
                    List<string> list = new List<string>
                    {
                        "Identifier",
                        parsedChar
                    };
                    tokens.Add(list);
                }
                // Then check for integer
                else if (dict["integer"].IsMatch(parsedChar))
                {
                    List<string> list = new List<string>
                    {
                        "Integer",
                        parsedChar
                    };
                    tokens.Add(list);
                }
                // Then operator
                else if (dict["operator"].IsMatch(parsedChar))
                {
                    List<string> list = new List<string>
                    {
                        "Operator",
                        parsedChar
                    };
                    tokens.Add(list);
                }
                // Finally parenthesis
                else if (dict["parenthesis"].IsMatch(parsedChar))
                {
                    List<string> list = new List<string>
                    {
                        "Parenthesis",
                        parsedChar
                    };
                    tokens.Add(list);
                }
            }
            return tokens;
        }
    }
}