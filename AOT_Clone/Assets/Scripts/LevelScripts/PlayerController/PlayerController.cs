using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3.0f;
    public float rotSpeed = 600.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraController cameraObj;
    private int health = 100;
    public TextMeshProUGUI healthText;

    Quaternion requiredRoation;
    private Rigidbody rb;
    private bool isJumping;
    private bool isGrounded;

  
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        healthText.text = "Health: " + health.ToString();

    }

    private void Update()
    {
        PlayerMovement();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var movementInput = (new Vector3(horizontal, 0, vertical)).normalized;

        var movementDirection = cameraObj.flatRotation * movementInput;

        if (movementAmount > 0)
        {
            transform.position += movementDirection * movementSpeed * Time.deltaTime;
            requiredRoation = Quaternion.LookRotation(movementDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, requiredRoation, rotSpeed * Time.deltaTime);

        animator.SetFloat("moveValue", movementAmount, 0.2f, Time.deltaTime);

        // Update animator parameters
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y <= 0);
        animator.SetBool("isGrounded", isGrounded);
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        isJumping = true;
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            health -= 2;
            healthText.text = "Health: " + health.ToString();
        }
    }


}
