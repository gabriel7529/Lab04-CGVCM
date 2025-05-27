using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLikeMovement : MonoBehaviour
{
    public float speed = 6.0f;           // Velocidad de avance/retroceso
    public float rotationSpeed = 120.0f; // Velocidad de giro
    public float gravity = 20.0f;
    public float jumpSpeed = 8.0f;       // Opcional, por si quieres salto

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");   // W/S o flechas arriba/abajo
        float horizontal = Input.GetAxis("Horizontal"); // A/D o flechas izquierda/derecha

        if (controller.isGrounded)
        {
            // Movimiento hacia adelante o atrás
            moveDirection = transform.forward * vertical * speed;

            // Salto (opcional)
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Aplicar gravedad
        moveDirection.y -= gravity * Time.deltaTime;

        // Rotación en el eje Y
        transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

        // Movimiento
        controller.Move(moveDirection * Time.deltaTime);
    }
}

