using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    public static scrPathfinding manager;

    private scrGrid<scrPathNode> grid;
    private List<scrPathNode> openList;
    private List<scrPathNode> closedList;

    public scrPathfinding(Vector2Int _dimension, int _cellSize, Vector3 _origin)
    {
        manager = this;
        grid = new scrGrid<scrPathNode>(_dimension, _cellSize, _origin, (scrGrid<scrPathNode> _grid, Vector2Int _pos) => new scrPathNode(_grid, _pos));
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.WorldPosToXY(startWorldPosition, out Vector2Int startPos);
        grid.WorldPosToXY(endWorldPosition, out Vector2Int endPos);

        List<scrPathNode> path = FindPath(startPos, endPos);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (scrPathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.nodePosition.x, pathNode.nodePosition.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<scrPathNode> FindPath(Vector2Int _startPos, Vector2Int _targetPos)
    {
        scrPathNode startNode = grid.GetGridObject(_startPos);
        scrPathNode endNode = grid.GetGridObject(_targetPos);
        openList = new List<scrPathNode> { startNode };
        closedList = new List<scrPathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                scrPathNode pathNode = grid.GetGridObject(new Vector2Int(x, y));
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.previousNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = GetCalculatedDistanceCost(startNode, endNode);
        startNode.CalculateFCost();


        while (openList.Count > 0)
        {
            scrPathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return GetCalculatedPath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (scrPathNode item in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(item)) continue; //Already searched
                if (item.IsWalkable() == false)
                {
                    closedList.Add(item);
                    continue;
                }
                int tempGCost = currentNode.gCost + GetCalculatedDistanceCost(currentNode, item);

                if (tempGCost < item.gCost)
                {
                    item.previousNode = currentNode;
                    item.gCost = tempGCost;
                    item.hCost = GetCalculatedDistanceCost(item, endNode);
                    item.CalculateFCost();

                    if (openList.Contains(item) == false)
                    {
                        openList.Add(item);
                    }
                }

            }
        }

        //Out of openList = no path
        return null;
    }

    private List<scrPathNode> GetCalculatedPath(scrPathNode _endNode)
    {
        List<scrPathNode> path = new List<scrPathNode>();
        path.Add(_endNode);
        scrPathNode currentNode = _endNode;

        while (currentNode.previousNode != null)
        {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }

        path.Reverse();

        return path;

    }

    public List<scrPathNode> GetNeighbourList(scrPathNode currentNode)
    {
        List<scrPathNode> neighbourList = new List<scrPathNode>();

        if (currentNode.nodePosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x - 1, currentNode.nodePosition.y)));
            // Left Down
            if (currentNode.nodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x - 1, currentNode.nodePosition.y - 1)));
            // Left Up
            if (currentNode.nodePosition.y + 1 < grid.GetHeight()) neighbourList.Add((GetNode(new Vector2Int(currentNode.nodePosition.x - 1, currentNode.nodePosition.y + 1))));
        }
        if (currentNode.nodePosition.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x + 1, currentNode.nodePosition.y)));
            // Right Down
            if (currentNode.nodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x + 1, currentNode.nodePosition.y - 1)));
            // Right Up
            if (currentNode.nodePosition.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x + 1, currentNode.nodePosition.y + 1)));
        }
        // Down
        if (currentNode.nodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x, currentNode.nodePosition.y - 1)));
        // Up
        if (currentNode.nodePosition.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(new Vector2Int(currentNode.nodePosition.x, currentNode.nodePosition.y + 1)));

        return neighbourList;
    }

    private int GetCalculatedDistanceCost(scrPathNode a, scrPathNode b)
    {
        int xDistance = Mathf.Abs(a.nodePosition.x - b.nodePosition.x);
        int yDistance = Mathf.Abs(a.nodePosition.y - b.nodePosition.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private scrPathNode GetLowestFCostNode(List<scrPathNode> _pathNodelist)
    {
        scrPathNode lowestFCostNode = _pathNodelist[0];
        for (int i = 1; i < _pathNodelist.Count; i++)
        {
            if (_pathNodelist[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = _pathNodelist[i];
            }
        }
        return lowestFCostNode;
    }

    public scrPathNode GetNode(Vector2Int _pos)
    {
        return grid.GetGridObject(_pos);
    }

    public scrGrid<scrPathNode> GetGrid()
    {
        return grid;
    }
}
