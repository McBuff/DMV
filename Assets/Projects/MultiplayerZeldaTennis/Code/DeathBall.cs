using UnityEngine;
using System.Collections;

public class DeathBall : Photon.MonoBehaviour {

    public Rigidbody rigidbody;


    public Vector3 Velocity;
    public float Speed = 10f;

    // synching
    private float syncTime = 0f;
    private float syncDelay;
    private float lastSyncTime;
    private Vector3 syncEndPosition;
    

    /// <summary>
    /// The LOCAL LIFE TIME, when this item is spawned. This value is shared between server and client
    /// </summary>
    private float m_Lifetime = 0;
    // this value is used to calculate the lifetime of this object

    private bool m_isInitialised = false;

    private Vector3 Sync_ReceivedPos;
    private Vector3 Sync_ReceivedVelocity;
    private float Sync_TimeSinceLastPackage;
    private float Sync_LastSyncTime;
    

    private float package_overtime = 0;

    private float timeToNextSyncedData = 0;

    private float serverSendrate = 0;
    private float clientReceiveRate = 0;
    public GameObject BounceEffectPrefab;

    // networkdata
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
        /// Object Lifetime
        /// </summary>
        public float Lifetime;
        /// <summary>
        /// Client time of package retrieval
        /// </summary>
        public float cT;
    }
    private ArrayList m_NetworkPackages;
    
    // Use this for initialization
    void Start () {

        m_NetworkPackages = new ArrayList();
        Velocity = new Vector3(1, 0, 1); // default velocity // is this needed?
    }
	
	// Update is called once per frame
	void Update () {
        
        // update object lifetime
        m_Lifetime += Time.deltaTime;

        if (photonView.isMine)
            UpdateServer();
        else {
            // freeze object if I need more network packages
            if( m_NetworkPackages.Count >= 2)
                UpdateSynched();
        };
        
	}
    void UpdateServer() {
        // 
        //transform.position += (Velocity * Time.deltaTime * Speed);
        transform.position = PredictUpdate(transform.position, Velocity, Time.deltaTime);
    }

    void OnGUI()
    {
        GUILayout.Box("");
        GUILayout.Box("Current deathball lifetime: " + m_Lifetime);
    }
    void UpdateSynched() {

        Sync_TimeSinceLastPackage += Time.deltaTime;
        
            // set up variables
        Vector3 PosX = ((networkData)m_NetworkPackages[0]).P;
        Vector3 VelX = ((networkData)m_NetworkPackages[0]).V;
        float LifetimeX = ((networkData)m_NetworkPackages[0]).Lifetime;

        Vector3 PosY = ((networkData)m_NetworkPackages[1]).P;
        Vector3 VelY = ((networkData)m_NetworkPackages[1]).V;
        float LifetimeY = ((networkData)m_NetworkPackages[1]).Lifetime;

        // figure out where in the timeLine LOCAL is

        float syncTimeSpan = LifetimeY - LifetimeX; // this is how old the object gets between 2 target values
        float syncLife = m_Lifetime - LifetimeX - syncTimeSpan;// - 0.1f; // this is how much the object has aged in between targets

        
        // calculate where LOCAL is
        // temp calc:
        //Vector3 newPos = Vector3.LerpUnclamped(PosX, PosY, syncLife / syncTimeSpan);
        Vector3 newPos = PredictUpdate(PosX, VelX, syncLife);

        //TODO: calculate if LOCAL will hit a wall between timeX and timeY,       

        // ball has altered course.. why?
        if (VelX != VelY)
        {
            Vector3 estimatedHitPoint = CalculateContactPoint(PosX, VelX, PosY, VelY);
            Debug.DrawLine(PosX, estimatedHitPoint, Color.red, 4f);
            Debug.DrawLine(estimatedHitPoint, PosY, Color.cyan, 4f);



            float distanceTohitpoint = (estimatedHitPoint - PosX).magnitude;

            float lifeTimeAtHit = distanceTohitpoint / (Speed); // note: Inverted movement algorithm
            //alt
            //lifeTimeAtHit = (estimatedHitPoint - PosX).magnitude / (VelX.magnitude * Speed);

            // not hit yet, head to wall
            if (syncLife < lifeTimeAtHit)
            {
                Debug.DrawLine(newPos, newPos + Vector3.up, Color.green, 2);
                // calculate to wall from startpoint
                newPos = PredictUpdate(PosX, VelX, syncLife);

            }
            else // hit! bounce off!
            {

                // calculate from endpoint to wall
                //newPos = Vector3.zero;
                newPos = PredictUpdate(estimatedHitPoint, VelY, syncLife - lifeTimeAtHit);
                Debug.DrawLine(newPos, newPos + Vector3.up, Color.blue, 2);
            }
        }
        else {
            //DEBUG: draw a white position marker
            Debug.DrawLine(newPos, newPos + Vector3.up, Color.white, 2);
        }



        // find out where a hit will take place, insert it into the predicion algorithm
        //RaycastHit[] hits = Physics.RaycastAll(collisionRay, MaxLifetimeTravelDistance, layerMask);                
        //if (hits.Length != 0)
        //{
        //    // first hit is all that matters
        //    Vector3 hitpoint = hits[0].point;
        //    float distanceTohitpoint = (hitpoint - PosX).magnitude - deathBallRadius;
        //    float lifeTimeAtHit = distanceTohitpoint / (Speed) + LifetimeX;

        //    Debug.Log("Lifetime at NEXT hit " + lifeTimeAtHit);
            

        //    // not hit yet, head to wall
        //    if (m_Lifetime < lifeTimeAtHit)
        //    {
        //        // calculate to wall from startpoint
        //        newPos = PredictUpdate(PosX, VelX, syncLife);
        //    }
        //    else // hit! bounce off!
        //    {
        //        // calculate from endpoint to wall
        //        newPos = PredictUpdate(PosY, -VelY,syncTimeSpan - syncLife);
                
        //    }

        //    //newPos = PredictUpdate(PosX, VelX, syncLife);

        //}

        // apply LOCAL
        transform.position = newPos;

    }

    Vector3 PredictUpdate(Vector3 start_position, Vector3 start_velocity, float dTime) {

        return start_position += start_velocity * ( dTime )  * Speed;
    }

    Vector3 CalculateContactPoint(Vector3 Startpos , Vector3 StartDirection , Vector3 EndPos, Vector3 EndDirection) {

        // rule:
        // http://www.cimt.plymouth.ac.uk/projects/mepres/step-up/sect4/index.htm
        // B = AB * c
        // c = sin(c) * |AC| / sin(b) 
        Vector3 contactPoint = Vector3.zero;

        Vector3 AC = EndPos - Startpos;
        Vector3 CA = Startpos - EndPos;

        Vector3 CB = EndDirection * -1f;
        Vector3 AB = StartDirection;


        float angleA = Mathf.Abs( Vector3.Angle(AB , AC));
        float angleC = Mathf.Abs( Vector3.Angle(CA , CB));
        float angleB = 180f - angleA - angleC;



        float AB_Length = Mathf.Sin(Mathf.Deg2Rad * angleC) * ( (AC).magnitude / Mathf.Sin(Mathf.Deg2Rad * angleB));

        contactPoint = AB.normalized * AB_Length;

        return contactPoint + Startpos;
        // OLD
        /*
        float angleStartEnd = Vector3.Angle(StartDirection, EndDirection);

        contactPoint = Startpos + (Mathf.Sin(angleStartEnd) * (Startpos - EndPos).magnitude * StartDirection);
        Vector3.
        return contactPoint;
        */
    }

    // reflects a vector along a normal 
    Vector3 Reflect(Vector3 v, Vector3 n) {
        Vector3 r;
        // reflect through normal
        // https://asalga.wordpress.com/2012/09/23/understanding-vector-reflection-visually/
        // tldr; R = 2(N \cdot L)N - L

        r = (2 * (Vector3.Dot(n, v)) * n) - v;

        // invert R because I now have a vector pointing towards the normal
        r *= -1f;

        return r;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (photonView.isMine)
        {
            // invert direction on collision
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.red, 5);
                Vector3 N = contact.normal;
                // fix normal
                //if (Mathf.Abs(N.x) > Mathf.Abs(N.y))
                //{
                //    N.y = 0;
                //    if (Mathf.Abs(N.x) > Mathf.Abs(N.z))
                //        N.z = 0;
                //    else N.x = 0;
                //}
                
                N.Normalize();
                               

                Vector3 D = Velocity;

                Vector3 R = Reflect(D, N);
                
                // make sure I remove Y
                R.y = 0;
                R.Normalize();
                Velocity = R;
                
                // create local particle effect               

                CreateBounceParticle(contact.point, contact.normal);
            }
        }
    }

    private void CreateBounceParticle( Vector3 position, Vector3 direction)
    {
        if (BounceEffectPrefab != null)
        {
            GameObject newParticle = GameObject.Instantiate(BounceEffectPrefab);
            newParticle.transform.position = position;
            newParticle.transform.LookAt(direction, Vector3.up);
            newParticle.transform.Rotate(Vector3.right, 90f, Space.Self);
            //newParticle.transform.Rotate(Vector3.right, 90f, Space.Self);
        }

    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // send server position, velocity, and the lifetime at which this snapshot was taken
            stream.SendNext(rigidbody.position);
            stream.SendNext(Velocity);
            stream.SendNext(m_Lifetime);
        }
        else
        {
            // I've chosen to use entity interpolation for the deathball
            // http://www.gabrielgambetta.com/fpm3.html
            // it means that the ball will be shown with a 1 packet delay, but with this, I always have an endpos for my interpolation to turn to

            //  Queue should always be packed with 2
            networkData newPacket = new networkData();

            newPacket.P = (Vector3)stream.ReceiveNext();
            newPacket.V = (Vector3)stream.ReceiveNext();
            newPacket.Lifetime = (float)stream.ReceiveNext();
            newPacket.cT = Time.time; // client time I received this package

            m_NetworkPackages.Add(newPacket);
            if (m_NetworkPackages.Count < 2)
            {
                // some initting for late joiners
                m_Lifetime = newPacket.Lifetime;// - 0.1f;
                //newPacket.Lifetime += .1f;
                // double package and teleport deathball to server's given position
                m_NetworkPackages.Add(newPacket);
                transform.position = newPacket.P;               
            }
            //else if (m_NetworkPackages.Count >= 2)
            //{
            //    // rreducelifteime of first packet with delay
            //    float packageDelay = ((networkData)(m_NetworkPackages[1])).Lifetime - ((networkData)(m_NetworkPackages[0])).Lifetime;
            //    syncDelay = packageDelay;
            //}


            // if MY LOCAL LIFETIME m_Lifetime is not yet at the SECOND PACKAGE'S lifetime, I do NOT remove the FIRST PACKAGE

            if (m_NetworkPackages.Count >= 2)
            {
                float nextPackageLifetime = ((networkData)m_NetworkPackages[1]).Lifetime;

                // remove first package, local lifetime has passed beyond first-second package range
                if (m_Lifetime > nextPackageLifetime)
                {
                    m_NetworkPackages.RemoveAt(0);
                }
            }

        }

    }

    
}
