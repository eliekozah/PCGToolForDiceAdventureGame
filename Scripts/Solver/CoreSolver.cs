using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class CoreSolver
    {
        //will store output of algorithm
        OutputGrid outputGrid;
        CoreHelper coreHelper;
        PropragationHelper propagationHelper;
        PatternManager patternManager;

        public CoreSolver(OutputGrid outputGrid, PatternManager patternManager)
        {
            this.outputGrid = outputGrid;
            this.patternManager = patternManager;
            this.coreHelper = new CoreHelper(this.patternManager);
            this.propagationHelper = new PropragationHelper(this.outputGrid, this.coreHelper);
        }

        public void Propagate()
        {
            while (propagationHelper.PairsToPropagate.Count > 0)
            {
                var propagatePair = propagationHelper.PairsToPropagate.Dequeue();
                if (propagationHelper.CheckIfPairShouldBeProcessed(propagatePair))
                {
                    ProcessCell(propagatePair);
                }
                if (propagationHelper.CheckForConflicts() || outputGrid.CheckIfGridIsSolved())
                {
                    return;
                }
            }
            if (propagationHelper.CheckForConflicts() && propagationHelper.PairsToPropagate.Count == 0 && propagationHelper.LowEntropySet.Count == 0)
            {
                propagationHelper.SetConflictFlag();
            }
        }

        private void ProcessCell(VectorPair propagatePair)
        {
            if (outputGrid.CheckIfCellIsCollapsed(propagatePair.CellToPropagatePosition))
            {
                propagationHelper.EnqueueUncollapseNeighbours(propagatePair);
            }
            else
            {
                PropagateNeighbour(propagatePair);
            }
        }

        private void PropagateNeighbour(VectorPair propagatePair)
        {
            var possibleValuesAtNeighbour = outputGrid.GetPossibleValueForPossition(propagatePair.CellToPropagatePosition);
            int startCount = possibleValuesAtNeighbour.Count;

            RemoveImpossibleNeighbours(propagatePair, possibleValuesAtNeighbour);

            int newPossiblePatternCount = possibleValuesAtNeighbour.Count;
            propagationHelper.AnalyzePropagationResults(propagatePair, startCount, newPossiblePatternCount);
        }

        private void RemoveImpossibleNeighbours(VectorPair propagatePair, HashSet<int> possibleValuesAtNeighbour)
        {
            HashSet<int> possibleIndices = new HashSet<int>();

            foreach (var patternIndexAtBase in outputGrid.GetPossibleValueForPossition(propagatePair.BaseCellPosition))
            {
                // this is all possible neighbours
                var possibleNeighboursForBase = patternManager.GetPossibleNeighboursForPatternInDirection(patternIndexAtBase, propagatePair.DirectionFromBase);
                possibleIndices.UnionWith(possibleNeighboursForBase);
            }
            possibleValuesAtNeighbour.IntersectWith(possibleIndices);
        }

        public Vector2Int GetLowestEntropyCell()
        {
            if (propagationHelper.LowEntropySet.Count <= 0)
            {
                return outputGrid.GetRandomCell();
            }
            else
            {
                var lowestEntropyElement = propagationHelper.LowEntropySet.First();
                Vector2Int returnVector = lowestEntropyElement.Position;
                propagationHelper.LowEntropySet.Remove(lowestEntropyElement);
                return returnVector;
            }
        }

        // this function solves each cell
        public void CollapseCell(Vector2Int cellCoordinates)
        {
            var possibleValue = outputGrid.GetPossibleValueForPossition(cellCoordinates).ToList();
            string combinedString = string.Join(",", possibleValue);
            if (possibleValue.Count == 0 || possibleValue.Count == 1)
            {
                return;
            }
            else
            {
                if (cellCoordinates == new Vector2Int(0, 0))
                {
                    string combinedString2 = string.Join(",", possibleValue);
                    List<int> newPossibleValue = new List<int>();
                    if (PatternFinder.legalPatterns.Intersect(possibleValue).Any())
                    {
                        foreach (int item in PatternFinder.legalPatterns.Intersect(possibleValue))
                        {
                            newPossibleValue.Add(item);
                        }
                    }
                    int i;
                    i = coreHelper.SelectSolutionPatternFromFrequency(newPossibleValue);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, newPossibleValue[i]);
                }
                else
                {
                    int index;
                    index = coreHelper.SelectSolutionPatternFromFrequency(possibleValue);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, possibleValue[index]);
                }
            }

            if (coreHelper.CheckCellSolutionForCollision(cellCoordinates, outputGrid) == false)
            {
                propagationHelper.AddNewPairsToPropagateQueue(cellCoordinates, cellCoordinates);
            }
            else
            {
                propagationHelper.SetConflictFlag();
            }

        }

        public bool CheckIfSolved()
        {
            return outputGrid.CheckIfGridIsSolved();
        }

        public bool CheckForConflicts()
        {
            return propagationHelper.CheckForConflicts();
        }
    }
}

