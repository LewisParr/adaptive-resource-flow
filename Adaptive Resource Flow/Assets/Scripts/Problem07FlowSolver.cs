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

    private void BuildAugmentedMatrix()
    {

    }
}
