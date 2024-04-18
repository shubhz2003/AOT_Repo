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
    [SerializeField] private Collider fist;
    private  int health = 100;
    public int Health { get { return health; } private set { health = value; } }
    public TextMeshProUGUI healthText;
    public GameObject leftBloodSword;
    public GameObject rightBloodSword;

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
    private bool isAttacking;

    //Audio
    private AudioSource backgroundAudio;
    [SerializeField] private AudioClip playerRageAudio;
    [SerializeField] private AudioClip takeDamage;

    private AudioSource audioSource;

    private void Start()
    {
        backgroundAudio = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        healthText.text = "Health: " + health.ToString();
        targetPosition = transform.position; 
        isAttacking = false;  
        leftBloodSword.SetActive(false);
        rightBloodSword.SetActive(false);
        fist.enabled = false;
    }

    private void Update()
    {

        //audioSource.clip = backgroundAudio;
        //audioSource.Play();
        if (Input.GetKeyDown(KeyCode.E) && StaminaBar.instance.currentStamina == 100 && !isRaging)
        {
            audioSource.PlayOneShot(playerRageAudio);
            backgroundAudio.volume = 0.3f;
            StaminaBar.instance.UseStamina(100);
            StartCoroutine(StartRageMode());
        }

        PlayerMovement();
        backgroundAudio.volume = 0.3f;
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
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

                if (target.tag == "Grap" || target.tag == "Enemy")
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
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Grap"))
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

        if(collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(),
                                     collision.gameObject.GetComponent<Collider>());
        }
        
    }

    public void DecreaseHealth()
    {
        AudioSource.PlayClipAtPoint(takeDamage, transform.position, 1.0f);
        Health -= 2;
        healthText.text = "Health: " + Health.ToString();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        if(animator.GetFloat("attackMode") == 0)
        {
            leftBloodSword.SetActive(true);
            rightBloodSword.SetActive(true);
        }
        if (animator.GetFloat("attackMode") == 1)
        {
            fist.enabled = true;
        }
        animator.SetBool("isAttacking", isAttacking);
        yield return new WaitForSeconds(3f);
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
        fist.enabled = false;
        leftBloodSword.SetActive(false);
        rightBloodSword.SetActive(false);
    }

    private IEnumerator StartRageMode()
    {
       // backgroundAudio.volume = 0f;
        //AudioSource.PlayClipAtPoint(playerRageAudio, transform.position, 5.0f);
        rageEndTime = Time.time + 30;
        animator.SetFloat("attackMode", 1);
        isRaging = true;
        animator.CrossFade("Raging", 0.1f);
        rb.constraints = RigidbodyConstraints.FreezePosition;

        float scaleTime = 3.0f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 5;
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

        animator.CrossFade("movement", 0.1f);
        rb.constraints = RigidbodyConstraints.None;
        //animator.CrossFade("RageAttack", 0.1f);
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
        Vector3 targetScale = originalScale / 5;
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
        animator.speed = 1.0f;
        animator.SetFloat("attackMode", 0);
        //backgroundAudio.volume = 0.3f;
    }
}
