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

    private Problem07FlowSolver flowSolver;

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

        flowSolver = new Problem07FlowSolver();

        if (preset) CreatePreset();





        LPTest();
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

    private void CreatePreset()
    {
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

        if (system.Count > 0 && body.Count > 0 && facility.Count > 0)
        {
            flowSolver.Solve(system, body, facility);
        }
    }






    private void LPTest()
    {
        //Debug.Log("MAXIMISATION TEST");
        //
        //float[,] t = new float[3, 7];
        //t[0, 0] = 2; t[0, 1] = 3; t[0, 2] = 2; t[0, 3] = 1; t[0, 4] = 0; t[0, 5] = 0; t[0, 6] = 1000;
        //t[1, 0] = 1; t[1, 1] = 1; t[1, 2] = 2; t[1, 3] = 0; t[1, 4] = 1; t[1, 5] = 0; t[1, 6] = 800;
        //t[2, 0] = -7; t[2, 1] = -8; t[2, 2] = -10; t[2, 3] = 0; t[2, 4] = 0; t[2, 5] = 1; t[2, 6] = 0;
        //
        //Debug.Log("----- INITIAL -----");
        //for (int row = 0; row < t.GetLength(0); row++)
        //{
        //    string s = "";
        //    for (int col = 0; col < t.GetLength(1); col++)
        //    {
        //        s += t[row, col];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}
        //Debug.Log("----------");
        //
        //float[,] result = Problem07LinearProgramming.Maximise(t);
        //
        //Debug.Log("----- RESULT -----");
        //for (int row = 0; row < result.GetLength(0); row++)
        //{
        //    string s = "";
        //    for (int col = 0; col < result.GetLength(1); col++)
        //    {
        //        s += result[row, col];
        //        s += "; ";
        //    }
        //    Debug.Log(s);
        //}
        //Debug.Log("----------");
        //
        //Debug.Log("--------------------");

        Debug.Log("MINIMISATION TEST");

        float[,] a = new float[4, 3];
        a[0, 0] = 60; a[0, 1] = 60; a[0, 2] = 300;
        a[1, 0] = 12; a[1, 1] = 6; a[1, 2] = 36;
        a[2, 0] = 10; a[2, 1] = 30; a[2, 2] = 90;
        a[3, 0] = 0.12f; a[3, 1] = 0.15f; a[3, 2] = 0;

        Debug.Log("----- INITIAL -----");
        for (int row = 0; row < a.GetLength(0); row++)
        {
            string s = "";
            for (int col = 0; col < a.GetLength(1); col++)
            {
                s += a[row, col];
                s += "; ";
            }
            Debug.Log(s);
        }
        Debug.Log("----------");

        float[] _result = Problem07LinearProgramming.Minimise(a);

        Debug.Log("----- RESULT -----");
        string resultString = "";
        for (int i = 0; i < _result.Length; i++)
        {
            resultString += _result[i];
            resultString += "; ";
        }
        Debug.Log(resultString);
        Debug.Log("----------");

        Debug.Log("--------------------");
    }
}
