using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Excel
{
    /// <summary>
    /// Represents one cell of excel sheet
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// IMPORTANT TRICK: if a cell is number, containts its value, if a cell if equation, contains its index in equation list
        /// to save memory
        /// </summary>
        private int _Value; 
        /// <summary>
        /// info about what kind of data does a cell contains
        /// </summary>
        public CellType Type;

        public int Value 
        {
            get 
            {
                //if (this.Type != CellType.Number)
                //    throw new Exception();
                return _Value; 
            }
            set => _Value = value; 
        }
        public int EquationIndex { get => _Value; set => _Value = value; }

        public bool IsError { get => (byte)this.Type >= 128; }

        /// <summary>
        /// to split input line
        /// </summary>
        private static readonly char[] delims = { '=', '+', '-', '*', '/' };
        /// <summary>
        /// to index equations in equation list
        /// </summary>
        public static int EquationCounter=0;

        public Cell(int value, CellType type)
        {
            this._Value = value;
            this.Type = type;
        }
        /// <summary>
        /// Creates equation cell is input string is valid equation, creates cell of Flawed formula or MissOperator type otherwise
        /// </summary>
        /// <param name="input">string probably containing address</param>
        /// <param name="AdrOfCellCreated"></param>
        /// <param name="equationList">list to store created equation struct</param>
        /// <param name="FilesToRead">for a multifile version, not implemented</param>
        /// <returns>returns cell created - equation cell, flawed formula cell or Missing Operator cell</returns>
        public static Cell TryCreateEquationCell(string input, Address AdrOfCellCreated , List<Equation> equationList, Queue<string> FilesToRead)
        {
            
            string[] splittedInput = input.Split(Cell.delims);
            //input surely contains =   Length<3 means missing operator 
            if (splittedInput.Length < 3)
            {
                Cell newCell = new Cell(default(int), CellType.MissOperator);
                return newCell;
            }
            //some extra operators somwhere, where they should not be
            if (splittedInput.Length > 3)
            {
                Cell newCell = new Cell(default(int), CellType.FlawedFormula);
                return newCell;
            }

            //Length==3 : one operator
            bool a = Address.TryParse(splittedInput[1], out Address firstArg, FilesToRead);
            bool b = Address.TryParse(splittedInput[2], out Address secondArg, FilesToRead);
            int opPosition = 1 + splittedInput[1].Length; //count where ope
            char Operator = input[opPosition];
            if (a && b) //valid equation, both address parses succeeded
            {
                //create new equation cell, equation counter is its index in Equation list
                Cell newCell = new Cell(Cell.EquationCounter, CellType.Equation);

                Cell.EquationCounter++;

                //create new equation, coresponding to equation cell just created
                Equation eq = new Equation(AdrOfCellCreated, firstArg, secondArg, Operator);
                //add equation list, index coresponds to value stored in equation cell
                equationList.Add(eq);
                return newCell;
            }
            else //address parse failed - flawed formula
            {
                Cell newCell = new Cell(default(int), CellType.FlawedFormula);
                return newCell;
            }
        }
        /// <summary>
        /// prints cell acording to its type and contend
        /// </summary>
        /// <param name="writer"></param>
        public void PrintCell(StreamWriter writer)
        {
            switch (this.Type)
            {
                case CellType.Number:
                    writer.Write(this._Value);
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
        InEquation, //important for solving equation, indicates that equation conected to cell is part of current solving process

        Inval = 128,
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

        /// <summary>
        /// determines whether string has valid syntax of excel cell address, if so, creates new Address instance
        /// </summary>
        /// <param name="probablyAddress">string to be parsed</param>
        /// <param name="address">address created</param>
        /// <param name="FilesToRead">for multifile version</param>
        /// <returns>true if probablyAddress has corect address syntax and was parsed succesfully, false otherwise</returns>
        public static bool TryParse(String probablyAddress, out Address address, Queue<string> FilesToRead )
        {
            const int A = 65; //just constants for ascii values
            const int Z = 90;
            const int zero = 48;
            const int nine = 57;

            int row = 0;
            int column = -1; //really important for making Horner to index from 0

            string cellPartOfAdr;
            string file = string.Empty;
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

            int recordCounter = 0; //we need to know, whether address contains at least one letter -  index of collumn
            int i = 0;
            while(i<cellPartOfAdr.Length && cellPartOfAdr[i]>=A && cellPartOfAdr[i] <= Z )
            {
                column = (column + 1) * 26 + cellPartOfAdr[i] - A; //Horner scheme - making number from letter index of column
                recordCounter++;
                i++;
            }
            if (recordCounter == 0) //column part of adr is missing
            {
                address = default(Address);
                return false;
            }
            recordCounter = 0; //we need to know, whether address contains at least one number -  index of row
            while (i < cellPartOfAdr.Length && cellPartOfAdr[i] >= zero && cellPartOfAdr[i] <= nine)
            {
                row = row * 10 + cellPartOfAdr[i] - zero; //Horner scheme 
                recordCounter++;
                i++;
            }
            row--; //index from 0
            if (recordCounter == 0 || row<0 ) //flawed row part
            {
                address = default(Address);
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
        public override string ToString()
        {
            return $"{this.Column}:{this.Row}";
        }
    }
    /// <summary>
    /// represents equation from excel sheet, is conected to particular Cell
    /// </summary>
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
        /// <summary>
        /// counts equation, whose both arguments are numbers
        /// </summary>
        /// <param name="arg1">args read from adequate cell</param>
        /// <param name="arg2"></param>
        /// <returns>sollution of equation</returns>
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
        public override string ToString()
        {
            return $"{ this.Arg1.Column}:{ this.Arg1.Row} {this.operand} { this.Arg2.Column}:{ this.Arg2.Row}";
             
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
    

