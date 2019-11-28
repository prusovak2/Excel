using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExcelTests
{
    public static class Utils
    {
        public static bool Diff(string A, string B)
        {
            bool t1 = true;
            bool t2;
            //int index = 0;
            for (int i = 0; i < Math.Min(A.Length, B.Length); i++)
            {
                if (A[i] != B[i])
                {
                    Console.WriteLine($"Utils.Diff: Difference at index {i}");

                    
                    Console.WriteLine($"B: {A.Substring(0, i)} !!! {A[i]} !!! {A.Substring(i+1)}");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine($"B: {B.Substring(0, i)} !!! {B[i]} !!! {B.Substring(i+1)}");

                    t1 = false;
                    break;
                }
            }

            t2 = A.Length == B.Length;
            if (A.Length != B.Length)
            {
                Console.WriteLine($"A.Length({A.Length}) != B.Length({B.Length})");
            }


            return t1 && t2;
        }

        public static bool FileDiff(string A, string B)
        {
            return Diff(File.ReadAllText(A), File.ReadAllText(B));
        }
    }
}
