using UnityEngine;
using System.Collections;

namespace Player { 
/// <summary>
/// This script reacts on trigger hits between weapons and players/deathballs, it sends a signal back to the owner
/// </summary>
public class WeaponCollisionManager : MonoBehaviour {

    private Player_Weapon m_Owner;

    // Use this for initialization
    void Start () {
        // set owned
        m_Owner = transform.parent.GetComponent<Player_Weapon>();
        if (m_Owner == null)
            Debug.LogError("This weaponcollisionmanager has no weapon attached!");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        // let's see if a deathball has been hit
        BouncingProjectile dBall = collider.transform.GetComponent<BouncingProjectile>() as BouncingProjectile;
        if (dBall != null)
        {

            // some debug info
            //Debug.Log("WeaponCollisionManager has hit a ball. Telling ball to be Hit!");

            // a deathball was hit, change its direction and increase its speed
            Vector3 newDirection = (dBall.transform.position - m_Owner.transform.position);
            newDirection.y = 0f;// clamp y
            //dBall.DoBallHit(newDirection.normalized, collider.transform.position);
            dBall.photonView.RPC("BallHit", PhotonTargets.Others, new object[] { dBall.transform.position, newDirection , PhotonNetwork.time });
            dBall.BallHit(dBall.transform.position, newDirection, PhotonNetwork.time);

        }

            // see if a player was hit
            PlayerController hitplayer = collider.transform.GetComponent<PlayerController>() as PlayerController;
            if(hitplayer != null)
            {

                PlayerController owner = GetComponentInParent<PlayerController>() as PlayerController;
                if (owner == hitplayer)
                    return;
#if DEBUG
                Debug.Log(string.Format("Player {0} was hit by {1}" , new object[]{ hitplayer, owner}));
#endif

                // todo: channel this thorugh the player so I can adjust health, hatdrops and cancel attacks as well as network info
                Vector3 hitDir = collider.transform.position - owner.transform.position;
                hitplayer.Conditions.AddCondition(typeof(Condition_Knockback), GameTime.Instance.Time, new object[] { hitDir.normalized });
            }
        

    }
}
}