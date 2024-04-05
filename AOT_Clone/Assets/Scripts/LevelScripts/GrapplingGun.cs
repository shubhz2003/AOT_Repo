using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, player, cameraObj;
    private float maxDistance = 100f;
    private SpringJoint joint;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0)) 
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        Debug.Log("Leftclick down");
        RaycastHit hit;

        // Draw the ray in the Scene view
        //Debug.DrawLine(ray.origin, ray.direction * maxDistance, Color.red);

        if (Physics.Raycast(cameraObj.position, cameraObj.forward, out hit, maxDistance, whatIsGrappleable))
        {
            Debug.Log("Raycast hit: " + hit.transform.name); // Log the name of the object hit by the raycast

            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            // Distance grapple will try to keep from grapple point
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Todo: Tweak up a little to adjust dusring gameplay
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            Debug.Log("Grapple started"); // Log when the grapple starts
        }
        else
        {
            Debug.Log("No hit"); // Log when the raycast doesn't hit anything
        }
    }

    void DrawRope()
    {
        // Not drawing if it doesnt exist
        if(!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);

        Debug.Log("Grapple stopped");
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplingPoint()
    {
        return grapplePoint;
    }
}
