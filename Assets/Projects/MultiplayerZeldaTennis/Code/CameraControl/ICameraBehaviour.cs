using UnityEngine;
using System.Collections;


namespace Unity_CameraControl
{

    // Learned about the "Strategy" paradigm https://en.wikipedia.org/wiki/Strategy_pattern

    /* 
    Now.. I can't say that the strategy pattern is really helping me out with this camera,
    but I figured that it's better to try it out on a simple setup, type it out at least once.
    Once I've got my mind around it, I can plan it into something that can actually use it. maybe like different weapon types.
    I just dread the idea of writing an infinite amount of Switch Statements for every single new thing I add.
    So because of that, I want to encapsulate as much of the functionality of a new type, to the type itself
    */

    // Interface for the CameraBehaviour Strategy
    public interface ICameraBehaviour
    {
                
        void Execute(Camera cam, Vector3 targetPos, Vector3 targetLookat);
    }


    // Strategies
    //Strategy 1: Idle
    class Idle : ICameraBehaviour
    {
        public void Execute( Camera cam , Vector3 targetPos, Vector3 targetLookat)
        {
            // Do Nothing but set position
            cam.transform.position = targetPos;
            cam.transform.LookAt(targetLookat);
        }
    }


    //Strategy 2: Lerp to target
    class Lerp: ICameraBehaviour
    {
        public void Execute(Camera cam, Vector3 targetPos, Vector3 targetLookat)
        {
            // Lerp and somesuch
            Vector3 oldPos = cam.transform.position;
            Vector3 oldLookat = cam.transform.forward;

            Vector3 newPos = Vector3.Lerp( oldPos , targetPos, .1f); // hard coded .1f
            Vector3 newTarget = Vector3.Lerp(oldLookat, targetLookat , 0.1f);

            cam.transform.position = newPos;
            cam.transform.LookAt(newTarget);
            
        }
    }

    //Strategy 2: Lerp to target
    class FollowPlayer : ICameraBehaviour
    {
        private Vector3 previousLookat;

        public void Execute(Camera cam, Vector3 targetPos, Vector3 targetLookat)
        {
            // Lerp and somesuch
            Vector3 oldPos = cam.transform.position;
            //Vector3 oldLookat = cam.transform.forward;


            Vector3 cameraOffset = cam.GetComponent<CameraController>().CameraOffset;

            Vector3 newPos = Vector3.Lerp(oldPos, targetLookat + cameraOffset, .1f); // hard coded .1f
            Vector3 newTarget = Vector3.Lerp(previousLookat, targetLookat, .1f);

            cam.transform.position = newPos;
            cam.transform.LookAt(newTarget);

            previousLookat = newTarget;

        }
    }






    class CalculateCameraBehaviour {
        private ICameraBehaviour cameraStrategy;

        //c-tor
        public CalculateCameraBehaviour( ICameraBehaviour strategy)
        {
            this.cameraStrategy = strategy;
        }

        // Execute strategy
        public void Execute(Camera cam, Vector3 targetPos, Vector3 targetLookat)
        {
            cameraStrategy.Execute( cam,  targetPos,  targetLookat);
            return; // obselete I know.. but I defy your anger!
        }
    }


}

