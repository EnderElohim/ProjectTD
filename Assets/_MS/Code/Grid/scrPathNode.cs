using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPathNode
{
    public int gCost;
    public int hCost;
    public int fCost;

    public Vector2Int nodePosition;
    public scrPathNode previousNode;

    private int buildingId;
    private scrGrid<scrPathNode> grid;
    private bool isWalkable;
    //private scrCellObject cellObject;


    public scrPathNode(scrGrid<scrPathNode> _grid, Vector2Int _pos)
    {
        grid = _grid;
        nodePosition = _pos;
        isWalkable = true;
    }

    public Vector3 NodePositionIIID()
    {
        return new Vector3(nodePosition.x, 0, nodePosition.y);
    }

    public scrGrid<scrPathNode> GetGrid()
    {
        return grid;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetWalkable(bool _val)
    {
        isWalkable = _val;
    }

    //public void SetCellObject(scrCellObject _cellObject)
    //{
    //    cellObject = _cellObject;
    //}

    //public scrCellObject GetCellObject()
    //{
    //    return cellObject;
    //}

    public void SetBuildingId(int _id)
    {
        buildingId = _id;
    }

    public int GetBuildingId()
    {
        return buildingId;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return nodePosition.ToString();
    }
}
