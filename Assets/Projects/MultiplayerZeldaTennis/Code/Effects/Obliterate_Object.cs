using UnityEngine;
using System.Collections;

public class Obliterate_Object : MonoBehaviour {

    public ParticleSystem ParticleEffect;
    public Transform ModelObject;

    public Vector3 LastKnownVelocity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        
        // ramp up colors of this model to fullbright & fade away
        MeshRenderer meshRenderer = ModelObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, new Color(1,1,1,0), .1f);

        if (meshRenderer.material.color.a < 0.05f)
            meshRenderer.enabled = false;
	}
}
