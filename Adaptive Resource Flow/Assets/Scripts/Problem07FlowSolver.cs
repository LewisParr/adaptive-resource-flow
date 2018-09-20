using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem07FlowSolver
{
    /*
     * Given a galactic network of planetary systems, planetary bodies, and facilities,
     * produce the optimal flow of resources through the network.
     */

    #region GalacticElements
    int nSys;
    int nBod;
    int nFac;
    int nRes;
    #endregion

    #region GraphElements
    int nSysNode;
    int nBodNode;
    int nFacNode;
    int nInterSysEdge;
    int nIntraSysEdge;
    int nSysBodEdge;
    int nIntraBodEdge;
    int nBodFacEdge;
    int nIntraFacEdge;
    int nNode;
    int nEdge;
    #endregion

    #region LinearProgrammingElements
    int nFlowConstr;
    int nCapConstr;
    int nRow;
    int nCol;
    float[,] augMat;
    #endregion

    #region EdgeData
    float[,] edgeCost;
    float[,] edgeCapacity;
    int[,] edgeIndex;
    int nNonInf;
    #endregion

    public Problem07FlowSolver()
    {

    }

    public void Solve(List<SystemObject> system, List<BodyObject> body, List<FacilityObject> facility)
    {
        /*
         * Accept the network elements and solve for optimal flow of resources.
         */

        CountElements(system, body, facility);
        CollectEdgeData(system, body, facility);
        IndexEdges();
        BuildAugmentedMatrix();
    }

    private void CountElements(List<SystemObject> system, List<BodyObject> body, List<FacilityObject> facility)
    {
        #region GalacticElements
        nSys = system.Count;
        nBod = body.Count;
        nFac = facility.Count;
        nRes = facility[0].Production.Length;
        #endregion

        #region GraphElements
        nSysNode = 5 * nSys;
        nBodNode = 5 * nBod;
        nFacNode = 3 * nFac;
        nInterSysEdge = nSys * (nSys - 1);
        nIntraSysEdge = 4 * nSys;
        nSysBodEdge = 2 * nBod;
        nIntraBodEdge = 4 * nBod;
        nBodFacEdge = 2 * nFac;
        nIntraFacEdge = 2 * nFac;
        nNode = nSysNode + nBodNode + nFacNode;
        nEdge = (nInterSysEdge + nIntraSysEdge + nSysBodEdge + nIntraBodEdge + nBodFacEdge + nIntraFacEdge) * nRes;
        #endregion

        #region LinearProgrammingElements
        nFlowConstr = nNode * nRes;
        nCapConstr = (nIntraSysEdge + nIntraBodEdge + nIntraFacEdge) * nRes;
        nRow = nFlowConstr + nCapConstr + 1;
        nCol = nEdge + 1;
        #endregion
    }

    private void CollectEdgeData(List<SystemObject> system, List<BodyObject> body, List<FacilityObject> facility)
    {
        edgeCost = new float[nNode, nNode];
        edgeCapacity = new float[nNode, nNode];

        for (int s = 0; s < nSys; s++)
        {
            #region IdentifySystemNodes
            int iSysExtRec = 0 + (5 * s); // System's external receiver node
            int iSysExtEmi = 1 + (5 * s); // System's external emitter node
            int iSysCen = 2 + (5 * s); // System's central node
            int iSysIntRec = 3 + (5 * s); // System's internal receiver node
            int iSysIntEmi = 4 + (5 * s); // System's internal emitter node
            #endregion

            for (int _s = 0; _s < nSys; _s++)
            {
                #region IntersystemEdges
                int _iSysExtRec = 0 + (5 * _s);
                if (s != _s)
                {
                    edgeCost[iSysExtEmi, _iSysExtRec] = Mathf.Pow((system[s].Position - system[_s].Position).magnitude, 1.1f);
                    edgeCapacity[iSysExtEmi, _iSysExtRec] = Mathf.Infinity;
                }
                else
                {
                    edgeCost[iSysExtEmi, _iSysExtRec] = Mathf.Infinity;
                }
                #endregion
            }

            #region IntrasystemEdges
            edgeCost[iSysExtRec, iSysCen] = system[s].ImportExportTax[0];
            edgeCapacity[iSysExtRec, iSysCen] = system[s].ImportExportCapacity[0];
            edgeCost[iSysCen, iSysExtEmi] = system[s].ImportExportTax[1];
            edgeCapacity[iSysCen, iSysExtEmi] = system[s].ImportExportCapacity[1];
            edgeCost[iSysIntRec, iSysCen] = system[s].InternalTax[0];
            edgeCapacity[iSysIntRec, iSysCen] = system[s].InternalCapacity[0];
            edgeCost[iSysCen, iSysIntEmi] = system[s].InternalTax[1];
            edgeCapacity[iSysCen, iSysIntEmi] = system[s].InternalTax[1];
            #endregion

            foreach (BodyObject bo in system[s].Body)
            {
                int b = FindBodyIndex(bo, body);

                #region IdentifyBodyNodes
                int iBodExtRec = 0 + (5 * b) + (5 * nSys);
                int iBodExtEmi = 1 + (5 * b) + (5 * nSys);
                int iBodCen    = 2 + (5 * b) + (5 * nSys);
                int iBodIntRec = 3 + (5 * b) + (5 * nSys);
                int iBodIntEmi = 4 + (5 * b) + (5 * nSys);
                #endregion

                #region SystemBodyEdges
                edgeCost[iSysIntEmi, iBodExtRec] = 1f; // TEMPORARY CONSTANT
                edgeCapacity[iSysIntEmi, iBodExtRec] = Mathf.Infinity;
                edgeCost[iBodExtEmi, iSysIntRec] = 1f; // TEMPORARY CONSTANT
                edgeCapacity[iBodExtEmi, iSysIntRec] = Mathf.Infinity;
                #endregion

                #region IntrabodyEdges
                edgeCost[iBodExtRec, iBodCen] = body[b].ImportExportTax[0];
                edgeCapacity[iBodExtRec, iBodCen] = body[b].ImportExportCapacity[0];
                edgeCost[iBodCen, iBodExtEmi] = body[b].ImportExportTax[1];
                edgeCapacity[iBodCen, iBodExtEmi] = body[b].ImportExportCapacity[1];
                edgeCost[iBodIntRec, iBodCen] = body[b].InternalTax[0];
                edgeCapacity[iBodIntRec, iBodCen] = body[b].InternalCapacity[0];
                edgeCost[iBodCen, iBodIntEmi] = body[b].InternalTax[1];
                edgeCapacity[iBodCen, iBodIntEmi] = body[b].InternalCapacity[1];
                #endregion

                foreach (FacilityObject fo in bo.Facility)
                {
                    int f = FindFacilityIndex(fo, facility);

                    #region IdentifyFacilityNodes
                    int iFacRec = 0 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacEmi = 1 + (3 * f) + (5 * nBod) + (5 * nSys);
                    int iFacPro = 2 + (3 * f) + (5 * nBod) + (5 * nSys);
                    #endregion

                    #region BodyFacilityEdges
                    edgeCost[iBodIntEmi, iFacRec] = 0.5f; // TEMPORARY CONSTANT
                    edgeCapacity[iBodIntEmi, iFacRec] = Mathf.Infinity;
                    edgeCost[iFacEmi, iBodIntRec] = 0.5f; // TEMPORARY CONSTANT
                    edgeCapacity[iFacEmi, iBodIntRec] = Mathf.Infinity;
                    #endregion

                    #region IntrafacilityEdges
                    edgeCost[iFacRec, iFacPro] = facility[f].ImportExportTax[0];
                    edgeCapacity[iFacRec, iFacPro] = facility[f].ImportExportCapacity[0];
                    edgeCost[iFacPro, iFacEmi] = facility[f].ImportExportTax[1];
                    edgeCapacity[iFacPro, iFacEmi] = facility[f].ImportExportCapacity[1];
                    #endregion
                }
            }
        }

        for (int a = 0; a < edgeCost.GetLength(0); a++)
            for (int b = 0; b < edgeCost.GetLength(1); b++)
                if (edgeCost[a, b] == 0)
                    edgeCost[a, b] = Mathf.Infinity;
    }

    private int FindBodyIndex(BodyObject b, List<BodyObject> l)
    {
        int i = -1;
        foreach (BodyObject c in l)
        {
            i++;
            if (c == b) return i;
        }
        Debug.LogError("BodyObject not found in list.");
        return -1;
    }

    private int FindFacilityIndex(FacilityObject f, List<FacilityObject> l)
    {
        int i = -1;
        foreach (FacilityObject c in l)
        {
            i++;
            if (c == f) return i;
        }
        Debug.LogError("FacilityObject not found in list.");
        return -1;
    }

    private void IndexEdges()
    {
        int[,] edgeIndex = new int[nNode, nNode];

        int i = -1;
        for (int n = 0; n < edgeCost.GetLength(0); n++)
        {
            for (int _n = 0; _n < edgeCost.GetLength(1); _n++)
            {
                if (edgeCost[n, _n] != Mathf.Infinity)
                {
                    i++;
                    edgeIndex[n, _n] = i;
                }
                else edgeIndex[n, _n] = -1;
            }
        }

        nNonInf = 0;
        for (int a = 0; a < edgeCost.GetLength(0); a++)
            for (int b = 0; b < edgeCost.GetLength(1); b++)
                if (edgeCost[a, b] != Mathf.Infinity) nNonInf++;
    }

    private void BuildAugmentedMatrix()
    {
        augMat = new float[nRow, nCol];

        #region FlowConservation
        for (int r = 0; r < 3; r++) // TEMPORARY CONSTANT
        {
            for (int n = 0; n < nNode; n++)
            {
                for (int _n = 0; _n < nNode; _n++)
                {
                    if (edgeCost[n, _n] != Mathf.Infinity)
                        augMat[(r * nNode) + n, (r * nNonInf)]
                }
            }
        }
        #endregion
    }
}
