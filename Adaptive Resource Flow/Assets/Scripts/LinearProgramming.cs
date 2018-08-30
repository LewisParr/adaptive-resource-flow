using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinearProgramming
{
	public static void Minimise(List<SystemNode> systemNode)
	{
        // Take a copy of the network
        List<SystemNode> copy = new List<SystemNode>();
        foreach (SystemNode n in systemNode) copy.Add(n.TakeCopy());

        Debug.Log("Beginning minimisation linear programming.");

        //Surrogate(copy);
        Surrogate(systemNode);

        //DefineProblem(copy);
        DefineProblem(systemNode);
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
            systemNode[i].maxOut = float.MaxValue;
        }
    }

    private static void DefineProblem(List<SystemNode> systemNode)
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
}
