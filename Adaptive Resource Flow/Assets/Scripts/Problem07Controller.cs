using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem07Controller : MonoBehaviour
{
    public float width = 12f;
    public float height = 10f;
    public int delay = 20;
    public bool preset = true;

    private List<SystemObject> system;
    private List<BodyObject> body;
    private List<FacilityObject> facility;

    enum ElementType { None, System, Body, Facility };

    private int frame = 0;
    private int element = -1;

    private List<ElementType> presetType;
    private List<Vector3> presetPos;
    private List<float[]> presetIntCap;
    private List<float[]> presetIntTax;
    private List<float[]> presetImExCap;
    private List<float[]> presetImExTax;
    private List<float> presetThruCap;
    private List<float> presetThruTax;
    private List<int> presetParent;
    private List<float[]> presetProd;

    private void OnEnable()
    {
        /*
         * Initialise world components.
         */

        system = new List<SystemObject>();
        body = new List<BodyObject>();
        facility = new List<FacilityObject>();

        presetType = new List<ElementType>();
        presetPos = new List<Vector3>();
        presetIntCap = new List<float[]>();
        presetIntTax = new List<float[]>();
        presetImExCap = new List<float[]>();
        presetImExTax = new List<float[]>();
        presetThruCap = new List<float>();
        presetThruTax = new List<float>();
        presetParent = new List<int>();
        presetProd = new List<float[]>();

        float[] intcap;
        float[] inttax;
        float[] imexcap;
        float[] imextax;
        float[] prod;

        // System 0
        presetType.Add(ElementType.System);
        presetPos.Add(new Vector3(0, 0, 0));
        intcap = new float[2]; intcap[0] = 1f; intcap[1] = 1f;
        presetIntCap.Add(intcap);
        inttax = new float[2]; inttax[0] = 0.1f; inttax[1] = 0.1f;
        presetIntTax.Add(inttax);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(-1);
        presetProd.Add(null);

        // Body 0
        presetType.Add(ElementType.Body);
        presetPos.Add(new Vector3(0, 0, 0));
        intcap = new float[2]; intcap[0] = 1f; intcap[1] = 1f;
        presetIntCap.Add(intcap);
        inttax = new float[2]; inttax[0] = 0.1f; inttax[1] = 0.1f;
        presetIntTax.Add(inttax);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(0);
        presetProd.Add(null);

        // Facility 0
        presetType.Add(ElementType.Facility);
        presetPos.Add(new Vector3(0, 0, 0));
        presetIntCap.Add(null);
        presetIntTax.Add(null);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(0);
        prod = new float[3]; prod[0] = -0.25f; prod[1] = -0.50f; prod[2] = -0.75f;
        presetProd.Add(prod);

        // System 1
        presetType.Add(ElementType.System);
        presetPos.Add(new Vector3(0, 0, 7));
        intcap = new float[2]; intcap[0] = 1f; intcap[1] = 1f;
        presetIntCap.Add(intcap);
        inttax = new float[2]; inttax[0] = 0.1f; inttax[1] = 0.1f;
        presetIntTax.Add(inttax);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(-1);
        presetProd.Add(null);

        // Body 1
        presetType.Add(ElementType.Body);
        presetPos.Add(new Vector3(0, 0, 7));
        intcap = new float[2]; intcap[0] = 1f; intcap[1] = 1f;
        presetIntCap.Add(intcap);
        inttax = new float[2]; inttax[0] = 0.1f; inttax[1] = 0.1f;
        presetIntTax.Add(inttax);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(1);
        presetProd.Add(null);

        // Facility 1
        presetType.Add(ElementType.Facility);
        presetPos.Add(new Vector3(0, 0, 7));
        presetIntCap.Add(null);
        presetIntTax.Add(null);
        imexcap = new float[2]; imexcap[0] = 1f; imexcap[1] = 1f;
        presetImExCap.Add(imexcap);
        imextax = new float[2]; imextax[0] = 0.1f; imextax[1] = 0.1f;
        presetImExTax.Add(imextax);
        presetThruCap.Add(1f);
        presetThruTax.Add(0.1f);
        presetParent.Add(1);
        prod = new float[3]; prod[0] = +0.25f; prod[1] = +0.50f; prod[2] = +0.75f;
        presetProd.Add(prod);
    }

    private void Update()
    {
        frame++;

        if (frame % delay == 0)
        {
            element++;

            AddElement(element);
            if (element == presetPos.Count) UpdateResourceFlows();
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

    private void AddElement(int element = 0)
    {
        /*
         * Add a new system, body, or facility to the world.
         */

        if (!preset)
        {
            ElementType elementType = ChooseElementType();               // Choose element type

            Debug.Log("Chosen element: " + elementType.ToString());

            if (elementType == ElementType.System) AddSystem();          // Add a system, ...
            else if (elementType == ElementType.Body) AddBody();         // or a body, ...
            else if (elementType == ElementType.Facility) AddFacility(); // or a facility, ...
            else { };                                                    // or nothing at all. 
        }
        else if (element < presetPos.Count)
        {
            Debug.Log("Inserting element " + element);

            ElementType elementType = presetType[element];
            if (elementType == ElementType.System)
            {
                system.Add(new SystemObject(presetPos[element], presetIntCap[element],
                    presetIntTax[element], presetImExCap[element], presetImExTax[element],
                    presetThruCap[element], presetThruTax[element]));
            }
            else if (elementType == ElementType.Body)
            {
                body.Add(new BodyObject(presetPos[element], presetIntCap[element], presetIntTax[element],
                    presetImExCap[element], presetImExTax[element]));
                system[presetParent[element]].Body.Add(body[body.Count - 1]);
                body[body.Count - 1].PlanetarySystem = system[presetParent[element]];
            }
            else if (elementType == ElementType.Facility)
            {
                facility.Add(new FacilityObject(presetPos[element], presetProd[element],
                    presetImExCap[element], presetImExTax[element]));
                body[presetParent[element]].Facility.Add(facility[facility.Count - 1]);
                facility[facility.Count - 1].PlanetaryBody = body[presetParent[element]];
            }
        }
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

        Debug.Log("Updating resource flows...");

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

        float[,] cost = BuildEdgeCostMatrix(nNode, nSys, nBod);

        //Debug.Log("----- COST -----");
        //for (int row = 0; row < cost.GetLength(0); row++)
        //{
        //    string s = "";
        //    for (int col = 0; col < cost.GetLength(1); col++)
        //    {
        //        s += cost[row, col];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}

        float[,] capacity = BuildEdgeCapacityMatrix(nNode, nSys, nBod);

        //Debug.Log("----- CAPACITY -----");
        //for (int row = 0; row < capacity.GetLength(0); row++)
        //{
        //    string s = "";
        //    for (int col = 0; col < capacity.GetLength(1); col++)
        //    {
        //        s += capacity[row, col];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}

        int[,] edgeind = BuildEdgeIndexMatrix(cost);

        //Debug.Log("----- EDGE INDEX -----");
        //for (int row = 0; row < edgeind.GetLength(0); row++)
        //{
        //    string s = "";
        //    for (int col = 0; col < edgeind.GetLength(1); col++)
        //    {
        //        s += edgeind[row, col];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}

        int nNonInf = 0;
        for (int a = 0; a < cost.GetLength(0); a++)
            for (int b = 0; b < cost.GetLength(1); b++)
                if (cost[a, b] != Mathf.Infinity) nNonInf++;

        //Debug.Log("Number of non-infinite cost edges: " + nNonInf);

        float[,] augmat = new float[nRow, nCol]; // Instantiate augmented matrix

        // Flow conservation values
        for (int r = 0; r < 3; r++)
        {
            for (int n = 0; n < nNode; n++)
            {
                for (int _n = 0; _n < nNode; _n++)
                {
                    if (cost[n, _n] != Mathf.Infinity) augmat[n, (r * nNonInf) + edgeind[n, _n]] = +1;
                    if (cost[_n, n] != Mathf.Infinity) augmat[n, (r * nNonInf) + edgeind[_n, n]] = -1;
                }

                // b-value
                if (n >= (5 * nSys) + (5 * nBod)) // If this is a facility node
                {
                    if ((n - (5 * nSys) - (5 * nBod)) % 2 == 0) // If this is a production node
                    {
                        augmat[n, nCol - 1] = facility[((n - (5 * nSys) - (5 * nBod)) - 2) / 3].Production[r];
                    }
                    else augmat[n, nCol - 1] = 0;
                }
                else augmat[n, nCol - 1] = 0;
            }
        }

        // Edge capacity values
        int iCapCon = -1;
        for (int e = 0; e < nEdge; e++)
        {
            int[] nodes = EdgeNodesFromIndex(edgeind, Mathf.FloorToInt(e / nRes));

            if (capacity[nodes[0], nodes[1]] != Mathf.Infinity) // If capacity is constrained
            {
                iCapCon++;

                augmat[iCapCon + nNode, e] = +1; // Identify edge
                augmat[iCapCon + nNode, nCol - 1] = capacity[nodes[0], nodes[1]]; // b-value
            }

            augmat[nRow - 1, e] = cost[nodes[0], nodes[1]];
        }

        Debug.Log("Nodes: " + nNode);

        Debug.Log("Cost (0, 0): " + cost[0, 0]);

        Debug.Log("----- AUGMENTED MATRIX -----");
        for (int row = 0; row < augmat.GetLength(0); row++)
        {
            string s = "";
            for (int col = 0; col < augmat.GetLength(1); col++)
            {
                s += augmat[row, col];
                s += "; ";
            }
            Debug.Log(s);
        }

        // Process the minimisation problem
        //float[] output = Minimise(augmat);

        // Interpret the result
        // ...
    }

    private float[,] BuildEdgeCostMatrix(int nNode, int nSys, int nBod)
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

                    //Debug.Log("System " + s + " (node " + iSysExtEmitter + ") to " + _s + " (" + _iSysExtReceive + ") costs " + eCost);

                    cost[iSysExtEmitter, _iSysExtReceive] = eCost;
                }
                else
                {
                    int _iSysExtReceive = 0 + (5 * _s);
                    cost[iSysExtEmitter, _iSysExtReceive] = Mathf.Infinity;
                }
                #endregion
            }

            #region Intrasystem
            cost[iSysExtReceive, iSysCentral] = system[s].ImportExportTax[0];
            //Debug.Log("System " + s + " import tax cost " + cost[iSysExtReceive, iSysCentral]);

            cost[iSysCentral, iSysExtEmitter] = system[s].ImportExportTax[1];
            //Debug.Log("System " + s + " export tax cost " + system[s].ImportExportTax[1]);

            cost[iSysIntReceive, iSysCentral] = system[s].InternalTax[0];
            //Debug.Log("System " + s + " inflow tax cost " + system[s].InternalTax[0]);

            cost[iSysCentral, iSysIntEmitter] = system[s].InternalTax[1];
            //Debug.Log("System " + s + " outflow tax cost " + system[s].InternalTax[1]);

            #endregion

            foreach (BodyObject bo in system[s].Body)
            {
                int b = BodyIndex(bo, body);

                int iBodExtReceive = 0 + (5 * b) + (5 * nSys);
                int iBodExtEmitter = 1 + (5 * b) + (5 * nSys);
                int iBodCentral = 2 + (5 * b) + (5 * nSys);
                int iBodIntReceive = 3 + (5 * b) + (5 * nSys);
                int iBodIntEmitter = 4 + (5 * b) + (5 * nSys);

                #region System-Body
                //Debug.Log("System " + s + " <-> body " + b + " cost " + 1f);

                cost[iSysIntEmitter, iBodExtReceive] = 1f;
                cost[iBodExtEmitter, iSysIntReceive] = 1f;
                #endregion

                #region Intrabody
                cost[iBodExtReceive, iBodCentral] = body[b].ImportExportTax[0];
                //Debug.Log("Body " + b + " import tax cost " + cost[iBodExtReceive, iBodCentral]);

                cost[iBodCentral, iBodExtEmitter] = body[b].ImportExportTax[1];
                //Debug.Log("Body " + b + " import tax cost " + cost[iBodCentral, iBodExtEmitter]);

                cost[iBodIntReceive, iBodCentral] = body[b].InternalTax[0];
                //Debug.Log("Body " + b + " import tax cost " + cost[iBodIntReceive, iBodCentral]);

                cost[iBodCentral, iBodIntEmitter] = body[b].InternalTax[1];
                //Debug.Log("Body " + b + " import tax cost " + cost[iBodCentral, iBodIntEmitter]);
                #endregion

                foreach (FacilityObject fo in bo.Facility)
                {
                    int f = FacilityIndex(fo, facility);

                    int iFacReceive = 0 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacEmitter = 1 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacProduce = 2 + (3 * f) + (5 * nBod) + (5 * nSys);

                    #region Body-Facility
                    //Debug.Log("Body " + b + " <-> facility " + f + " cost " + 0.5f);

                    cost[iBodIntEmitter, iFacReceive] = 0.5f;
                    cost[iFacEmitter, iBodIntReceive] = 0.5f;
                    #endregion

                    #region Intrafacility
                    cost[iFacReceive, iFacProduce] = facility[f].ImportExportTax[0];
                    //Debug.Log("Facility " + f + " import tax cost " + cost[iFacReceive, iFacProduce]);

                    cost[iFacProduce, iFacEmitter] = facility[f].ImportExportTax[1];
                    //Debug.Log("Facility " + f + " export tax cost " + cost[iFacProduce, iFacEmitter]);
                    #endregion
                }
            }
        }

        for (int a = 0; a < cost.GetLength(0); a++)
            for (int b = 0; b < cost.GetLength(1); b++)
                if (cost[a, b] == 0)
                    cost[a, b] = Mathf.Infinity;

        return cost;
    }

    private float[,] BuildEdgeCapacityMatrix(int nNode, int nSys, int nBod)
    {
        float[,] capacity = new float[nNode, nNode];

        for (int s = 0; s < nSys; s++)
        {
            int iSysExtReceive = 0 + (5 * s);
            int iSysExtEmitter = 1 + (5 * s);
            int iSysCentral = 2 + (5 * s);
            int iSysIntReceive = 3 + (5 * s);
            int iSysIntEmitter = 4 + (5 * s);
            float eCap = 0;

            for (int _s = 0; _s < nSys; _s++)
            {
                #region Intersystem
                if (s != _s)
                {
                    int _iSysExtReceive = 0 + (5 * _s);
                    eCap = Mathf.Infinity;

                    //Debug.Log("System " + s + " (node " + iSysExtEmitter + ") to " + _s + " (" + _iSysExtReceive + ") capacity " + eCap);

                    capacity[iSysExtEmitter, _iSysExtReceive] = eCap;
                }
                #endregion
            }

            #region Intrasystem
            capacity[iSysExtReceive, iSysCentral] = system[s].ImportExportCapacity[0];
            //Debug.Log("System " + s + " import capacity " + capacity[iSysExtReceive, iSysCentral]);

            capacity[iSysCentral, iSysExtEmitter] = system[s].ImportExportCapacity[1];
            //Debug.Log("System " + s + " export capacity " + capacity[iSysCentral, iSysExtEmitter]);

            capacity[iSysIntReceive, iSysCentral] = system[s].InternalCapacity[0];
            //Debug.Log("System " + s + " inflow capacity " + capacity[iSysIntReceive, iSysCentral]);

            capacity[iSysCentral, iSysIntEmitter] = system[s].InternalCapacity[1];
            //Debug.Log("System " + s + " outflow capacity " + capacity[iSysCentral, iSysIntEmitter]);
            #endregion

            foreach (BodyObject bo in system[s].Body)
            {
                int b = BodyIndex(bo, body);

                int iBodExtReceive = 0 + (5 * b) + (5 * nSys);
                int iBodExtEmitter = 1 + (5 * b) + (5 * nSys);
                int iBodCentral = 2 + (5 * b) + (5 * nSys);
                int iBodIntReceive = 3 + (5 * b) + (5 * nSys);
                int iBodIntEmitter = 4 + (5 * b) + (5 * nSys);

                #region System-Body
                //Debug.Log("System " + s + " <-> body " + b + " capacity " + Mathf.Infinity);

                capacity[iSysIntEmitter, iBodExtReceive] = Mathf.Infinity;
                capacity[iBodExtEmitter, iSysIntReceive] = Mathf.Infinity;
                #endregion

                #region Intrabody
                capacity[iBodExtReceive, iBodCentral] = body[b].ImportExportCapacity[0];
                //Debug.Log("Body " + b + " import capacity " + capacity[iBodExtReceive, iBodCentral]);

                capacity[iBodCentral, iBodExtEmitter] = body[b].ImportExportCapacity[1];
                //Debug.Log("Body " + b + " import capacity " + capacity[iBodCentral, iBodExtEmitter]);

                capacity[iBodIntReceive, iBodCentral] = body[b].InternalCapacity[0];
                //Debug.Log("Body " + b + " import capacity " + capacity[iBodIntReceive, iBodCentral]);

                capacity[iBodCentral, iBodIntEmitter] = body[b].InternalCapacity[1];
                //Debug.Log("Body " + b + " import capacity " + capacity[iBodCentral, iBodIntEmitter]);
                #endregion

                foreach (FacilityObject fo in bo.Facility)
                {
                    int f = FacilityIndex(fo, facility);

                    int iFacReceive = 0 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacEmitter = 1 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacProduce = 2 + (3 * f) + (5 * nBod) + (5 * nSys);

                    #region Body-Facility
                    //Debug.Log("Body " + b + " <-> facility " + f + " capacity " + Mathf.Infinity);

                    capacity[iBodIntEmitter, iFacReceive] = Mathf.Infinity;
                    capacity[iFacEmitter, iBodIntReceive] = Mathf.Infinity;
                    #endregion

                    #region Intrafacility
                    capacity[iFacReceive, iFacProduce] = facility[f].ImportExportCapacity[0];
                    //Debug.Log("Facility " + f + " import capacity " + capacity[iFacReceive, iFacProduce]);

                    capacity[iFacProduce, iFacEmitter] = facility[f].ImportExportCapacity[1];
                    //Debug.Log("Facility " + f + " export capacity " + capacity[iFacProduce, iFacEmitter]);
                    #endregion
                }
            }
        }
        return capacity;
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

    private int FacilityIndex(FacilityObject f, List<FacilityObject> l)
    {
        int i = -1;
        foreach (FacilityObject c in l)
        {
            i++;
            if (c == f) return i;
        }
        Debug.LogError("FacilityObject not found.");
        return -1;
    }

    private int[,] BuildEdgeIndexMatrix(float[,] cost)
    {
        int i = -1;
        int[,] edgeind = new int[cost.GetLength(0), cost.GetLength(1)];
        for (int n = 0; n < cost.GetLength(0); n++)
        {
            for (int _n = 0; _n < cost.GetLength(1); _n++)
            {
                if (cost[n, _n] != Mathf.Infinity)
                {
                    i++;
                    edgeind[n, _n] = i;
                }
                else
                {
                    edgeind[n, _n] = -1;
                }
            }
        }
        return edgeind;
    }

    private static int[] EdgeNodesFromIndex(int[,] edgeind, int e)
    {
        for (int a = 0; a < edgeind.GetLength(0); a++)
        {
            for (int b = 0; b < edgeind.GetLength(1); b++)
            {
                if (edgeind[a, b] == e)
                {
                    int[] nodes = new int[2];
                    nodes[0] = a;
                    nodes[1] = b;
                    return nodes;
                }
            }
        }
        Debug.LogError("Edge index not found.");
        return null;
    }

    private static float[] Minimise(float[,] augmat)
    {
        // Form the transpose of the augmented matrix
        float[,] _augmat = Transpose(augmat);

        // Form the dual maximisation problem
        float[,] tableau = InsertSlackVariables(_augmat);

        Debug.Log("Number of tableau rows: " + tableau.GetLength(0));
        Debug.Log("Number of tableau columns: " + tableau.GetLength(1));

        // Process the maximisation problem
        float[] output = Maximise(tableau, true);

        // Return the result
        return output;
    }

    private static float[,] Transpose(float[,] augmat)
    {
        int numRow = augmat.GetLength(1);
        int numCol = augmat.GetLength(0);
        float[,] transpose = new float[numRow, numCol];
        for (int r = 0; r < numRow; r++)
            for (int c = 0; c < numCol; c++)
                transpose[r, c] = augmat[c, r];
        return transpose;
    }

    private static float[,] InsertSlackVariables(float[,] augmat)
    {
        int numRow = augmat.GetLength(0);
        int numCol = augmat.GetLength(1) + (numRow - 1);
        float[,] tableau = new float[numRow, numCol];

        // Insert matrix values (except b-values)
        for (int r = 0; r < numRow; r++)
            for (int c = 0; c < augmat.GetLength(1) - 1; c++)
                tableau[r, c] = augmat[r, c];

        // Insert b-values
        for (int r = 0; r < numRow; r++)
            tableau[r, numCol - 1] = augmat[r, augmat.GetLength(1) - 1];

        // Insert slack values
        for (int r = 0; r < numRow - 1; r++)
            tableau[r, r + augmat.GetLength(1) - 1] = 1f;

        // Return the completed tableau
        return tableau;
    }

    private static float[] Maximise(float[,] tableau, bool dual = false)
    {
        // Run the simplex method
        tableau = SimplexMethod(tableau);

        if (!dual)
        {
            // Read results normally
            Debug.LogError("NOT IMPLEMENTED");
            return null;
        }
        else
        {
            // Return the bottom row
            float[] output = new float[tableau.GetLength(1)];
            for (int c = 0; c < tableau.GetLength(1); c++)
                output[c] = tableau[tableau.GetLength(0) - 1, c];
            return output;
        }
    }

    private static float[,] SimplexMethod(float[,] tableau)
    {
        int iterCount = 0;
        bool terminate = false;
        while (!terminate)
        {
            iterCount++;

            // Locate the most negative entry in the bottom row
            int enteringColumn = SelectEntering(tableau);

            if (enteringColumn != -1)
            {
                // Locate the smallest nonnegative ratio
                int departingRow = SelectDeparting(tableau, enteringColumn);

                if (departingRow != -1)
                {
                    // Set pivot to 1 all others to zero
                    tableau = Pivot(tableau, enteringColumn, departingRow);
                }
                else terminate = true;
            }
            else terminate = true;

            // !!!
            //if (iterCount == 1000) terminate = true;

            Debug.Log("Simplex Method pass " + iterCount + " completed at: " + Time.realtimeSinceStartup);
        }
        return tableau;
    }

    private static int SelectEntering(float[,] tableau)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minVal = float.MaxValue;
        int enteringColumn = 0;
        for (int c = 0; c < numCol; c++)
            if (tableau[numRow - 1, c] < minVal)
            {
                minVal = tableau[numRow - 1, c];
                enteringColumn = c;
            }
        if (!(minVal < 0)) return -1;
        else
        {
            if (minVal == float.MaxValue) return -1;
            else return enteringColumn;
        }
    }

    private static int SelectDeparting(float[,] tableau, int enteringColumn)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minRatio = float.MaxValue;
        int departingRow = 0;
        for (int r = 0; r < numRow - 1; r++)
        {
            float ratio = tableau[r, numCol - 1] / tableau[r, enteringColumn];
            if (!(ratio < 0))
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    departingRow = r;
                }
        }
        if (minRatio == float.MaxValue) return -1;
        else return departingRow;
    }

    private static float[,] Pivot(float[,] tableau, int enteringColumn, int departingRow)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);

        // Scale departing row so that pivot value is 1
        float scaleValue = tableau[departingRow, enteringColumn];
        for (int c = 0; c < numCol; c++)
            tableau[departingRow, c] = tableau[departingRow, c] / scaleValue;

        // Set other values in entering column to 0
        for (int r = 0; r < numRow; r++)
            if (r != departingRow)
            {
                float coefficient = -tableau[r, enteringColumn];
                for (int c = 0; c < numCol; c++)
                    tableau[r, c] = (coefficient * tableau[departingRow, c]) + tableau[r, c];
            }

        // Return the new tableau
        return tableau;
    }
}
