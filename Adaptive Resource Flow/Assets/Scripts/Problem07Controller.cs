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
            flowSolver.Solve();
        }
    }

    private void BuildAugmentedMatrix()
    {
        // Process the minimisation problem
        float[] output = Minimise(augmat);
    }

    private static float[] Minimise(float[,] augmat)
    {
        // Form the transpose of the augmented matrix
        float[,] _augmat = Transpose(augmat);

        // Form the dual maximisation problem
        float[,] tableau = InsertSlackVariables(_augmat);

        //Debug.Log("Number of tableau rows: " + tableau.GetLength(0));
        //Debug.Log("Number of tableau columns: " + tableau.GetLength(1));

        // Process the maximisation problem
        float[] output = Maximise(tableau, true);

        // Return the result
        return output;
    }

    private static float[] Maximise(float[,] tableau, bool dual = false)
    {
        // Run the simplex method
        tableau = SimplexMethod(tableau);

        //Debug.Log("----- FINAL TABLEAU -----");
        //for (int r = 0; r < tableau.GetLength(0); r++)
        //{
        //    string row = "";
        //    for (int c = 0; c < tableau.GetLength(1); c++)
        //    {
        //        row += tableau[r, c];
        //        row += ";";
        //    }
        //    Debug.Log(row);
        //}

        // Read the solution
        ReadTableau(tableau);

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
                    //Debug.Log("Entering column: " + enteringColumn + "; Departing row: " + departingRow);
                    
                    // Set pivot to 1 all others to zero
                    tableau = Pivot(tableau, enteringColumn, departingRow);
                }
                else terminate = true;
            }
            else terminate = true;

            // !!!
            if (iterCount == 1000) terminate = true;

            //Debug.Log("Simplex Method pass " + iterCount + " completed at: " + Time.realtimeSinceStartup);
        }
        return tableau;
    }

    private static int SelectEntering(float[,] tableau)
    {
        int numRow = tableau.GetLength(0);
        int numCol = tableau.GetLength(1);
        float minVal = float.MaxValue;
        int enteringColumn = 0;

        List<float> values = new List<float>();

        for (int c = 0; c < numCol; c++)
        {
            values.Add(tableau[numRow - 1, c]);

            if (tableau[numRow - 1, c] < minVal)
            {
                minVal = tableau[numRow - 1, c];
                enteringColumn = c;
            }
        }

        //string s = " chosen from (col): ";
        //foreach (float value in values) { s += value; s += "; "; }

        //if (!(minVal < 0)) { Debug.Log("None" + s); return -1; }
        if (!(minVal < 0)) return -1;
        else
        {
            //if (minVal == float.MaxValue) { Debug.Log("None" + s); return -1; }
            //else { Debug.Log(tableau[numRow - 1, enteringColumn] + " (" + enteringColumn + ")" + s); return enteringColumn; }
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

        List<float> values = new List<float>();

        for (int r = 0; r < numRow - 1; r++)
        {
            float ratio = tableau[r, numCol - 1] / tableau[r, enteringColumn];

            values.Add(ratio);

            if (!(ratio < 0))
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    departingRow = r;
                }
        }

        //string s = " chosen from (row): ";
        //foreach (float value in values) { s += value; s += "; "; }

        //if (!(minRatio > 0)) { Debug.Log("None" + s); return -1; }
        if (!(minRatio > 0)) return -1;
        else
        {
            //if (minRatio == float.MaxValue) { Debug.Log("None" + s); return -1; }
            //else { Debug.Log(values[departingRow] + " (" + departingRow + ")" + s); return departingRow; }
            if (minRatio == float.MaxValue) return -1;
            else return departingRow;
        }
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

    private static void ReadTableau(float[,] tableau)
    {
        for (int c = 0; c < tableau.GetLength(1); c++) // For each column, ...
        {
            float nZero = 0;
            float nOne = 0;
            float nOther = 0;

            for (int r = 0; r < tableau.GetLength(0); r++) // iterate through its rows ...
            {
                float thisVal = tableau[r, c];
                if (thisVal == 0) nZero++;
                else if (thisVal == 1) nOne++;
                else nOther++;
            }

            if (nOne == 1 && nOther == 0)
            {
                Debug.Log("Column " + c + " is part of the solution.");
            }
        }
    }
}
