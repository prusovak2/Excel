using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Excel
{
    public class Table
    {
        List<Cell[]> Cells;

        public void ReadTable(StreamReader Reader, List<Equation> EquationList)
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
                        Cell newCell = new Cell(default, CellType.Empty);
                        tableLine[columns] = newCell;
                    }
                    //cell is equation
                    else if (splittedLine[columns][0] == '=')
                    {
                        Address cellAdr = new Address(rows, columns, this);
                        Cell newCell = Cell.TryCreateEquationCell(splittedLine[columns], cellAdr, EquationList);
                        tableLine[columns] = newCell;
                    }
                    //content of cell is invalid
                    else
                    {
                        Cell newCell = new Cell(default, CellType.Inval);
                        tableLine[columns] = newCell;
                    }
                    
                }

                this.Cells.Add(tableLine); //add row
                rows++;
                line = Reader.ReadLine();
            }
        }
    }


}
