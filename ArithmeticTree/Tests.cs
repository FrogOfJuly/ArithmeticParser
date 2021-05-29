using System.Text;
using NUnit.Framework;

namespace ArithmeticTree
{
    public interface IExpressionVisitor
    {
        void Visit(Literal expression);
        void Visit(Variable expression);
        void Visit(BinaryExpression expression);
        void Visit(ParenExpression expression);
    }

    public class DumpVisitor : IExpressionVisitor
    {
        private readonly StringBuilder myBuilder;

        public DumpVisitor()
        {
            myBuilder = new StringBuilder();
        }

        public void Visit(Literal expression)
        {
            myBuilder.Append("Literal(" + expression.Value + ")");
        }

        public void Visit(Variable expression)
        {
            myBuilder.Append("Variable(" + expression.Name + ")");
        }

        public void Visit(BinaryExpression expression)
        {
            myBuilder.Append("Binary(");
            expression.FirstOperand.Accept(this);
            myBuilder.Append(expression.Operator);
            expression.SecondOperand.Accept(this);
            myBuilder.Append(")");
        }

        public void Visit(ParenExpression expression)
        {
            myBuilder.Append("Paren(");
            expression.Operand.Accept(this);
            myBuilder.Append(")");
        }

        public override string ToString()
        {
            return myBuilder.ToString();
        }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var dumpVisitor = new DumpVisitor();
            new BinaryExpression(new Literal("1"), new Literal("2"), "+").Accept(dumpVisitor);
            Assert.AreEqual("Binary(Literal(1)+Literal(2))", dumpVisitor.ToString());
            
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            var line = "1 + 2"; 
            line = Parser.Preprocess(line);
            var pices = Parser.SplitString(line);
            var reveredPolskiNot = Parser.ReversePolskiNotation(pices);
            // foreach (string item in reveredPolskiNot)
            // {
            //     Console.WriteLine(item);
            // }

            var end = 0;
            var res = Parser.Evaluate(reveredPolskiNot, out end, 0);
            var dumpVisitor = new DumpVisitor();
            res.Accept(dumpVisitor);
            Assert.AreEqual("Binary(Literal(1)+Literal(2))", dumpVisitor.ToString());
            Assert.Pass();
        }
        
        [Test]
        public void Test3()
        {
            var line = "(1 + 2) + 4"; 
            line = Parser.Preprocess(line);
            var pices = Parser.SplitString(line);
            var reveredPolskiNot = Parser.ReversePolskiNotation(pices);
            // foreach (string item in reveredPolskiNot)
            // {
            //     Console.WriteLine(item);
            // }

            var end = 0;
            var res = Parser.Evaluate(reveredPolskiNot, out end, 0);
            var dumpVisitor = new DumpVisitor();
            res.Accept(dumpVisitor);
            Assert.AreEqual("Binary(Binary(Literal(1)+Literal(2))+Literal(4))", dumpVisitor.ToString());
            Assert.Pass();
        }

        [Test]
        public void Test4()
        {
            var line = "(1 + 2) * 4"; 
            line = Parser.Preprocess(line);
            var pices = Parser.SplitString(line);
            var reveredPolskiNot = Parser.ReversePolskiNotation(pices);
            // foreach (string item in reveredPolskiNot)
            // {
            //     Console.WriteLine(item);
            // }

            var end = 0;
            var res = Parser.Evaluate(reveredPolskiNot, out end, 0);
            var dumpVisitor = new DumpVisitor();
            res.Accept(dumpVisitor);
            Assert.AreEqual("Binary(Binary(Literal(1)+Literal(2))*Literal(4))", dumpVisitor.ToString());
            Assert.Pass();
        }
        
        [Test]
        public void Test5()
        {
            var line = "1 + 2 * 4"; 
            line = Parser.Preprocess(line);
            var pices = Parser.SplitString(line);
            var reveredPolskiNot = Parser.ReversePolskiNotation(pices);
            // foreach (string item in reveredPolskiNot)
            // {
            //     Console.WriteLine(item);
            // }

            var end = 0;
            var res = Parser.Evaluate(reveredPolskiNot, out end, 0);
            var dumpVisitor = new DumpVisitor();
            res.Accept(dumpVisitor);
            Assert.AreEqual("Binary(Literal(1)+Binary(Literal(2)*Literal(4)))", dumpVisitor.ToString());
            Assert.Pass();
        }
    }
}