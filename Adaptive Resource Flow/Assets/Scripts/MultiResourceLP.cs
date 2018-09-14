using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MultiResourceLP
{
    public static void MinimumCostFlow(List<SystemObject> system, List<BodyObject> body, List<FacilityObject> facility)
    {
        int numSys = system.Count;
        int numBod = body.Count;
        int numFac = facility.Count;
        int numRes = facility[0].Production.Length;

        Debug.Log("Number of systems: " + numSys);
        Debug.Log("Number of bodies: " + numBod);
        Debug.Log("Number of facilities: " + numFac);
        Debug.Log("Number of resources: " + numRes);

        int numNode = (5 * numSys) + (5 * numBod) + (3 * numFac);
        int numEdge = ((numSys * (numSys - 1)) + (5 * numSys) + (6 * numBod) + (4 * numFac)) * numRes;

        Debug.Log("Number of nodes: " + numNode);
        Debug.Log("Number of edges: " + numEdge);

        int numFlowCons = numNode * numRes;
        int numCapCons = numEdge;

        Debug.Log("Number of flow conservation constraints: " + numFlowCons);
        Debug.Log("Number of capacity constraints: " + numCapCons);

        int numArtVar = numFlowCons + numCapCons;

        Debug.Log("Number of artificial variables: " + numArtVar);

        int numRow = numFlowCons + numCapCons + 1;
        int numCol = numEdge + numArtVar + 1;

        Debug.Log("Number of augmented matrix rows: " + numRow);
        Debug.Log("Number of augmented matrix columns: " + numCol);

        // Instantiate edge cost matrix
        float[,] cost = new float[numNode, numNode];

        // Insert edge costs
        for (int s1 = 0; s1 < numSys; s1++)
        {
            // Insert intersystem edge costs
            for (int s2 = 0; s2 < numSys; s2++)
            {
                // s1 external emitter node index
                int a = (5 * s1) + 1;

                // s2 external receiver node index
                int b = (5 * s2);

                // Cost is distance between them
                float c = (system[s1].Position - system[s2].Position).magnitude;

                //Debug.Log("System " + s1 + " to " + s2 + " costs " + c);

                // Insert values
                cost[a, b] = c;
            }

            // Insert intrasystem edge costs

            // External receiver node
            int ser = (5 * s1);

            // External emitter node
            int see = (5 * s1) + 1;

            // Central node
            int sc = (5 * s1) + 2;

            // Internal receiver node
            int sir = (5 * s1) + 3;

            // Internal emitter node
            int sie = (5 * s1) + 4;

            // Insert throughflow tax
            cost[ser, see] = system[s1].ThroughflowTax;

            // Insert import/export taxes
            cost[ser, sc] = system[s1].ImportExportTax[0];
            cost[sc, see] = system[s1].ImportExportTax[1];

            // Insert internal taxes
            cost[sir, sc] = system[s1].InternalTax[0];
            cost[sc, sie] = system[s1].InternalTax[1];

            foreach (BodyObject bo in system[s1].Body)
            {
                int b = BodyIndex(bo, body);

                // External receiver node
                int ber = (5 * b) + (5 * numSys);

                // External emitter node
                int bee = (5 * b) + 1 + (5 * numSys);

                // Insert system-body edge costs
                cost[sie, ber] = (system[s1].Position - body[b].Position).magnitude;
                cost[bee, sir] = (system[s1].Position - body[b].Position).magnitude;

                // Insert intrabody edge costs

                // Central node
                int bc = (5 * b) + 2 + (5 * numSys);

                // Internal receiver node
                int bir = (5 * b) + 3 + (5 * numSys);

                // Internal emitter node
                int bie = (5 * b) + 4 + (5 * numSys);

                // Insert import/export taxes
                cost[ber, bc] = body[b].ImportExportTax[0];
                cost[bc, bee] = body[b].ImportExportTax[1];

                // Insert internal taxes
                cost[bir, bc] = body[b].InternalTax[0];
                cost[bc, bie] = body[b].InternalTax[1];

                foreach (FacilityObject fo in bo.Facility)
                {
                    int f = FacilityIndex(fo, facility);

                    // Receiver node
                    int fr = (3 * f) + (5 * numSys) + (5 * numBod);

                    // Emitter node
                    int fe = (3 * f) + 1 + (5 * numSys) + (5 * numBod);

                    // Insert body-facility edge costs
                    cost[bie, fr] = (body[b].Position - facility[f].Position).magnitude;
                    cost[fe, bir] = (body[b].Position - facility[f].Position).magnitude;

                    // Insert intrafacility edge costs

                    // Producer node
                    int fp = (3 * f) + 2 + (5 * numSys) + (5 * numBod);

                    // Insert import/export taxes
                    cost[fr, fp] = facility[f].ImportExportTax[0];
                    cost[fp, fe] = facility[f].ImportExportTax[1];
                }
            }
        }

        // Set all zero costs to infinity
        int noConnection = 0;
        for (int n1 = 0; n1 < numNode; n1++)
        {
            for (int n2 = 0; n2 < numNode; n2++)
            {
                if (cost[n1, n2] == 0) { noConnection++; cost[n1, n2] = Mathf.Infinity; }
            }
        }

        Debug.Log("Number of non-infinite edge costs: " + ((numNode * numNode) - noConnection));

        // Instantiate augmented matrix
        float[,] augmat = new float[numRow, numCol];

        // Insert flow conservation values
        for (int r = 0; r < numRes; r++)
        {
            for (int n = 0; n < numNode; n++)
            {

            }
        }
    }

    private static int BodyIndex(BodyObject b, List<BodyObject> l)
    {
        int i = -1;
        foreach (BodyObject c in l)
        {
            i++;
            if (c == b)
            {
                return i;
            }
        }
        Debug.LogError("Object not found.");
        return -1;
    }

    private static int FacilityIndex(FacilityObject f, List<FacilityObject> l)
    {
        int i = -1;
        foreach (FacilityObject c in l)
        {
            i++;
            if (c == f)
            {
                return i;
            }
        }
        Debug.LogError("Object not found.");
        return -1;
    }
}
