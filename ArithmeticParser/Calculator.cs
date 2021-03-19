using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ArithmeticParser
{
    public static class Calculator
    {
        public enum Token
        {
            Plus,
            Minus,
            Div,
            Mult,
            Number,
            Bra,
            Ket
        };
        public static int GetPrior(Token t)
        {
            switch (t)
            {
                case Token.Bra:
                    return 0;
                case Token.Ket:
                    return 0;
                case Token.Plus:
                    return 1;
                case Token.Minus:
                    return 1;
                case Token.Div:
                    return 2;
                case Token.Mult:
                    return 2;
                default:
                    return 3;
                    
            }
        }

        static Token ParseValue(string s)
        {
            if (s == "(")
            {
                return Token.Bra;
            }if (s == ")")
            {
                return Token.Ket;
            }if (s == "+")
            {
                return Token.Plus;
            }if (s == "-")
            {
                return Token.Minus;
            }if (s == "*")
            {
                return Token.Mult;
            }if (s == "/")
            {
                return Token.Div;
            }
            return Token.Number;
        }

        static Func<int, int, int> ParseOp(string  s)
        {
            var t = ParseValue(s); 
            int Plus(int x, int y) => x + y;
            int Minus(int x, int y) => x - y;
            int Mult(int x, int y) => x * y;
            int Div(int x, int y) => x / y;
            switch (t)
            {
                case Token.Plus:
                    return Plus;
                case Token.Minus:
                    return Minus;
                case Token.Mult:
                    return Mult;
                case Token.Div:
                    return Div;
                default:
                    throw new DataException("Invalid operation in ParseOperation: " + s);
            }
        }
        public static List<string> SplitString(string s)
        {
            string[] res = Regex.Split(s, @"([\(])|([\)])|([\+\-\*\/])|(\d+)");
            var list = new List<string>();  
            foreach (string value in res)
            {
                if (value != "")
                {
                    // Console.WriteLine("I am parsing this: \'" + value + "\'");
                    list.Add(value);
                }
                
            }

            return list;
        }
        public static string Preprocess(string input)
        {
            var preprocessedInput = input.Replace(" ", string.Empty);
            return preprocessedInput;
        }

        public static List<string> ReversePolskiNotation(List<string> input)
        {
            var stack = new Stack<string>();
            var res = new List<string>();
            foreach (string stocken in input)
            {
                // Console.WriteLine("Handling: " + stocken);
                var t = ParseValue(stocken);
                if (t == Token.Number && !int.TryParse(stocken, out _))
                {
                    throw new DataException("This is not a number: " + stocken);
                }

                switch (t)
                {
                    case Token.Number:
                        res.Add(stocken);
                        break;
                    case Token.Bra:
                        stack.Push(stocken);
                        break;
                    case Token.Plus:
                    case Token.Minus:
                    case Token.Mult:
                    case Token.Div:
                        var myPr = GetPrior(t);
                        int hisPr;
                        if (stack.Count == 0)
                        {
                            stack.Push(stocken);
                            break;
                        }
                        do
                        {
                            var lastToken = ParseValue(stack.Peek());
                            if (lastToken == Token.Ket)
                            {
                                throw new DataException("Something is very wrong got bra while emptying stack");
                            }
                            if (lastToken == Token.Bra)
                            {
                                break;
                            }
                            hisPr = GetPrior(lastToken);
                            if (hisPr > myPr)
                            {
                                res.Add(stack.Peek());
                                stack.Pop();
                            }
                        } while (hisPr > myPr && stack.Count > 0);
                        stack.Push(stocken);
                        break;
                    case Token.Ket:
                        string nextStoken;
                        do{
                            if (stack.Count == 0)
                            {
                                throw new DataException("Brackets did not match!");
                            }
                            nextStoken = (stack.Peek());
                            stack.Pop();
                            if (ParseValue(nextStoken) != Token.Bra)
                            {
                                res.Add(nextStoken);
                            }
                        } while (ParseValue(nextStoken) != Token.Bra);
                        break;
                }
                // Console.Write("--Current res: ");
                // foreach (string re in res)
                // {
                // Console.Write(re + " ");
                // }
                // Console.WriteLine("");
                // Console.Write("--Current stack: ");
                // foreach (string st in stack)
                // {
                //     Console.Write(st + " ");
                // }
                // Console.WriteLine("");
            }

            while (stack.Count != 0)
            {
                res.Add(stack.Peek());
                stack.Pop();
            }

            res.Reverse();
            return res;
        }
        
        public static int Evaluate(List<string> reversedRevPolNot, out int end, int start=0)
        {
            var stocken = reversedRevPolNot[start];
            var token = ParseValue(stocken);
            // Console.WriteLine("Calculating value of this:" + stocken + " at position " + start);
            switch (token)
            {
                case Token.Bra:
                case Token.Ket:
                    throw new DataException("You passed a bad string here: " + stocken);
                case Token.Mult:
                case Token.Minus:
                case Token.Plus:
                case Token.Div:
                    int secondOpStart;
                    int rightOp = Evaluate(reversedRevPolNot, out secondOpStart, start + 1);
                    int leftOp = Evaluate(reversedRevPolNot, out end, secondOpStart);
                    var op = ParseOp(stocken);
                    return op(leftOp, rightOp);
                case Token.Number:
                    end = start + 1;
                    return int.Parse(stocken);
            }

            throw new Exception("How did you get here?? With this??: \'" + stocken + "\'");
        }
    }
}