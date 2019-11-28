using System;
using System.Collections.Generic;
using System.Text;

namespace Excel
{
    public static class EquationSolver
    { 
        public static void Solve(Table MainTable, Equation EqToSolve, List<Equation> EquationList)
        {
            CellType incomingType = MainTable.GetType(EqToSolve.OwnAdr);

            //this equation has already been solved
            if (incomingType != CellType.Equation)
            {
                return;
            }

            Stack<Equation> stack = new Stack<Equation>();
            //change equation type so that it represents that equation is in stack
            MainTable.SetType(EqToSolve.OwnAdr, CellType.InEquation);
           // MainTable.Cells[EqToSolve.OwnAdr.Row][EqToSolve.OwnAdr.Column].Type = CellType.InEquation;
            stack.Push(EqToSolve);

            //Table curentTable = MainTable;
            //bool inCycle = false;

            while (stack.Count > 0)
            {
                Equation BeingSolved = stack.Peek();
                CellType type1 = MainTable.GetType(BeingSolved.Arg1);
                CellType type2 = MainTable.GetType(BeingSolved.Arg2);

               /* if (inCycle)
                {
                    MainTable.SetType(BeingSolved.OwnAdr, CellType.Cycle);
                    continue;
                }*/

                CellType BeingSolvedType = MainTable.GetType(BeingSolved.OwnAdr);
                if(BeingSolvedType!= CellType.InEquation)
                {
                    //equation has already been solved
                    continue;
                }

                //equation can be solved right now
                if((type1==CellType.Number||type1==CellType.Empty)&&(type2 == CellType.Number || type2 == CellType.Empty))
                {
                    int val1 = MainTable.GetValue(BeingSolved.Arg1);
                    int val2 = MainTable.GetValue(BeingSolved.Arg2);
                    
                    if ((BeingSolved.operand == Operand.div) && (val2 == 0))
                    {
                        //devision by zero
                        MainTable.SetType(BeingSolved.OwnAdr, CellType.DivZero);
                    }
                    else
                    {
                        //lets count it
                        int result = BeingSolved.CountEquation(val1, val2);
                        MainTable.SetNumberTypeAndValue(BeingSolved.OwnAdr, result);
                    }
                    stack.Pop();
                    continue;
                }

                if (type1 == CellType.InEquation || type2 == CellType.InEquation)
                {
                    //we found cycle - one of arguments is equation, that already is in current solving process
                    if (type1 == CellType.InEquation)
                    {
                        MainTable.SetType(BeingSolved.Arg1, CellType.Cycle);
                    }
                    else if(type2 == CellType.InEquation)
                    {
                        MainTable.SetType(BeingSolved.Arg2, CellType.Cycle);
                    }
                    if(type1 == CellType.InEquation && type2 == CellType.InEquation)
                    {
                        //TODO:remove
                        throw new Exception("unexpected behaviour, cycle in cycle");
                    }

                    //returnInCycle should be set to same equation as BeingSolved
                    Equation returnInCycle = stack.Pop();
                    CellType returnType = MainTable.GetType(returnInCycle.OwnAdr);
                    while (returnType != CellType.Cycle)
                    {
                        //while do not encounter cell already marked as cycle return through cycle and mark all its cells
                        MainTable.SetType(returnInCycle.OwnAdr, CellType.Cycle);
                        if (stack.Count > 0)
                        {
                            returnInCycle = stack.Pop();
                        }
                        returnType = MainTable.GetType(returnInCycle.OwnAdr);
                    }

                        while (stack.Count > 0)
                    {
                        Equation pointsToCycle = stack.Pop();
                        MainTable.SetType(pointsToCycle.OwnAdr, CellType.Error);
                    }
                    continue;
                }

                if(type1==CellType.DivZero||type1==CellType.Cycle || type1==CellType.MissOperator || type1== CellType.Inval || type1==CellType.FlawedFormula || type1 == CellType.Error ||
                    type2 == CellType.DivZero || type2 == CellType.Cycle || type2 == CellType.MissOperator || type2 == CellType.Inval || type2 == CellType.FlawedFormula || type2 == CellType.Error)
                {
                    //at least one of arguments is somehow invalid 
                    MainTable.SetType(BeingSolved.OwnAdr, CellType.Error);
                    stack.Pop();
                    continue;
                }

                
                if(type1 == CellType.Equation)
                {
                    MainTable.SetType(BeingSolved.Arg1, CellType.InEquation);
                    //stack.Push(BeingSolved);
                    Equation eq1 = EquationList[MainTable.GetValue(BeingSolved.Arg1)];
                    stack.Push(eq1);
                    continue;
                }
                if (type2 == CellType.Equation)
                {
                    MainTable.SetType(BeingSolved.Arg2, CellType.InEquation);
                    //stack.Push(BeingSolved);
                    Equation eq2 = EquationList[MainTable.GetValue(BeingSolved.Arg2)];
                    stack.Push(eq2);
                    continue;
                }


                
            }
        } 

 
    }
}
