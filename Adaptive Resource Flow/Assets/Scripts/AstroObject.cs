using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroObject
{
    private Vector3 pos; public Vector3 Position { get { return pos; } }
    private List<AstroEdge> edge; public List<AstroEdge> Edge { get { return edge; } }

    public AstroObject(Vector3 pos)
    {
        this.pos = pos;
    }

    public void AddEdge(AstroEdge edge)
    {
        if (this.edge == null) this.edge = new List<AstroEdge>();
        this.edge.Add(edge);
    }
}

public class FacilityObject : AstroObject
{
    private float[] prod; public float[] Production { get { return prod; } }
    private float[] imexcap; public float[] ImportExportCapacity { get { return imexcap; } }
    private float[] imextax; public float[] ImportExportTax { get { return imextax; } }
    private BodyObject body; public BodyObject PlanetaryBody { get { return body; } set { body = value; } }

    public FacilityObject(Vector3 pos, float[] prod, float[] imexcap, float[] imextax) : base(pos)
    {
        this.prod = prod;
        this.imexcap = imexcap;
        this.imextax = imextax;
    }
}

public class BodyObject : AstroObject
{
    private float[] intcap; public float[] InternalCapacity { get { return intcap; } }
    private float[] inttax; public float[] InternalTax { get { return inttax; } }
    private float[] imexcap; public float[] ImportExportCapacity { get { return imexcap; } }
    private float[] imextax; public float[] ImportExportTax { get { return imextax; } }
    private SystemObject system; public SystemObject PlanetarySystem { get { return system; } set { system = value; } }
    private List<FacilityObject> facility; public List<FacilityObject> Facility { get { return facility; } set { facility = value; } }

    public BodyObject(Vector3 pos, float[] intcap, float[] inttax, float[] imexcap, float[] imextax) 
        : base(pos)
    {
        this.intcap = intcap;
        this.inttax = inttax;
        this.imexcap = imexcap;
        this.imextax = imextax;
        facility = new List<FacilityObject>();
    }
}

public class SystemObject : AstroObject
{
    private float[] intcap; public float[] InternalCapacity { get { return intcap; } }
    private float[] inttax; public float[] InternalTax { get { return inttax; } }
    private float[] imexcap; public float[] ImportExportCapacity { get { return imexcap; } }
    private float[] imextax; public float[] ImportExportTax { get { return imextax; } }
    private float thrutax; public float ThroughflowTax { get { return thrutax; } }
    private List<BodyObject> body; public List<BodyObject> Body { get { return body; } set { body = value; } }

    public SystemObject(Vector3 pos, float[] intcap, float[] inttax, float[] imexcap, float[] imextax, 
        float thrutax) : base(pos)
    {
        this.intcap = intcap;
        this.inttax = inttax;
        this.imexcap = imexcap;
        this.imextax = imextax;
        this.thrutax = thrutax;
        body = new List<BodyObject>();
    }
}
