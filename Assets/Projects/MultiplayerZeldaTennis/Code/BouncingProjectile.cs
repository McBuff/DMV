using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Version 2 of the deathball script.
/// This is a cleaned up version.
/// </summary>
public class BouncingProjectile : Photon.MonoBehaviour{

    // Object Interface
    //------------------

    public Vector3 Direction;
    public float MovementSpeed;



    // Network Data:
    //------------------

    private float TrueGameTime;
    private float ArticifialDelay = 2f;
    private List<Projectile2DState> m_PacketBuffer;



    // Use this for initialization
    void Start () {
        m_PacketBuffer = new List<Projectile2DState>();
    }
	
	// Update is called once per frame
	void Update () {

        TrueGameTime += Time.deltaTime;

       if (photonView.isMine)
            Update_Server();
        else Update_NetSync();
	}

    void DrawBuffer( Color color)
    {
        for (int i = 0; i < m_PacketBuffer.Count; i++)
        {
            if( i > 0 )
            {
                Vector2 startpos = m_PacketBuffer[i-1].Position;
                Vector2 endpos = m_PacketBuffer[i].Position; ;

                Debug.DrawLine(new Vector3(startpos.x, 1, startpos.y), new Vector3(endpos.x, 1, endpos.y), color);
            }
        }

    }
    void Update_NetSync() {

        if (m_PacketBuffer.Count > 1)
        {
            DrawBuffer(Color.blue);

            Projectile2DState leftState =   GetPackageBefore(PhotonNetwork.time - ArticifialDelay);
            Projectile2DState rightState=   GetPackageAfter(PhotonNetwork.time - ArticifialDelay);
            
            double currentDTIME = PhotonNetwork.time - leftState.stateTime - ArticifialDelay;
            double targetDTIME = rightState.stateTime - leftState.stateTime;

            float lerpValue = (float) (currentDTIME / targetDTIME);
            transform.position = new Vector3(Mathf.Lerp(leftState.Position.x , rightState.Position.x , lerpValue)
                                                , 0,
                                            Mathf.Lerp(leftState.Position.y, rightState.Position.y, lerpValue));
        }

    }

    void Update_Server() {

        // debug: follow the mouse
        Ray screenray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 endpos = screenray.origin + screenray.direction * 10.0f;

        transform.position = endpos;

    }

    // Network Code
    //------------
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        // SERVER SIDE , stream out projectile info
        if (PhotonNetwork.isMasterClient)
        {
            Projectile2DState newState = new Projectile2DState( PhotonNetwork.time, transform.position, Direction);
            if (stream.isWriting)
            {

                stream.SendNext(newState.stateTime);
                stream.SendNext(newState.Position);
                stream.SendNext(newState.Direction);
            }
        }
        // CLIENT SIDE
        else
        {
            if (stream.isReading)
            {
                // receive packets and add them to the list of packets, ( then sort )
                Projectile2DState receivedState = new Projectile2DState();
                receivedState.stateTime = (double)stream.ReceiveNext();
                receivedState.Position = (Vector2)stream.ReceiveNext();
                receivedState.Direction = (Vector2)stream.ReceiveNext();

                AddPacketToBuffer(receivedState);
                SortPacketBuffer();
                //CleanPacketBuffer(receivedState.stateTime - 10f); // remove old packets if possible
            }
        }
    }

    /// <summary>
    /// Adds a packet to the bufferlist
    /// </summary>
    /// <param name="packet"></param>
    void AddPacketToBuffer(Projectile2DState packet)
    {
        m_PacketBuffer.Add(packet);
    }
    /// <summary>
    /// adds/inserts a packet to a list of packages using the gameTime variable for sorting purposes
    /// </summary>
    /// <param name="packet"></param>
    void AddPacketToBufferSorted(Projectile2DState packet)
    {
        // always sort NEWEST ( highest gametime ) LAST
        if (m_PacketBuffer == null)
            m_PacketBuffer = new List<Projectile2DState>();

        int insertIndex = 0;
        double previousIterationStateTime = 0;
        for (int i = 0; i < m_PacketBuffer.Count; i++)
        {
            if (previousIterationStateTime < packet.stateTime) // if packet is newer
            {
                insertIndex = i;
            }

            previousIterationStateTime = m_PacketBuffer[i].stateTime;
        }
        m_PacketBuffer.Insert(insertIndex, packet);
         //}
                //else
                //{
                //    // A < B ... no C
         
    }

    /// <summary>
    /// Sorts the packet buffer from oldest to newest
    /// </summary>
    void SortPacketBuffer() {

        m_PacketBuffer.Sort(
            delegate( Projectile2DState p1, Projectile2DState p2)
            {
                return p1.stateTime.CompareTo(p2.stateTime);
            }
            );
    }

    
    /// <summary>
    /// Returns the package immediatly following gameTime
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    Projectile2DState GetPackageAfter(double gameTime, bool sortFirst = false)
    {
        if (sortFirst)
            SortPacketBuffer();

        // run down the buffer from OLDEST TO NEWEST, as soon as I hit a time that is BIGGEr I'm in bussiness
        for (int i = 0; i < m_PacketBuffer.Count; i++)
        {
            if (m_PacketBuffer[i].stateTime > gameTime) // first instance hit!
                return m_PacketBuffer[i];
        }

        // no package was found
        Debug.LogError("no package found after given gameTime, returning LAST package");
        return m_PacketBuffer[m_PacketBuffer.Count - 1];

    }

    /// <summary>
    /// Returns the package immediatly before gameTime
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="sortFirst"></param>
    /// <returns></returns>
    Projectile2DState GetPackageBefore( double gameTime, bool sortFirst = false)
    {
        if (sortFirst)
            SortPacketBuffer();

        Projectile2DState returnObject = m_PacketBuffer[0];

        // run down the buffer from OLDEST TO NEWEST, as soon as I hit a time that is BIGGEr I'm in bussiness
        for (int i = 0; i < m_PacketBuffer.Count; i++)
        {
            if (m_PacketBuffer[i].stateTime < gameTime) // first instance hit!
                returnObject = m_PacketBuffer[i];
        }
        return returnObject;
    }

    /// <summary>
    /// removes all past packages from the list from before a specific time
    /// </summary>
    void CleanPacketBuffer(double stateTimeThresshold)
    {
        int removeRangeEnd = 0;

        removeRangeEnd = m_PacketBuffer.FindLastIndex(
            delegate( Projectile2DState p)
            {

                return p.stateTime < stateTimeThresshold;
            }
            );

        if (removeRangeEnd != -1)
        {
            Debug.Log("CleanPacketBuffer: Removing packets until index: " + removeRangeEnd + " of " + m_PacketBuffer.Count);
            m_PacketBuffer.RemoveRange(0, removeRangeEnd);
        }

    }


    private double SimulationTime() {

        return PhotonNetwork.time - ArticifialDelay;
    }

    
}

public struct Projectile2DState {
    public Projectile2DState(double time, Vector2 pos, Vector2 dir) {
        stateTime = time;
        Position = pos;
        Direction = dir;
    }

    public Projectile2DState(double time, Vector3 pos, Vector3 dir)
    {
        stateTime = time;
        Position = new Vector2( pos.x, pos.z );
        Direction = new Vector2(dir.x, dir.z);
    }

    public double stateTime;
    public Vector2 Position;
    public Vector2 Direction; 

}
