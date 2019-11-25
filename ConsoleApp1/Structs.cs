using System;
using System.Collections.Generic;
using System.Text;

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

        public static Cell TryCreateEquationCell(string input, Address AdrOfCellCreated , List<Equation> equationList)
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
            bool a = Address.TryParse(splittedInput[1], out Address firstArg);
            bool b = Address.TryParse(splittedInput[1], out Address secondArg);
            if (a && b) //valid equation
            {
                Cell newCell = new Cell(default, CellType.Equation);
                Equation eq = new Equation(AdrOfCellCreated, firstArg, secondArg);
                equationList.Add(eq);
                return newCell;
            }
            else //address parse failed - flawed formula
            {
                Cell newCell = new Cell(default, CellType.FlawedFormula);
                return newCell;
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
        int Row;
        int Column;
        Table File;

        public Address(int row, int column, Table file)
        {
            this.Row = row;
            this.Column = column;
            this.File = file;
        }

        public static bool TryParse(String probaplyAddress, out Address address )
        {
            const int A = 65;
            const int Z = 90;
            const int zero = 48;
            const int nine = 57;

            int row = 0;
            int column = 0;

            for (int i = 0; i < probaplyAddress.Length; i++)
            {
                while(probaplyAddress[i]>=A && probaplyAddress[i] <= Z)
                {

                }
            }
        }
    }

    public struct Equation
    {
        Address OwnAdr;
        Address Arg1;
        Address Arg2;

        public Equation(Address own, Address arg1, Address arg2)
        {
            this.OwnAdr = own;
            this.Arg1 = arg1;
            this.Arg2 = arg2;
        }
    }


}
    

