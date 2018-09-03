using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
	public Node target;

	public Edge()
	{

	}
}

public class DistanceEdge : Edge
{
	public float distance;

    public DistanceEdge(Node target, float distance)
	{
		this.target = target;
		this.distance = distance;
	}
}

public class ResourceEdge : Edge
{
    public float magnitude;

    public ResourceEdge(Node target, float magnitude)
    {
        this.target = target;
        this.magnitude = magnitude;
    }
}