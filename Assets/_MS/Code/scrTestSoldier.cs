using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class scrTestSoldier : MonoBehaviour
{
    public Transform targetPos;

    public float movementSpeed = 3f;
    public List<Vector3> path;
    //private NavMeshAgent agent;
    private int currentNodeIndex;
    private bool isCalculatingFinished;
    private NavMeshPath navMeshPath;

    public void CalculatePath()
    {
        //isCalculatingFinished = false;
        //var _node = scrPathfinding.manager.GetGrid().GetGridObject(new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, 0));
        //var _pos = (new Vector3(_node.GetNodePosition().x, 0, _node.GetNodePosition().y) * scrPathfinding.manager.GetGrid().GetCellSize() + Vector3.one * scrPathfinding.manager.GetGrid().GetCellSize() * .5f);
        //path = scrPathfinding.manager.FindPath(transform.position, _pos);
        
        //path = scrGameData.navigationData[new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, scrPathfinding.manager.dimension.y - 1)];
        if(scrGameData.TryGetNavigationData(new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, scrPathfinding.manager.dimension.y - 1), out path) == true)
        {
            isCalculatingFinished = true;
        }
        else
        {
            print("BUG from " + new Vector2Int(scrPathfinding.manager.dimension.x / 2 + 1, scrPathfinding.manager.dimension.y - 1));
        }
        //agent = GetComponent<NavMeshAgent>();

        //if (path != null && path.Count > 0)
        //{
        //    currentNodeIndex = 0;
        //    Vector3 _targetPos = path[currentNodeIndex];
        //    _targetPos.y = transform.position.y;
        //    agent.SetDestination(_targetPos);
        //}
        //path.RemoveAt(0);

        //Vector3[] pathArray = path.ToArray();
        //navMeshPath = new NavMeshPath();
        //NavMesh.CalculatePath(agent.transform.position, pathArray[pathArray.Length - 1], NavMesh.AllAreas, navMeshPath);

        

    }
    private void Update()
    {
        if (isCalculatingFinished == false) return;
        HandleMovement();
        //if (path != null && path.Count > 0)
        //{
        //    Vector3 _targetPos = path[currentNodeIndex];
        //    _targetPos.y = transform.position.y;
        //    float distanceToTarget = Vector3.Distance(agent.transform.position, _targetPos);

        //    if (distanceToTarget < 0.1f && currentNodeIndex < path.Count - 1)
        //    {
        //        currentNodeIndex++;
        //        agent.SetDestination(_targetPos);
        //    }
        //    else
        //    {
        //        print("AA");
        //    }

        //    if (currentNodeIndex == path.Count - 1 && distanceToTarget < 0.1f)
        //    {
        //        // We've reached the end of the path
        //        Debug.Log("Path complete!");
        //    }
        //}
        //else
        //{
        //    print("WTF");
        //}
    }

    private void HandleMovement()
    {
        if (path != null)
        {
            Vector3 targetPosition = path[currentNodeIndex];
            targetPosition.y = transform.position.y;

            if (Vector3.SqrMagnitude(transform.position - targetPosition) > 0.5f * 0.5f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * movementSpeed * Time.deltaTime;
            }
            else
            {
                currentNodeIndex++;
                if (currentNodeIndex >= path.Count)
                {
                    StopMoving();
                }
            }
        }
    }

    public void StopMoving()
    {
        path = null;
    }


}
