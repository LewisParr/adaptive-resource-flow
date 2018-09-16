using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem07Controller : MonoBehaviour
{
    public float width = 12f;
    public float height = 10f;
    public int delay = 20;

    private List<SystemObject> system;
    private List<BodyObject> body;
    private List<FacilityObject> facility;

    enum ElementType { None, System, Body, Facility };

    private int frame = 0;

    private void OnEnable()
    {
        /*
         * Initialise world components.
         */

        system = new List<SystemObject>();
        body = new List<BodyObject>();
        facility = new List<FacilityObject>();
    }

    private void Update()
    {
        frame++;

        if (frame % delay == 0)
        {
            AddElement();
        }
    }

    public void OnDrawGizmos()
    {
        if (system != null)
        {
            foreach (SystemObject s in system)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(s.Position, 0.25f);

                if (s.Body != null)
                {
                    for (int b = 0; b < s.Body.Count; b++)
                    {
                        float angle = b * ((2 * Mathf.PI) / s.Body.Count);
                        float radius = 0.75f;
                        Vector3 _pos = new Vector3(radius * Mathf.Sin(angle), 0, radius * Mathf.Cos(angle));
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(s.Position + _pos, 0.10f);

                        if (s.Body[b].Facility != null)
                        {
                            for (int f = 0; f < s.Body[b].Facility.Count; f++)
                            {
                                float _angle = f * ((2 * Mathf.PI) / s.Body[b].Facility.Count);
                                float _radius = 0.25f;
                                Vector3 __pos = new Vector3(_radius * Mathf.Sin(_angle), 0, _radius * Mathf.Cos(_angle));
                                Gizmos.color = Color.green;
                                Gizmos.DrawWireSphere(s.Position + _pos + __pos, 0.05f);
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddElement()
    {
        /*
         * Add a new system, body, or facility to the world.
         */

        ElementType elementType = ChooseElementType();               // Choose element type

        Debug.Log("Chosen element: " + elementType.ToString());

        if (elementType == ElementType.System) AddSystem();          // Add a system, ...
        else if (elementType == ElementType.Body) AddBody();         // or a body, ...
        else if (elementType == ElementType.Facility) AddFacility(); // or a facility, ...
        else { };                                                    // or nothing at all. 
    }

    private ElementType ChooseElementType()
    {
        ElementType elementType = ElementType.None;
        if (system.Count == 0)
        {
            elementType = ElementType.System;
        }
        else if (body.Count == 0)
        {
            elementType = ElementType.Body;
        }
        else if (facility.Count == 0)
        {
            elementType = ElementType.Facility;
        }
        else
        {
            int i = Mathf.FloorToInt(Random.value * 4);
            elementType = (ElementType)i;
        }
        return elementType;
    }

    private void AddSystem()
    {
        // Position
        float x = Random.value * width;
        float y = Random.value * height;
        Vector3 pos = new Vector3(x, 0, y);

        // Internal capacities
        float[] intcap = new float[2];
        intcap[0] = 4; intcap[1] = 4;

        // Internal taxes
        float[] inttax = new float[2];
        inttax[0] = 0.05f; inttax[1] = 0.05f;

        // Import/Export capacities
        float[] imexcap = new float[2];
        imexcap[0] = 4; imexcap[1] = 4;

        // Import/Export taxes
        float[] imextax = new float[2];
        imextax[0] = 0.10f; imextax[1] = 0.10f;

        // Throughflow capacity
        float thrucap = 4;

        // Throughflow tax
        float thrutax = 0.10f;

        // Instantiate system
        system.Add(new SystemObject(pos, intcap, inttax, imexcap, imextax, thrucap, thrutax));

        Debug.Log("System created.");
    }

    private void AddBody()
    {
        // Parent system
        int systemIndex = Mathf.FloorToInt(Random.value * system.Count);

        // Position
        Vector3 pos = system[systemIndex].Position + new Vector3(0, system[systemIndex].Body.Count, 0);

        // Internal capacities
        float[] intcap = new float[2];
        intcap[0] = 2; intcap[1] = 2;

        // Internal taxes
        float[] inttax = new float[2];
        inttax[0] = 0.05f; inttax[1] = 0.05f;

        // Import/Export capacities
        float[] imexcap = new float[2];
        imexcap[0] = 2; imexcap[1] = 2;

        // Import/Export taxes
        float[] imextax = new float[2];
        imextax[0] = 0.10f; imextax[1] = 0.10f;

        // Instantiate body
        body.Add(new BodyObject(pos, intcap, inttax, imexcap, imextax));

        // Connect to system
        system[systemIndex].Body.Add(body[body.Count - 1]);
        body[body.Count - 1].PlanetarySystem = system[systemIndex];

        Debug.Log("Body created.");
    }

    private void AddFacility()
    {
        // Parent body
        int bodyIndex = Mathf.FloorToInt(Random.value * body.Count);

        // Position
        Vector3 pos = body[bodyIndex].Position + new Vector3(0, 0, body[bodyIndex].Facility.Count);

        // Production
        float[] prod = new float[3];
        for (int r = 0; r < 3; r++)
        {
            if (Random.value < 0.25f)
            {
                prod[r] = Random.value - 0.5f;
            }
        }

        // Import/Export capacity
        float[] imexcap = new float[2];
        imexcap[0] = 1; imexcap[1] = 1;

        // Import/Export taxes
        float[] imextax = new float[2];
        imextax[0] = 0.10f; imextax[1] = 0.10f;

        // Instantiate facility
        facility.Add(new FacilityObject(pos, prod, imexcap, imextax));

        // Connect to body
        body[bodyIndex].Facility.Add(facility[facility.Count - 1]);
        facility[facility.Count - 1].PlanetaryBody = body[bodyIndex];

        Debug.Log("Facility created.");
    }
}
