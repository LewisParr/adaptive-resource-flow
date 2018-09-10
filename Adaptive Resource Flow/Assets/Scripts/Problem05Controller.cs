using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem05Controller : MonoBehaviour
{
    [Range(8, 24)]
    public int numNodes = 20;
    [Range(0.1f, 0.5f)]
    public float productionProb = 0.25f;

    private List<SystemNode> node;

    private int frameCounter;
    private bool allNodesGenerated;
    private bool flowsCalculated;

    private void OnEnable()
    {
        frameCounter = 0;
        allNodesGenerated = false;
        flowsCalculated = false;

        node = new List<SystemNode>();
    }

    private void Update()
    {
        frameCounter++;

        if (!allNodesGenerated)
        {
            if (frameCounter % 10 == 0)
            {
                GenerateNode();

                if (node.Count == numNodes)
                {
                    allNodesGenerated = true;
                    Debug.Log("All nodes generated.");
                }
            }
        }
        else
        {
            if (!flowsCalculated)
            {
                IndependentLP.MinimumCostFlow(node);

                flowsCalculated = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw nodes
        if (node != null)
        {
            foreach (SystemNode n in node)
            {
                if (n.prod[0] > 0) Gizmos.color = Color.blue;
                else if (n.prod[0] < 0) Gizmos.color = Color.red;
                else Gizmos.color = Color.green;

                Gizmos.DrawSphere(n.pos, 0.1f);
            }
        }
    }

    private void GenerateNode()
    {
        if (node == null) node = new List<SystemNode>();

        int n = node.Count;

        // Position
        float x = Random.value * numNodes;
        float y = Random.value * numNodes;
        Vector3 pos = new Vector3(x, 0, y);

        // Production
        float[] prod = new float[1];
        if (Random.value < productionProb) prod[0] = (Random.value - 0.5f) * 2f;
        else prod[0] = 0f;

        // Infrastructure
        float maxOut = (Random.value * 0.5f) + 0.5f;

        // Create node
        node.Add(new SystemNode(pos, prod, maxOut));

        // Add new distance edges
        for (int i = 0; i < n; i++)
        {
            node[i].AddDistanceEdge(new DistanceEdge(node[n], (node[n].pos - node[i].pos).magnitude));
            node[n].AddDistanceEdge(new DistanceEdge(node[i], (node[i].pos - node[n].pos).magnitude));
        }
    }
}
