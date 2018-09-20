using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Problem07LinearProgramming
{
    public static float[,] Maximise(float[,] augMat, bool dual = false)
    {
        if (dual)
        {
            float[,] tableau = augMat;
            float[,] finalTableau = SimplexMethod(tableau);
            return finalTableau;
        }
        else
        {
            Debug.LogError("None-dual problem maximisation not implemented.");
            return null;
        }
    }

    public static float[,] Minimise(float[,] augMat)
    {
        float[,] _augMat = Transpose(augMat);
        float[,] tableau = ArtificialVariables(_augMat);
        float[,] finalTableau = Maximise(tableau, true);
        return finalTableau;
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
}
