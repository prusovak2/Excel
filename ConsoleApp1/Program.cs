using System;
using System.Collections.Generic;
using System.IO;

namespace Excel
{
    class Program
    {
        static void Main(string[] args)
        {
            string pokus = "=AB4+CD5";
            char[] delims = { '=', '+', '-', '*', '/' };
            string[] splittedInput = pokus.Split(delims);

            Console.WriteLine(splittedInput.Length);

            foreach (var item in splittedInput)
            {
                Console.WriteLine("x: {0}", item);
            }

            HashSet<string> FilesThatHadBeenRead = new HashSet<string>();
            FilesThatHadBeenRead.Add(args[0]);

        }

        private static int OpenFile(string[] args, out StreamReader Reader, out StreamWriter Writer)
        {
            if (args.Length != 2)
            {
                Reader = null;
                Writer = null;
                Console.WriteLine("Argument Error");
                return 0;
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
                return 0;
            }
            try
            {
                Writer = new StreamWriter(args[1], false); //TODO:append?
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is UnauthorizedAccessException || e is DirectoryNotFoundException || e is IOException || e is PathTooLongException)
            {
                Writer = null;
                Reader = null;
                Console.WriteLine("File Error");
                return 0;
            }
            return 1;

        }

    }
}

