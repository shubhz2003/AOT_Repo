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
    [SerializeField] private CameraController camera;
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



       /* if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            StartCoroutine(StartGrapple());
        }*/
    }

  /*  private IEnumerator StartGrapple()
    {
        
        isGrappling = true;

        Debug.Log("StartGrapple function");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxGrappleDistance) && hit.collider.CompareTag(grappleTag))
        {
            grapplePoint = hit.point;
            lineRenderer.SetPosition(0, transform.position);  // Set the start of the line
            lineRenderer.SetPosition(1, grapplePoint);  // Set the end of the line
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.5f);  // Delay before grappling
            GrappleToPosition();
        }
        else
        {
            isGrappling = false;
        }
    }*/
/*
    private void GrappleToPosition()
    {
        Debug.Log("GrappleToPosition function");

        // Calculate the direction and distance to the grapple point
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        // Apply a force in the direction of the grapple point
        rb.AddForce(direction * distance * movementSpeed);
    }*/

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var movementInput = (new Vector3(horizontal, 0, vertical)).normalized;

        var movementDirection = camera.flatRotation * movementInput;

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

        if(collision.gameObject.tag == "Enemy")
        {
            health -= 2;
            healthText.text = "Health: " + health.ToString();
        }
       /* if (isGrappling)
        {
            isGrappling = false;
            lineRenderer.enabled = false;
        }*/
    }


}
