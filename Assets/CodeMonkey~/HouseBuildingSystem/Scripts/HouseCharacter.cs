using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class HouseCharacter : MonoBehaviour {

    public enum Axis {
        XZ,
        XY,
    }

    [SerializeField] private Axis axis = Axis.XZ;
    [SerializeField] private float moveSpeed = 3f;


    private CharacterController characterController;
    private Animator animator;
    private Vector3 lastMoveDir;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            moveX = +1f;
        }

        Vector3 moveDir;

        switch (axis) {
            default:
            case Axis.XZ:
                moveDir = new Vector3(moveX, 0, moveY).normalized;
                break;
            case Axis.XY:
                moveDir = new Vector3(moveX, moveY).normalized;
                break;
        }

        moveDir = UtilsClass.ApplyRotationToVectorXZ(moveDir, Camera.main.transform.eulerAngles.y);

        if (moveX != 0 || moveY != 0) {
            // Not idle
            animator.SetBool("isRunning", true);
            lastMoveDir = moveDir;
        } else {
            animator.SetBool("isRunning", false);
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
