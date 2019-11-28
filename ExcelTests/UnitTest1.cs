using Microsoft.VisualStudio.TestTools.UnitTesting;
using Excel;
using System.Collections.Generic;
using System.IO;

namespace ExcelTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TryParseAddress()
        {
            string normal = "B3";
            string high = "AOT1";       
            string longer = "ABC500";
            string flawed = "abcd444";
            string file = "mnau!CC42";

            Queue<string> q = new Queue<string>();

            Address adr = new Address();
            bool valid = Address.TryParse(normal,out adr, q);

            Assert.IsTrue(valid);
            Assert.AreEqual(1, adr.Column); System.Console.WriteLine(adr.Column);
            Assert.AreEqual(2, adr.Row); System.Console.WriteLine(adr.Row);

            valid = Address.TryParse(longer, out adr, q);

            Assert.IsTrue(valid);
            Assert.AreEqual(730, adr.Column); System.Console.WriteLine(adr.Column);
            Assert.AreEqual(499, adr.Row); System.Console.WriteLine(adr.Row);

            valid = Address.TryParse(flawed, out adr, q);
            Assert.IsTrue(!valid);

            flawed = "AAA";

            valid = Address.TryParse(flawed, out adr, q);
            Assert.IsTrue(!valid);

            flawed = "1234";
            valid = Address.TryParse(flawed, out adr, q);
            Assert.IsTrue(!valid);

            flawed = "aaa!someShit";
            valid = Address.TryParse(flawed, out adr, q);
            Assert.IsTrue(!valid);

            valid = Address.TryParse(file, out adr, q);

            Assert.IsTrue(valid);
            Assert.AreEqual(80, adr.Column); System.Console.WriteLine(adr.Column);
            Assert.AreEqual(41, adr.Row); System.Console.WriteLine(adr.Row);
            Assert.AreEqual("mnau", adr.File); System.Console.WriteLine(adr.File);

             
            valid = Address.TryParse(high, out adr, q);          
            Assert.IsTrue(valid);
            System.Console.WriteLine(adr.Column);
            Assert.AreEqual(1085, adr.Column); 
            Assert.AreEqual(0, adr.Row); System.Console.WriteLine(adr.Row);

        }

        [TestMethod]
        public void TryCreateequationCellTest()
        {
            string normal = "=AB3-C4";
            Address adr= default;
            List<Equation> equations = new List<Equation>();
            Queue<string> files = new Queue<string>();
            Cell cell = new Cell();

            cell =Cell.TryCreateEquationCell(normal, adr, equations, files);

            Assert.AreEqual(CellType.Equation, cell.Type); System.Console.WriteLine(cell.Type);
             
            Assert.AreEqual(27, equations[0].Arg1.Column); System.Console.WriteLine(equations[0].Arg1.Column);
            Assert.AreEqual(2, equations[0].Arg1.Row); System.Console.WriteLine(equations[0].Arg1.Row);
            Assert.AreEqual(2, equations[0].Arg2.Column); System.Console.WriteLine(equations[0].Arg2.Column);
            Assert.AreEqual(3, equations[0].Arg2.Row); System.Console.WriteLine(equations[0].Arg2.Row);
            Assert.AreEqual(Operand.minus, equations[0].operand);

            equations.Clear();

            //****************************************
            normal = "=AB44444443/C4";
            cell = Cell.TryCreateEquationCell(normal, adr, equations, files);
            Assert.AreEqual(Operand.div, equations[0].operand);
            equations.Clear();

            //****************************************
            normal = "=ABCC444443*C4";
            cell = Cell.TryCreateEquationCell(normal, adr, equations, files);
            Assert.AreEqual(Operand.multi, equations[0].operand);
            equations.Clear();

            //****************************************
            normal = "=A3+C4";
            cell = Cell.TryCreateEquationCell(normal, adr, equations, files);
            Assert.AreEqual(Operand.plus, equations[0].operand);
            equations.Clear();

            //****************************************
            string missOp = "=autobus";
            cell = Cell.TryCreateEquationCell(missOp, adr, equations, files);
            Assert.AreEqual(CellType.MissOperator, cell.Type); System.Console.WriteLine(cell.Type);

            //****************************************
            string moreOp = "=AA3+BB3+CC3";
            cell = Cell.TryCreateEquationCell(moreOp, adr, equations, files);
            Assert.AreEqual(CellType.FlawedFormula, cell.Type); System.Console.WriteLine(cell.Type);

            //****************************************
            string flawedAdr = "=aaa5*A4";
            cell = Cell.TryCreateEquationCell(flawedAdr, adr, equations, files);
            Assert.AreEqual(CellType.FlawedFormula, cell.Type); System.Console.WriteLine(cell.Type);

            //****************************************
            flawedAdr = "=D5*A";
            cell = Cell.TryCreateEquationCell(flawedAdr, adr, equations, files);
            Assert.AreEqual(CellType.FlawedFormula, cell.Type); System.Console.WriteLine(cell.Type);


        }
        [TestMethod]
        public void ReadTableTest()
        {
           // Assert.Fail();

            Queue<string> q = new Queue<string>();
            List<Equation> equations = new List<Equation>();
            StreamReader reader = new StreamReader(@"TestFiles/Ins/SimpleTable.txt");

            Table t = new Table();
            t.ReadTable(reader, equations, q, "SimpleTable.txt");

            for (int i = 0; i < t.Cells.Count; i++)
            {
                for (int j = 0; j < t.Cells[i].Length; j++)
                {
                    System.Console.Write(t.Cells[i][j].Type);
                    System.Console.Write(" ");
                }
                System.Console.WriteLine();
            }

            System.Console.WriteLine();
            System.Console.WriteLine();

            foreach (var item in equations)
            {
                System.Console.WriteLine("{0} {1}:{2}",item.operand, item.OwnAdr.Column, item.OwnAdr.Row);
            }
        }
        [TestMethod]
        public void CountEquationTest()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void PrintCellTest()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void PrintTable()
        {
            Queue<string> q = new Queue<string>();
            List<Equation> equations = new List<Equation>();
            StreamReader reader = new StreamReader(@"TestFiles/Ins/SimpleTable.txt");

            StreamWriter writer = new StreamWriter(@"TestFiles/Outs/SimpleTable.txt");

            Table t = new Table();
            t.ReadTable(reader, equations, q, "SimpleTable.txt");
            t.PrintTable(writer);
            writer.Close();
        }
        [TestMethod]
        public void generalTest()
        {
            Queue<string> q = new Queue<string>();
            List<Equation> equations = new List<Equation>();
            StreamReader reader = new StreamReader(@"TestFiles/Ins/HugeFile.txt");

            StreamWriter writer = new StreamWriter(@"TestFiles/Tmps/HugeOut.txt");
            Table t = new Table();
            t.ReadTable(reader, equations, q, "Huge.txt");

            for (int i = 0; i < equations.Count; i++)
            {
                EquationSolver.Solve(t, equations[i], equations);
            }
            t.PrintTable(writer);
            writer.Close();
            bool same = Utils.FileDiff(@"TestFiles/Outs/HugeFile.eval.txt", @"TestFiles/Tmps/HugeOut.txt");
            Assert.IsTrue(same);       
        }
        [TestMethod]
        public void Cycle1Test()
        {
            Queue<string> q = new Queue<string>();
            List<Equation> equations = new List<Equation>();
            StreamReader reader = new StreamReader(@"TestFiles/Ins/CycleTest.txt");

            StreamWriter writer = new StreamWriter(@"TestFiles/Tmps/CycleMy.txt");
            Table t = new Table();
            t.ReadTable(reader, equations, q, "Huge.txt");

            for (int i = 0; i < equations.Count; i++)
            {
                EquationSolver.Solve(t, equations[i], equations);
            }
            t.PrintTable(writer);
            writer.Close();
            bool same = Utils.FileDiff(@"TestFiles/Outs/CycleTestResult.txt", @"TestFiles/Tmps/CycleMy.txt");
            Assert.IsTrue(same);
        }
        [TestMethod]
        public void SimpleCycleTest()
        {
            Queue<string> q = new Queue<string>();
            List<Equation> equations = new List<Equation>();
            StreamReader reader = new StreamReader(@"TestFiles/Ins/SimpleCycle.txt");

            StreamWriter writer = new StreamWriter(@"TestFiles/Tmps/SimpleCycleMy.txt");
            Table t = new Table();
            t.ReadTable(reader, equations, q, "Huge.txt");

            for (int i = 0; i < equations.Count; i++)
            {
               EquationSolver.Solve(t, equations[i], equations);
            }
            t.PrintTable(writer);
            writer.Close();
            bool same = Utils.FileDiff(@"TestFiles/Outs/SimpleCycleRes.txt", @"TestFiles/Tmps/SimpleCycleMy.txt");
            Assert.IsTrue(same);
        }


    }
}
