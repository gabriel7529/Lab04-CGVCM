using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50F;

    // Update is called once per frame
    private void Update()
    {
        Vector3 temp = new Vector3(0F, rotationSpeed, 0F);
        transform.Rotate(temp * Time.deltaTime);
    }
}
