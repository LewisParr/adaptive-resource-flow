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
    }
}
