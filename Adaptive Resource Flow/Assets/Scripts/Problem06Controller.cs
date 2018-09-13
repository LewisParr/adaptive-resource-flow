using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem06Controller : MonoBehaviour
{
    [Range(3, 24)]
    public int numNodes = 20;
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
}
