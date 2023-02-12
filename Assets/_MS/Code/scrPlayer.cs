using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlayer : MonoBehaviour
{
    public Transform testObject;
    private void Update()
    {
        Test();
    }

    private void Test()
    {
        var mousePos = Mouse3D.GetMouseWorldPosition();
        scrPathNode _node = scrPathfinding.manager.GetGrid().GetGridObject(mousePos);
        if (_node == null) return;

        var _pos = (new Vector3(_node.nodePosition.x, 0, _node.nodePosition.y) * scrPathfinding.manager.GetGrid().GetCellSize() + Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        _pos.y = 2;
        testObject.position = _pos;
    }
}
