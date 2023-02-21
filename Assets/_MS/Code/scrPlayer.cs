using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlayer : MonoBehaviour
{
    //TEST
    public Transform testObject;
    public scrTestSoldier soldier;
    //TEST

    public float moveSpeed = 5f;  // The speed the player moves
    public Camera cam;  // The camera that will be used to determine the mouse position

    CharacterController characterController;  // The CharacterController component attached to the player
    private Vector3 direction;  // The vector to store the player's movement
    private Vector3 mousePos;
    private Animator anim;

    private float turnSmoothVelocity;
    private Vector3 cameraOffset;
    private void Start()
    {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraOffset = Camera.main.transform.position - transform.position;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        HandleMove(new Vector2(horizontal, vertical));

        PlaceBuildingTest();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = transform.position + cameraOffset;
        AimTowardEnemy();
    }

    private void HandleMove(Vector2 _direction)
    {
        var horizontal = _direction.x; var vertical = _direction.y;

        Vector3 _telDir = Vector3.zero;
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (/*currentFocusState == scrEnumTypes.FocusStateEnum.Focused*/ true)
            {
                characterController.Move(direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f/*basicStats.turnSmoothTime*/);

                transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
                _telDir = moveDir;
            }
            anim.SetBool("Moving", true);
            //statsManager.isMoving = true;
        }
        else
        {
            anim.SetBool("Moving", false);
            //statsManager.isMoving = false;
        }


    }
    private void AimTowardEnemy()
    {
        Vector3 lookDir = Mouse3D.GetMouseWorldPosition() ;
        lookDir.y = transform.position.y;
        //transform.DOLookAt(lookDir, 0.3f);
        transform.LookAt(lookDir);

        float velocityZ = Vector3.Dot(direction.normalized, transform.forward);
        float velocityX = Vector3.Dot(direction.normalized, transform.right);

        anim.SetFloat("Velocity X", velocityX);
        anim.SetFloat("Velocity Z", velocityZ);
    }


    private void SendTestEnemy()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            soldier.CalculatePath();
        }
    }

    private void PlaceBuildingTest()
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
                return;
            }

            _node.SetWalkable(false);
            _node.SetBuildable(false);

            if (scrGameData.CalculateNavigation() == true)
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
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }
}
