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

        Debug.Log("Production data collected.");

        // Collect node max outflow data
        float[] maxout = CollectMaxOutflow(originalNodes);

        Debug.Log("Max outflow data collected.");

        // Build distance matrix
        float[,] distance = BuildDistanceMatrix(originalNodes);

        Debug.Log("Distance matrix built.");

        // Insert surrogate nodes
        Surrogate();
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

    private static void Surrogate()
    {

    }
}
