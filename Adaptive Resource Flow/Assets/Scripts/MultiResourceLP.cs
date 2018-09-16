using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MultiResourceLP
{
    public static void MinimumCostFlow(List<SystemObject> system, List<BodyObject> body, List<FacilityObject> facility)
    {
        int numSys = system.Count;
        int numBod = body.Count;
        int numFac = facility.Count;
        int numRes = facility[0].Production.Length;

        Debug.Log("Number of systems: " + numSys);
        Debug.Log("Number of bodies: " + numBod);
        Debug.Log("Number of facilities: " + numFac);
        Debug.Log("Number of resources: " + numRes);

        int numNode = (5 * numSys) + (5 * numBod) + (3 * numFac);
        int numEdge = ((numSys * (numSys - 1)) + (5 * numSys) + (6 * numBod) + (4 * numFac)) * numRes;

        Debug.Log("Number of nodes: " + numNode);
        Debug.Log("Number of edges: " + numEdge);

        int numFlowCons = numNode * numRes;
        int numCapCons = (5 * numSys) + (4 * numBod) + (2 * numFac);

        Debug.Log("Number of flow conservation constraints: " + numFlowCons);
        Debug.Log("Number of capacity constraints: " + numCapCons);

        int numRow = numFlowCons + numCapCons + 1;
        int numCol = numEdge + 1;

        Debug.Log("Number of augmented matrix rows: " + numRow);
        Debug.Log("Number of augmented matrix columns: " + numCol);

        Debug.Log("Components counted at: " + Time.realtimeSinceStartup);

        float[,] cost = new float[numNode, numNode];
        float[,] capacity = new float[numNode, numNode];

        for (int s1 = 0; s1 < numSys; s1++)
        {
            // Insert intersystem edge costs
            for (int s2 = 0; s2 < numSys; s2++)
            {
                // s1 external emitter node index
                int a = (5 * s1) + 1;

                // s2 external receiver node index
                int b = (5 * s2);

                // Cost is distance between them
                float c = (system[s1].Position - system[s2].Position).magnitude;

                //Debug.Log("System " + s1 + " to " + s2 + " costs " + c);

                // Insert values
                cost[a, b] = c;
                capacity[a, b] = Mathf.Infinity;
            }

            // Insert intrasystem edge costs

            // External receiver node
            int ser = (5 * s1);

            // External emitter node
            int see = (5 * s1) + 1;

            // Central node
            int sc = (5 * s1) + 2;

            // Internal receiver node
            int sir = (5 * s1) + 3;

            // Internal emitter node
            int sie = (5 * s1) + 4;

            // Insert throughflow tax
            cost[ser, see] = system[s1].ThroughflowTax;
            capacity[ser, see] = system[s1].ThroughflowCapacity;

            // Insert import/export taxes
            cost[ser, sc] = system[s1].ImportExportTax[0];
            capacity[ser, sc] = system[s1].ImportExportCapacity[0];
            cost[sc, see] = system[s1].ImportExportTax[1];
            capacity[sc, see] = system[s1].ImportExportCapacity[1];

            // Insert internal taxes
            cost[sir, sc] = system[s1].InternalTax[0];
            capacity[sir, sc] = system[s1].InternalCapacity[0];
            cost[sc, sie] = system[s1].InternalTax[1];
            capacity[sc, sie] = system[s1].InternalCapacity[1];

            foreach (BodyObject bo in system[s1].Body)
            {
                int b = BodyIndex(bo, body);

                // External receiver node
                int ber = (5 * b) + (5 * numSys);

                // External emitter node
                int bee = (5 * b) + 1 + (5 * numSys);

                // Insert system-body edge costs
                cost[sie, ber] = (system[s1].Position - body[b].Position).magnitude;
                capacity[sie, ber] = Mathf.Infinity;
                cost[bee, sir] = (system[s1].Position - body[b].Position).magnitude;
                capacity[bee, sir] = Mathf.Infinity;

                // Insert intrabody edge costs

                // Central node
                int bc = (5 * b) + 2 + (5 * numSys);

                // Internal receiver node
                int bir = (5 * b) + 3 + (5 * numSys);

                // Internal emitter node
                int bie = (5 * b) + 4 + (5 * numSys);

                // Insert import/export taxes
                cost[ber, bc] = body[b].ImportExportTax[0];
                capacity[ber, bc] = body[b].ImportExportCapacity[0];
                cost[bc, bee] = body[b].ImportExportTax[1];
                capacity[bc, bee] = body[b].ImportExportCapacity[1];

                // Insert internal taxes
                cost[bir, bc] = body[b].InternalTax[0];
                capacity[bir, bc] = body[b].InternalCapacity[0];
                cost[bc, bie] = body[b].InternalTax[1];
                capacity[bc, bie] = body[b].InternalCapacity[1];

                foreach (FacilityObject fo in bo.Facility)
                {
                    int f = FacilityIndex(fo, facility);

                    // Receiver node
                    int fr = (3 * f) + (5 * numSys) + (5 * numBod);

                    // Emitter node
                    int fe = (3 * f) + 1 + (5 * numSys) + (5 * numBod);

                    // Insert body-facility edge costs
                    cost[bie, fr] = (body[b].Position - facility[f].Position).magnitude;
                    capacity[bie, fr] = Mathf.Infinity;
                    cost[fe, bir] = (body[b].Position - facility[f].Position).magnitude;
                    capacity[fe, bir] = Mathf.Infinity;

                    // Insert intrafacility edge costs

                    // Producer node
                    int fp = (3 * f) + 2 + (5 * numSys) + (5 * numBod);

                    // Insert import/export taxes
                    cost[fr, fp] = facility[f].ImportExportTax[0];
                    capacity[fr, fp] = facility[f].ImportExportCapacity[0];
                    cost[fp, fe] = facility[f].ImportExportTax[1];
                    capacity[fp, fe] = facility[f].ImportExportCapacity[1];
                }
            }
        }

        // Set all zero costs to infinity
        int noConnection = 0;
        for (int n1 = 0; n1 < numNode; n1++)
        {
            for (int n2 = 0; n2 < numNode; n2++)
            {
                if (cost[n1, n2] == 0) { noConnection++; cost[n1, n2] = Mathf.Infinity; }
            }
        }
        int numNonInf = (numNode * numNode) - noConnection;

        Debug.Log("Number of non-infinite edge costs: " + numNonInf);

        Debug.Log("Edge costs and capacities stored at: " + Time.realtimeSinceStartup);

        // Instantiate edge index matrix
        int i = -1;
        int[,] edgeind = new int[cost.GetLength(0), cost.GetLength(1)];
        for (int n = 0; n < cost.GetLength(0); n++)
        {
            for (int _n = 0; _n < cost.GetLength(1); _n++)
            {
                if (cost[n, _n] != Mathf.Infinity)
                {
                    i++;
                    edgeind[n, _n] = i;
                }
                else
                {
                    edgeind[n, _n] = -1;
                }
            }
        }

        // Instantiate augmented matrix
        float[,] augmat = new float[numRow, numCol];

        // Insert flow conservation values
        for (int r = 0; r < numRes; r++)
        {
            for (int n = 0; n < numNode; n++)
            {
                for (int _n = 0; _n < numNode; _n++)
                {
                    // Check for edge to _n
                    if (cost[n, _n] != Mathf.Infinity)
                    {
                        augmat[n, (r * numNonInf) + edgeind[n, _n]] = +1;
                    }

                    // Check for edge from _n
                    if (cost[_n, n] != Mathf.Infinity)
                    {
                        augmat[n, (r * numNonInf) + edgeind[_n, n]] = -1;
                    }
                }

                // Insert b-value
                if (n >= (5 * numSys) + (5 * numBod))
                {
                    // This is a facility node
                    if ((n - (5 * numSys) - (5 * numBod)) % 2 == 0)
                    {
                        // This is a facility production node
                        augmat[n, numCol - 1] = facility[((n - (5 * numSys) - (5 * numBod)) - 2) / 3].Production[r];
                    }
                    else augmat[n, numCol - 1] = 0;
                }
                else augmat[n, numCol - 1] = 0;
            }
        }

        // Insert edge capacity constraint values
        int iCapCon = -1;
        for (int e = 0; e < numEdge; e++)
        {
            int[] nodes = EdgeNodesFromIndex(edgeind, Mathf.FloorToInt(e / numRes));

            // Check if capacity is constrained
            if (capacity[nodes[0], nodes[1]] != Mathf.Infinity)
            {
                iCapCon++;

                // Insert edge capacity constraint values
                augmat[iCapCon + numNode, iCapCon] = +1; // Identify edge
                augmat[iCapCon + numNode, numCol - 1] = capacity[nodes[0], nodes[1]]; // Insert b-value

                // Insert objective function values
                augmat[numRow - 1, iCapCon] = cost[nodes[0], nodes[1]];
            }
        }

        Debug.Log("Augmented matrix constructured at: " + Time.realtimeSinceStartup);

        // Process the minimisation problem
        float[] output = Minimise(augmat);

        Debug.Log("Linear programming completed at: " + Time.realtimeSinceStartup);

        // Interpret result
        //Debug.Log("Length of output: " + output.Length);
    }

    private static int BodyIndex(BodyObject b, List<BodyObject> l)
    {
        int i = -1;
        foreach (BodyObject c in l)
        {
            i++;
            if (c == b)
            {
                return i;
            }
        }
        Debug.LogError("Object not found.");
        return -1;
    }

    private static int FacilityIndex(FacilityObject f, List<FacilityObject> l)
    {
        int i = -1;
        foreach (FacilityObject c in l)
        {
            i++;
            if (c == f)
            {
                return i;
            }
        }
        Debug.LogError("Object not found.");
        return -1;
    }

    private static int[] EdgeNodesFromIndex(int[,] edgeind, int e)
    {
        for (int a = 0; a < edgeind.GetLength(0); a++)
        {
            for (int b = 0; b < edgeind.GetLength(1); b++)
            {
                if (edgeind[a, b] == e)
                {
                    int[] nodes = new int[2];
                    nodes[0] = a;
                    nodes[1] = b;
                    return nodes;
                }
            }
        }
        Debug.LogError("Edge index not found.");
        return null;
    }

    private static float[] Minimise(float[,] augmat)
    {
        // Form the transpose of the augmented matrix
        float[,] _augmat = Transpose(augmat);

        // Form the dual maximisation problem
        float[,] tableau = InsertSlackVariables(_augmat);

        Debug.Log("Number of tableau rows: " + tableau.GetLength(0));
        Debug.Log("Number of tableau columns: " + tableau.GetLength(1));

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
        int iterCount = 0;
        bool terminate = false;
        while (!terminate)
        {
            iterCount++;

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

            // !!!
            //if (iterCount == 1000) terminate = true;

            Debug.Log("Simplex Method pass " + iterCount + " completed at: " + Time.realtimeSinceStartup);
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
