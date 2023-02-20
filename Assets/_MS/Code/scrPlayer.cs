using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlayer : MonoBehaviour
{
    public Transform testObject;
    public scrTestSoldier soldier;

    private void Start()
    {
       

    }

    private void Update()
    {
        Test();
    }

    private void Test()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Mouse3D.GetMouseWorldPosition();
            scrPathNode _node = scrPathfinding.manager.GetGrid().GetGridObject(mousePos);
            if (_node == null) 
            {
                Debug.LogError("NoNode");
                return;
            }
            if (_node.IsHaveBuilding() == true)
            {
                Debug.LogError(_node.GetNodePosition() + " is have building");
                return;
            }
            
            _node.SetWalkable(false);
            _node.SetBuildable(false);

            if(scrGameData.CalculateNavigation() == true)
            {
                Transform _tr = Instantiate(testObject);
                _tr.position = _node.GetCalculatedWorldPositionFromNodePosition();
            }
            else
            {
                //Failed to build because of navigation blocked
                print("A");
                _node.SetWalkable(true);
                _node.SetBuildable(true);
            }

        }

        //var _pos = (new Vector3(_node.GetNodePosition().x, 0, _node.GetNodePosition().y) * scrPathfinding.manager.GetGrid().GetCellSize() + Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        //_pos.y = 2;
        //_tr.position = _pos;

        if (Input.GetKeyDown(KeyCode.T))
        {
            soldier.CalculatePath();
        }
    }
}
