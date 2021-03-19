using System;
using static ArithmeticParser.Calculator;

namespace ArithmeticParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write a one line arithmetic expression you want to evaluate:");
            var line = Console.ReadLine();
            line = Calculator.Preprocess(line);
            var pices = Calculator.SplitString(line);
            var reveredPolskiNot = Calculator.ReversePolskiNotation(pices);
            // foreach (string item in reveredPolskiNot)
            // {
            //     Console.WriteLine(item);
            // }

            var end = 0;
            var res = Calculator.Evaluate(reveredPolskiNot, out end, 0);
            Console.WriteLine("Result: " + res);
        }
        
    }
}