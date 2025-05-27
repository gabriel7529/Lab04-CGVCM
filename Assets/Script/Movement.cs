using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float speed = 6.0F;
    public float speedRotation;
    public CharacterController characterController;
    public Camera cameraPersonajePrincipal;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    public Camera cameraTerceraPersona;

    public float rotationX;


    void Update()
    {
        // Detecta si el botón izquierdo del mouse está presionado
        if (Input.GetMouseButton(0))
        {
            cameraPersonajePrincipal.enabled = true;
            cameraTerceraPersona.enabled = false;
            MovimientoCamara(); // solo cuando estás en primera persona
        }
        else
        {
            cameraPersonajePrincipal.enabled = false;
            cameraTerceraPersona.enabled = true;
        }
    }


    void FixedUpdate()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void MovimientoCamara()
    {
        float ratonX = Input.GetAxis("Mouse X") * speedRotation;
        float ratonY = Input.GetAxis("Mouse Y") * speedRotation;

        transform.Rotate(Vector3.up * ratonX);

        // Acumular y clamping de rotación vertical
        rotationX -= ratonY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Aplicar la rotación vertical solo a la cámara
        cameraPersonajePrincipal.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("Colisión con zombie. Reiniciando nivel...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        
    }
}