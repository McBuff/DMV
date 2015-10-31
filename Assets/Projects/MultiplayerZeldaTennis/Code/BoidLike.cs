using UnityEngine;
using System.Collections;


/// <summary>
/// A boidlike steers a target "evasive object" found in it's hierarchy out of the way of other boidlikes. Creaitng a BOIDLIKE flow
/// </summary>
public class BoidLike : MonoBehaviour {

    public int BoidLayer;
    public Transform EvasiveObject;

    public Vector3 evasionVector;
    public Vector3 originalPosition;

    // Use this for initialization
    void Start () {
        originalPosition = EvasiveObject.transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {

        ArrayList boids = FindBoidsInLayer(BoidLayer);
        evasionVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            BoidLike other = ((BoidLike)boids[i]);
            if ( other != this)
            {
                //Debug.DrawLine(this.transform.position, other.transform.position);
                Vector3 ToMe =  this.transform.position - other.transform.position;
                float distanceToMe = ToMe.magnitude;
                float evasiveMod = 0.0f;
                if( distanceToMe < 1f)
                {
                    evasiveMod = Mathf.Lerp(0.5f, 0.05f, distanceToMe);
                }
                evasionVector += ToMe.normalized * evasiveMod * .3f;

            }           
            
        }

        EvasiveObject.transform.localPosition = originalPosition + evasionVector;
        Debug.DrawLine(this.transform.position, this.transform.position + evasionVector, Color.magenta);
    }

    public ArrayList FindBoidsInLayer(int layer)
    {
        // add boidlike to returnlist if Boidlayer matches given: LAYER

        BoidLike[] boidsfound = GameObject.FindObjectsOfType<BoidLike>();
        ArrayList boidsinLayer = new ArrayList();

        foreach (BoidLike boidlike in boidsfound)
        {
            if (boidlike.BoidLayer == layer)
                boidsinLayer.Add(boidlike);
        }

        return boidsinLayer;

    }
}
