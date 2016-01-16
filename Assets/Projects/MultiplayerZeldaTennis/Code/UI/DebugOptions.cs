using UnityEngine;
using System.Collections;

[System.Obsolete("Use DebugGUI instead")]
public class DebugOptions : MonoBehaviour {

    //Singleton
    private static DebugOptions h_Instance;




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    

    public static DebugOptions GetInstance()
    {
        if(h_Instance == null)
        {
            h_Instance = GameObject.FindObjectOfType<DebugOptions>();

            if (h_Instance == null)
                Debug.LogWarning("DebugOptions instance is being requested, but no object has been found using it");

        }
        return h_Instance;
    }
}
