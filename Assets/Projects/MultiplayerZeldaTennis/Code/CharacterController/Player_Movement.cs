using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles player movement
/// </summary>
public class Player_Movement : Photon.MonoBehaviour
{

    public float MovementSpeed = 10.0f;

    private KeyframeList<Vector3> m_PositionKeyFrames;

    private Dictionary<double, Vector3> m_Keyframes;

    // Use this for initialization
    void Start()
    {
        m_PositionKeyFrames = new KeyframeList<Vector3>();
        #region testCode

        /*
        // testing out my new Keyframelist class
        //

        KeyframeList<Vector3> m_AutoSortedList = new KeyframeList<Vector3>();
        KeyframeList<Vector3> m_NonAutosortedList = new KeyframeList<Vector3>();
        m_NonAutosortedList.Autosort = false;

        // add some test data
        m_AutoSortedList.Add( 1d, Vector3.forward * 1);
        m_AutoSortedList.Add(2d, Vector3.right * 2);
        m_AutoSortedList.Add(3d, Vector3.up * 3);
        m_AutoSortedList.Add(0d, Vector3.left * 4);

        m_NonAutosortedList.Add(1d, Vector3.forward * 1);
        m_NonAutosortedList.Add(2d, Vector3.right * 2);
        m_NonAutosortedList.Add(3d, Vector3.up *3);
        m_NonAutosortedList.Add(0d, Vector3.left * 4);

        //
        Debug.Log("m_AutoSortedList after adding data");
        Debug.Log(m_AutoSortedList.ToString());

        Debug.Log("m_NonAutosortedList after adding data");
        Debug.Log(m_NonAutosortedList.ToString());


        Debug.Log("Getting values by Index retreival");
        Debug.Log("index 0: " + m_AutoSortedList[0].ToString());
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());
        Debug.Log("index 2: " + m_AutoSortedList[2].ToString());
        Debug.Log("index 3: " + m_AutoSortedList[3].ToString());

        Debug.Log("m_AutoSortedList range 1-2");
        KeyframeList<Vector3> autoSortedRange = m_AutoSortedList.Range(1, 2);
        Debug.Log(autoSortedRange.ToString());

        Debug.Log("m_AutoSortedList range 1-2");
        KeyframeList<Vector3> non_autoSortedRange = m_NonAutosortedList.Range(1, 2);
        Debug.Log(non_autoSortedRange.ToString());


        Debug.Log("Getting values by DOUBLE retreival");
        Debug.Log("index 1d: " + m_AutoSortedList[1d].ToString());

        Debug.Log("FINDING INDICES-");
        Debug.Log("FINDING First Before 2 =" + m_AutoSortedList.GetIndexFirstBefore( 2d ));
        Debug.Log("FINDING First Before 2.5 =" + m_AutoSortedList.GetIndexFirstBefore(2.5d));
        Debug.Log("FINDING First Before 0 =" + m_AutoSortedList.GetIndexFirstBefore(0));

        Debug.Log("FINDING First After 0 =" + m_AutoSortedList.GetIndexFirstAfter(0));
        Debug.Log("FINDING First After 1 =" + m_AutoSortedList.GetIndexFirstAfter(1));
        Debug.Log("FINDING First After 1.5 =" + m_AutoSortedList.GetIndexFirstAfter(1.5));
        Debug.Log("FINDING First After 5 =" + m_AutoSortedList.GetIndexFirstAfter(5));

        Debug.Log("FINDING Closest to 5 =" + m_AutoSortedList.GetIndexClosestTo(5));
        Debug.Log("FINDING Closest to -10 =" + m_AutoSortedList.GetIndexClosestTo(-10));
        Debug.Log("FINDING Closest to 2 =" + m_AutoSortedList.GetIndexClosestTo(2));
        Debug.Log("FINDING Closest to 2.3 =" + m_AutoSortedList.GetIndexClosestTo(2.3));
        Debug.Log("FINDING Closest to 2.5 =" + m_AutoSortedList.GetIndexClosestTo(2.5));
        Debug.Log("FINDING Closest to 2.6 =" + m_AutoSortedList.GetIndexClosestTo(2.6));

        // 
        Debug.Log("REMOVING Stuff");
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());
        m_AutoSortedList.Remove(m_AutoSortedList[1].Key, m_AutoSortedList[1].Value);
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());

        Debug.Log("Overwriting Stuff");
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());
        m_AutoSortedList[1] = new KeyValuePair<double, Vector3>(11, Vector3.zero);
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());
        m_AutoSortedList.Sort();
        Debug.Log("Overwriting Sorting");
        Debug.Log("index 1: " + m_AutoSortedList[1].ToString());
        m_AutoSortedList[11d] = Vector3.down;
        int indexoff11d = m_AutoSortedList.GetIndexClosestTo(9);
        Debug.Log("index of 11d: " + indexoff11d);
        Debug.Log("value of 11d: " + m_AutoSortedList[ indexoff11d ]);
        // Notes:
        /*
        Add() -> Keyvaluepair
        
        */

        
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.isMine)
        {
            Vector3 inputDirection;
            inputDirection = GetInputDirection(ControllerType.Keyboard);
            Vector3 actualMoveDirection = GetMaxMovementDirection(inputDirection);

            transform.position += actualMoveDirection * MovementSpeed * Time.deltaTime;
        }
        else
        {
            int index = m_PositionKeyFrames.GetIndexClosestTo(PhotonNetwork.time);

            if (index != -1 && m_PositionKeyFrames.Count > 1)
            {
                // smoothing those movements, no prediction going on, just Interpolation
                Vector3 mostRecentPosition = m_PositionKeyFrames[index].Value;
                Vector3 currentPosition = transform.position;                

                Vector3 newPosition = Vector3.Lerp( currentPosition, mostRecentPosition , .2f);

                transform.position = newPosition;
            }
        }
    }

    /// <summary>
    /// Get input for local player controller
    /// </summary>
    /// <returns>Raw input direction</returns>
    Vector3 GetInputDirection(ControllerType controller = ControllerType.Keyboard)
    {

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
        //LayerMask rayCastMask = (1 << LayerMask.NameToLayer("WorldCollision"));



        // check axis component
        float xDist = GetDistanceToWall(new Vector3(xComp, 0, 0)) - .5f;
        float zDist = GetDistanceToWall(new Vector3(0, 0, zComp)) - .5f;

        if (xComp == 0)
            xDist = 0;
        if (zComp == 0)
            zDist = 0;

        xDist = Mathf.Clamp(xDist, 0f, 1f);
        zDist = Mathf.Clamp(zDist, 0f, 1f);

        result = new Vector3(xComp * xDist/* * Mathf.Sign(xComp) */, 0, zComp * zDist/* * Mathf.Sign(zComp)*/);

        return result;
    }

    float GetDistanceToWall(Vector3 direction)
    {

        float closestWallDistance = 1000000;
        //Vector3 result = direction;

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


    // Photon Code:
    // Write/Read from data stream
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.isWriting)
        {
            // write time & position to stream
            stream.SendNext(PhotonNetwork.time);
            stream.SendNext(transform.position);

        }
        else
        {
            // receive keyframe
            double time = (double)stream.ReceiveNext();
            Vector3 position = (Vector3)stream.ReceiveNext();
            if (m_PositionKeyFrames == null) m_PositionKeyFrames = new KeyframeList<Vector3>();

            m_PositionKeyFrames.Add(time, position);

            if (m_PositionKeyFrames.Count > 2)
            {
                Debug.Log("removing old keyframes");
                // remove old keyframes ( let's say 5 seconds old? )
                m_PositionKeyFrames.RemoveAllBefore(time - 5);
            }
        }
    }
}

public enum ControllerType
{
    Keyboard,
    Gamepad,
    SteamController
}
