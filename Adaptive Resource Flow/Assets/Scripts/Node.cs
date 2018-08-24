using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public Vector3 pos;
	public float[] prod;
	public float maxOut;

	public List<Edge> edge;

	public Node(Vector3 pos, float[] prod, float maxOut)
	{
		this.pos = pos;
		this.prod = prod;
		this.maxOut = maxOut;
	}

	public void AddEdge(Edge edge)
	{
		if (this.edge == null)
		{
			this.edge = new List<Edge>();
		}

		this.edge.Add(edge);
	}
}
