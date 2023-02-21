using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scrPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    public static scrPathfinding manager;
    public Vector2Int dimension;
    private scrGrid<scrPathNode> grid;
    private List<scrPathNode> openList;
    //private List<scrPathNode> closedList;

    public scrPathfinding(Vector2Int _dimension, int _cellSize, Vector3 _origin)
    {
        manager = this;
        dimension = _dimension;
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
                vectorPath.Add(new Vector3(pathNode.GetNodePosition().x, 0, pathNode.GetNodePosition().y) * grid.GetCellSize() + Vector3.right * grid.GetCellSize() * .5f);
            }

            return vectorPath;
        }
    }

    public List<scrPathNode> FindPath(Vector2Int _startPos, Vector2Int _targetPos)
    {
        scrPathNode startNode = grid.GetGridObject(_startPos);
        scrPathNode endNode = grid.GetGridObject(_targetPos);
        openList = new List<scrPathNode> { startNode };
        //closedList = new List<scrPathNode>();
        HashSet<scrPathNode> closedList = new HashSet<scrPathNode>();

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

            foreach (scrPathNode item in GetNeighbourListWithoutDiagonal(currentNode))
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

    public List<scrPathNode> GetNeighbourListWithDiagonal(scrPathNode currentNode)
    {
        List<scrPathNode> neighbourList = new List<scrPathNode>();
        Vector2Int currentNodePosition = currentNode.GetNodePosition();
        if (currentNodePosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x - 1, currentNodePosition.y)));
            // Left Down
            if (currentNodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x - 1, currentNodePosition.y - 1)));
            // Left Up
            if (currentNodePosition.y + 1 < grid.GetHeight()) neighbourList.Add((GetNode(new Vector2Int(currentNodePosition.x - 1, currentNodePosition.y + 1))));
        }
        if (currentNodePosition.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x + 1, currentNodePosition.y)));
            // Right Down
            if (currentNodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x + 1, currentNodePosition.y - 1)));
            // Right Up
            if (currentNodePosition.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x + 1, currentNodePosition.y + 1)));
        }
        // Down
        if (currentNodePosition.y - 1 >= 0) neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x, currentNodePosition.y - 1)));
        // Up
        if (currentNodePosition.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x, currentNodePosition.y + 1)));

        return neighbourList;
    }

    public List<scrPathNode> GetNeighbourListWithoutDiagonal(scrPathNode currentNode)
    {
        List<scrPathNode> neighbourList = new List<scrPathNode>();
        Vector2Int currentNodePosition = currentNode.GetNodePosition();

        // Check left node
        if (currentNodePosition.x - 1 >= 0)
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x - 1, currentNodePosition.y)));

        // Check right node
        if (currentNodePosition.x + 1 < grid.GetWidth())
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x + 1, currentNodePosition.y)));

        // Check down node
        if (currentNodePosition.y - 1 >= 0)
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x, currentNodePosition.y - 1)));

        // Check up node
        if (currentNodePosition.y + 1 < grid.GetHeight())
            neighbourList.Add(GetNode(new Vector2Int(currentNodePosition.x, currentNodePosition.y + 1)));

        return neighbourList;
    }


    //return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    private int GetCalculatedDistanceCost(scrPathNode a, scrPathNode b)
    {
        int xDistance = Mathf.Abs(a.GetNodePosition().x - b.GetNodePosition().x);
        int yDistance = Mathf.Abs(a.GetNodePosition().y - b.GetNodePosition().y);
        //int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
        //return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        
    }
    private int GetCalculatedDistanceCostForcedStraight(scrPathNode a, scrPathNode b)
    {
        int xDistance = Mathf.Abs(a.GetNodePosition().x - b.GetNodePosition().x);
        int yDistance = Mathf.Abs(a.GetNodePosition().y - b.GetNodePosition().y);

        // Only straight moves allowed
        int remaining = Mathf.Max(xDistance, yDistance) - Mathf.Min(xDistance, yDistance);

        return MOVE_STRAIGHT_COST * (Mathf.Min(xDistance, yDistance) + remaining);
    }

    //int xDistance = Mathf.Abs(a.nodePosition.x - b.nodePosition.x);
    //int yDistance = Mathf.Abs(a.nodePosition.y - b.nodePosition.y);

    //// Calculate the remaining distance bias based on the difference between x and y distances
    //int remainingBias = Mathf.Abs(xDistance - yDistance);

    //// Calculate the diagonal distance
    //int diagonalDistance = Mathf.Min(xDistance, yDistance);

    //// Add a diagonal bias when the path is far from the target
    //if (diagonalDistance > 0 && diagonalDistance >= remainingBias)
    //{
    //    diagonalDistance += diagonalDistance / 2; // Increase the diagonal distance by 50%
    //}

    //// Calculate the remaining distance
    //int remainingDistance = Mathf.Max(xDistance, yDistance) - diagonalDistance;

    //return MOVE_DIAGONAL_COST * diagonalDistance + MOVE_STRAIGHT_COST * remainingDistance;
    //}

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

    public bool IsPathOpen()
    {
        //var _node = scrPathfinding.manager.GetGrid().GetGridObject(new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, 0));
        //var _pos = (new Vector3(_node.nodePosition.x, 0, _node.nodePosition.y) * scrPathfinding.manager.GetGrid().GetCellSize() + Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        //path = scrPathfinding.manager.FindPath(transform.position, _pos);
        return true;
    }

    public scrGrid<scrPathNode> GetGrid()
    {
        return grid;
    }

    public List<scrPathNode> GenerateObstacleNodes(float obstacleDensity)
    {
        List<scrPathNode> obstacleNodes = new List<scrPathNode>();
        scrGrid<scrPathNode> grid = scrPathfinding.manager.GetGrid();
        float cellSize = grid.GetCellSize();
        float noiseScale = 0.1f;

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                scrPathNode node = grid.GetGridObject(new Vector2Int(x, y));
                if (node.IsHaveBuilding()) continue; // Skip non-buildable or already occupied nodes

                Vector2 nodePos = new Vector2(x, y);
                float noiseValue = Mathf.PerlinNoise(nodePos.x * noiseScale, nodePos.y * noiseScale);

                if (noiseValue < obstacleDensity)
                {
                    obstacleNodes.Add(node);
                }
            }
        }

        return obstacleNodes;
    }

    public List<scrPathNode> GenerateObstacleNodes(float obstacleProbability, float noiseScale)
    {
        List<scrPathNode> obstacleNodes = new List<scrPathNode>();
        scrGrid<scrPathNode> grid = scrPathfinding.manager.GetGrid();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                scrPathNode node = grid.GetGridObject(new Vector2Int(x, y));
                if (node.IsHaveBuilding() == false)
                {
                    float noiseValue = Mathf.PerlinNoise((float)x / grid.GetWidth() * noiseScale, (float)y / grid.GetHeight() * noiseScale);
                    if (noiseValue < obstacleProbability)
                    {
                        obstacleNodes.Add(node);
                    }
                }
            }
        }
        return obstacleNodes;
    }

    public List<scrPathNode> GetGeneratedObstacleNodes(float obstacleDensity, Dictionary<Vector2Int, List<Vector3>> computedPaths)
    {
        List<scrPathNode> obstacleNodes = new List<scrPathNode>();
        scrGrid<scrPathNode> grid = scrPathfinding.manager.GetGrid();
        float cellSize = grid.GetCellSize();
        float noiseScale = 0.1f;
        float testMax = 0; float textMin = Mathf.Infinity;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                scrPathNode node = grid.GetGridObject(new Vector2Int(x, y));
                if (node.IsHaveBuilding() ||node.IsWalkable() == false) continue;

                foreach (KeyValuePair<Vector2Int, List<Vector3>> item in computedPaths)
                {
                   if (IsNodeOnComputedPath(node, item.Value))
                   {
                        continue;
                   }
                }

                node.SetWalkable(false);
                node.SetBuildable(false);

                if (scrGameData.CalculateNavigation() == false)
                {
                    node.SetWalkable(true);
                    node.SetBuildable(true);
                    continue;
                }

                Vector2 nodePos = new Vector2(x, y);
                //noiseScale = Random.Range(0.4f, 0.7f);
                float noiseValue = Mathf.PerlinNoise(nodePos.x * noiseScale, nodePos.y * noiseScale);
                float centerBias = Mathf.Clamp(Vector2Int.Distance(new Vector2Int(x, y), new Vector2Int(grid.GetWidth() / 2, grid.GetHeight() / 2)), 0, 1 * grid.GetCellSize()) / 10;

                if (testMax < centerBias) testMax = centerBias;
                if (textMin > centerBias) textMin = centerBias;

                if (noiseValue - centerBias + Random.Range(0.4f,0.55f)< obstacleDensity)
                {
                    obstacleNodes.Add(node);
                }
                else
                {
                    node.SetWalkable(true);
                    node.SetBuildable(true);
                }
            }
        }
        Debug.Log("min: " + textMin + " max: " + testMax);
        return obstacleNodes;
    }

    private bool IsNodeOnComputedPath(scrPathNode node, List<Vector3> computedPath)
    {
        Vector3 nodePos = node.GetCalculatedWorldPositionFromNodePosition();
        foreach (Vector3 pathNode in computedPath)
        {
            if (Vector3.Distance(nodePos, pathNode) <= 0.1f) return true;
        }
        return false;
    }

    private float CalculateFalloff(float distance, float maxDistance)
    {
        float falloff = distance / maxDistance;
        return falloff * falloff;
    }

}
