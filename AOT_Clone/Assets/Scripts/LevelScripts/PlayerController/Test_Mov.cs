using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestMove : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3.0f;
    public float rotSpeed = 600.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraController cameraObj;
    private int health = 100;
    public TextMeshProUGUI healthText;

    public float grappleSpeed = 1.0f; // Speed of grappling movement
    private Vector3 targetPosition;
    private bool isGrappling = false; // Flag to control if the player is grappling

  //  public GameObject cube; // The cube that will be attached to the character
  //  public LineRenderer line;
    //public Transform raycastStartPoint;


    Quaternion requiredRoation;
    private Rigidbody rb;
    private bool isJumping;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthText.text = "Health: " + health.ToString();
        targetPosition = transform.position; // Initialize targetPosition to the current position

       // line = cube.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        PlayerMovement();
        /*Debug.Log("LineRenderer: " + line);
        Debug.Log("Cube: " + cube);*/
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Check if the right mouse button was clicked
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button was pressed");
            // Create a ray from the mouse cursor on screen in the direction of the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           // Ray ray = new Ray(raycastStartPoint.position, Camera.main.transform.forward);

            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast Check");

                // If we hit a game object with a collider
                var target = hit.transform.gameObject;

                // Check if the game object has the tag "Grap"
                if (target.tag == "Grap")
                {
                    // Set the target position to the exact point of impact
                    targetPosition = hit.point;
                    isGrappling = true;
                    // Disable gravity while grappling
                    rb.useGravity = false;
                    // Disable root motion while grappling
                    animator.applyRootMotion = false;
                    Debug.Log("Target Locked");
                   /* line.enabled = true;
                    line.SetPosition(0, cube.transform.position);
                    line.SetPosition(1, targetPosition);*/

                }
            }
        }

        // Move this game object towards the target position only when grappling
        if (isGrappling)
        {
            Debug.Log("GRAPPLE MODE");

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, grappleSpeed * Time.deltaTime);
            // If the game object has reached the target position, re-enable gravity
            if (transform.position == targetPosition)
            {
                isGrappling = false;
                rb.useGravity = true;
                // Re-enable root motion when not grappling
                animator.applyRootMotion = true;
              //  line.enabled = false;
            }
            else
            {
                // Update the LineRenderer's positions
                /*line.SetPosition(0, cube.transform.position);
                line.SetPosition(1, targetPosition);*/
            }
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var movementInput = (new Vector3(horizontal, 0, vertical)).normalized;

        var movementDirection = cameraObj.flatRotation * movementInput;

        if (movementAmount > 0 && !isGrappling)
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

        if (collision.gameObject.tag == "Grap")
        {
            isGrappling = false;
            // Re-enable gravity when a collision occurs
            rb.useGravity = true;
            // Re-enable root motion when a collision occurs
            animator.applyRootMotion = true;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            health -= 2;
            healthText.text = "Health: " + health.ToString();
        }
    }
}
