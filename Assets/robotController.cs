using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotController : MonoBehaviour
{
    private CharacterController controller;
    public Vector3 playerVelocity;
    public Vector3 playerAcceleration;
    public Vector3 playerForce;
    public Vector3 playerDirection = Vector3.zero;
    public bool groundedPlayer;

    // tunable parameters
    public float playerMass = 2.0f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -1.0f;
    public float playerMaxSpeed = 1.0f;
    public float playerAccelerationScale = 1.0f;
    public float playerDrag = 0.5f;

    private Vector2 playerInput;
    private Vector2 cameraInput;
    private bool jumpInput;

    private void Start()
    {
      controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {

      playerInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
      cameraInput.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
      jumpInput = Input.GetButton("Jump");

      groundedPlayer = controller.isGrounded; // ignoring jumps for now

      if (groundedPlayer && playerVelocity.y <= 0)
      {
        playerVelocity.y = 0f;

        // apply drag and momentum in grounded case
        playerForce = new Vector3(playerInput.x, 0, playerInput.y);
      }

      if (!groundedPlayer) {
        playerForce = new Vector3(playerInput.x, gravityValue, playerInput.y);
      }

      playerAcceleration = playerForce/playerMass - (playerVelocity * playerDrag); // drag effects force directly, and f = ma
      playerVelocity += playerAcceleration*Time.deltaTime*playerAccelerationScale;

      // Handle jumps
      if (jumpInput && groundedPlayer)
      {
        playerVelocity.y += jumpHeight;
        playerVelocity.y += gravityValue * Time.deltaTime;
      }
      // controller.Move(playerVelocity * Time.deltaTime);

      // clamp velocity to playerSpeed
      // playerVelocity = Vector3.ClampMagnitude(playerVelocity, playerMaxSpeed);

      // Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      controller.Move(playerVelocity*Time.deltaTime);

      // rotate player based on direction of motion
      if (playerVelocity != Vector3.zero)
      {
        playerDirection = transform.InverseTransformDirection(playerVelocity);
        playerDirection = new Vector3(0,playerDirection.x,0);
        gameObject.transform.Rotate(playerDirection);
      }

      // rotates player based on move input
      // if (move != Vector3.zero)
      // {
      //     gameObject.transform.forward = move;
      // }


    }
  }
