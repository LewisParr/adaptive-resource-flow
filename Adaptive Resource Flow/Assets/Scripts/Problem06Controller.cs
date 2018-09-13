using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem06Controller : MonoBehaviour
{
    [Range(3, 24)]
    public int numSys = 20;
    [Range(0.1f, 0.5f)]
    public float productionProb = 0.25f;
    [Range(1, 10)]
    public int numResources = 2;

    private List<AstroObject> all;
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
            }
        }
    }

    private void GenerateSystem()
    {
        if (all == null) all = new List<AstroObject>();

        int nAll = all.Count;

        // Position
        float x = Random.value * numSys;
        float y = Random.value * numSys;
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

        // Throughflow tax
        float thrutax = 0.10f;

        // Instantiate system
        SystemObject S = new SystemObject(pos, intcap, inttax, imexcap, imextax, thrutax);

        // Number of planetary bodies
        int numBody = Mathf.FloorToInt(Random.value * 8);

        for (int b = 0; b < numBody; b++)
        {
            // Position
            float _x = Random.value;
            float _y = Random.value;
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
            BodyObject B = new BodyObject(_pos, _intcap, _inttax, _imexcap, _imextax);

            // Number of facilities
            int numFacility = Mathf.FloorToInt(Random.value * 16);

            for (int f = 0; f < numFacility; f++)
            {
                // Position
                float __x = Random.value / 2f;
                float __y = Random.value / 2f;
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
                FacilityObject F = new FacilityObject(__pos, prod, __imexcap, __imextax);
            }
        }
    }
}
