using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{

    public float speed = 5f;

    public Rigidbody rigidbody;
    public Player_Weapon m_Weapon;
    private Player_Movement m_PlayerMover;

    // State Machine related:
    //-----------------------
    private PlayerState m_CurrentState;
    private float m_CurrentStateStartTime;

    // State synchronisation
    //-----------------------
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    public bool isSurpressed = false;
    
    // Network related
    private struct networkData
    {
        /// <summary>
        /// Server Position
        /// </summary>
        public Vector3 P;
        /// <summary>
        /// Server Velocity
        /// </summary>
        public Vector3 V;
        /// <summary>
        /// Server Velocity
        /// </summary>
        public float Rotation;
        /// <summary>
        /// Object Lifetime
        /// </summary>
        public float Lifetime;
        /// <summary>
        /// Client time of package retrieval
        /// </summary>
        public float cT;
    }

    private Vector3 Direction;
    private float m_Lifetime;
    private ArrayList m_NetworkPackagesList;

    void Start()
    {
        m_NetworkPackagesList = new ArrayList();
        rigidbody = GetComponent<Rigidbody>();

        m_PlayerMover = GetComponent<Player_Movement>();

        m_CurrentState = PlayerState.movement;
        m_CurrentStateStartTime = 0;
    }
    void Update()
    {
        m_Lifetime += Time.deltaTime;

        rigidbody = GetComponent<Rigidbody>();

        //DBUG: Set the text above the player head to current state
        TextMesh text = GetComponentInChildren<TextMesh>();
        text.text = m_CurrentState.ToString();

        if (photonView.isMine)
        {


            // act on current state
            switch (m_CurrentState)
            {
                case PlayerState.movement:
                    //InputMovement();
                    InputLookat();

                    if (Attack_Primary_Down())
                    {
                        
                        object[] parameters = { PlayerState.attacking, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y };
                        photonView.RPC("SetPlayerState", PhotonTargets.Others, parameters);
                        SetPlayerState((int) PlayerState.attacking, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y);
                    }

                    break;
                case PlayerState.attacking:

                    if (m_Lifetime - m_CurrentStateStartTime > .3f)
                    {
                        object[] parameters = { PlayerState.movement, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y };
                        photonView.RPC("SetPlayerState", PhotonTargets.Others, parameters);
                        SetPlayerState((int)PlayerState.movement, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y);
                    }

                    if (Attack_Primary_Down())
                    {

                        object[] parameters = { PlayerState.attacking, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y };
                        photonView.RPC("SetPlayerState", PhotonTargets.Others, parameters);
                        SetPlayerState((int)PlayerState.attacking, m_Lifetime, transform.position, Direction, transform.rotation.eulerAngles.y);
                    }

                    break;
                case PlayerState.launched:
                    break;
                case PlayerState.frozen:
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (m_NetworkPackagesList.Count >= 2)
            {
                SynchedMovement();
                SynchedLookat();
            }

        }
    }

    /// <summary>
    /// Handle movement using local inputs
    /// </summary>
    void InputMovement()
    {
        Vector3 newmovement = new Vector3();
        Vector3 MaxMovement = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            newmovement += CalcMaxMoveDistanceInDirection(Vector3.forward);
        }

        if (Input.GetKey(KeyCode.S))
        {
            newmovement += CalcMaxMoveDistanceInDirection(Vector3.back);            
        }

        if (Input.GetKey(KeyCode.D))
        {

            newmovement += CalcMaxMoveDistanceInDirection(Vector3.right);
        }

        if (Input.GetKey(KeyCode.A))
        {
            newmovement += CalcMaxMoveDistanceInDirection(Vector3.left);
        }



        Direction = newmovement;

        transform.position = transform.position + newmovement * speed * Time.deltaTime;
        //rigidbody.MovePosition(rigidbody.position + newmovement * speed * Time.deltaTime);
    }

    /// <summary>
    /// Handle movement using network data
    /// </summary>
    void SynchedMovement()
    {
        // set up vars
        Vector3 PosX = ((networkData)m_NetworkPackagesList[0]).P;
        Vector3 VelX = ((networkData)m_NetworkPackagesList[0]).V;
        float LifetimeX = ((networkData)m_NetworkPackagesList[0]).Lifetime;

        Vector3 PosY = ((networkData)m_NetworkPackagesList[1]).P;
        Vector3 VelY = ((networkData)m_NetworkPackagesList[1]).V;
        float LifetimeY = ((networkData)m_NetworkPackagesList[1]).Lifetime;

        // magic
        float syncTimeSpan = LifetimeY - LifetimeX; // this is how old the object gets between 2 target values
        float syncLife = m_Lifetime - LifetimeX - syncTimeSpan;// - 0.1f; // this is how much the object has aged in between targets

        //Vector3 newPos = EstimatePositionLinear(PosX, VelX, syncLife);

        
        Vector3 newPos = Vector3.Lerp(PosX, PosY, syncLife/ syncTimeSpan);

        // the player's direction has altered
        //if (VelX != VelY)
        //{
            
        //    Vector3 estimatedTurnPoint =  Toolbox.VectorTools.IntersectionPoint(PosX, VelX, PosY, VelY);

        //    // Draw angle of estimated turnpoint
        //    Debug.DrawLine(PosX, estimatedTurnPoint, Color.red, 4f);
        //    Debug.DrawLine(estimatedTurnPoint, PosY, Color.cyan, 4f);

        //    float distanceToTurnpoint = (estimatedTurnPoint - PosX).magnitude;

        //    float lifeTimeAtHit = distanceToTurnpoint / (speed);

        //    // if not yet turned in locla time, proceed as planned
        //    if (syncLife < lifeTimeAtHit)
        //    {
                
        //        // don't really do anything
        //        newPos = EstimatePositionLinear(PosX, VelX, syncLife); 
        //    }
        //    else // the turnpoint has been passed
        //    {
        //        // calculate pos from turnpoint to current                
        //        newPos = EstimatePositionLinear(estimatedTurnPoint, VelY, syncLife - lifeTimeAtHit);
        //    }

        //}
        

        // apply LOCAL
        transform.position = newPos;
    }

    void InputLookat() {
        // handlemouselook
        Ray screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // debug value of 20 camera distance

        transform.LookAt(screenToWorldRay.origin + screenToWorldRay.direction * 21f, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y , 0f);
        
    }

    void SynchedLookat() {

        // Lerp between beginrot and endrot
        float newRot = 0;

        float RotX = ((networkData)m_NetworkPackagesList[0]).Rotation;
        float lifeX = ((networkData)m_NetworkPackagesList[0]).Lifetime;

        float RotY = ((networkData)m_NetworkPackagesList[1]).Rotation;
        float lifeY = ((networkData)m_NetworkPackagesList[1]).Lifetime;


        newRot = Mathf.LerpAngle(RotX, RotY, (m_Lifetime - lifeX) / (lifeY - lifeX));

        transform.rotation = Quaternion.Euler(0, newRot, 0 );

    }

    bool Attack_Primary_Down() {
        if (Input.GetMouseButtonDown(0)) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Estimate where a player is given a startpos, and a direction.
    /// Works 2 ways.
    /// </summary>
    /// <param name="startpos"></param>
    /// <param name="direction"></param>
    /// <param name="dTime"></param>
    /// <returns></returns>
    Vector3 EstimatePositionLinear( Vector3 startpos, Vector3 direction, float dTime) {
        // init
        Vector3 estimatedPos = Vector3.zero;
        Vector3 maxMovement = CalcMaxMoveDistanceInDirection(direction);

        estimatedPos = startpos + maxMovement.normalized * dTime * speed;

        return estimatedPos;
    }

    Vector3 CalcMaxMoveDistanceInDirection(Vector3 direction) {
        Vector3 adjusteddirection = Vector3.zero;
        float minDistanceToMove = .5f;
        // let's see if I CAN actually go the direction 
        //TODO: check colliision layers
        LayerMask mask = (1 << LayerMask.NameToLayer("WorldCollision"));
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction , mask);
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

   

    // NETWORK
    // -------

    [PunRPC]
    void SetPlayerState(int newState, float EventTime, Vector3 playerPosition, Vector3 playerDirection, float playerOrientation) {

        // convert state to State
        PlayerState newPlayerState = (PlayerState)(newState);

        m_CurrentState = newPlayerState;
        m_CurrentStateStartTime = EventTime;


        if (newPlayerState == PlayerState.attacking)
        {
            // enable attack weapon thing and fire it
            m_Weapon.gameObject.SetActive(true);
            m_Weapon.Attack(playerOrientation);
        }

    }



    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // send position to clients & server?
        if (stream.isWriting)
        {
            // send server position, direction, and the lifetime at which this snapshot was taken
            stream.SendNext(transform.position);
            stream.SendNext(Direction);
            stream.SendNext(transform.rotation.eulerAngles.y); // send Y rotation
            stream.SendNext(m_Lifetime);
        }
        else
        {
            // receive and store data for prediction
            networkData newPacket = new networkData();

            newPacket.P = (Vector3)stream.ReceiveNext();
            newPacket.V = (Vector3)stream.ReceiveNext();
            newPacket.Rotation = (float)stream.ReceiveNext();
            newPacket.Lifetime = (float)stream.ReceiveNext();
            newPacket.cT = Time.time; // client time I received this package

            if (m_NetworkPackagesList == null)
                m_NetworkPackagesList = new ArrayList();

            m_NetworkPackagesList.Add(newPacket);

            // wiat for 2 first packages
            if (m_NetworkPackagesList.Count < 2)
            {
                // some initting for late joiners
                m_Lifetime = newPacket.Lifetime;// - 0.1f;
                //newPacket.Lifetime += .1f;
                // double package and teleport deathball to server's given position
                m_NetworkPackagesList.Add(newPacket);
                transform.position = newPacket.P;
            }

            // remove packages that are no longer needed
            if (m_NetworkPackagesList.Count >= 2)
            {
                float nextPackageLifetime = ((networkData)m_NetworkPackagesList[1]).Lifetime;

                // remove first package, local lifetime has passed beyond first-second package range
                if (m_Lifetime > nextPackageLifetime)
                {
                    m_NetworkPackagesList.RemoveAt(0);
                }
            }

        }

    }

    
}

public enum PlayerState{
    movement,
    attacking,
    launched,
    frozen
}