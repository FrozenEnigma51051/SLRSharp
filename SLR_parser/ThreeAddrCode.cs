using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLR_parser
{
    class ThreeAddrCode
    {
        private int tempCount = 0;
        private readonly Regex identifierRegex = new Regex(@"^[a-z]+$");
        private readonly HashSet<string> baseIndicators = new HashSet<string> { "o", "d" }; // Add base indicators

        public string GenerateCode(Node root)
        {
            StringBuilder sb = new StringBuilder();
            string result = ProcessNode(root, sb);

            // For simple inputs without operators
            if (string.IsNullOrEmpty(sb.ToString().Trim()) && !string.IsNullOrEmpty(result))
            {
                string temp = GetTemp();
                sb.AppendLine($"{temp} = {result}");
            }

            return sb.ToString();
        }

        private string ProcessNode(Node node, StringBuilder sb)
        {
            if (node == null) return "";

            // If it's a leaf node (operand)
            if (node.Left == null && node.Right == null)
            {
                // For simple values like numbers or identifiers
                if (IsNumber(node.Value) || IsIdentifier(node.Value) || IsBaseIndicator(node.Value))
                {
                    return node.Value;
                }
            }

            // Process operator node
            if (IsOperator(node.Value))
            {
                string leftOperand = ProcessNode(node.Left, sb);
                string rightOperand = ProcessNode(node.Right, sb);
                string temp = GetTemp();
                sb.AppendLine($"{temp} = {leftOperand} {node.Value} {rightOperand}");
                return temp;
            }

            // Handle number with base indicator (like "5 o")
            if (node.Left != null && node.Right != null)
            {
                string leftValue = ProcessNode(node.Left, sb);
                string rightValue = ProcessNode(node.Right, sb);

                // If right value is a base indicator, combine without generating temp
                if (IsBaseIndicator(rightValue))
                {
                    return $"{leftValue}{rightValue}";
                }
                else if (IsNumber(leftValue) && IsIdentifier(rightValue))
                {
                    return $"{leftValue}{rightValue}";
                }
                else
                {
                    string temp = GetTemp();
                    sb.AppendLine($"{temp} = {leftValue} {rightValue}");
                    return temp;
                }
            }

            return node.Value;
        }

        private string GetTemp()
        {
            return $"t{tempCount++}";
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private bool IsIdentifier(string token)
        {
            return identifierRegex.IsMatch(token);
        }

        private bool IsNumber(string token)
        {
            return int.TryParse(token, out _);
        }

        private bool IsBaseIndicator(string token)
        {
            return baseIndicators.Contains(token);
        }
    }
}