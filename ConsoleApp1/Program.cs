using System;
using System.Collections.Generic;
using System.IO;

namespace Excel
{
    class Program
    {
        static void Main(string[] args)
        {

            bool opened = OpenFile(args, out StreamReader Reader, out StreamWriter Writer);
            if (!opened)
            {
                return;
            }
            
            Table table = new Table();

            List<Equation> EquationList = new List<Equation>();
            Queue<string> FilestoRead = new Queue<string>();

            table.ReadTable(Reader, EquationList, FilestoRead, args[0]);
            for (int i = 0; i < EquationList.Count; i++)
            {
                EquationSolver.Solve(table, EquationList[i], EquationList);
            }
            table.PrintTable(Writer);
            

        }

        private static bool OpenFile(string[] args, out StreamReader Reader, out StreamWriter Writer)
        {
            if (args.Length != 2)
            {
                Reader = null;
                Writer = null;
                Console.WriteLine("Argument Error");
                return false;
            }
            try
            {
                Reader = new StreamReader(args[0]);
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is FileNotFoundException || e is DirectoryNotFoundException || e is IOException)
            {
                Writer = null;
                Reader = null;
                Console.WriteLine("File Error");
                return false;
            }
            try
            {
                Writer = new StreamWriter(args[1], false); 
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is UnauthorizedAccessException || e is DirectoryNotFoundException || e is IOException || e is PathTooLongException)
            {
                Writer = null;
                Reader = null;
                Console.WriteLine("File Error");
                return false;
            }
            return true;

        }

    }
}

