using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WaveFunctionCollapse
{
    public class WFCCore
    {
        OutputGrid outputGrid;
        PatternManager patternManager;

        private int maxIterations = 0;

        public WFCCore(int outputWidth, int outputHeight, int maxIterations, PatternManager patternManager)
        {
            this.outputGrid = new OutputGrid(outputWidth, outputHeight, patternManager.GetNumberOfPatterns());
            this.patternManager = patternManager;
            this.maxIterations = maxIterations;
        }

        public int[][] CreateOutputGrid(PatternManager manager)
        {
            int iteration = 0;
            while (iteration < this.maxIterations)
            {
                CoreSolver solver = new CoreSolver(this.outputGrid, this.patternManager);
                int innerIteration = 500;
                while (!solver.CheckForConflicts() && !solver.CheckIfSolved())
                {
                    //solve (0,0) first, and if (0,0) is solved, then move on to getting lowest entropy
                    if (!outputGrid.CheckIfCellIsCollapsed(new Vector2Int(0, 0)))
                    {
                        solver.CollapseCell(new Vector2Int(0, 0));
                        solver.Propagate();
                        innerIteration--;

                    }
                    else
                    {
                        Vector2Int position = solver.GetLowestEntropyCell();
                        if (position != new Vector2Int(0, 0))
                        {
                            solver.CollapseCell(position);
                            solver.Propagate();

                            innerIteration--;
                            if (innerIteration <= 0)
                            {
                                Debug.Log("Propagation is taking too long");
                                return new int[0][];
                            }
                        }
                    }
                }
                //once out of while loop
                if (solver.CheckForConflicts())
                {
                    Debug.Log("\n Conflict occured. Iteration: " + iteration);
                    iteration++;
                    outputGrid.ResetAllPossibilities();
                    solver = new CoreSolver(this.outputGrid, this.patternManager);
                }
                else
                {
                    Debug.Log("Solved on (supposedly): " + iteration);
                    var gridToCheck = outputGrid.GetSolvedOutputGrid();
                    int[][] valueGrid;
                    valueGrid = manager.ConvertPatternToValues<int>(gridToCheck);
                    // check for one 4 (G), one 3 (A(G)), one 7 (A(H)), one 6 (A(D)) 
                    List<int> outputContent = new List<int>();
                    for (int row = 0; row < outputGrid.height; row++)
                    {
                        for (int col = 0; col < outputGrid.width; col++)
                        {
                            outputContent.Add(valueGrid[row][col]);
                            Debug.Log(row + ", " + col + ": " + valueGrid[row][col]);
                        }
                    }
                    var g = outputContent.GroupBy(i => i);
                    bool onlyOneG = false;
                    bool onlyOneAG = false;
                    bool onlyOneAH = false;
                    bool onlyOneAD = false;
                    if (outputContent.Contains(4) && outputContent.Contains(3) && outputContent.Contains(7) && outputContent.Contains(6))
                    {
                        foreach (var grp in g)
                        {
                            if (grp.Key == 4)
                            {
                                 onlyOneG = (grp.Count() == 1);
                            }
                            else if (grp.Key == 3)
                            {
                                 onlyOneAG = (grp.Count() == 1);
                            }
                            else if (grp.Key == 7)
                            {
                                 onlyOneAH = (grp.Count() == 1);
                            }
                            else if (grp.Key == 6)
                            {
                                 onlyOneAD = (grp.Count() == 1);
                            }
                        }
                        if (onlyOneG && onlyOneAG && onlyOneAH && onlyOneAD)
                        {
                            this.outputGrid.PrintResultsToConsol();
                            break;
                        }
                        else
                        {
                            // if doesn't contain or contain multiple of the above elements, backtrack
                            outputGrid.ResetAllPossibilities();
                            solver = new CoreSolver(this.outputGrid, this.patternManager);
                        }
                    }
                }
                
            }
            if (iteration >= this.maxIterations)
            {
                Debug.Log("Coulnd solve the tilemap");
            }
            return outputGrid.GetSolvedOutputGrid();

        }

    }
}
