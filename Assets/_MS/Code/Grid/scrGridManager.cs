using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrGridManager : MonoBehaviour
{
    public GameObject testObject;
    public Material[] testMaterials;
    public Vector2Int gridDimension;
    public int cellSize;


    private scrPathfinding pathfinding;

    private void Start()
    {
        pathfinding = new scrPathfinding(gridDimension, cellSize, Vector3.zero);
        Vector2Int _enemyPosOne = new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, scrPathfinding.manager.dimension.y - 1);

        scrGameData.endPointNodeCord = new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, scrPathfinding.manager.dimension.y / 2 + 1);
        scrGameData.enemySpawnPositions = new Dictionary<Vector2Int, Vector3>();
        scrGameData.enemySpawnPositions.Add(_enemyPosOne, pathfinding.GetNode(_enemyPosOne).GetCalculatedWorldPositionFromNodePosition());
        pathfinding.GetNode(_enemyPosOne).SetBuildable(false);
        scrGameData.InitNavData();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var _targetNode = scrPathfinding.manager.GetGrid().GetGridObject(scrGameData.endPointNodeCord + new Vector2Int(x,y));
                _targetNode.SetBuildable(false);
            }
        }

        var obstacleNodes = pathfinding.GetGeneratedObstacleNodes(0.4f, scrGameData.GetNavigationData());

        if (obstacleNodes == null) return;

        foreach (scrPathNode item in obstacleNodes)
        {
            GameObject _go = Instantiate(testObject);
            _go.GetComponent<Renderer>().material = testMaterials[Random.Range(0, testMaterials.Length)];
            _go.transform.position = item.GetCalculatedWorldPositionFromNodePosition();
        }

    }

    private void Update()
    {
        //return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 mouseWorldPosition = scrUtilities.GetMouseWorldPosition();
        //    pathfinding.GetGrid().WorldPosToXY(mouseWorldPosition, out Vector3Int _pos);
        //    List<scrPathNode> path = pathfinding.FindPath(new Vector3Int(0, 0), _pos);

        //    if (path != null)
        //    {
        //        for (int i = 0; i < path.Count - 1; i++)
        //        {
        //            Debug.DrawLine(new Vector3(path[i].nodePosition.x, path[i].nodePosition.y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].nodePosition.x, path[i + 1].nodePosition.y) * 10f + Vector3.one * 5f, Color.red, 100);
        //        }
        //    }
        //}

    }

    public bool PlaceEnemyBuilding(int _buildingId, scrPathNode _node, out Vector3 _pos, out List<scrPathNode> _foundations)
    {
        _pos = Vector3.zero;
        _foundations = new List<scrPathNode>();
        Vector3 mouseWorldPosition = scrUtilities.GetMouseWorldPosition();

        if (_node == null) return false;

        if (_node.IsWalkable() == true)
        {
            Vector2Int _dimension = new Vector2Int(1, 1);//scrGameData.values.cellObjects[_buildingId].dimension;
            //List<scrPathNode> _controlledNodes = new List<scrPathNode>();
            for (int x = 0; x < _dimension.x; x++) //Check nods with build size if it already had a building or obstacle 
            {
                for (int y = 0; y < _dimension.y; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (_node.GetNodePosition().x + x >= gridDimension.x || _node.GetNodePosition().y + y >= gridDimension.y)
                    {
                        return false;
                    }

                    scrPathNode _controlNode = pathfinding.GetNode(_node.GetNodePosition() + new Vector2Int(x, y));

                    if (_controlNode.IsWalkable() == false)
                    {
                        return false;
                    }

                    _foundations.Add(_controlNode);
                }
            }

            foreach (scrPathNode item in _foundations)
            {
                item.SetWalkable(false);
                item.SetBuildingId(_buildingId);
            }

            _foundations.Add(_node);
            _foundations.Reverse(); // Making _node first member

            _node.SetWalkable(false);
            _node.SetBuildingId(_buildingId);

            _pos = (new Vector3(_node.GetNodePosition().x, 0,_node.GetNodePosition().y) * pathfinding.GetGrid().GetCellSize() + Vector3.one * pathfinding.GetGrid().GetCellSize() * .5f);
            return true;
        }

        return false;
    }

    public bool PlaceBuilding(int _buildingId, out Vector3 _pos, out List<scrPathNode> _foundations)
    {
        _pos = Vector3.zero;
        _foundations = new List<scrPathNode>();
        Vector3 mouseWorldPosition = scrUtilities.GetMouseWorldPosition();
        scrPathNode _node = pathfinding.GetGrid().GetGridObject(scrUtilities.GetMouseWorldPosition());
        if (_node == null) return false;

        if (_node.IsWalkable() == true)
        {
            Vector2Int _dimension = new Vector2Int(1, 1);//scrGameData.values.cellObjects[_buildingId].dimension;
            //List<scrPathNode> _controlledNodes = new List<scrPathNode>();
            for (int x = 0; x < _dimension.x; x++) //Check nods with build size if it already had a building or obstacle 
            {
                for (int y = 0; y < _dimension.y; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (_node.GetNodePosition().x + x >= gridDimension.x || _node.GetNodePosition().y + y >= gridDimension.y)
                    {
                        return false;
                    }

                    scrPathNode _controlNode = pathfinding.GetNode(_node.GetNodePosition() + new Vector2Int(x, y));

                    if (_controlNode.IsWalkable() == false)
                    {
                        return false;
                    }

                    _foundations.Add(_controlNode);
                }
            }

            foreach (scrPathNode item in _foundations)
            {
                item.SetWalkable(false);
                item.SetBuildingId(_buildingId);
            }

            _foundations.Add(_node);
            _foundations.Reverse(); // Making _node first member

            _node.SetWalkable(false);
            _node.SetBuildingId(_buildingId);

            _pos = (new Vector3(_node.GetNodePosition().x, _node.GetNodePosition().y) * pathfinding.GetGrid().GetCellSize() + Vector3.one * pathfinding.GetGrid().GetCellSize() * .5f);
            return true;
        }

        return false;
    }
}
