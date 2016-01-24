using UnityEngine;
using System.Collections.Generic;
using System;

namespace Player
{
    /// <summary>
    /// Handles player movement and netsynching of positions
    /// </summary>
    public class Player_Movement : Photon.MonoBehaviour
    {

        public float MovementSpeed = 10.0f;

        private Vector3 m_PreviousMovement;
        public Vector3 PreviousMovement { get { return m_PreviousMovement; } }

        // Contains received server keyframes sorted by timestamp (double)
        private KeyframeList<Vector3> m_PositionKeyFrames;

        [System.Obsolete("use  Playercontroller.Conditions instead")]
        public bool isFrozen;

        // Component References
        PlayerController m_PlayerController;


        // Use this for initialization
        void Start()
        {

            m_PositionKeyFrames = new KeyframeList<Vector3>();
            m_PlayerController  = GetComponent<PlayerController>();

        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.isMine)
            {
                Vector3 inputDirection;
                inputDirection = GetInputDirection(ControllerType.Keyboard);
                Vector3 actualMoveDirection = GetMaxMovementDirection(inputDirection);

                if (m_PlayerController.Conditions.AllowPlayerMovement())
                {
                    transform.position += actualMoveDirection * MovementSpeed * Time.deltaTime;
                    m_PreviousMovement = actualMoveDirection * MovementSpeed * Time.deltaTime;
                }

            }
            else
            {
                int index = m_PositionKeyFrames.GetIndexClosestTo(PhotonNetwork.time);

                if (index != -1 && m_PositionKeyFrames.Count > 1)
                {
                    Vector3 oldPos = transform.position;
                    // smoothing those movements, no prediction going on, just Interpolation
                    Vector3 mostRecentPosition = m_PositionKeyFrames[index].Value;
                    Vector3 currentPosition = transform.position;

                    Vector3 newPosition = Vector3.Lerp(currentPosition, mostRecentPosition, .2f);

                    transform.position = newPosition;
                    m_PreviousMovement = newPosition - oldPos;
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

                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
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
                    //Debug.Log("removing old keyframes");
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
}
