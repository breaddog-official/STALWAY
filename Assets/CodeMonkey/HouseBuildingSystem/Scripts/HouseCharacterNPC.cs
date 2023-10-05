using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class HouseCharacterNPC : MonoBehaviour {

    public enum Axis {
        XZ,
        XY,
    }

    public enum State {
        Idle,
        MovingToWaypoint,
    }

    [System.Serializable]
    public class Waypoint {
        public Vector3 position;
        public float waitTime;
    }

    [SerializeField] private Axis axis = Axis.XZ;
    [SerializeField] private float moveSpeed = 6f;

    [SerializeField] private List<Waypoint> waypointList;

    private State state;
    private CharacterController characterController;
    private Animator animator;
    private Vector3 lastMoveDir;
    private Waypoint waypoint;
    private float waitingTime;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        state = State.MovingToWaypoint;
        waypoint = waypointList[0];
        waitingTime = 0f;
    }

    private void Update() {
        Vector3 moveDir = Vector3.zero;

        switch (state) {
            case State.Idle:
                waitingTime += Time.deltaTime;
                if (waitingTime >= waypoint.waitTime) {
                    state = State.MovingToWaypoint;
                    waypoint = waypointList[(waypointList.IndexOf(waypoint) + 1) % waypointList.Count];
                }
                break;
            case State.MovingToWaypoint:
                moveDir = (waypoint.position - transform.position);
                moveDir.y = 0f;

                if (moveDir.magnitude < .2f) {
                    state = State.Idle;
                    waitingTime = 0f;
                }
                break;
        }

        moveDir = moveDir.normalized;

        if (moveDir.x != 0 || moveDir.z != 0) {
            // Not idle
            animator.SetBool("isWalking", true);
            lastMoveDir = moveDir;
        } else {
            animator.SetBool("isWalking", false);
        }

        characterController.Move(moveDir * moveSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, -UtilsClass.GetAngleFromVectorFloatXZ(lastMoveDir) + 90f, 0), Time.deltaTime * 25f);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        if (hitRigidbody != null && !hitRigidbody.isKinematic) {
            hitRigidbody.AddForceAtPosition(transform.forward, hit.point);
        }
    }

}
