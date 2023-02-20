using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class scrGameData : MonoBehaviour
{
    private static soGameData _gameData;
    public static soGameData values
    {
        get
        {
            if (_gameData == null)
            {
                _gameData = Resources.Load("Game Data") as soGameData; // Resources.Load("Game Data") as soGameData;
            }

            return _gameData;
        }
        set
        {
            _gameData = value;
        }
    }

    public static Dictionary<Vector2Int, List<Vector3>> navigationData;
    public static Dictionary<Vector2Int, Vector3> enemySpawnPositions;
    public static Vector2Int endPointNodeCord;
    public static Vector3 endPointPos;

    public static void InitNavData()
    {
        navigationData = new Dictionary<Vector2Int, List<Vector3>>();
        //var _node = scrPathfinding.manager.GetNode(endPointNodeCord);
        //var _pos = (new Vector3(_node.GetNodePosition().x, 0, _node.GetNodePosition().y)  * scrPathfinding.manager.GetGrid().GetCellSize() + Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        endPointPos = scrPathfinding.manager.GetNode(endPointNodeCord).GetCalculatedWorldPositionFromNodePosition();//* scrPathfinding.manager.GetGrid().GetCellSize() + (Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        


        foreach (KeyValuePair<Vector2Int, Vector3> item in enemySpawnPositions)
        {
            print(item.Value + ":" + endPointPos);
            var _calculatedPath = scrPathfinding.manager.FindPath(item.Value, endPointPos);
            navigationData.Add(item.Key, _calculatedPath);
        }
    }

    public static Dictionary<Vector2Int, List<Vector3>> GetNavigationData()
    {
        return navigationData;
    }

    public static bool TryGetNavigationData(Vector2Int _key, out List<Vector3> _val)
    {
        return navigationData.TryGetValue(_key, out _val);
    }

    public static bool CalculateNavigation()
    {
        Dictionary<Vector2Int, List<Vector3>> tempNavigationData = navigationData.ToDictionary(x => x.Key, x => x.Value); //new Dictionary<Vector2Int, List<Vector3>>();

        navigationData.Clear();
        foreach (KeyValuePair<Vector2Int, Vector3> item in enemySpawnPositions)
        {
            var _calculatedPath = scrPathfinding.manager.FindPath(item.Value, endPointPos);
            if(_calculatedPath == null)
            {
                navigationData = tempNavigationData;
                return false;
            }
            else
            {
                navigationData.Add(item.Key, _calculatedPath);
            }
        }

        return true;
    }


    /*
        foreach (KeyValuePair<Vector2Int, List<Vector3>> item in navigationData)
        {
            oldNavigationData.Add(item.Key, item.Value);
        } 

     */
}
