using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IndependentLP
{
    public static void MinimumCostFlow(List<SystemNode> originalNodes)
    {
        Debug.Log("Performing minimum-cost flow analysis.");

        // Collect node production data
        float[][] prod = CollectProduction(originalNodes);

        Debug.Log("Production data collected.");

        // Collect node max outflow data
        float[] maxout = CollectMaxOutflow(originalNodes);

        Debug.Log("Max outflow data collected.");

        // Build distance matrix
        float[,] distance = BuildDistanceMatrix(originalNodes);

        Debug.Log("Distance matrix built.");

        // Insert surrogate node production
        prod = SurrogateProduction(prod);

        Debug.Log("Surrogate production inserted.");

        // Build capacity matrix
        float[,] capacity = BuildCapacityMatrix(maxout);

        Debug.Log("Capacity matrix built.");

        // Insert surrogate distances
        distance = SurrogateDistance(distance);

        Debug.Log("Surrogate distances inserted.");

        #region PrintAssembledData
        //Debug.Log("----- PRODUCTION -----");
        //for (int n = 0; n < prod.Length; n++)
        //{
        //    string s = "";
        //    for (int p = 0; p < prod[n].Length; p++)
        //    {
        //        s += prod[n][p];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}

        //Debug.Log("----- CAPACITY -----");
        //for (int a = 0; a < capacity.GetLength(1); a++)
        //{
        //    string s = "";
        //    for (int b = 0; b < capacity.GetLength(0); b++)
        //    {
        //        s += capacity[a, b];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}

        //Debug.Log("----- DISTANCE -----");
        //for (int a = 0; a < distance.GetLength(1); a++)
        //{
        //    string s = "";
        //    for (int b = 0; b < distance.GetLength(0); b++)
        //    {
        //        s += distance[a, b];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}
        #endregion

        // Build edge index matrix
        int[,] edgeind = BuildEdgeIndexMatrix(distance);

        // Build augmented matrix
        float[,] augmat = BuildAugmentedMatrix(distance, edgeind, prod, capacity);

        Debug.Log("Augmented matrix built.");

        #region PrintAugmentedMatrix
        //Debug.Log("----- INIT AUG MAT -----");
        //for (int a = 0; a < augmat.GetLength(0); a++)
        //{
        //    string s = "";
        //    for (int b = 0; b < augmat.GetLength(1); b++)
        //    {
        //        s += augmat[a, b];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}
        #endregion

        // Process the minimisation problem
        float[] output = Minimise(augmat);

        Debug.Log("Minimisation operation performed.");

        #region PrintOutput
        Debug.Log("----- RAW OUTPUT -----");
        string s = "";
        foreach (float o in output)
        {
            s += o;
            s += "; ";
        }
        Debug.Log(s);
        #endregion

        // Interpret result
        int numNode = distance.GetLength(0);
        int numEdge = ((numNode / 2) * ((numNode / 2) - 1)) + numNode;
        for (int e = 0; e < numEdge; e++)
        {
            int index = output.Length - numEdge - 1 + e;
            Debug.Log("Edge " + e + " has flow " + output[index]);
        }
    }

    private static float[][] CollectProduction(List<SystemNode> originalNodes)
    {
        float[][] prod = new float[originalNodes.Count][];
        for (int n = 0; n < originalNodes.Count; n++)
        {
            prod[n] = new float[originalNodes[n].prod.Length];
            for (int p = 0; p < originalNodes[n].prod.Length; p++) prod[n][p] = originalNodes[n].prod[p];
        }
        return prod;
    }

    private static float[] CollectMaxOutflow(List<SystemNode> originalNodes)
    {
        float[] maxout = new float[originalNodes.Count];
        for (int n = 0; n < originalNodes.Count; n++) maxout[n] = originalNodes[n].maxOut;
        return maxout;
    }

    private static float[,] BuildDistanceMatrix(List<SystemNode> originalNodes)
    {
        float[,] distance = new float[originalNodes.Count, originalNodes.Count];
        for (int currentNode = 0; currentNode < originalNodes.Count; currentNode++)
        {
            foreach (DistanceEdge d in originalNodes[currentNode].distance)
            {
                int targetNode = -1; int candidateNode = -1;
                while (targetNode == -1)
                {
                    candidateNode++;
                    if (d.target == originalNodes[candidateNode])
                    {
                        targetNode = candidateNode;
                    }
                    else if (candidateNode == originalNodes.Count)
                    {
                        Debug.LogError("Target node not found.");
                        Application.Quit();
                    }
                }
                distance[currentNode, targetNode] = d.distance;
            }
        }
        return distance;
    }

    private static float[][] SurrogateProduction(float[][] prod)
    {
        int _N = prod.Length;
        int _2N = _N * 2;

        float[][] newProd = new float[_2N][];

        for (int n = 0; n < _N; n++)
        {
            newProd[n] = new float[prod[n].Length];
            newProd[n + _N] = prod[n];
        }

        return newProd;
    }

    private static float[,] BuildCapacityMatrix(float[] maxout)
    {
        int _N = maxout.Length;
        int _2N = _N * 2;

        float[,] capacity = new float[_2N, _2N];

        for (int n = 0; n < _N; n++)
        {
            capacity[n + _N, n] = maxout[n];
        }

        for (int a = 0; a < _2N; a++)
            for (int b = 0; b < _2N; b++)
                if (capacity[a, b] == 0) capacity[a, b] = Mathf.Infinity;

        return capacity;
    }

    private static float[,] SurrogateDistance(float[,] distance)
    {
        int _N = distance.GetLength(0);
        int _2N = _N * 2;

        float[,] newDistance = new float[_2N, _2N];

        for (int a = 0; a < _N; a++)
            for (int b = 0; b < _N; b++)
                newDistance[a, b] = distance[a, b];

        for (int a = 0; a < _N; a++)
        {
            newDistance[a, a + _N] = 0.01f;
            newDistance[a + _N, a] = 0.01f;
        }

        for (int a = 0; a < _2N; a++)
            for (int b = 0; b < _2N; b++)
                if (newDistance[a, b] == 0) newDistance[a, b] = Mathf.Infinity;

        return newDistance;
    }

    private static int[,] BuildEdgeIndexMatrix(float[,] distance)
    {
        int i = -1;
        int[,] edgeind = new int[distance.GetLength(0), distance.GetLength(1)];
        for (int a = 0; a < distance.GetLength(0); a++)
        {
            for (int b = 0; b < distance.GetLength(1); b++)
            {
                if (distance[a, b] != Mathf.Infinity)
                {
                    i++;
                    edgeind[a, b] = i;
                }
                else
                {
                    edgeind[a, b] = -1;
                }
            }
        }
        return edgeind;
    }

    private static float[,] BuildAugmentedMatrix(float[,] distance, int[,] edgeind, float[][] prod, float[,] capacity)
    {
        // Initialise matrix
        int numNode = distance.GetLength(0);
        int numEdge = ((numNode / 2) * ((numNode / 2) - 1)) + numNode;
        int numRow = numNode + numEdge + 1;
        int numCol = numEdge + 1;
        float[,] augmat = new float[numRow, numCol];

        // Add node flow conservation values
        for (int n = 0; n < numNode; n++) // n <- current node index
        {
            for (int _n = 0; _n < numNode; _n++) // _n <- target node index
            {
                if (distance[n, _n] != Mathf.Infinity)
                {
                    augmat[n, edgeind[n, _n]] = 1;
                    augmat[n, edgeind[_n, n]] = -1;
                }
            }
        }

        // Add edge capacity constraint values
        for (int e = 0; e < numEdge; e++)
        {
            augmat[e + numNode, e] = 1;
        }

        // Add objective function values
        for (int e = 0; e < numEdge; e++)
        {
            augmat[numRow - 1, e] = EdgePropertyByIndex(edgeind, distance, e);
        }

        // Add b-values
        int p = 0;
        for (int n = 0; n < numNode; n++)
        {
            augmat[n, numCol - 1] = prod[n][p];
        }
        for (int e = 0; e < numEdge; e++)
        {
            augmat[e + numNode, numCol - 1] = EdgePropertyByIndex(edgeind, capacity, e);
        }

        // Return the completed matrix
        return augmat;
    }

    private static float EdgePropertyByIndex(int[,] edgeind, float[,] property, int i)
    {
        for (int a = 0; a < edgeind.GetLength(0); a++)
        {
            for (int b = 0; b < edgeind.GetLength(1); b++)
            {
                if (edgeind[a, b] == i)
                {
                    return property[a, b];
                }
            }
        }
        Debug.LogError("Edge index not found.");
        return Mathf.Infinity;
    }

    private static float[] Minimise(float[,] augmat)
    {
        // Form the transpose of the augmented matrix
        float[,] _augmat = Transpose(augmat);

        // Form the dual maximisation problem
        float[,] tableau = InsertSlackVariables(_augmat);

        // Process the maximisation problem
        float[] output = Maximise(tableau, true);

        // Return the result
        return output;
    }

    private static float[,] Transpose(float[,] augmat)
    {
        int numRow = augmat.GetLength(1);
        int numCol = augmat.GetLength(0);
        float[,] transpose = new float[numRow, numCol];
        for (int r = 0; r < numRow; r++)
            for (int c = 0; c < numCol; c++)
                transpose[r, c] = augmat[c, r];
        return transpose;
    }

    private static float[,] InsertSlackVariables(float[,] augmat)
    {
        int numRow = augmat.GetLength(0);
        int numCol = augmat.GetLength(1) + (numRow - 1);
        float[,] tableau = new float[numRow, numCol];

        // Insert matrix values (except b-values)
        for (int r = 0; r < numRow; r++)
            for (int c = 0; c < augmat.GetLength(1) - 1; c++)
                tableau[r, c] = augmat[r, c];

        // Insert b-values
        for (int r = 0; r < numRow; r++)
            tableau[r, numCol - 1] = augmat[r, augmat.GetLength(1) - 1];

        // Insert slack values
        for (int r = 0; r < numRow - 1; r++)
            tableau[r, r + augmat.GetLength(1) - 1] = 1f;

        // Return the completed tableau
        return tableau;
    }

    private static float[] Maximise(float[,] tableau, bool dual = false)
    {
        // Run the simplex method
        tableau = SimplexMethod(tableau);

        if (!dual)
        {
            // Read results normally
            Debug.LogError("NOT IMPLEMENTED");
            return null;
        }
        else
        {
            // Return the bottom row
            float[] output = new float[tableau.GetLength(1)];
            for (int c = 0; c < tableau.GetLength(1); c++)
                output[c] = tableau[tableau.GetLength(0) - 1, c];
            return output;
        }
    }

    private static float[,] SimplexMethod(float[,] tableau)
    {
        bool terminate = false;
        while (!terminate)
        {
            // Locate the most negative entry in the bottom row
            int enteringColumn = SelectEntering(tableau);

            if (enteringColumn != -1)
            {
                // Locate the smallest nonnegative ratio
                int departingRow = SelectDeparting(tableau, enteringColumn);

                if (departingRow != -1)
                {
                    // Set pivot to 1 all others to zero
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
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minVal = float.MaxValue;
        int enteringColumn = 0;
        for (int c = 0; c < numCol; c++)
            if (tableau[numRow - 1, c] < minVal)
            {
                minVal = tableau[numRow - 1, c];
                enteringColumn = c;
            }
        if (!(minVal < 0)) return -1;
        else
        {
            if (minVal == float.MaxValue) return -1;
            else return enteringColumn;
        }
    }

    private static int SelectDeparting(float[,] tableau, int enteringColumn)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minRatio = float.MaxValue;
        int departingRow = 0;
        for (int r = 0; r < numRow - 1; r++)
        {
            float ratio = tableau[r, numCol - 1] / tableau[r, enteringColumn];
            if (!(ratio < 0))
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    departingRow = r;
                }
        }
        if (minRatio == float.MaxValue) return -1;
        else return departingRow;
    }

    private static float[,] Pivot(float[,] tableau, int enteringColumn, int departingRow)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);

        // Scale departing row so that pivot value is 1
        float scaleValue = tableau[departingRow, enteringColumn];
        for (int c = 0; c < numCol; c++)
            tableau[departingRow, c] = tableau[departingRow, c] / scaleValue;

        // Set other values in entering column to 0
        for (int r = 0; r < numRow; r++)
            if (r != departingRow)
            {
                float coefficient = -tableau[r, enteringColumn];
                for (int c = 0; c < numCol; c++)
                    tableau[r, c] = (coefficient * tableau[departingRow, c]) + tableau[r, c];
            }

        // Return the new tableau
        return tableau;
    }
}
