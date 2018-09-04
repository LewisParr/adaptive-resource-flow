using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public Vector3 pos;
    public bool surrogate;

	public List<DistanceEdge> distance;
    public List<ResourceEdge> resource;

	public Node(Vector3 pos, bool surrogate = false)
	{
		this.pos = pos;
        this.surrogate = surrogate;
	}

	public void AddDistanceEdge(DistanceEdge distance)
	{
		if (this.distance == null) this.distance = new List<DistanceEdge>();

		this.distance.Add(distance);
	}

    public void AddResourceEdge(ResourceEdge resource)
    {
        if (this.resource == null) this.resource = new List<ResourceEdge>();

        this.resource.Add(resource);
    }
}

public class SystemNode : Node
{
    public float[] prod;
    public float maxOut;

    public SystemNode(Vector3 pos, float[] prod, float maxOut, bool surrogate = false) : base(pos, surrogate)
    {
        this.pos = pos;
        this.prod = prod;
        this.maxOut = maxOut;
    }

    public SystemNode TakeCopy()
    {
        SystemNode copy = new SystemNode(this.pos, this.prod, this.maxOut);
        return copy;
    }
}
