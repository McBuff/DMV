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

    public static float MaxMovementSpeed = 35f;

    public Vector3 previousPosition;
    public Vector3 nextPosition;

    private Projectile2DState m_lastUsedPacket;
    private float m_PacketTime;
    private float m_gameTimeOfLastPacketAssign;

    // Network Data:
    //------------------

    private float TrueGameTime;
    public float ArticifialDelay = 0f;
    private Vector3 ServerRigidVel;
    private List<Projectile2DState> m_PacketBuffer;



    // Use this for initialization
    void Start () {
        m_PacketBuffer = new List<Projectile2DState>();
    }
	
	// Update is called once per frame
	void Update () {

        TrueGameTime += Time.deltaTime;

       if (photonView.isMine)
            Update_ServerCompute();
        else Update_ClientPredict_Lerp();
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

    #region Updates


    /// <summary>
    /// Clientside prediction algorithm, for players posessing this object
    /// </summary>
    [System.Obsolete]
    void Update_ClientPredict() {

        // get most recent package and calculate position from there?
        if (m_PacketBuffer.Count > 0)
        {
                // init variables
            Projectile2DState currentPackage = GetPackageBefore( PhotonNetwork.time - ArticifialDelay);

            Rigidbody rbdy = GetComponent<Rigidbody>();

            Vector3 clientPos = transform.position; 
            Vector3 serverPos = currentPackage.Position3();

            //Vector3 clientDir = Direction;
            Vector3 serverDir = currentPackage.Direction3();

            
            double clientTime = PhotonNetwork.time - ArticifialDelay;
            double serverTime = currentPackage.stateTime;

            rbdy.velocity = Vector3.zero; // override unity's ridigbody

            Vector3 currentPos = clientPos;
            float dTime = Time.deltaTime;
            

            if(m_lastUsedPacket.stateTime.CompareTo(currentPackage.stateTime) == -1)
            {
                // new packet is used for the first time
                float packAge =(float)( clientTime - serverTime);


                // if collision is imminent, .. don't overwrite?
                // imminent = within packAge
                RaycastHit[] hitinfo = Physics.SphereCastAll(serverPos, .75f, serverDir, packAge * MovementSpeed, LayerMask.GetMask("WorldCollision") );

                if (hitinfo.Length != 0) {
                    
                    // this means a hit would have happened between ServerTime and Now ,
                    // todo: predict clientpos further, or leave it like this
                    Debug.LogWarning("predicted a hit");
                }
                else currentPos = CalculateProjectilePos(serverPos, serverDir, MovementSpeed, packAge);

                //dTime = (float)(clientTime - serverTime); // <--- THIS IS WRONG!, this is the time to the current frame!
                // calculate where the projecitle SHOULD BE!
            }

            // prediction code:
            Vector3 nextBallPos = CalculateProjectilePos(currentPos, Direction, MovementSpeed, dTime);

            // Using rigidbody;'s move, I get physix calculations for everything I do, so this is prefered
            rbdy.MovePosition(nextBallPos);

            nextPosition = nextBallPos;
            previousPosition = currentPos;
            m_lastUsedPacket = currentPackage;
            
        }
    }

    /// <summary>
    /// Updates local client version of the ball, mixing server data and locally simulated data
    /// </summary>
    void Update_ClientPredict_Lerp()
    {

        if (m_PacketBuffer.Count == 0)
            return;
        
        
        // Estimated Result given by server data
        Vector3 predictionResult = Vector3.zero;
        Projectile2DState lastPackage = GetLastPackage();

        double timeSinceLastPackageState = PhotonNetwork.time - lastPackage.stateTime;

        predictionResult = CalculateProjectilePos(lastPackage.Position3() + Vector3.up * .5f, lastPackage.Direction3(), MovementSpeed, (float)timeSinceLastPackageState);// + Time.deltaTime);


        // Locally Simulated result
        Vector3 simulationResult = CalculateProjectilePos(transform.position, Direction, MovementSpeed, Time.deltaTime);

  
        Direction.y = 0;
        Direction.Normalize();

        // blend the 2 results
        float epsillon = (predictionResult - simulationResult).magnitude;
        float maxEpsillon = 1f;

        Vector3 blendedPosition = Vector3.Lerp(simulationResult, predictionResult , epsillon / maxEpsillon);
        //Vector3 blendedDirection = Vector3.Lerp(Direction, lastPackage.Direction3(), epsillon / maxEpsillon); // to not blend yet

        transform.position = blendedPosition;
        Rigidbody rbdy = GetComponent<Rigidbody>();
        rbdy.velocity = Vector3.zero;

        
        
        Direction = lastPackage.Direction3();
        //rbdy.MovePosition(blendedPosition);

    }

    /// <summary>
    /// Owner calculation algorithm , for Servercontroller object to stream to clients
    /// </summary>
    void Update_ServerCompute() {

        // debug: follow the mouse
        //Ray screenray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Vector3 endpos = screenray.origin + screenray.direction * 10.0f;

        //transform.position = endpos;

        // raycast to see how far my 
        Direction.y = 0;
        Direction.Normalize();

        //Vector3 translation = Direction * MovementSpeed * Time.deltaTime;

        Rigidbody rbdy = GetComponent<Rigidbody>();
        rbdy.velocity = Vector3.zero;

        // Using rigidbody;'s move, I get physix calculations for everything I do, so this is prefered
        Vector3 targetPos = CalculateProjectilePos(transform.position, Direction, MovementSpeed, Time.deltaTime);
        rbdy.MovePosition(targetPos);
        //transform.position += (translation); 

    }
    #endregion

    /// <summary>
    /// Returns true if this projectile has received packages
    /// </summary>
    /// <returns></returns>
    public bool HasPackages()
    {
        if (m_PacketBuffer == null)
            return false;

        return (m_PacketBuffer.Count > 0);
    }


    /// <summary>
    /// Calculates ball endpos with given parameters. Used by Server and Client alike
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    /// <param name="dTime"></param>
    /// <returns></returns>
    public Vector3 CalculateProjectilePos(Vector3 startPos, Vector3 direction, float speed, float dTime) {

        Vector3 endPos = startPos + direction * speed * dTime;
        return endPos;

    }


    /// <summary>
    /// Returns a list of all packages
    /// </summary>
    /// <returns></returns>
    public List<Projectile2DState> GetAllPackages() {
        return m_PacketBuffer;
    }


    void DrawPackageInfo(Projectile2DState pack, Color col) {

        Debug.DrawLine(pack.Position3() + Vector3.up, pack.Position3() + pack.Direction3() + Vector3.up , col, 1);
    }
    /// <summary>
    /// Owner side collision detection, fixes ball offset
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
    // invert direction on collision
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point + Vector3.up, contact.normal, Color.red, 5);
            Vector3 N = contact.normal;

            N.Normalize();


            Vector3 D = Direction;

            Vector3 R = Reflect(D, N);

            // send a package to all clients that a bounce took place here!

            // make sure I remove Y
            R.y = 0;
            R.Normalize();


            // calc Overshoot and adjust ball position accordingly
            float collisionSphereRadius = 1.5f / 2f; // TODO: using collisionsphere size set in Collision method
            float overshoot = collisionSphereRadius - (contact.point - transform.position).magnitude;

            // only correct the position of this object is heading TOWARD the wall
            // if the angle between NORMAL and R is less than 90 degrees. I can estimate that the object is already moving away from
            // the contact point
            float deltaNormalCurrentDirection = Vector3.Angle(contact.normal, Direction);

            if (deltaNormalCurrentDirection < 90)
                return; // exit

            if (PhotonNetwork.isMasterClient)
            {
                Direction = R;

            }

            Rigidbody rbdy = GetComponent<Rigidbody>();
            rbdy.velocity = Vector3.zero;

            Vector3 correctedPosition = transform.position + (Direction * overshoot);
            
            rbdy.MovePosition(correctedPosition);

            
            
            // Send custom packet to clients. With some luck it arrives 
            if (PhotonNetwork.isMasterClient == false)
            {
                // I let the CLIENT add a package right after the collision, so that it is later on fooled into thinking the server orchestrated a bounce here.
                //AddManualPackage(correctedPosition, R, PhotonNetwork.time);
                Projectile2DState newPacket = new Projectile2DState(PhotonNetwork.time, correctedPosition, R, false);
                AddPacketToBuffer(newPacket);
                SortPacketBuffer();

            }
                
        }
        
    }
    Vector3 Reflect(Vector3 v, Vector3 n)
    {
        Vector3 r;
        // reflect through normal
        // https://asalga.wordpress.com/2012/09/23/understanding-vector-reflection-visually/
        // tldr; R = 2(N \cdot L)N - L

        r = (2 * (Vector3.Dot(n, v)) * n) - v;

        // invert R because I now have a vector pointing towards the normal
        r *= -1f;

        return r;
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
                //Debug.LogWarning("Reading writing @" + PhotonNetwork.time);
                //Rigidbody rbdy = GetComponent<Rigidbody>();
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
                
                //Debug.Log();

                //Debug.Log("Reading reading @" + PhotonNetwork.time);
                // receive packets and add them to the list of packets, ( then sort )
                Projectile2DState receivedState = new Projectile2DState();
                receivedState.stateTime = (double)stream.ReceiveNext();
                receivedState.Position = (Vector2)stream.ReceiveNext();
                receivedState.Direction = (Vector2)stream.ReceiveNext();
                receivedState.ServerVerified = true;

                if (m_PacketBuffer == null)
                    m_PacketBuffer = new List<Projectile2DState>();

                AddPacketToBuffer(receivedState);
                SortPacketBuffer();
                CleanPacketBuffer( PhotonNetwork.time - 1f); // remove old packets if possible
            }
        }
    }

    /// <summary>
    /// Add a custom keyframe to a client's list of keyframes. Helps improve accuracy
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="stateTime"></param>
    [PunRPC]    
    void AddManualPackage(Vector3 pos, Vector3 dir, double stateTime)
    {
        Projectile2DState newState = new Projectile2DState(stateTime, pos, dir);
        AddPacketToBuffer(newState);
        SortPacketBuffer();
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
    /// adds/inserts a packet t
    /// o a list of packages using the gameTime variable for sorting purposes
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
                //int returnVal = p1.stateTime.CompareTo(p2.stateTime);
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

        Projectile2DState returnObject = m_PacketBuffer[0]; // set to first object

        // run down the buffer from OLDEST TO NEWEST, as soon as I hit a time that is BIGGEr I'm in bussiness
        for (int i = 0; i < m_PacketBuffer.Count; i++)
        {
                
            if (m_PacketBuffer[i].stateTime < gameTime) // as long as stored time < given time it's ok.
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
            //Debug.Log("CleanPacketBuffer: Removing packets until index: " + removeRangeEnd + " of " + m_PacketBuffer.Count);
            m_PacketBuffer.RemoveRange(0, removeRangeEnd);
        }

    }

    [PunRPC]
    public void BallHit(Vector3 hitpos, Vector3 newDirection , double netTime)
    {

        if (MovementSpeed == 0)
            AdjustMovementSpeed(+8.0f);
        else AdjustMovementSpeed(+5f);

        Direction = newDirection;
        // add new keyframe
        Projectile2DState newKeyFrame = new Projectile2DState(netTime, hitpos, newDirection, false);
        AddPacketToBuffer(newKeyFrame);
    }

    /// <summary>
    /// Adjust movement speed ( add or subtract )
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustMovementSpeed( float amount)
    {
        MovementSpeed += amount;

        MovementSpeed = Mathf.Clamp(MovementSpeed, 0, MaxMovementSpeed);
    }


    Projectile2DState GetLastPackage() {
        return m_PacketBuffer[m_PacketBuffer.Count - 1];
    }

    Projectile2DState GetFirstPackage()
    {
        return m_PacketBuffer[0];
    }

    private double SimulationTime() {

        return PhotonNetwork.time - ArticifialDelay;
    }

    
}

public struct Projectile2DState {

    //public Projectile2DState(double time, Vector2 pos, Vector2 dir , bool serververified = true ) {
    //    stateTime = time;
    //    Position = pos;
    //    Direction = dir;
    //    ServerVerified = serververified;
    //}

    public Projectile2DState(double time, Vector3 pos, Vector3 dir, bool serververified = true)
    {
        stateTime = time;
        Position = new Vector2( pos.x, pos.z );
        Direction = new Vector2(dir.x, dir.z);
        ServerVerified = serververified;
    }

    public double stateTime;
    public Vector2 Position;
    public Vector2 Direction;
    public bool ServerVerified;

    public Vector3 Position3() { return new Vector3(Position.x, 0, Position.y); }
    public Vector3 Direction3() { return new Vector3(Direction.x, 0, Direction.y); }

    public override string ToString() {
        return "Projectile2DState: position " + Position + " ,direction " + Direction + " ,statetime " + stateTime;
    }

}
