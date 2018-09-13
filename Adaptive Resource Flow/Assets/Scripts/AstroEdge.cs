using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroEdge
{
    private AstroObject target; public AstroObject Target { get { return Target; } }

    public AstroEdge(AstroObject target)
    {
        this.target = target;
    }
}

public class SystemEdge : AstroEdge
{
    private float distance; public float Distance { get { return distance; } }

    public SystemEdge(AstroObject target, float distance) : base(target)
    {
        this.distance = distance;
    }
}
