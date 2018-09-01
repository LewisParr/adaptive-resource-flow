using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinearProgramming
{
    public static void Maximise()
    {
        /*
         * A linear programming problem is in STANDARD FORM if it seeks to MAXIMISE the objective
         * function z = c1x1 + c2x2 + ... + cnxn subject to the constraints:
         * a11x1 + a12x2 + ... + a1nxn <= b1
         * a21x1 + a22x2 + ... + a2nxn <= b2
         *  ...     ...    ...    ...    ...
         * am1x1 + am2x2 + ... + amnxn <= bm
         * where xi >= 0 and bi >= 0.
         * After adding slack variables, the corresponding system of CONSTRAINT EQUATIONS is:
         * a11x1 + a12x2 + ... + a1nxn + s1           = b1
         * a21x1 + a22x2 + ... + a2nxn      + s2      = b2
         *  ...     ...    ...    ...                  ...
         * am1x1 + am2x2 + ... + amnxn           + sm = bm
         * where si >= 0.
         */

        /*
         * Note that for a linear programming problem in standard form, the objective is to be 
         * maximised, not minimised. 
         */

        /* A BASIC SOLUTION of a linear programming problem in standard form is a solution
         * (x1, x2, ..., xn, s1, s2, ..., sm) of the constraint equations in which AT MOST m variables
         * are nonzero - the variables that are nonzero are called BASIC VARIABLES. A basic solution
         * for which all variables are nonnegative is called a BASIC FEASIBLE SOLUTION.
         */

        /*
         * The simplex method is carried out by performing elementary row operations on a matrix that 
         * we call the SIMPLEX TABLEAU. This tableau consists of the augmented matrix corresponding 
         * to the constraint equations together with the coefficients of the objective function written
         * in the form
         * -c1x1 - c2x2 - ... - cnxn + (0)s1 + (0)s2 + ... + (0)sm + z = 0
         * In the tableau, it is customary to omit the coefficient of z. For instance, the simplex
         * tableau for the linear programming problem
         * z = 4x1 + 6x2       objective function
         * x1, x2 >= 0         constraint
         * -x1 + x2 + s1 = 11  constraint
         * x1 + x2 + s2 = 27   constraint
         * 2x1 + 5x2 + s3 = 90 constraint
         * is as follows.
         */

        /*
         * x1   x2   s1   s2   s3   b   basic variables
         * -1   +1   +1    0    0   11   s1
         * +1   +1    0   +1    0   27   s2
         * +2   +5    0    0   +1   90   s3
         * ---------------------------
         * -4   -6    0    0    0    0 <-- current z-value
         */

        /*
         * From this INITIAL SIMPLEX TABLEAU, the BASIC VARIABLES are s1, s2, and s3, and the 
         * NONBASIC VARIABLES (which have a value of zero) are x1 and x2. Hence from the two columns
         * that are farthest to the right, we see that the current solution is
         * x1 = 0,   x2 = 0,   s1 = 11,   s2 = 27,   s3 = 90.
         * This solution is a basic feasible solution and is often written as
         * (x1, x2, s1, s2, s3) = (0, 0, 11, 27, 90).
         */

        /*
         * The entry in the lower-right corner of the simplex tableau is the current value of z. Note
         * that the bottom-row entries under x1 and x2 are negatives of the coefficients of x1 and x2
         * in the objective function
         * z = 4x1 + 6x2.
         * To perform an OPTIMALITY CHECK for a solution represented by a simplex tableau, we look at
         * the entries in the bottom row of the tableau. If any of these entries are negative (as above),
         * then the current solution is NOT optimal.
         */

        /*
         * Once we have set up the initial simplex tableau for a linear programming problem, the simplex
         * method consists of checking for optimality and then, if the current solution is not optimal, 
         * improving the current solution. (An improved solution is one that has a larger z-value than
         * the current solution.) To improve the current solution, we bring a new basic variable into the
         * solution - we call this variable the ENTERING VARIABLE. This implies that one of the current
         * basic variables must leave, otherwise we would have too many variables for a basic solution - 
         * we call this variable the DEPARTING VARIABLE. We choose the entering and departing variables 
         * as follows.
         * 1. The ENTERING VARIABLE corresponds to the smallest (the most negative) entry in the bottom
         *    row of the tableau.
         * 2. The DEPARTING VARIABLE corresponds to the smallest nonnegative ratio of bi / aij, in the 
         *    column determined by the entering variable.
         * 3. The entry in the simplex tableau in the entering variable's column and the departing 
         *    variable's row is called the pivot.
         * Finally, to form the improve solution, we apply Gauss-Jordan elimination to the column that 
         * contains the pivot, as illustrated in the following example. (This process is called 
         * PIVOTING.)
         */

        /*
         * From example above. To improve this solution, we determine that x2 is the entering variable,
         * because -6 is the smallest entry in the bottom row.
         * To find the departing variable, we locate the bi's that have corresponding positive elements 
         * in the entering variables column and form the following ratios.
         * 11/1 = 11,   27/1 = 27,   90/5 = 18
         * Here the smallest positive ratio is 11, so we choose s1 as the departing variable.
         */

        /*
         * Note that the pivot is the entry in the first row and second column. Now, we use Gauss-Jordan
         * elimination to obtain the following improved solution.
         * x1   x2   s1   s2   s3   b              x1   x2   s1   s2   s3   b    basic variables
         * -1   +1   +1    0    0   11     -->     -1   +1   +1    0    0   11   x2
         * +1   +1    0   +1    0   27     -->     +2    0   -1   +1    0   16   s2
         * +2   +5    0    0   +1   90     -->     +7    0   -5    0   +1   35   s3
         * ---------------------------             ---------------------------
         * -4   -6    0    0    0    0     -->     -10   0   +6    0    0   66
         * Note that x2 has replaced s1 in the basis column and the improved solution
         * (x1, x2, s1, s2, s3) = (0, 11, 0, 16, 35)
         * has a z-value of 
         * z = 4x1 + 6x2 = 4(0) + 6(11) = 66.
         */

        /*
         * The improved solution is not yet optimal since the bottom row still has a negative entry.
         * Thus, we can apply another iteration of the simplex method to further improve out solution.
         * We choose x1 as the entering variable. The smallest nonnegative ratio of
         * 11/(-1) = -11,   16/2 = 8,   35/7 = 5
         * is 5, so s3 is the departing variable. Gauss-Jordan elimination produces the following.
         * x1   x2   s1   s2   s3   b    -->   x1   x2   s1   s2   s3   b    -->   x1   x2   s1   s2   s3   b
         * -1   +1   +1    0    0   11   -->   -1   +1   +1    0    0   11   -->    0   +1   2/7   0   1/7  16
         * +2    0   -1   +1    0   16   -->   +2    0   -1   +1    0   16   -->    0    0   3/7  +1  -2/7  6
         * +7    0   -5    0   +1   35   -->   +1    0  -5/7   0   1/7  5    -->   +1    0  -5/7   0   1/7  5
         * ---------------------------         ---------------------------         ---------------------------
         * -10   0   +6    0    0   66   -->   -10   0   +6    0    0   66   -->    0    0  -8/7   0  10/7  116
         * In this tableau, there is still a negative entry in the bottom row. Thus, we choose s1 as
         * the entering variable and s2 as the departing variable.
         * By performing one more iteration of the simplex method, we obtain the following tableau.
         * x1   x2   s1   s2   s3   b    basic variables
         *  0   +1    0  -2/3  1/3  12   x2
         *  0    0   +1   7/3 -2/3  14   s1
         * +1    0    0   5/3 -1/3  15   x1
         * ---------------------------
         *  0    0    0   8/3  2/3  132 <-- maximum z-value
         * In this tableau, there are no negative elements in the bottom row. We have therefore
         * determined the optimal solution to be
         * (x1, x2, s1, s2, s3) = (15, 12, 14, 0, 0)
         * with
         * z = 4x1 + 6x2 = 4(15) + 6(12) = 132.
         */

        /*
         * Ties may occur in choosing entering and/or departing variables. Should this happen, any 
         * choice among the tied variables may be made.
         */

        /* 
         * To solve a linear programming problem in standard form, use the following steps.
         * 1. Convert each inequality in the set of constraints to an equation by adding slack variables.
         * 2. Create the initial simplex tableau.
         * 3. Locate the most negative entry in the bottom row. The column for this entry is called the
         *    ENTERING COLUMN. (If ties occur, any of the tied entries can be used to determine the 
         *    entering column.)
         * 4. Form the ratios of the entries in the "b-column" with their corresponding positive entries
         *    in the entering column. The DEPARTING ROW corresponds to the smallest nonnegative ratio
         *    bi / aij. (If all entries in the entering column are 0 or negative, then there is no
         *    maximum solution. For ties, choose either entry.) The entry in the departing row and the
         *    entering column is called the PIVOT.
         * 5. Use elementary row operations so that the pivot is 1, and all other entries in the entering
         *    column are 0. This process is called PIVOTING. 
         * 6. If all entries in the bottom row are zero or positive, this is the final tableau. If not, 
         *    go back to Step 3. 
         * 7. If you obtain a final tableau, then the linear programming problem has a maximum solution, 
         *    which is given by the entry in the lower-right corner of the tableau. 
         */

        /* 
         * Note that the basic feasible solution of an initial simplex tableau is
         * (x1, x2, ..., xn, s1, s2, ..., sm) = (0, 0, ..., 0, b1, b2, ..., bm).
         * This solution is basic because at most m variables are nonzero (namely the slack variables).
         * If is feasible because each variable is nonnegative.
         */

        int numRow = 4;
        int numCol = 6;

        // Create a test tableau with two decision variables (this is Step 2)
        float[,] tableau = new float[numRow, numCol]; // (row, col)
        tableau[0, 0] = -1; tableau[0, 1] = 1; tableau[0, 2] = 1; tableau[0, 3] = 0; tableau[0, 4] = 0; tableau[0, 5] = 11;
        tableau[1, 0] = 1; tableau[1, 1] = 1; tableau[1, 2] = 0; tableau[1, 3] = 1; tableau[1, 4] = 0; tableau[1, 5] = 27;
        tableau[2, 0] = 2; tableau[2, 1] = 5; tableau[2, 2] = 0; tableau[2, 3] = 0; tableau[2, 4] = 1; tableau[2, 5] = 90;
        tableau[3, 0] = -4; tableau[3, 1] = -6; tableau[3, 2] = 0; tableau[3, 3] = 0; tableau[3, 4] = 0; tableau[3, 5] = 0;

        PrintTableau(tableau);

        // Step 3
        // Locate the most negative entry in the bottom row.
        int enteringColumn = SelectEntering(tableau);
        
        if (enteringColumn != -1)
        {
            // Step 4
            // Locate the smallest nonnegative ratio bi / aij.
            int departingRow = SelectDeparting(tableau, enteringColumn);

            if (departingRow != -1)
            {
                // Step 5
                // Set pivot to 1, all others to zero.
                tableau = Pivot(tableau, enteringColumn, departingRow);
            }
        }

        PrintTableau(tableau);
    }

    public static int SelectEntering(float[,] tableau)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minVal = float.MaxValue;
        int enteringColumn = 0;
        for (int c = 0; c < numCol; c++)
        {
            if (tableau[numRow - 1, c] < minVal)
            {
                minVal = tableau[numRow - 1, c];
                enteringColumn = c;
            }
        }
        if (!(minVal < 0))
        {
            Debug.Log("No bottom row values are nonnegative.");
            return -1;
        }
        else
        {
            if (minVal == float.MaxValue)
            {
                Debug.Log("No minimum value was found.");
                return -1;
            }
            else
            {
                Debug.Log("Entering column: " + enteringColumn);
                return enteringColumn;
            }
        }
    }

    public static int SelectDeparting(float[,] tableau, int enteringColumn)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minRatio = float.MaxValue;
        int departingRow = 0;
        for (int r = 0; r < numRow - 1; r++)
        {
            float ratio = tableau[r, numCol - 1] / tableau[r, enteringColumn];
            if (!(ratio < 0))
            {
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    departingRow = r;
                }
            }
        }
        if (minRatio == float.MaxValue)
        {
            Debug.Log("No minimum ratio was found.");
            return -1;
        }
        else
        {
            Debug.Log("Departing row: " + departingRow);
            return departingRow;
        }
    }

    public static float[,] Pivot(float[,] tableau, int enteringColumn, int departingRow)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float[,] newTableau = new float[numRow, numCol];
        for (int r = 0; r < numRow; r++)
        {
            if (r != departingRow)
            {
                for (int c = 0; c < numCol; c++)
                {
                    newTableau[r, c] = ((-1 * tableau[r, enteringColumn]) * tableau[departingRow, c]) + tableau[r, c];
                }
            }
            else
            {
                for (int c = 0; c < numCol; c++)
                {
                    newTableau[r, c] = tableau[r, c];
                }
            }
        }
        return newTableau;
    }

    public static void PrintTableau(float[,] tableau)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        Debug.Log("Rows: " + numRow + ", Columns: " + numCol);

        for (int r = 0; r < numRow; r++)
        {
            string row = "";
            for (int c = 0; c < numCol; c++)
            {
                row += tableau[r, c];
                row += " ; ";
            }
            Debug.Log(row);
        }
    }

    public static void Minimise(List<SystemNode> systemNode)
	{
        // Take a copy of the network
        List<SystemNode> copy = new List<SystemNode>();
        foreach (SystemNode n in systemNode) copy.Add(n.TakeCopy());

        Debug.Log("Beginning minimisation linear programming.");

        //Surrogate(copy);
        Surrogate(systemNode);

        //DefineProblem(copy);
        float[,] matrix = DefineProblem(systemNode);

        PrintMatrix(matrix);
        matrix = SimplexMethodIteration(matrix);
        PrintMatrix(matrix);
        matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        //matrix = SimplexMethodIteration(matrix);
        PrintMatrix(matrix);
    }

    private static void Surrogate(List<SystemNode> systemNode)
    {
        Debug.Log("Inserting surrogate nodes.");
        /*
         * For each system node, add a surrogate node with edges
         * connecting only to and from the original node. These 
         * edges represent flow out of / into the system and are
         * where the infrastructure constraints can be applied.
         */

        int nOriginal = systemNode.Count;
        for (int i = 0; i < nOriginal; i++)
        {
            int j = systemNode.Count;

            // Create surrogate node
            float[] prod = new float[1];
            prod[0] = systemNode[i].prod[0];
            systemNode.Add(new SystemNode(systemNode[i].pos - new Vector3(0, 0.5f, 0), prod, systemNode[i].maxOut));

            // Create surrogate edges
            systemNode[i].AddDistanceEdge(new DistanceEdge(systemNode[j], 0f));
            systemNode[j].AddDistanceEdge(new DistanceEdge(systemNode[i], 0f));

            // Set original production to zero
            for (int k = 0; k < systemNode[i].prod.Length; k++) systemNode[i].prod[k] = 0;

            // Set original max outflow to infinite
            systemNode[i].maxOut = Mathf.Infinity;
        }
    }

    private static float[,] DefineProblem(List<SystemNode> systemNode)
    {
        // 1 objective function
        // An augmented constraint for each node
        // A flow/capacity constraint for each edge

        // Find the number of nodes and edges
        int numNodes = 0;
        int numEdges = 0;
        foreach (SystemNode n in systemNode)
        {
            numNodes++;
            foreach (DistanceEdge d in n.distance)
            {
                numEdges++;
            }
        }

        Debug.Log("Number of nodes: " + numNodes);
        Debug.Log("Number of edges: " + numEdges);

        // Width of matrix (number of columns) is:
        // 1 for Z
        // 1 for each flow variable (1 for each edge)
        // 1 artificial variable for each constraint
        //  (conservation of flow at each node and 
        //  capacity limit of each edge)
        // Artificial variables are needed for the node
        //  constraints because producers may not flow
        //  out all that is produced
        // 1 for values of each constraint

        // Height of matrix (number of rows) is:
        // 1 for objective function
        // 1 for each node
        // 1 for each edge

        // Collect edge costs
        float[] edgeCost = new float[numEdges];
        int[] edgeSource = new int[numEdges];
        int[] edgeTarget = new int[numEdges];

        int nodeIndex = -1;
        int edgeIndex = -1;
        foreach (SystemNode n in systemNode)
        {
            nodeIndex++;
            foreach (DistanceEdge d in n.distance)
            {
                edgeIndex++;

                // Record edge cost
                edgeCost[edgeIndex] = d.distance;

                // Record source node index
                edgeSource[edgeIndex] = nodeIndex;

                // Find and record target node index
                Node targetNode = d.target;
                for (int i = 0; i < systemNode.Count; i++)
                    if (systemNode[i] == targetNode)
                        edgeTarget[edgeIndex] = i;
            }
        }

        int numCol = 1 + numEdges + numNodes + numEdges + 1;
        int numRow = 1 + numNodes + numEdges;

        // Create matrix
        float[,] m = new float[numCol, numRow];

        // Add objective function variables
        m[0, 0] = 1f;
        for (int c = 1; c < numEdges + 1; c++)
        {
            // m[c, 0] is the cost of that edge per unit flow
            m[c, 0] = -edgeCost[c - 1];
        }

        // Add flow conservation variables for nodes
        for (int r = 1; r < numNodes + 1; r++)
        {
            // A row corresponds to each node
            for (int c = 1; c < numEdges + 1; c++)
            {
                // If the corresponding edge with index (c-1)
                // flows out of node with index (r-1) then assign
                // +1; if it flows into node then assign -1; 
                // otherwise leave at 0.
                if (edgeSource[c-1] == (r-1))
                {
                    //Debug.Log("Edge " + (c-1).ToString() + " flows out of node " + (r-1).ToString());
                    m[c, r] = +1;
                }
                else if (edgeTarget[c-1] == (r-1))
                {
                    //Debug.Log("Edge " + (c - 1).ToString() + " flows into node " + (r - 1).ToString());
                    m[c, r] = -1;
                }
                else
                {
                    //Debug.Log("Edge " + (c - 1).ToString() + " does not flow into or out of node " + (r - 1).ToString());
                }

                // Add artificial variable
                m[1 + numEdges + (r - 1), r] = +1;
            }
        }

        // Add capacity constraint variables for edges
        for (int r = numNodes + 1; r < numNodes + 1 + numEdges; r++)
        {
            // A row corresponds to each edge
            // edge index: r - numNodes - 1

            // Assign +1 to column corresponding to this edge
            // Edge index: c - 1
            m[r - numNodes, r] = +1;

            // Add +1 to column corresponding to this artificial variable
            m[r - numNodes + numNodes + numEdges, r] = +1;
        }

        // Add constraint values to final column
        // Objective function
        m[numCol - 1, 0] = 0f;
        // Each node's production rate
        for (int r = 1; r < numNodes + 1; r++)
        {
            m[numCol - 1, r] = systemNode[r - 1].prod[0];
        }
        // Each edge's capacity
        for (int r = numNodes + 1; r < numNodes + 1 + numEdges; r++)
        {
            // If the source node of this edge has a maxOut value
            // then apply it as the capacity of this edge.
            // Should be infinite for edges between variables that
            // are not surrogates.

            //Debug.Log(systemNode[edgeSource[r - numNodes - 1]].maxOut);
            m[numCol - 1, r] = systemNode[edgeSource[r - numNodes - 1]].maxOut;
        }

        // Output matrix
        return m;
    }

    private static float[,] SimplexMethodIteration(float[,] matrix)
    {
        // Entering variable selection
        // Select the most negative entry in the objective row
        int numCol = matrix.GetLength(0);
        int numRow = matrix.GetLength(1);

        // Find minimum value
        float minObjValue = float.MaxValue;
        for (int col = 0; col < numCol; col++) if (matrix[col, 0] < minObjValue) minObjValue = matrix[col, 0];

        PrintMatrixTopRow(matrix);
        Debug.Log("Pivot column value: " + minObjValue);

        // Check if minimum value is negative
        if (minObjValue < 0)
        {
            // Find column index of minimum value
            int pivotCol = -1;
            int c = 0;
            while (pivotCol == -1)
            {
                c++;
                if (matrix[c, 0] == minObjValue) pivotCol = c;
            }

            Debug.Log("Pivot column: " + pivotCol);

            // Leaving variable selection
            // Divide the final column values by the value in
            // that row at the pivot column. Select the lowest
            // value.
            float minEndValue = float.MaxValue;
            for (int row = 1; row < numRow; row++)
            {
                float endValue = matrix[numCol - 1, row];
                float pivotValue = matrix[pivotCol, row];
                float testValue = endValue / pivotValue;
                if (testValue < minEndValue)
                {
                    // Do not allow selection of negative infinity
                    if (testValue != -Mathf.Infinity)
                    {
                        //Debug.Log("End value: " + endValue + "; pivot value: " + pivotValue + ";  calculated value: " + testValue);
                        minEndValue = testValue;
                    }
                    else
                    {
                        //Debug.Log("Resulting value is negative infinity.");
                    }
                }
            }
            if (minEndValue == float.MaxValue) Debug.LogError("No suitable leaving variable was found.");

            PrintMatrixTestValue(matrix, pivotCol);
            Debug.Log("Pivot row value: " + minEndValue);

            // Find row index of minimum value
            int pivotRow = -1;
            int r = 0;
            while (pivotRow == -1)
            {
                r++;
                if (matrix[numCol - 1, r] / matrix[pivotCol, r] == minEndValue) pivotRow = r;
            }

            Debug.Log("Pivot row: " + pivotRow);

            // Adjust each other row so that value in pivot column
            // becomes zero.
            for (int row = 0; row < numRow; row++)
            {
                if (row == pivotRow)
                {
                    // Do nothing, leave values as they are
                }
                else
                {
                    for (int col = 0; col < numCol; col++)
                    {
                        // Calculate new value
                        float newThisRowVal = (-matrix[pivotCol, pivotRow] * matrix[col, row]) + matrix[col, pivotRow];

                        // Apply new value
                        matrix[col, row] = newThisRowVal;
                    }
                }
            }
        }
        else Debug.Log("Entering variable is not negative.");

        // Output adjusted matrix
        return matrix;
    }

    private static void PrintMatrix(float[,] m)
    {
        int numCol = m.GetLength(0);
        int numRow = m.GetLength(1);

        Debug.Log("Col: " + numCol + ", Row: " + numRow);

        // Display matrix
        for (int r = 0; r < numRow; r++)
        {
            string row = "";
            for (int c = 0; c < numCol; c++)
            {
                row += m[c, r];
                row += " ; ";
            }
            Debug.Log(row);
        }
    }

    private static void PrintMatrixTopRow(float[,] m)
    {
        int numCol = m.GetLength(0);

        string row = "";
        for (int c = 0; c < numCol; c++)
        {
            row += m[c, 0];
            row += " ; ";
        }
        Debug.Log("Top row: " + row);
    }

    private static void PrintMatrixTestValue(float[,] m, int c)
    {
        int numCol = m.GetLength(0);
        int numRow = m.GetLength(1);

        string row = "";
        for (int r = 0; r < numRow; r++)
        {
            row += (m[numCol - 1, r] / m[c, r]);
            row += " ; ";
        }
        Debug.Log("Test values: " + row);
    }
}
