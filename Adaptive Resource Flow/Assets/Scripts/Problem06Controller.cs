﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem06Controller : MonoBehaviour
{
    [Range(4, 20)]
    public float width = 12;
    [Range(4, 20)]
    public float height = 12;
    [Range(3, 24)]
    public int numSys = 20;
    [Range(0.1f, 0.5f)]
    public float productionProb = 0.25f;
    [Range(1, 10)]
    public int numResources = 2;

    private List<SystemObject> system;
    private List<BodyObject> body;
    private List<FacilityObject> facility;

    private int frameCounter;
    private bool allNodesGenerated;
    private bool flowsCalculated;

    private void OnEnable()
    {
        frameCounter = 0;
        allNodesGenerated = false;
        flowsCalculated = false;
    }

    private void Update()
    {
        frameCounter++;

        if (!allNodesGenerated)
        {
            if (frameCounter % 10 == 0)
            {
                GenerateSystem();

                if (system.Count == numSys)
                {
                    allNodesGenerated = true;
                }
            }
        }
        else
        {
            if (!flowsCalculated)
            {
                MultiResourceLP.MinimumCostFlow(system, body, facility);

                flowsCalculated = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (system != null)
        {
            Gizmos.color = Color.yellow;
            foreach (SystemObject s in system)
            {
                Gizmos.DrawWireSphere(s.Position, 0.25f);
                if (s.Body != null)
                {
                    foreach (BodyObject b in s.Body)
                    {
                        Gizmos.DrawLine(s.Position, b.Position);
                    }
                }
            }
        }
        if (body != null)
        {
            Gizmos.color = Color.white;
            foreach (BodyObject b in body)
            {
                Gizmos.DrawSphere(b.Position, 0.1f);
                if (b.Facility != null)
                {
                    foreach (FacilityObject f in b.Facility)
                    {
                        Gizmos.DrawLine(b.Position, f.Position);
                    }
                }
            }
        }
        if (facility != null)
        {
            Gizmos.color = Color.green;
            foreach (FacilityObject f in facility)
            {
                Gizmos.DrawSphere(f.Position, 0.05f);
            }
        }
    }

    private void GenerateSystem()
    {
        if (system == null) system = new List<SystemObject>();
        if (body == null) body = new List<BodyObject>();
        if (facility == null) facility = new List<FacilityObject>();

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
        int si = system.Count - 1;

        // Number of planetary bodies
        int numBody = Mathf.FloorToInt(Random.value * 8);

        for (int b = 0; b < numBody; b++)
        {
            // Position
            float angle = b * ((2 * Mathf.PI) / numBody);
            float radius = 0.75f;
            float _x = radius * Mathf.Sin(angle);
            float _y = radius * Mathf.Cos(angle);
            Vector3 _pos = pos + new Vector3(_x, 0, _y);

            // Internal capacities
            float[] _intcap = new float[2];
            _intcap[0] = 2; _intcap[1] = 2;

            // Internal taxes
            float[] _inttax = new float[2];
            _inttax[0] = 0.05f; _inttax[1] = 0.05f;

            // Import/Export capacities
            float[] _imexcap = new float[2];
            _imexcap[0] = 2; _imexcap[1] = 2;

            // Import/Export taxes
            float[] _imextax = new float[2];
            _imextax[0] = 0.10f; _imextax[1] = 0.10f;

            // Instantiate body
            body.Add(new BodyObject(_pos, _intcap, _inttax, _imexcap, _imextax));
            int bi = body.Count - 1;

            // Connect to system
            system[si].Body.Add(body[bi]);
            body[bi].PlanetarySystem = system[si];

            // Number of facilities
            int numFacility = Mathf.FloorToInt(Random.value * 16);

            for (int f = 0; f < numFacility; f++)
            {
                // Position
                float _angle = f * ((2 * Mathf.PI) / numFacility);
                float _radius = 0.25f;
                float __x = _radius * Mathf.Sin(_angle);
                float __y = _radius * Mathf.Cos(_angle);
                Vector3 __pos = _pos + new Vector3(__x, 0, __y);

                // Production
                float[] prod = new float[numResources];
                for (int r = 0; r < numResources; r++)
                {
                    if (Random.value < productionProb)
                    {
                        prod[r] = Random.value - 0.5f;
                    }
                }

                // Import/Export capacity
                float[] __imexcap = new float[2];
                __imexcap[0] = 1; __imexcap[1] = 1;

                // Import/Export taxes
                float[] __imextax = new float[2];
                __imextax[0] = 0.10f; __imextax[1] = 0.10f;

                // Instantiate facility
                facility.Add(new FacilityObject(__pos, prod, __imexcap, __imextax));
                int fi = facility.Count - 1;

                // Connect to body
                body[bi].Facility.Add(facility[fi]);
                facility[fi].PlanetaryBody = body[bi];
            }
        }
    }
}
