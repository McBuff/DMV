using UnityEngine;
using System.Collections;

/// <summary>
/// List of spawnpoints , interface directly
/// </summary>
public class PlayerSpawns : MonoBehaviour {

    public Transform[] SpawnList;
    private static PlayerSpawns h_instance;


	// Use this for initialization
	void Start () {
        h_instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public static PlayerSpawns GetInstance()
    {
        if (h_instance == null)
        {
            GameObject playerSpawnsObject = GameObject.Find("Spawners");
            PlayerSpawns instance = playerSpawnsObject.GetComponent<PlayerSpawns>();
            h_instance = instance;
        }
        return h_instance;
    }
}
