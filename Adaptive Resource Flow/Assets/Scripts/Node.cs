using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public Vector3 pos;
	public float[] prod;

	public Node(Vector3 pos, float[] prod)
	{
		this.pos = pos;
		this.prod = prod;
	}
}
