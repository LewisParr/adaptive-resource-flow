using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Problem07LinearProgramming
{
    public static float[,] Maximise(float[,] augMat)
    {
        #region SimplexMethodMaximisationDescription
        /*
         * 
         * ...
         * 
         */
        #endregion

        Debug.LogError("None-dual problem maximisation not implemented.");
        return null;
    }

    public static float[] Minimise(float[,] augMat)
    {
        #region SimplexMethodMinimisationDescription
        /*
         * An objective function is to be minimised, subject to constraints.
         * 
         * The basic procedure is to convert it to a maximisation problem, and
         * then apply the simplex method. 
         * 
         * The augmented matrix is provided. 
         * 
         * We begin by forming the transpose of this matrix. We then interpret
         * the new matrix as a maximisation problem by introducing new variables.
         * 
         * The corresponding maximisation problem is the dual of the original
         * minimisation problem.
         * 
         * The original values corresponding to the optimal solution are obtained
         * from the entries in the bottom row corresponding to slack variable
         * columns.
         * 
         * The fact that a dual maximisation problem has the same solution as its
         * original minimisation problem is stated formally in a result called the
         * von Neumann Duality Principle.
         */
        #endregion

        int nVal = augMat.GetLength(1) - 1; // Number of values to be found

        float[,] _augMat = Transpose(augMat); // Find the transpose of the augmented matrix
        float[,] _tableau = ArtificialVariables(_augMat); // Insert the artificial (slack) variables
        float[,] tableau = NegativeObjFunc(_tableau); // Make the objective function values negative
        float[,] finalTableau = SimplexMethod(tableau); // Perform the simplex method
        float[] bottomRow = ExtractBottomRow(finalTableau); // Extract the bottom row of tableau
        float[] values = ExtractArtVars(bottomRow, nVal); // Extract the artificial values corresponding to the original variables
        return values;
    }

    private static float[,] Transpose(float[,] matrix)
    {
        int nRow = matrix.GetLength(1);
        int nCol = matrix.GetLength(0);

        float[,] transpose = new float[nRow, nCol];

        for (int r = 0; r < nRow; r++)
            for (int c = 0; c < nCol; c++)
                transpose[r, c] = matrix[c, r];

        return transpose;
    }

    private static float[,] ArtificialVariables(float[,] augMat)
    {
        int nRow = augMat.GetLength(0);
        int nCol = augMat.GetLength(1) + (nRow - 1);

        float[,] tableau = new float[nRow, nCol];

        #region MatrixValues
        for (int r = 0; r < nRow; r++)
            for (int c = 0; c < augMat.GetLength(1) - 1; c++)
                tableau[r, c] = augMat[r, c];
        #endregion

        #region b-Values
        for (int r = 0; r < nRow; r++)
            tableau[r, nCol - 1] = augMat[r, augMat.GetLength(1) - 1];
        #endregion

        #region ArtificialValues
        for (int r = 0; r < nRow - 1; r++)
            tableau[r, r + augMat.GetLength(1) - 1] = 1f;
        #endregion

        return tableau;
    }

    private static float[,] SimplexMethod(float[,] tableau)
    {
        bool terminate = false;

        while (!terminate)
        {
            int enteringColumn = SelectEntering(tableau);

            if (enteringColumn >= 0)
            {
                int departingRow = SelectDeparting(tableau, enteringColumn);

                if (departingRow >= 0)
                {
                    tableau = Pivot(tableau, enteringColumn, departingRow);
                }
                else terminate = true;
            }
            else terminate = true;
        }

        return tableau;
    }

    private static int SelectEntering(float[,] tableau)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        float minVal = float.MaxValue;
        int enteringColumn = -3;

        for (int c = 0; c < nCol; c++)
            if (tableau[nRow - 1, c] < minVal)
            {
                minVal = tableau[nRow - 1, c];
                enteringColumn = c;
            }

        if (minVal < 0)
        {
            if (minVal == float.MaxValue)
            {
                Debug.Log("No minimum value found for entering column.");
                return -2;
            }
            else
            {
                return enteringColumn;
            }
        }
        else
        {
            Debug.Log("No negative values for entering column.");
            return -1;
        }
    }

    private static int SelectDeparting(float[,] tableau, int enteringColumn)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        float minRatio = float.MaxValue;
        int departingRow = -3;

        for (int r = 0; r < nRow - 1; r++)
        {
            float ratio = tableau[r, nCol - 1] / tableau[r, enteringColumn];

            if (ratio > 0)
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    departingRow = r;
                }
        }

        if (minRatio <= 0)
        {
            Debug.Log("No positive ratios for departing row.");
            return -1;
        }
        else
        {
            if (minRatio == float.MaxValue)
            {
                Debug.Log("No minimum ratio found for departing row.");
                return -2;
            }
            else
            {
                return departingRow;
            }
        }
    }

    private static float[,] Pivot(float[,] tableau, int enteringColumn, int departingRow)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        #region ScaleRowToPivotValue1
        float scaleValue = tableau[departingRow, enteringColumn];
        for (int c = 0; c < nCol; c++)
            tableau[departingRow, c] = tableau[departingRow, c] / scaleValue;
        #endregion

        #region SetColsToValue0
        for (int r = 0; r < nRow; r++)
            if (r != departingRow)
            {
                float coefficient = -tableau[r, enteringColumn];
                for (int c = 0; c < nCol; c++)
                    tableau[r, c] = (coefficient * tableau[departingRow, c]) + tableau[r, c];
            }
        #endregion

        return tableau;
    }

    private static float[,] NegativeObjFunc(float[,] tableau)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        for (int c = 0; c < nCol; c++)
            tableau[nRow - 1, c] = -tableau[nRow - 1, c];

        return tableau;
    }

    private static float[] ExtractBottomRow(float[,] tableau)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        float[] bottomRow = new float[nCol];
        for (int c = 0; c < nCol; c++)
            bottomRow[c] = tableau[nRow - 1, c];

        return bottomRow;
    }

    private static float[] ExtractArtVars(float[] bottomRow, int nVal)
    {
        float[] values = new float[nVal];

        int v = -1;
        for (int i = bottomRow.Length - nVal - 1; i < bottomRow.Length - 1; i++)
        {
            v++;
            values[v] = bottomRow[i];
        }

        return values;
    }
}
