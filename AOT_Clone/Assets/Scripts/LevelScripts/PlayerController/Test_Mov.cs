using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    public bool isRaging = false;
    private float rageEndTime;
    private float rageMovementSpeed = 9.0f;

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
        //Stamina testing
        //if (Input.GetKeyDown(KeyCode.E))
        //    StaminaBar.instance.UseStamina(15);

        if (Input.GetKeyDown(KeyCode.E) && StaminaBar.instance.currentStamina == 100 && !isRaging)
        {
            StaminaBar.instance.UseStamina(100);
            StartCoroutine(StartRageMode());
        }

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
            float speed = isRaging ? rageMovementSpeed : movementSpeed;
            transform.position += movementDirection * speed * Time.deltaTime;
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

    private IEnumerator StartRageMode()
    {
        rageEndTime = Time.time + 30;

        isRaging = true;
        animator.SetBool("isRaging", true); // Start the Rage animation

        // Scale the player slowly over time
        float scaleTime = animator.GetCurrentAnimatorStateInfo(0).length; // Get the length of the Rage animation
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 3;
        float originalGap = cameraObj.gap;
        float targetGap = originalGap * 3;
        for (float t = 0; t < scaleTime; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / scaleTime);
            cameraObj.gap = Mathf.Lerp(originalGap, targetGap, t / scaleTime);
            yield return null;
        }
        transform.localScale = targetScale; // Ensure the target scale is reached
        cameraObj.gap = targetGap;
        animator.speed = 0.3f;

        //isRaging = false;
        animator.SetBool("isRaging", false);

        // Wait until the Rage mode should end
        while (Time.time < rageEndTime)
        {
            yield return null;
        }

        StartCoroutine(EndRageMode());
    }

    private IEnumerator EndRageMode()
    {
        // Scale the player and the camera gap back to normal slowly over time
        float scaleTime = animator.GetCurrentAnimatorStateInfo(0).length; // Get the length of the Rage animation
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale / 3;
        float originalGap = cameraObj.gap;
        float targetGap = originalGap / 3;
        for (float t = 0; t < scaleTime; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / scaleTime);
            cameraObj.gap = Mathf.Lerp(originalGap, targetGap, t / scaleTime);
            yield return null;
        }
        transform.localScale = targetScale; // Ensure the target scale is reached
        cameraObj.gap = targetGap; // Ensure the target gap is reached

        isRaging = false;
        animator.SetBool("isRaging", false);
        animator.speed = 1.0f;
    }
}
