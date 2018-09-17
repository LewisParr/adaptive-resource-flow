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
            UpdateResourceFlows();
        }
    }

    public void OnDrawGizmos()
    {
        if (system != null)
        {
            foreach (SystemObject s in system)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(s.Position, 0.30f);

                if (s.Body != null)
                {
                    for (int b = 0; b < s.Body.Count; b++)
                    {
                        float angle = b * ((2 * Mathf.PI) / s.Body.Count);
                        float radius = 1.20f;
                        Vector3 _pos = new Vector3(radius * Mathf.Sin(angle), 0, radius * Mathf.Cos(angle));
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(s.Position + _pos, 0.25f);

                        if (s.Body[b].Facility != null)
                        {
                            for (int f = 0; f < s.Body[b].Facility.Count; f++)
                            {
                                float _angle = f * ((2 * Mathf.PI) / s.Body[b].Facility.Count);
                                float _radius = 0.60f;
                                Vector3 __pos = new Vector3(_radius * Mathf.Sin(_angle), 0, _radius * Mathf.Cos(_angle));
                                Gizmos.color = Color.green;
                                Gizmos.DrawWireSphere(s.Position + _pos + __pos, 0.10f);

                                if (s.Body[b].Facility[f].Production != null)
                                {
                                    for (int p = 0; p < s.Body[b].Facility[f].Production.Length; p++)
                                    {
                                        float __angle = p * ((2 * Mathf.PI) / s.Body[b].Facility[f].Production.Length);
                                        float __radius = 0.20f;
                                        Vector3 ___pos = new Vector3(__radius * Mathf.Sin(__angle), 0, __radius * Mathf.Cos(__angle));
                                        if (s.Body[b].Facility[f].Production[p] != 0) Gizmos.color = Color.Lerp(Color.red, Color.blue, 0.5f + s.Body[b].Facility[f].Production[p]);
                                        else Gizmos.color = Color.white;
                                        Gizmos.DrawCube(s.Position + _pos + __pos + ___pos, new Vector3(0.10f, 0.10f, 0.10f));
                                    }
                                }
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

        //Debug.Log("System created.");
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

        //Debug.Log("Body created.");
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

        //Debug.Log("Facility created.");
    }

    private void UpdateResourceFlows()
    {
        /*
         * Update the flow of resources through the world.
         */

        if (system.Count > 0 && body.Count > 0 && facility.Count > 0)
        {
            BuildAugmentedMatrix();
        }
    }

    private void BuildAugmentedMatrix()
    {
        int nSys = system.Count;
        int nBod = body.Count;
        int nFac = facility.Count;
        int nRes = facility[0].Production.Length;

        int nSysNode = 5 * nSys;
        int nBodNode = 5 * nBod;
        int nFacNode = 3 * nFac;

        int nInterSysEdge = nSys * (nSys - 1);
        int nIntraSysEdge = 4 * nSys;
        int nSysBodEdge = 2 * nBod;
        int nIntraBodEdge = 4 * nBod;
        int nBodFacEdge = 2 * nFac;
        int nIntraFacEdge = 2 * nFac;

        int nNode = nSysNode + nBodNode + nFacNode;
        int nEdge = (nInterSysEdge + nIntraSysEdge + nSysBodEdge + nIntraBodEdge + nBodFacEdge
            + nIntraFacEdge) * nRes;

        int nFlowConstr = nNode * nRes;
        int nCapConstr = (nIntraSysEdge + nIntraBodEdge + nIntraFacEdge) * nRes;

        int nRow = nFlowConstr + nCapConstr;
        int nCol = nEdge + 1;

        BuildEdgeCostMatrix(nNode, nSys);

        //BuildEdgeIndexMatrix();

        float[,] augmat = new float[nRow, nCol]; // Instantiate augmented matrix


    }

    private void BuildEdgeCostMatrix(int nNode, int nSys)
    {
        float[,] cost = new float[nNode, nNode];

        for (int s = 0; s < nSys; s++)
        {
            int iSysExtReceive = 0 + (5 * s);
            int iSysExtEmitter = 1 + (5 * s);
            int iSysCentral = 2 + (5 * s);
            int iSysIntReceive = 3 + (5 * s);
            int iSysIntEmitter = 4 + (5 * s);
            float eCost = 0;

            for (int _s = 0; _s < nSys; _s++)
            {
                #region Intersystem
                if (s != _s)
                {
                    int _iSysExtReceive = 0 + (5 * _s);
                    eCost = Mathf.Pow((system[s].Position - system[_s].Position).magnitude, 1.1f); // Cost of travel

                    Debug.Log("System " + s + " (node " + iSysExtEmitter + ") to " + _s + " (" + _iSysExtReceive + ") costs " + eCost);

                    cost[iSysExtEmitter, _iSysExtReceive] = eCost;
                }
                #endregion
            }

            #region Intrasystem
            cost[iSysExtReceive, iSysCentral] = system[s].ImportExportTax[0];
            Debug.Log("System " + s + " import tax cost " + cost[iSysExtReceive, iSysCentral]);

            cost[iSysCentral, iSysExtEmitter] = system[s].ImportExportTax[1];
            Debug.Log("System " + s + " export tax cost " + system[s].ImportExportTax[1]);

            cost[iSysIntReceive, iSysCentral] = system[s].InternalTax[0];
            Debug.Log("System " + s + " inflow tax cost " + system[s].InternalTax[0]);

            cost[iSysCentral, iSysIntEmitter] = system[s].InternalTax[1];
            Debug.Log("System " + s + " outflow tax cost " + system[s].InternalTax[1]);

            #endregion

            foreach (BodyObject bo in system[s].Body)
            {
                int b = BodyIndex(bo, body);

                int iBodExtReceive = 0 + (5 * b) + (5 * nSys);
                int iBodExtEmitter = 1 + (5 * b) + (5 * nSys);
                int iBodCentral = 2 + (5 * b) + (5 * nSys);
                int iBodIntReceive = 3 + (5 * b) + (5 * nSys);
                int iBodIntEmitter = 3 + (5 * b) + (5 * nSys);

                #region System-Body
                Debug.Log("System " + s + " <-> body " + b + " cost " + 1f);

                cost[iSysIntEmitter, iBodExtReceive] = 1f;
                cost[iBodIntEmitter, iSysIntReceive] = 1f;
                #endregion

                #region Intrabody
                cost[iBodExtReceive, iBodCentral] = body[b].ImportExportTax[0];
                Debug.Log("Body " + b + " import tax cost " + cost[iBodExtReceive, iBodCentral]);

                cost[iBodCentral, iBodExtEmitter] = body[b].ImportExportTax[1];
                Debug.Log("Body " + b + " import tax cost " + cost[iBodCentral, iBodExtEmitter]);

                cost[iBodIntReceive, iBodCentral] = body[b].InternalTax[0];
                Debug.Log("Body " + b + " import tax cost " + cost[iBodIntReceive, iBodCentral]);

                cost[iBodCentral, iBodIntEmitter] = body[b].InternalTax[1];
                Debug.Log("Body " + b + " import tax cost " + cost[iBodCentral, iBodIntEmitter]);
                #endregion
            }
        }
    }

    private int BodyIndex(BodyObject b, List<BodyObject> l)
    {
        int i = -1;
        foreach (BodyObject c in l)
        {
            i++;
            if (c == b) return i;
        }
        Debug.LogError("BodyObject not found.");
        return -1;
    }

    private void BuildEdgeIndexMatrix(float[,] cost)
    {
        int i = -1;
        int[,] edgeind = new int[cost.GetLength(0), cost.GetLength(1)];

    }
}
