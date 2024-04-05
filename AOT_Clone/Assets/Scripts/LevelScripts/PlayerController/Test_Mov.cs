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

    public float grappleSpeed = 1.0f;
    private Vector3 targetPosition;
    private bool isGrappling = false; 
    

  
    public LineRenderer lineRendererLeft;
    public Transform startPointLeft;

    public LineRenderer lineRendererRight;
    public Transform startPointRight;

    Quaternion requiredRoation;
    private Rigidbody rb;
    private bool isJumping;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthText.text = "Health: " + health.ToString();
        targetPosition = transform.position; 
    }

    private void Update()
    {
        PlayerMovement();
        
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right mouse button was pressed");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast Check");


                var target = hit.transform.gameObject;

                if (target.tag == "Grap")
                {
                    lineRendererLeft.enabled = true;
                    lineRendererRight.enabled = true;

                    lineRendererLeft.positionCount = 2;
                    lineRendererRight.positionCount = 2;

                    targetPosition = hit.point;
                    isGrappling = true;

                    rb.useGravity = false;

                    animator.applyRootMotion = false;
                    Debug.Log("Target Locked");
                    lineRendererLeft.SetPosition(0, startPointLeft.position);
                    lineRendererLeft.SetPosition(1, targetPosition);

                    lineRendererRight.SetPosition(0, startPointRight.position);
                    lineRendererRight.SetPosition(1, targetPosition);

                    Vector3 direction = (targetPosition - transform.position).normalized;
                }
            }
        }

        if (isGrappling)
        {
            Debug.Log("GRAPPLE MODE");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, grappleSpeed * Time.deltaTime);
            lineRendererLeft.SetPosition(0, startPointLeft.position);
            lineRendererRight.SetPosition(0, startPointRight.position);


            if (transform.position == targetPosition)
            {
                isGrappling = false;
                rb.useGravity = true;
                animator.applyRootMotion = true;
              
                lineRendererLeft.enabled = false;
                lineRendererRight.enabled = false;

                lineRendererLeft.positionCount = 0; 
                lineRendererRight.positionCount = 0;

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
            rb.useGravity = true;
            animator.applyRootMotion = true;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            health -= 2;
            healthText.text = "Health: " + health.ToString();
        }
    }
}
