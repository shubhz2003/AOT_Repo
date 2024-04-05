using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Controller")]
    [SerializeField] private Transform target;
    public float gap = 6.0f;
    [SerializeField] private float rotSpeed = 3.0f;

    [Header("Camera Handling")]
    [SerializeField] private float minVerAngle = -10f;
    [SerializeField] private float maxVerAngle = 45f;
    [SerializeField] private Vector2 framingBalance;
    private float rotX;
    private float rotY;
    public bool invertX;
    public bool invertY;
    float invertXValue;
    float invertYValue;

    private void Start()
    {
       // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        invertXValue = (invertX) ? -1 : 1;
        invertYValue = (invertY) ? -1 : 1;

        rotX += Input.GetAxis("Mouse Y") * invertYValue * rotSpeed;
        rotX = Mathf.Clamp(rotX, minVerAngle, maxVerAngle);
        rotY += Input.GetAxis("Mouse X") * invertXValue * rotSpeed;

        var targetRotation = Quaternion.Euler(rotX, rotY, 0);

        var focusPos = target.position + new Vector3(framingBalance.x, framingBalance.y);

        transform.position = focusPos - targetRotation * new Vector3(0, 0, gap);
        transform.rotation = targetRotation;
    }

    public Quaternion flatRotation => Quaternion.Euler(0, rotY, 0);
}
