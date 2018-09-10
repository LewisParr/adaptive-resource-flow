using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IndependentLP
{
    public static void MinimumCostFlow(List<SystemNode> originalNodes)
    {
        float[][] top = new float[3][];
        top[0] = new float[2];
        top[0][0] = 0.4f;

        Debug.Log("Performing minimum-cost flow analysis.");

        // Collect node production data
        float[][] prod = CollectProduction(originalNodes);





        // Take a copy of the network
        //List<SystemNode> node = new List<SystemNode>();
        //foreach (SystemNode n in originalNodes) node.Add(n.TakeCopy());

        List<SystemNode> node = CopyNetwork(originalNodes);

        Debug.Log("Network copy taken.");

        // Insert surrogate nodes
        List<Vector2Int> nodePair = Surrogate(node);
        int nSurrogate = nodePair.Count;

        Debug.Log(nSurrogate + " surrogate nodes inserted.");

        // Count nodes and edges
        int numNodes = 0; int numEdges = 0;
        foreach (SystemNode n in node)
        {
            numNodes++;
            foreach (DistanceEdge d in n.distance) numEdges++;
        }

        Debug.Log("Nodes: " + numNodes + "; Edges: " + numEdges);
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

    private static List<SystemNode> CopyNetwork(List<SystemNode> originalNodes)
    {
        // Count nodes
        int numNodes = 0;
        foreach (SystemNode n in originalNodes) numNodes++;

        // Collect max outflow data
        float[] nodeMaxOut = new float[numNodes];
        for (int n = 0; n < numNodes; n++)
        {
            nodeMaxOut[n] = originalNodes[n].maxOut;
        }

        // Create distance matrix
        float[,] distance = new float[numNodes, numNodes]; // Matrix of distances between all nodes
        for (int currentNode = 0; currentNode < numNodes; currentNode++)
        {
            foreach (DistanceEdge d in originalNodes[currentNode].distance)
            {
                int targetNode = -1;
                int candidateNode = -1;
                while (targetNode == -1)
                {
                    candidateNode++;
                    if (d.target == originalNodes[candidateNode]) targetNode = candidateNode;
                    if (candidateNode == numNodes)
                    {
                        Debug.LogError("Target node not found.");
                        Application.Quit();
                    }
                }
                distance[currentNode, targetNode] = d.distance;
            }
        }

        return originalNodes;
    }

    private static List<Vector2Int> Surrogate(List<SystemNode> node)
    {
        int nOriginal = node.Count;
        List<Vector2Int> nodePair = new List<Vector2Int>();
        
        for (int i = 0; i < nOriginal; i++)
        {
            int j = node.Count;

            // Copy original node's production
            float[] prod = new float[node[i].prod.Length];
            for (int p = 0; p < node[i].prod.Length; p++) prod[p] = node[i].prod[p];

            Vector3 tempOffset = new Vector3(0, 0.5f, 0);

            // Create surrogate node
            node.Add(new SystemNode(node[i].pos - tempOffset, prod, node[i].maxOut, true));

            // Create surrogate edges
            node[i].AddDistanceEdge(new DistanceEdge(node[j], 0.01f));
            node[j].AddDistanceEdge(new DistanceEdge(node[i], 0.01f));

            // Set original production to zero
            for (int p = 0; p < node[i].prod.Length; p++) node[i].prod[p] = 0f;

            // Set original max outflow to infinity
            node[i].maxOut = Mathf.Infinity;

            // Record original-surrogate node pairs
            nodePair.Add(new Vector2Int(i, j)); // (original node index, surrogate node index)
        }

        return nodePair;
    }
}
