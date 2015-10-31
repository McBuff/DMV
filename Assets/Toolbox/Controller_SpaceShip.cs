using UnityEngine;
using System.Collections;

public class Controller_SpaceShip : MonoBehaviour {

    public Quaternion AngularVelocity;
    //public Matrix4x4 AngularVelocity;

	// Use this for initialization
	void Start () {

        AngularVelocity = new Quaternion(0,0,0,1);
        
        
    }
	
	// Update is called once per frame
	void Update () {

        float xForce =0, yForce=0, zForce = 0;
        float rotationForce = 0.08f * Time.deltaTime;
        float angularDecayRate = 0.99f;
        bool buttonpressed = false;

        Quaternion addedAngular = new Quaternion();
        
        // pan/yaw controls
        if (Input.GetKey(KeyCode.A))
        {
            zForce = rotationForce;
            buttonpressed = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            zForce = -rotationForce;
            buttonpressed = true;
        }

        // roll
        if (Input.GetKey(KeyCode.Q))
        {
            yForce = rotationForce;
            buttonpressed = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            yForce = -rotationForce;
            buttonpressed = true;
        }

        // Pitch
        if (Input.GetKey(KeyCode.W))
        {
            xForce = rotationForce;
            buttonpressed = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            xForce = -rotationForce;
            buttonpressed = true;
        }

        addedAngular.Set(xForce, yForce, zForce, 1);
        //AngularVelocity *= addedAngular; // add angular velocity using added force 

        AngularVelocity *= addedAngular;
        transform.rotation *= AngularVelocity; // apply total angular to rotation

        AngularVelocity = CalculateDecay(AngularVelocity);
    }

    /// <summary>
    /// decays rotation , 
    /// </summary>
    /// <param name="q"></param>
    /// <param name="decayRate"></param> 0 == instant decay
    /// <returns></returns>
    private Quaternion CalculateDecay( Quaternion q, float decayRate = 0.99f)
    {
        if (decayRate == 0)
            return new Quaternion(0, 0, 0, 1);

        q.Set(q.x * decayRate, q.y * decayRate, q.z * decayRate, q.w * 1 / decayRate);
        return q;
    }
}
