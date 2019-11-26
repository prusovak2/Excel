using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Excel
{

    public struct Cell
    {
        public int Value;
        public CellType Type;

        public Cell(int value, CellType type)
        {
            this.Value = value;
            this.Type = type;
        }

        public static Cell TryCreateEquationCell(string input, Address AdrOfCellCreated , List<Equation> equationList, Queue<string> FilesToRead)
        {
            char[] delims = { '=', '+', '-', '*', '/' };
            string[] splittedInput = input.Split(delims);
            //input surely contains =   Length<3 means missing operator 
            if (splittedInput.Length < 3)
            {
                Cell newCell = new Cell(default, CellType.MissOperator);
                return newCell;
            }
            //some extra operators somwhere, where they should not be
            if (splittedInput.Length > 3)
            {
                Cell newCell = new Cell(default, CellType.FlawedFormula);
                return newCell;
            }

            //Length==3
            bool a = Address.TryParse(splittedInput[1], out Address firstArg, FilesToRead);
            bool b = Address.TryParse(splittedInput[2], out Address secondArg, FilesToRead);
            int opPosition = 1 + splittedInput[1].Length;
            char operand = input[opPosition];
            if (a && b) //valid equation
            {
                Cell newCell = new Cell(default, CellType.Equation);
                Equation eq = new Equation(AdrOfCellCreated, firstArg, secondArg, operand);
                equationList.Add(eq);
                return newCell;
            }
            else //address parse failed - flawed formula
            {
                Cell newCell = new Cell(default, CellType.FlawedFormula);
                return newCell;
            }
        }
        public void PrintCell(StreamWriter writer)
        {
            switch (this.Type)
            {
                case CellType.Number:
                    writer.Write(this.Value);
                    return;
                case CellType.Empty:
                    writer.Write("[]");
                    return;
                case CellType.Inval:
                    writer.Write("#INVVAL");
                    return;
                case CellType.Error:
                    writer.Write("#ERROR");
                    return;
                case CellType.DivZero:
                    writer.Write("#DIV0");
                    return;
                case CellType.Cycle:
                    writer.Write("#CYCLE");
                    return;
                case CellType.MissOperator:
                    writer.Write("#MISSOP");
                    return;
                case CellType.FlawedFormula:
                    writer.Write("#FORMULA");
                    return;
                case CellType.Equation:
                    writer.Write("eq");
                    return;
                default:
                    throw new Exception("unexpected cell type to be printed");
            }
        }
        
    }
    public enum CellType : Byte
    {
        Number,
        Empty,
        Equation,
        InEquation,

        Inval,
        Error,
        DivZero,
        Cycle,
        MissOperator,
        FlawedFormula

    }

    public struct Address
    {
        public int Row;
        public int Column;
        // Table File;
        public string File;

        public Address(int row, int column, string file)
        {
            this.Row = row;
            this.Column = column;
            this.File = file;
        }

        public static bool TryParse(String probablyAddress, out Address address, Queue<string> FilesToRead )
        {
            const int A = 65;
            const int Z = 90;
            const int zero = 48;
            const int nine = 57;

            int row = 0;
            int column = -1; //really important for making Horner to index from 0

            string cellPartOfAdr;
            string file = default;
            string[] splittedAdr = probablyAddress.Split('!', StringSplitOptions.RemoveEmptyEntries);
            
            if(splittedAdr.Length == 2) //address of cell from another file
            {
                cellPartOfAdr = splittedAdr[1];
                file = splittedAdr[0];
                //FilesToRead.Enqueue(file); //another file to read has been found
            }
            else
            {
                cellPartOfAdr = probablyAddress; //address of cell from the same file
            }

            int recordCounter = 0;
            int i = 0;
            while(i<cellPartOfAdr.Length && cellPartOfAdr[i]>=A && cellPartOfAdr[i] <= Z )
            {
                column = (column + 1) * 26 + cellPartOfAdr[i] - A; //Horner scheme
                recordCounter++;
                i++;
            }
            if (recordCounter == 0) //column part of adr is missing
            {
                address = default;
                return false;
            }
            recordCounter = 0;
            while (i < cellPartOfAdr.Length && cellPartOfAdr[i] >= zero && cellPartOfAdr[i] <= nine)
            {
                row = row * 10 + cellPartOfAdr[i] - zero;
                recordCounter++;
                i++;
            }
            row--; //index from 0
            if (recordCounter == 0 || row<0 ) //flawed row part
            {
                address = default;
                return false;
            }

            address = new Address(row, column, file);
            
            //TODO:improve system of file reading
            if (file != null)
            {
                FilesToRead.Enqueue(file); //another file to read has been found
            }

            return true;
        }
    }

    public struct Equation
    {
        public Address OwnAdr;
        public Address Arg1;
        public Address Arg2;
        public Operand operand;


        public Equation(Address own, Address arg1, Address arg2, char operand)
        {
            this.OwnAdr = own;
            this.Arg1 = arg1;
            this.Arg2 = arg2;

            switch (operand)
            {
                case '+':
                    this.operand = Operand.plus;
                    break;
                case '-':
                    this.operand = Operand.minus;
                    break;
                case '*':
                    this.operand = Operand.multi;
                    break;
                case '/':
                    this.operand = Operand.div;
                    break;
                default:
                    this.operand = Operand.plus;
                    throw new Exception("Invalid behavior of equation builder");
            }
        }
        public int CountEquation(int arg1, int arg2)
        {
            switch (this.operand)
            {
                case Operand.plus:
                    return arg1 + arg2;
                case Operand.minus:
                    return arg1 - arg2;
                case Operand.multi:
                    return arg1 * arg2;
                case Operand.div:
                    return arg1 / arg2;
                default:
                    throw new Exception("totaly senceless behaviour");

            }
        }
    }
    public enum Operand :byte
    {
        plus,
        minus,
        multi,
        div,
    }

}
    

