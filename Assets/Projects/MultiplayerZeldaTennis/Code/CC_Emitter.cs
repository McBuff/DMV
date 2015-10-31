using UnityEngine;
using System.Collections;

public class CC_Emitter : MonoBehaviour {

    public CC_Type CrowdControlType;

    public float KnockBack_Distance = 4;
    public float KnockBack_Duration = .5f;

    public float Stun_Duration = .5f;

    

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider) {

        // can I apply CC? ( is player? )
        Player player = collider.GetComponent<Player>();
        if (player)
        {

            if (CrowdControlType == CC_Type.Knockback)
            {
                KnockBack component = player.gameObject.AddComponent<KnockBack>();
                component.StartKnockBack(Vector3.forward, KnockBack_Distance, KnockBack_Duration);
                //TOOD: Send RPC

            }
            else if (CrowdControlType == CC_Type.Stun)
            {
                  /// todo
            }
            else if ( CrowdControlType == CC_Type.Death)
            {

            }
            


        }
    }
}
public enum CC_Type {
    Knockback,
    Stun,
    Death
}
