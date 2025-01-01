using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLR_parser
{
    public class Node
    {
        public string Value { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public string Annotation { get; set; }
        public int NodeId { get; set; }
        private static int nextId = 1;

        public Node(string value)
        {
            Value = value;
            NodeId = nextId++;
            Annotation = GenerateAnnotation(value);
        }

        private string GenerateAnnotation(string value)
        {
            if (IsOperator(value))
            {
                return $"{value}.val";
            }
            else if (int.TryParse(value, out _))
            {
                return $"num{NodeId}.val";
            }
            else if (IsIdentifier(value))
            {
                return $"id{NodeId}.val";
            }
            else
            {
                return $"{value}.val";
            }
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private bool IsIdentifier(string token)
        {
            return new Regex(@"^[a-z]+$").IsMatch(token);
        }
    }

    public class MainSyntaxTree
    {
        private StringBuilder annotationOutput;
        private readonly Regex identifierRegex;

        public MainSyntaxTree()
        {
            annotationOutput = new StringBuilder();
            identifierRegex = new Regex(@"^[a-z]+$");
        }

        public Node getRoot(string[] postfixExpression)
        {
            Stack<Node> stack = new Stack<Node>();

            foreach (string token in postfixExpression)
            {
                if (IsOperator(token))
                {
                    Node node = new Node(token);
                    node.Right = stack.Pop();
                    node.Left = stack.Pop();
                    GenerateAnnotationRule(node);
                    stack.Push(node);
                }
                else if (token != "(" && token != ")")
                {
                    Node node = new Node(token);
                    if (int.TryParse(token, out int value))
                    {
                        annotationOutput.AppendLine($"{node.Annotation} = {value}");
                    }
                    else if (IsIdentifier(token))
                    {
                        annotationOutput.AppendLine($"{node.Annotation} = {token}");
                    }
                    stack.Push(node);
                }
            }

            return stack.Count > 0 ? stack.Pop() : null;
        }

        private void GenerateAnnotationRule(Node node)
        {
            if (node == null || !IsOperator(node.Value)) return;

            string rule = $"{node.Annotation} = {node.Left.Annotation} {node.Value} {node.Right.Annotation}";
            annotationOutput.AppendLine(rule);
        }

        public string GenerateAnnotatedSyntaxTree(string[] postfixExpression)
        {
            annotationOutput.Clear();
            Node root = getRoot(postfixExpression);

            StringBuilder output = new StringBuilder();
            output.AppendLine("Syntax Tree with Annotations:");
            output.AppendLine("----------------------------");

            output.Append(PrintAnnotatedTree(root));

            output.AppendLine("\nSemantic Rules:");
            output.AppendLine("---------------");
            output.Append(annotationOutput.ToString());

            return output.ToString();
        }

        public string PrintAnnotatedTree(Node root)
        {
            if (root == null) return "";

            StringBuilder output = new StringBuilder();
            PrintAnnotatedTreeRecursive(root, "", true, output);
            return output.ToString();
        }

        private void PrintAnnotatedTreeRecursive(Node node, string indent, bool isLast, StringBuilder output)
        {
            if (node == null) return;

            output.Append(indent);
            output.Append(isLast ? "└── " : "├── ");
            output.AppendLine($"{node.Value} [{node.Annotation}]");

            string childIndent = indent + (isLast ? "    " : "│   ");

            if (node.Left != null)
            {
                PrintAnnotatedTreeRecursive(node.Left, childIndent, node.Right == null, output);
            }
            if (node.Right != null)
            {
                PrintAnnotatedTreeRecursive(node.Right, childIndent, true, output);
            }
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private bool IsIdentifier(string token)
        {
            return identifierRegex.IsMatch(token);
        }

        public string InfixToPostfix(string infix)
        {
            Console.WriteLine("INFIX: " + infix);
            Stack<char> stack = new Stack<char>();
            string postfix = "";

            for (int i = 0; i < infix.Length; i++)
            {
                char c = infix[i];

                if (char.IsDigit(c))
                {
                    while (i + 1 < infix.Length && char.IsDigit(infix[i + 1]))
                    {
                        postfix += c;
                        i++;
                        c = infix[i];
                    }
                    postfix += c + " ";
                }
                else if (char.IsLetter(c) && char.IsLower(c))
                {
                    while (i + 1 < infix.Length && char.IsLetter(infix[i + 1]) && char.IsLower(infix[i + 1]))
                    {
                        postfix += c;
                        i++;
                        c = infix[i];
                    }
                    postfix += c + " ";
                }
                else if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        postfix += stack.Pop() + " ";
                    }
                    if (stack.Count > 0) stack.Pop();
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
                {
                    while (stack.Count > 0 && GetPrecedence(c) <= GetPrecedence(stack.Peek()))
                    {
                        postfix += stack.Pop() + " ";
                    }
                    stack.Push(c);
                }
            }

            while (stack.Count > 0)
            {
                postfix += stack.Pop() + " ";
            }

            Console.WriteLine("POSTFIX: ");
            Console.WriteLine(postfix.ToString());
            return postfix;
        }

        public int GetPrecedence(char c)
        {
            if (c == '+' || c == '-')
            {
                return 1;
            }
            else if (c == '*' || c == '/')
            {
                return 2;
            }
            else if (c == '^')
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }
}