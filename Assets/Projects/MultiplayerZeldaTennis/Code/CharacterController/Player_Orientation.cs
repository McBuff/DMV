using UnityEngine;
using System.Collections;

public class Player_Orientation : Photon.MonoBehaviour
{

    private KeyframeList<double> m_OrientationKeyframesList;

	// Use this for initialization
	void Start () {
        m_OrientationKeyframesList = new KeyframeList<double>();
    }
	
	// Update is called once per frame
	void Update () {

        // Owner Logic
        if (photonView.isMine)
            update_owner();

        // Follower
        else update_other();

    }

    // Updates internal logic as owner of object (player)
    void update_owner() {

        // handlemouselook
        Ray screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // debug value of 20 camera distance

        transform.LookAt(screenToWorldRay.origin + screenToWorldRay.direction * 21f, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0f);
    }

    // Simulates internal logic using received data
    void update_other() {

        // Lerp between beginrot and endrot
        float newRot = 0;

        if (m_OrientationKeyframesList == null)
            return;
        if( m_OrientationKeyframesList.Count > 1)
        {        
            // get last 2 keyframes and lerp between them ( angular Lerp )
            int lastIndex = m_OrientationKeyframesList.GetIndexClosestTo(PhotonNetwork.time);
            int otherIndex = lastIndex - 1;

            newRot = Mathf.LerpAngle( (float)m_OrientationKeyframesList[otherIndex].Value, (float)m_OrientationKeyframesList[lastIndex].Value, .2f);
            
        }

        transform.rotation = Quaternion.Euler(0, newRot, 0);

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.isWriting)
        {
            // write time & position to stream
            stream.SendNext(PhotonNetwork.time);

            // send orientation
            stream.SendNext( (double)transform.rotation.eulerAngles.y); // get Y-rotation

        }
        else
        {
            // receive keyframe
            double time = (double)stream.ReceiveNext();
            double orientation = (double)stream.ReceiveNext();
            if (m_OrientationKeyframesList == null) m_OrientationKeyframesList = new KeyframeList<double>();

            m_OrientationKeyframesList.Add(time, orientation);

            if (m_OrientationKeyframesList.Count > 2)
            {
                //Debug.Log("removing old keyframes");
                // remove old keyframes ( let's say 5 seconds old? )
                m_OrientationKeyframesList.RemoveAllBefore(time - 5);
            }
        }
    }
}
