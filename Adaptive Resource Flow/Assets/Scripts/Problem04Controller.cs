using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem04Controller : MonoBehaviour
{
	[Range(8, 24)]
	public int numNodes = 20;
	[Range(0.1f, 0.5f)]
	public float productionProb = 0.25f;

	private List<SystemNode> node;

	private int frameCounter;
	private bool allNodesGenerated;

	void OnEnable()
	{
		frameCounter = 0;
		allNodesGenerated = false;
		
		node = new List<SystemNode>();
		float[] prod = new float[1]; prod[0] = +1f;
		node.Add(new SystemNode(new Vector3(0, 0, 1), prod, 1f));
		prod = new float[1]; prod[0] = -0.4f;
		node.Add(new SystemNode(new Vector3(-0.6f, 0, 0), prod, 1f));
		prod = new float[1]; prod[0] = -0.6f;
		node.Add(new SystemNode(new Vector3(+0.6f, 0, 0), prod, 1f));
		for (int a = 0; a < node.Count; a++)
		{
			for (int b = 0; b < node.Count; b++)
			{
				if (b != a) node[a].AddDistanceEdge(new DistanceEdge(node[b], (node[a].pos - node[b].pos).magnitude));
			}
		}

        LinearProgrammingSolution solution = LinearProgramming.MinimumCostFlow(node);

        Debug.Log("Minimum cost: " + solution.cost);
        string flows = "";
        foreach (float f in solution.flows)
        {
            flows += f;
            flows += ", ";
        }
        Debug.Log("Flows: " + flows);
	}

	void Update()
	{
		//frameCounter++;
//
		//if (!allNodesGenerated)
		//{
		//	if (frameCounter % 10 == 0)
		//	{
		//		GenerateNode();
//
		//		if (node.Count == numNodes)
		//		{
		//			allNodesGenerated = true;
		//			Debug.Log("All nodes generated.");
		//		}
		//	}
		//}
	}

	void OnDrawGizmos()
	{
		// Draw nodes
		if (node != null)
		{
			foreach (SystemNode n in node)
			{
				if (n.prod[0] > 0) Gizmos.color = Color.blue;
				else if (n.prod[0] < 0) Gizmos.color = Color.red;
				else Gizmos.color = Color.green;
				//Gizmos.DrawSphere(n.pos, n.maxOut / 5f);
				Gizmos.DrawSphere(n.pos, 0.1f);
			}
		}

		// Draw distance edges
		if (node != null)
		{
			Gizmos.color = Color.white;
			foreach (SystemNode n in node)
			{
				foreach (DistanceEdge d in n.distance)
				{
					Gizmos.DrawLine(n.pos, d.target.pos);
				}
			}
		}
	}

	private void GenerateNode()
	{
		if (node == null) node = new List<SystemNode>();

		int n = node.Count;

		// Position
		float x = Random.value * numNodes; // [0, numNodes]
		float y = Random.value * numNodes; // [0, numNodes]
		Vector3 pos = new Vector3(x, 0, y);

		// Production
		float[] prod = new float[1];
		if (Random.value < productionProb) prod[0] = (Random.value - 0.5f) * 2f; // [-1, +1]
		else prod[0] = 0f;

		// Infrastructure
		float maxOut = (Random.value * 0.5f) + 0.5f; // [+0.5, +1.0]

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
