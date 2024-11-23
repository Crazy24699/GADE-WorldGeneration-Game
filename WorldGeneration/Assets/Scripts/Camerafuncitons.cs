using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafuncitons : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public Vector3 MinBounds = new Vector3(-71, 95, -180);  // Far left and far down
    public Vector3 MaxBounds = new Vector3(72, 95, -65);    // Far right and far up

    private Vector3 MovingDirection;

    void Update()
    {
        HandleMovement();
        ClampPosition();
    }

    void HandleMovement()
    {
        MovingDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            MovingDirection += Vector3.forward;

        }

        if (Input.GetKey(KeyCode.S))
        {
            MovingDirection += Vector3.back;

        }

        if (Input.GetKey(KeyCode.A))
        {
            MovingDirection += Vector3.left;

        }

        if (Input.GetKey(KeyCode.D))
        {
            MovingDirection += Vector3.right;

        }

        this.transform.Translate(MovingDirection * MoveSpeed * Time.deltaTime, Space.World);
    }

    void ClampPosition()
    {
        Vector3 ClamppedPosition = transform.position;
        ClamppedPosition.x = Mathf.Clamp(ClamppedPosition.x, MinBounds.x, MaxBounds.x);
        ClamppedPosition.z = Mathf.Clamp(ClamppedPosition.z, MinBounds.z, MaxBounds.z);

        this.transform.position = ClamppedPosition;
    }
}

/*
 far left and far down: Vector3(-71,95,-180)
 far right and far up: Vector3(72,95,-65)
 */
