using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{

    // Settings
    //------------
    public GameObject Prefab_FX_DeathEffect;
    public GameObject Prefab_PlayerWeapon;

    // Network player, used to identity the player owning this object
    public PhotonPlayer Photonplayer;

    

    [System.Obsolete]
    public float speed = 5f;

    new public Rigidbody rigidbody;
    public Player_Weapon m_Weapon;

    // State Machine related:
    //-----------------------
    private PlayerState m_CurrentState;
    private float m_CurrentStateStartTime;


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
        Direction = Vector3.zero;

        m_CurrentState = PlayerState.movement;
        m_CurrentStateStartTime = 0;

        // Load Player Weapon
        if (Prefab_PlayerWeapon == null)
            Debug.LogWarning("Player: " + Photonplayer.name + " does not have a weapon assigned!");
        GameObject playerWeaponObject = (GameObject) Instantiate(Prefab_PlayerWeapon , transform.position, transform.rotation);
        m_Weapon = playerWeaponObject.GetComponent<Player_Weapon>();
        playerWeaponObject.transform.SetParent(this.transform);
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // I need to figure out who owns me, and update that knowledge in PlayerManager
        PhotonPlayer sender = info.sender;
        Photonplayer = sender;
        PlayerManager.GetInstance().AssignPlayerObjectToPlayer(this, sender);
        
        //DEBUG:      
        // Set color of object to Slot Color
        int photonPlayerSlot = PlayerManager.GetInstance().GetPlayerSlot(sender);
        GetComponentInChildren<Renderer>().material.color = PlayerManager.GetInstance().GetPlayerSlotColor(photonPlayerSlot);
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
            
            
        }

        // disable/enable player movement when attacking
        // disable player movement while attacking
        if (m_Weapon.isAttacking())
        {
            GetComponent<Player_Movement>().enabled = false;
            GetComponent<Player_Orientation>().enabled = false;
        }
        else
        {
            GetComponent<Player_Movement>().enabled = true;
            GetComponent<Player_Orientation>().enabled = true;
        }
    }


    bool Attack_Primary_Down() {
        if (Input.GetMouseButtonDown(0)) {
            return true;
        }
        return false;
    }

    [PunRPC]
    public void Kill()
    {
        Debug.Log("Destroying player object of player: " + Photonplayer);


        // add desintegration to my own Mesh
        //gameObject.AddComponent<Obliterate_Object>();
        if(Prefab_FX_DeathEffect != null)
        {
            GameObject deathEffect =  Instantiate(Prefab_FX_DeathEffect);
            deathEffect.transform.position = transform.position;
    
        }

        // announce player death
        if(photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
        // TODO: Move camera to countdown position

        // Log player death
        string deathMessage = "";
        
        int playerSlot = PlayerManager.GetInstance().GetPlayerSlot(Photonplayer);
        Color playerColor = PlayerManager.GetInstance().GetPlayerSlotColor(playerSlot);
        deathMessage = "<b>" + ColorUtility.ColorRichtText(playerColor, Photonplayer.name) + "</b>" + " was vaporized!";

        EventLog.GetInstance().LogMessage(deathMessage);
    }

    public void Freeze(bool freeze_position, bool freeze_lookat)
    {
        Player_Movement movementComp = GetComponent<Player_Movement>();
        Player_Orientation lookAtComp = GetComponent<Player_Orientation>();
        //movementComp.enabled = !freeze;
        movementComp.isFrozen = freeze_position;
        lookAtComp.isFrozen = freeze_lookat;
    }

    /// <summary>
    /// Called when either I, or a child of mine triggers with another object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // do nothing if colliding with child object
        if (other.transform.IsChildOf(this.transform))
            return;

        // identify Other as deathball , and destroy player
        BouncingProjectile proj = other.GetComponent<BouncingProjectile>();
        if(proj != null)
        {

            if (photonView.isMine)
            {
                
                Debug.Log(this.name + " has hit the ball, should I terminate?");

                // kill self, send message to others that this player has died
                photonView.RPC("Kill", PhotonTargets.Others, null);
                Kill();
            }


        }
    }

    // NETWORK
    // -------

    /// <summary>
    /// Changes player's state
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="EventTime"></param>
    /// <param name="playerPosition"></param>
    /// <param name="playerDirection"></param>
    /// <param name="playerOrientation"></param>
    [PunRPC]
    void SetPlayerState(int newState, float EventTime, Vector3 playerPosition, Vector3 playerDirection, float playerOrientation) {

        // convert state to State
        PlayerState newPlayerState = (PlayerState)(newState);

        m_CurrentState = newPlayerState;
        m_CurrentStateStartTime = EventTime;

        // Player Attack Code
        if (newPlayerState == PlayerState.attacking)
        {
            // enable attack weapon thing and fire it
            //m_Weapon.gameObject.SetActive(true);
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