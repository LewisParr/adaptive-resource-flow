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
    public FacilityObject()
    {

    }
}

public class BodyObject : AstroObject
{
    public BodyObject()
    {

    }
}

public class SystemObject : AstroObject
{
    public SystemObject()
    {

    }
}
