using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Excel
{
    public class Table
    {
        public List<Cell[]> Cells;
        //string FileName;

        public void ReadTable(StreamReader Reader, List<Equation> EquationList, Queue<string> FilesToRead, string FileName)
        {
            int rows = 0;
            string line = Reader.ReadLine();

            while (line != null)
            {
                string[] splittedLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Cell[] tableLine = new Cell[splittedLine.Length];
                
                for (int columns = 0; columns < splittedLine.Length; columns++)
                {
                    //cell is number
                    if(Int32.TryParse(splittedLine[columns], out int value)) //TODO:nezaporna?
                    {
                        Cell newCell = new Cell(value, CellType.Number);
                        tableLine[columns] = newCell;
                    }
                    //cell is empty
                    else if(splittedLine[columns] == "[]")
                    {
                        Cell newCell = new Cell(default(int), CellType.Empty);
                        tableLine[columns] = newCell;
                    }
                    //cell is equation
                    else if (splittedLine[columns][0] == '=')
                    {
                        Address cellAdr = new Address(rows, columns, /*this*/ FileName);
                        Cell newCell = Cell.TryCreateEquationCell(splittedLine[columns], cellAdr, EquationList, FilesToRead);
                        tableLine[columns] = newCell;
                    }
                    //content of cell is invalid
                    else
                    {
                        Cell newCell = new Cell(default(int), CellType.Inval);
                        tableLine[columns] = newCell;
                    }
                    
                }

                this.Cells.Add(tableLine); //add row
                rows++;
                line = Reader.ReadLine();
            }
        }

        public void PrintTable(StreamWriter writer)
        {
            for (int i = 0; i < this.Cells.Count; i++)
            {
                for (int j = 0; j < this.Cells[i].Length; j++)
                {
                    this.Cells[i][j].PrintCell(writer);
                    if(j + 1 < this.Cells[i].Length)
                    {
                        writer.Write(" ");
                    }
                    
                }
                //do not add space after last record on line
                //this.Cells[i][this.Cells[i].Length - 1].PrintCell(writer);
                writer.WriteLine();
            }
            writer.Flush();
        }
        /// <summary>
        /// To prevent accesing non existing cells
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        public CellType GetType(Address adr)
        {
            if(adr.Row>= this.Cells.Count || adr.Column>= this.Cells[adr.Row].Length)
            {
                return CellType.Empty;
            }
            return this.Cells[adr.Row][adr.Column].Type;
        }

        /// <summary>
        /// To prevent accesing non existing cells
        /// </summary>
        /// <param name="adr"></param>
        /// <returns></returns>
        public int GetValue(Address adr)
        {
            if (adr.Row >= this.Cells.Count || adr.Column >= this.Cells[adr.Row].Length)
            {
                return 0;
            }
            return this.Cells[adr.Row][adr.Column].Value;
        }

        public void SetType(Address adr, CellType type)
        {
            this.Cells[adr.Row][adr.Column].Type = type;
        }

        //setting value sets cell type to number
        public void SetNumberTypeAndValue(Address adr, int value)
        {
            this.Cells[adr.Row][adr.Column].Type = CellType.Number;
            this.Cells[adr.Row][adr.Column].Value = value;

        }

        public Table (/*string fileName*/)
        {
            //this.FileName = fileName;
            this.Cells = new List<Cell[]>();
        }
    }


}
