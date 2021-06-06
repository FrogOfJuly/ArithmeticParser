using System.IO;
using System.Text;
using NUnit.Framework;
using Antlr4.Runtime;
using System.Linq;

namespace ArithmeticTree
{
    public class EvalVisitor : ArithmeticsBaseVisitor<int>
    {
        public override int VisitOpExpr(ArithmeticsParser.OpExprContext context)
        {
            int left = Visit(context.left);
            int right = Visit(context.right);
            string op = context.op.Text;
            switch (op[0])
            {
                case '*': return left * right;
                case '/': return left / right;
                case '+': return left + right;
                case '-': return left - right;
                default: throw new InvalidDataException("Unknown operator " + op);
            }
        }

        public override int VisitAtomExpr(ArithmeticsParser.AtomExprContext context)
        {
            return int.Parse(context.atom.Text);
        }

        public override int VisitParenExpr(ArithmeticsParser.ParenExprContext context)
        {
            return Visit(context.expr());
        }
    }

    public class DumpVisitor : ArithmeticsBaseVisitor<string>
    {
        public override string VisitOpExpr(ArithmeticsParser.OpExprContext context)
        {
            string left = Visit(context.left);
            string right = Visit(context.right);
            string op = context.op.Text;
            switch (op[0])
            {
                case '*': return "MultOP(" + left + ", " + right + ")";
                case '/': return "DivOP(" + left + ", " + right + ")";
                case '+': return "AddOP(" + left + ", " + right + ")";
                case '-': return "SubOP(" + left + ", " + right + ")";
                default: throw new InvalidDataException("Unknown operator " + op);
            }
        }

        public override string VisitAtomExpr(ArithmeticsParser.AtomExprContext context)
        {
            return "Atom(" + context.atom.Text + ")";
        }

        public override string VisitParenExpr(ArithmeticsParser.ParenExprContext context)
        {
            return "ParenOP(" + Visit(context.expr()) + ")";
        }
    }

    [TestFixture]
    public class Tests
    {
        private ArithmeticsParser parser;
        private EvalVisitor evis;
        private DumpVisitor dvis;

        private void Setup(string text)
        {
            var inputStream = new AntlrInputStream(text);
            var lexer = new ArithmeticsLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            parser = new ArithmeticsParser(tokens);
            evis = new EvalVisitor();
            dvis = new DumpVisitor();
        }

        [Test]
        public void Test1()
        {
            Setup("1");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            Assert.AreEqual(res, 1);
            var sres = dvis.Visit(tree);
            Assert.AreEqual("Atom(1)", sres);
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            Setup("1 + 2*3");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            var sres = dvis.Visit(tree);
            Assert.AreEqual(res, 7);
            Assert.AreEqual("AddOP(Atom(1), MultOP(Atom(2), Atom(3)))", sres);
            Assert.Pass();
        }

        [Test]
        public void Test3()
        {
            Setup("(1 + 2)*3");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            var sres = dvis.Visit(tree);
            Assert.AreEqual(res, 9);
            Assert.AreEqual("MultOP(ParenOP(AddOP(Atom(1), Atom(2))), Atom(3))", sres);
            Assert.Pass();
        }

        [Test]
        public void Test4()
        {
            Setup("3*(1 + 2)");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            var sres = dvis.Visit(tree);
            Assert.AreEqual(res, 9);
            Assert.AreEqual("MultOP(Atom(3), ParenOP(AddOP(Atom(1), Atom(2))))", sres);
            Assert.Pass();
        }

        [Test]
        public void Test5()
        {
            Setup("1 + 3*(1 + 2) + 2");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            var sres = dvis.Visit(tree);
            Assert.AreEqual(res, 12);
            Assert.AreEqual("AddOP(AddOP(Atom(1), MultOP(Atom(3), ParenOP(AddOP(Atom(1), Atom(2))))), Atom(2))", sres);
            Assert.Pass();
        }

        [Test]
        public void Test6()
        {
            Setup("1 + 3*3 + 7 + 2");
            var tree = parser.expr();
            var res = evis.Visit(tree);
            var sres = dvis.Visit(tree);
            Assert.AreEqual(res, 19);
            Assert.AreEqual("AddOP(AddOP(AddOP(Atom(1), MultOP(Atom(3), Atom(3))), Atom(7)), Atom(2))", sres);
            Assert.Pass();
        }
    }
}