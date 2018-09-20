using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Problem07LinearProgramming
{
    private static void Maximise(float[,] augMat, bool dual = false)
    {
        if (dual)
        {
            float[,] tableau = augMat;
            SimplexMethod(tableau);
        }
        else
        {
            Debug.LogError("None-dual problem maximisation not implemented.");
        }
    }

    private static void Minimise(float[,] augMat)
    {
        float[,] _augMat = Transpose(augMat);
        float[,] tableau = ArtificialVariables(_augMat);
        Maximise(tableau, true);
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

    private static void SimplexMethod(float[,] tableau)
    {
        bool terminate = false;

        while (!terminate)
        {
            int enteringColumn = SelectEntering(tableau);

            if (enteringColumn >= 0)
            {
                int departingRow = SelectDeparting(tableau);
            }
        }
    }

    private static int SelectEntering(float[,] tableau)
    {
        int nRow = tableau.GetLength(0);
        int nCol = tableau.GetLength(1);

        float minVal = float.MaxValue;
        int enteringColumn;

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

    private static int SelectDeparting(float[,] tableau)
    {

    }
}
