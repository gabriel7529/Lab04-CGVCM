using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] private float sneakSpeed   = 2.0F;
    [SerializeField] private float normalSpeed  = 5.0F;
    [SerializeField] private float sprintSpeed  = 10.0F;
    [SerializeField] private float jumpHeight   = 1.0F;
    [SerializeField] public  float gravity      = -9.80F;

    private CharacterController controller;
    [SerializeField] private float speed;
    private bool sprinting = false;
    private Vector3 playerVelocity;

    private static string FirstPerson = "FP";
    private static string ThirdPerson = "TP";
    private static string ThirdPerson2 = "TP2";
    private string[] cameraTypes = {FirstPerson, ThirdPerson, ThirdPerson2};
    [SerializeField] private CinemachineCamera[] cameras;
    [SerializeField] private Transform[] cameraTransforms;
    public int currentIdCamera = 0;
    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        speed = normalSpeed;

        AssignCamera();
    }

    private void Update()
    {
        AssignCamera();

        CheckJump();

        CheckSprint();

        CheckSneak();

        CheckCameraUpdate();

        CheckDeath();
    }

    private void FixedUpdate()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if(Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }

        if(cameraTypes[currentIdCamera] == ThirdPerson2)
        {
            inputVector.x *= -1;
            //* inputVector.y *= -1;
        }

        inputVector = inputVector.normalized;

        // Get camera forward and right vectors on the horizontal plane
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Calculate movement direction relative to the camera
        Vector3 moveDirection = (camForward * inputVector.y + camRight * inputVector.x).normalized;

        // Only rotate and move if there is input
        if (moveDirection != Vector3.zero)
        {
            // Rotate the character smoothly toward the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            // Move in that direction
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        // Gravity and jump handling
        playerVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckJump()
    {
        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    private void CheckSprint()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            sprinting = true;
            speed = sprintSpeed;
        }
        else
        {
            sprinting = false;
            speed = normalSpeed;
        }
    }

    private void CheckSneak()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(!sprinting)
            {
                speed = sneakSpeed;
            } 
            else
            {
                speed = sneakSpeed;
                sprinting = false;
            }
        }
    }

    private void AssignCamera()
    {
        cameraTransform = cameraTransforms[currentIdCamera];
        for(int i = 0; i < 3; i++)
        {
            if(i == currentIdCamera)
            {
                cameras[i].Priority = 1000;
            } 
            else
            {
                cameras[i].Priority = 0;
            }
        }
    }

    private void CheckCameraUpdate()
    {
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            currentIdCamera++;
            if(currentIdCamera == 3)
            {
                currentIdCamera = 0;
            }
        }
    }

    private void CheckDeath()
    {
        if(transform.position.y < -10F)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
