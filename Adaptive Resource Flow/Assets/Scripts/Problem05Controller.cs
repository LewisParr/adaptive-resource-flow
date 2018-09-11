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

        //float[] prod;
        //
        //prod = new float[1]; prod[0] = 0f;
        //node.Add(new SystemNode(new Vector3(0, 0, 0), prod, 1f));
        //prod = new float[1]; prod[0] = 1f;
        //node.Add(new SystemNode(new Vector3(3, 0, 3), prod, 1f));
        //prod = new float[1]; prod[0] = 0f;
        //node.Add(new SystemNode(new Vector3(2, 0, 1), prod, 1f));
        //prod = new float[1]; prod[0] = -1f;
        //node.Add(new SystemNode(new Vector3(0, 0, 2), prod, 1f));
        //prod = new float[1]; prod[0] = 0f;
        //node.Add(new SystemNode(new Vector3(2, 0, 4), prod, 1f));
        //
        //for (int a = 0; a < node.Count; a++)
        //{
        //    for (int b = 0; b < node.Count; b++)
        //    {
        //        if (b != a)
        //        {
        //            node[a].AddDistanceEdge(new DistanceEdge(node[b], (node[a].pos - node[b].pos).magnitude));
        //        }
        //    }
        //}
        //
        //IndependentLP.MinimumCostFlow(node);
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
