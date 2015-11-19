using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles player movement
/// </summary>
public class Player_Movement : Photon.MonoBehaviour {

    public float MovementSpeed = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if( photonView.isMine)
        {
            Vector3 inputDirection;
            inputDirection = GetInputDirection(ControllerType.Keyboard);
            Vector3 actualMoveDirection = GetMaxMovementDirection(inputDirection);

            transform.position += actualMoveDirection * MovementSpeed * Time.deltaTime;
        }
	}

    /// <summary>
    /// Get input for local player controller
    /// </summary>
    /// <returns>Raw input direction</returns>
    Vector3 GetInputDirection(ControllerType controller = ControllerType.Keyboard) {

        Vector3 inputDirection = Vector3.zero;

        switch (controller)
        {
            case ControllerType.Keyboard:

                // keyboard
                bool inputRegistered = false;

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Z))
                {
                    inputRegistered = true;
                    inputDirection += Vector3.left;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    inputRegistered = true;
                    inputDirection += Vector3.right;
                }
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
                {
                    inputRegistered = true;
                    inputDirection += Vector3.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    inputRegistered = true;
                    inputDirection += Vector3.back;
                }

                if (inputRegistered)
                    inputDirection.Normalize();

                break;
            case ControllerType.Gamepad:
                Debug.LogWarning("Controller: Assigned to gamepad but no code has been written.");
                break;
            case ControllerType.SteamController:
                Debug.LogWarning("Controller: Assigned to SteamController but no code has been written.");
                break;
            default:
                break;
        }
         // controller

        return inputDirection;
    }

    Vector3 GetMaxMovementDirection(Vector3 inputDirection)
    {
        Vector3 result = inputDirection;
        // do 3 checks:
        // - 1 in actual direction
        // - 2 in directional components

        float xComp = inputDirection.x;
        float zComp = inputDirection.z;
        LayerMask rayCastMask = (1 << LayerMask.NameToLayer("WorldCollision"));

        

        // check axis component
        float xDist =  GetDistanceToWall(new Vector3( xComp, 0, 0) )-.5f;
        float zDist =  GetDistanceToWall(new Vector3( 0, 0, zComp) )-.5f;

        if (xComp == 0)
            xDist = 0;
        if (zComp == 0)
            zDist = 0;

        xDist = Mathf.Clamp(xDist , 0f, 1f) ;
        zDist = Mathf.Clamp(zDist, 0f, 1f);

        result = new Vector3(xComp * xDist/* * Mathf.Sign(xComp) */, 0, zComp *  zDist/* * Mathf.Sign(zComp)*/);

        return result;
    }

    float GetDistanceToWall(Vector3 direction)
    {

        float closestWallDistance = 1000000;
        Vector3 result = direction;

        LayerMask rayCastMask = (1 << LayerMask.NameToLayer("WorldCollision"));

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, rayCastMask);

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                float hitDistance = (hits[i].point - transform.position).magnitude;

                if (hitDistance < closestWallDistance)
                    closestWallDistance = hitDistance;
            }
        }
        return closestWallDistance;
    }

    [Obsolete("CalcMaxMoveDistanceInDirection is deprecated, Please Use GetmaxMovementDirection instead")]
    Vector3 CalcMaxMoveDistanceInDirection(Vector3 direction)
    {
        Vector3 adjusteddirection = Vector3.zero;
        float minDistanceToMove = .5f;
        // let's see if I CAN actually go the direction 
        //TODO: check colliision layers
        LayerMask mask = (1 << LayerMask.NameToLayer("WorldCollision"));
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, mask);
        if (hits != null)
        {

            // if hits are registered ( and there should be )
            float shortestdistance = 10000.0f; // ultimate value

            foreach (RaycastHit hit in hits)
            {
                // see if hitpoint is closer than the previous by measuring a difference in distance
                float hitdistance = (hit.point - transform.position).magnitude;

                if (shortestdistance > hitdistance) // if this distance is closer than the last one
                    shortestdistance = hitdistance;
            }

            // now adjust the movement depending on if a thresshold is reached
            if (shortestdistance > minDistanceToMove)
                adjusteddirection = direction.normalized;
            else adjusteddirection = direction * shortestdistance * 0f;

            // otherwise return zero vector
            return adjusteddirection;

            //newmovement += Vector3.forward;
        }
        else Debug.LogWarning("Wallcheck has detected no colliders, something is off!");


        return adjusteddirection;
    }
}

public enum ControllerType
{
    Keyboard,
    Gamepad,
    SteamController
}
