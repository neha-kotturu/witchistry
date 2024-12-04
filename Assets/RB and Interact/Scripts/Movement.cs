using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPlayerMovement : MonoBehaviour
{
    
    private float hInput;
    private float vInput;
    private float rbDrag = 4.0f;
    public float moveSpeed = 30f;

    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    void Update()
    {
        uInput();
        ControlDrag();
    }


    private void FixedUpdate() 
    {
        MovePlayer();
    }

    // Function that collects and computes input values from raw input
    private void uInput() 
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
        moveDirection = transform.forward * vInput + transform.right * hInput;
    }

    // Function to apply force in order to move player, based on collected input values
    private void MovePlayer()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
    }

    // Function to set the drag of the rigidbody
    void ControlDrag()
    {
        rb.drag = rbDrag;
    }
}
