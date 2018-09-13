using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroObject
{
    private Vector3 pos; public Vector3 Position { get { return pos; } }

    public AstroObject(Vector3 pos)
    {
        this.pos = pos;
    }
}

public class FacilityObject : AstroObject
{
    private float[] prod; public float[] Production { get { return prod; } }
    private float[] imexcap; public float[] ImportExportCapacity { get { return imexcap; } }
    private float[] imextax; public float[] ImportExportTax { get { return imextax; } }

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

    public BodyObject(Vector3 pos, float[] intcap, float[] inttax, float[] imexcap, float[] imextax) 
        : base(pos)
    {
        this.intcap = intcap;
        this.inttax = inttax;
        this.imexcap = imexcap;
        this.imextax = imextax;
    }
}

public class SystemObject : AstroObject
{
    public SystemObject(Vector3 pos) : base(pos)
    {

    }
}
