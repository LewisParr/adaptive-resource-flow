using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinearProgramming
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

    public static void MinimumCostFlow(List<SystemNode> systemNode)
    {
        Debug.Log("Performing minimum-cost flow analysis.");

        // Take a copy of the network
        List<SystemNode> copy = new List<SystemNode>();
        foreach (SystemNode n in systemNode) copy.Add(n.TakeCopy());

        Debug.Log("Network copy taken.");

        //Surrogate(copy);
        Surrogate(systemNode);

        Debug.Log("Surrogate nodes inserted.");

        //float[,] tableau = BuildTableau(copy);
        float[,] tableau = BuildTableau(systemNode);

        Debug.Log("Tableau constructed.");

        // ... transpose, etc. ...
    }

    public static void Maximise(float[,] tableau)
    {
        #region Description
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
        #endregion

        int numRow = 4;
        int numCol = 6;

        // Create a test tableau with two decision variables (this is Step 2)
        //float[,] tableau = new float[numRow, numCol]; // (row, col)
        //tableau[0, 0] = -1; tableau[0, 1] = 1; tableau[0, 2] = 1; tableau[0, 3] = 0; tableau[0, 4] = 0; tableau[0, 5] = 11;
        //tableau[1, 0] = 1; tableau[1, 1] = 1; tableau[1, 2] = 0; tableau[1, 3] = 1; tableau[1, 4] = 0; tableau[1, 5] = 27;
        //tableau[2, 0] = 2; tableau[2, 1] = 5; tableau[2, 2] = 0; tableau[2, 3] = 0; tableau[2, 4] = 1; tableau[2, 5] = 90;
        //tableau[3, 0] = -4; tableau[3, 1] = -6; tableau[3, 2] = 0; tableau[3, 3] = 0; tableau[3, 4] = 0; tableau[3, 5] = 0;

        tableau = SimplexMethod(tableau);
    }

    public static float[,] SimplexMethod(float[,] tableau)
    {
        bool terminate = false;
        while (!terminate)
        {
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
                    Pivot(tableau, enteringColumn, departingRow);
                }
                else
                {
                    terminate = true;
                }
            }
            else
            {
                terminate = true;
            }
        }
        return tableau;
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
                //Debug.Log("Entering column: " + enteringColumn);
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
            //Debug.Log("Departing row: " + departingRow);
            return departingRow;
        }
    }

    public static void Pivot(float[,] tableau, int enteringColumn, int departingRow)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);

        // Scale departing row so that pivot value is 1
        float scaleValue = tableau[departingRow, enteringColumn];
        for (int c = 0; c < numCol; c++)
        {
            tableau[departingRow, c] = tableau[departingRow, c] / scaleValue;
        }

        // Set other values in entering column to 0
        for (int r = 0; r < numRow; r++)
        {
            if (r != departingRow)
            {
                float coefficient = -tableau[r, enteringColumn];
                for (int c = 0; c < numCol; c++)
                {
                    tableau[r, c] = (coefficient * tableau[departingRow, c]) + tableau[r, c];
                }
            }
        }
    }

    public static void PrintTableau(float[,] tableau)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        //Debug.Log("Rows: " + numRow + ", Columns: " + numCol);

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

    public static void Minimise()
	{
        /* 
         * A minimisation problem is in STANDARD FORM if the objective function
         * x = c1x1 + c2x2 + ... + cnxn
         * is to be minimised, subject to the constraints
         * a11x1 + a12x2 + ... + a1nxn >= b1
         * a21x1 + a22x2 + ... + a2nxn >= b2
         *  ...  +  ...  + ... +  ...  >= ...
         * am1x1 + am2x2 + ... + amnxn >= bm
         * where xi >= 0 and bi >= 0.
         */

        /*
         * The basic procedure used to solve such a problem is to convert it to a MAXIMISATION PROBLEM 
         * in standard form, and then apply the simplex method.
         */

        /*
         * The first step in converting this problem to a maximisation problem is to form the augmented
         * matrix for this system of inequalities. To this augmented matrix we add a last row that
         * represents the coefficients of the objective function.
         * 
         * Next, we form the TRANSPOSE of this matrix by interchanging its rows and columns.
         * 
         * Finally, we interpret the new matrix as a MAXIMISATION problem as follows. (To do this, we
         * introduce new variables, yi). We call this corresponding maximisation problem the DUAL of the
         * original minimisation problem.
         */

        /*
         * 1. Form the AUGMENTED MATRIX for the given system of inequalities, and add a bottom row
         *    consisting of the coefficients of the objective function.
         *    a11   a12   ...   a1n   b1
         *    a21   a22   ...   a2n   b2
         *    ...   ...         ...   ...
         *    am1   am2   ...   amn   bm
         *    ---------------------------
         *     c1    c2   ...    cn   0 
         * 2. Form the TRANSPOSE of this matrix.
         *    a11   a21   ...   am1   c1
         *    a12   a22   ...   am2   c2
         *    ...   ...         ...   ...
         *    a1n   a2n   ...   amn   cn
         *    ---------------------------
         *     b1    b2   ...    bm   0
         * 3. Form the DUAL MAXIMISATION PROBLEM corresponding to this transposed matrix. That is, find
         *    the maximum of the objective function by 
         *    z = b1y1 + b2y2 + ... + bmym
         *    subject to the constraints
         *    a11y1 + a21y2 + ... + am1ym <= c1
         *    a12y1 + a22y2 + ... + am2ym <= c2
         *     ...  +  ...  + ... +  ...     ...
         *    a1ny1 + a2ny2 + ... + amnym <= cn
         *    where y1>= 0, y2 >= 0, ..., and ym >= 0.
         * 4. Apply the SIMPLEX METHOD to the dual maximisation problem. The maximum value of z will be
         *    the minimum value of w. Moreover, the values of x1, x2, ..., and xn will occur in the
         *    bottom row of the final simplex tableau, in the columns corresponding to the slack variables.
         */

        int numRow = 3;
        int numCol = 3;

        // Create a test augmented matrix with 2 decision variables (Step 1)
        float[,] matrix = new float[numRow, numCol]; // (row, col)
        matrix[0, 0] = 2f; matrix[0, 1] = 1f; matrix[0, 2] = 6f;
        matrix[1, 0] = 1f; matrix[1, 1] = 1f; matrix[1, 2] = 4f;
        matrix[2, 0] = 3f; matrix[2, 1] = 2f; matrix[2, 2] = 0f;

        // Step 2
        // Form the transpose of the augmented matrix.
        matrix = Transpose(matrix);

        // Step 3
        // Form the dual maximisation problem.
        float[,] tableau = InsertSlackVariables(matrix);

        // Step 4
        // Apply the simplex method.
        Maximise(tableau);
    }

    private static float[,] InsertSlackVariables(float[,] matrix)
    {
        /*
         * Insert a slack variable for each constraint.
         */
        
        int numRow = matrix.GetLength(0);
        int numCol = matrix.GetLength(1) + (numRow - 1); // (numRow - 1) is the number of constraints

        float[,] tableau = new float[numRow, numCol];

        // Insert matrix values
        for (int r = 0; r < numRow; r++)
        {
            for (int c = 0; c < matrix.GetLength(1); c++)
            {
                tableau[r, c] = matrix[r, c];
            }
        }

        // Insert slack values
        for (int r = 0; r < numRow - 1; r++)
        {
            /*
             * Constraint with index r has a slack variable in column matrix.GetLength(1) + r.
             */

            tableau[r, r + matrix.GetLength(1)] = 1f;
        }

        // Return tableau
        return tableau;
    }

    private static float[,] Transpose(float[,] matrix)
    {
        int numRow = matrix.GetLength(1);
        int numCol = matrix.GetLength(0);

        float[,] transpose = new float[numRow, numCol];

        for (int r = 0; r < numRow; r++)
        {
            for (int c = 0; c < numCol; c++)
            {
                transpose[r, c] = matrix[c, r];
            }
        }

        return transpose;
    }

    private static void Surrogate(List<SystemNode> systemNode)
    {
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

    public static float[,] BuildTableau(List<SystemNode> systemNode)
    {
        /*
         * From this INITIAL SIMPLEX TABLEAU, the BASIC VARIABLES are s1, s2, and s3, and the 
         * NONBASIC VARIABLES (which have a value of zero) are x1 and x2. Hence from the two columns
         * that are farthest to the right, we see that the current solution is
         * x1 = 0,   x2 = 0,   s1 = 11,   s2 = 27,   s3 = 90.
         * This solution is a basic feasible solution and is often written as
         * (x1, x2, s1, s2, s3) = (0, 0, 11, 27, 90).
         */

        /*
         * There is one objective function, an augmented constraint for each node, and a flow/capacity
         * constraint for each edge.
         */

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

        Debug.Log("Nodes and edges have been counted.");

        Debug.Log("Number of nodes: " + numNodes);
        Debug.Log("Number of edges: " + numEdges);

        // Collect edge data
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
                edgeCost[edgeIndex] = d.distance;
                edgeSource[edgeIndex] = nodeIndex;
                Node targetNode = d.target;
                for (int i = 0; i < systemNode.Count; i++)
                    if (systemNode[i] == targetNode)
                        edgeTarget[edgeIndex] = i;
            }
        }

        Debug.Log("Edge data collected.");

        /*
         * Columns in the tableau will represent the flow along each edge, the slack variables for 
         * each node constraint, the slack variables for each edge constraint, and the b column.
         *
         * Rows in the tableau will represent the node constraints, the edge constraints, and the 
         * objective function.
         */

        // Initialise tableau        
        int numRow = numNodes + numEdges + 1;
        int numCol = numEdges + numNodes + numEdges + 1;
        float[,] tableau = new float[numRow, numCol];

        Debug.Log("Tableau initialised.");

        // Add node flow conservation values
        for (int r = 0; r < numNodes; r++)
        {
            for (int c = 0; c < numEdges; c++)
            {
                /*
                 * If the corresponding edge with index c flows out of node with index r, then assign
                 * +1. If it flows into node with index r, then assign -1. Otherwise, assign 0.
                 * 
                 * Then add the artificial slack variable.
                 */
                
                if (edgeSource[c] == r)
                {
                    //Debug.Log("Edge " + c + " flows out of node " + r);
                    tableau[r, c] = 1f;
                }
                else if (edgeTarget[c] == r)
                {
                    //Debug.Log("Edge " + c + " flows into node " + r);
                    tableau[r, c] = -1f;
                }
                else
                {
                    //Debug.Log("Edge " + c + " does not interact with node " + r);
                    tableau[r, c] = 0f;
                }

                tableau[r, numEdges + r] = 1f;
            }
        }

        Debug.Log("Node flow conservation constraint values inserted.");

        // Add edge capacity constraint values
        for (int r = numNodes; r < numNodes + numEdges; r++)
        {
            /*
             * Assign +1 to the column corrsponding to this edge, with index r - numNodes.
             * 
             * Then add the artificial slack variable.
             */

            tableau[r, r - numNodes] = 1f;

            tableau[r, r - numNodes + numNodes + numEdges] = 1f;
        }

        Debug.Log("Edge capacity constraint values inserted.");

        // Add objective function values
        for (int c = 0; c < numEdges; c++)
        {
            /*
             * Insert the cost of the edge corresponding to the column with index c.
             */

            tableau[numRow - 1, c] = edgeCost[c];
        }

        Debug.Log("Objective function values inserted.");

        // Add b-values
        for (int r = 0; r < numNodes; r++)
        {
            /*
             * Insert the production rate value for node with index r.
             */

            tableau[r, numCol - 1] = systemNode[r].prod[0];
        }
        for (int r = numNodes; r < numNodes + numEdges; r++)
        {
            /*
             * If the source node of the edge with index r - numNodes has a maxOut value,
             * then apply it as the capacity of this edge.
             * 
             * If not, then the edge's capacity is infinite.
             */

            tableau[r, numCol - 1] = systemNode[edgeSource[r - numNodes]].maxOut;
        }

        Debug.Log("b-values have been inserted.");

        // Return the tableau
        return tableau;
    }
}
