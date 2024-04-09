using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveLogic : MonoBehaviour
{
    public float speed = 1.0f;
    public float maxSpeed = 5.0f;
    private float characterSpeed;
    public Animator animator;
    private GameObject player;
    private Rigidbody rb;

    private GameObject[] waypoints;
    private Vector3 targetPosition;
    private bool isWaiting = false;
    private bool isChasingPlayer = false;
    private bool isAttacking = false;

    private GameObject rightHandBone;
    private Collider rightHandCollider;


    private void Start()
    {
        rightHandBone = GameObject.FindWithTag("TitanRightHand");
        if (rightHandBone != null)
        {
            rightHandCollider = rightHandBone.GetComponent<Collider>();
        }

        //Debug.LogWarning(rightHandBone);
        //rightHandCollider = rightHandBone.GetComponent<Collider>();

        waypoints = GameObject.FindGameObjectsWithTag("checkpoint");
        player = GameObject.FindGameObjectWithTag("Player");
        GenerateNewTarget();
        rb = GetComponent<Rigidbody>();
        characterSpeed = speed;
    }

    private void ChasePlayer()
    {
        targetPosition = player.transform.position;
        characterSpeed = maxSpeed;
        isChasingPlayer = true;
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("titanIsAttacking", isAttacking);
        rb.isKinematic = true;
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        characterSpeed = 0;
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        animator.SetBool("titanIsAttacking", isAttacking);
        yield return new WaitForSeconds(0.4f);
        characterSpeed = speed;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= 4f)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer > 4f && distanceToPlayer <= 11f)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer > 11f)
        {
            isChasingPlayer = false;
            characterSpeed = speed;
        }

        if (!isChasingPlayer && !isAttacking) // Add this check
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= 2f && !isWaiting)
            {
                StartCoroutine(WaitAndGenerateNewTarget());
            }
            else if (isWaiting)
            {
                animator.SetFloat("titanMoveValue", 0);
            }
            else
            {
                MoveTowardsTarget();
            }
        }
        else if (isChasingPlayer && !isAttacking) // Add this check
        {
            MoveTowardsTarget();
        }
       // Debug.Log($"Distance = {distanceToPlayer}");
    }


    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, characterSpeed * Time.deltaTime);
        float moveValue = Mathf.Clamp(characterSpeed / maxSpeed, 0, 1);
        animator.SetFloat("titanMoveValue", moveValue);

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        if (directionToTarget != Vector3.zero)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget > 0.5f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * characterSpeed);
            }
        }
    }


    private void GenerateNewTarget()
    {
        GameObject waypoint = waypoints[Random.Range(0, waypoints.Length)];
        targetPosition = waypoint.transform.position;
    }

    private IEnumerator WaitAndGenerateNewTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2);
        isWaiting = false;
        GenerateNewTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider thisCollider = this.GetComponent<Collider>();

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<AIMoveLogic>() != null)
        {
            Collider otherCollider = collision.collider;
            Physics.IgnoreCollision(thisCollider, otherCollider);
        }

    }
}

