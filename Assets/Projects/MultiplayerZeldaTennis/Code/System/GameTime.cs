using UnityEngine;
using System.Collections;

class GameTime
{
    // my instance
    private static GameTime h_Instance;
    public static GameTime Instance
    {
        get
        {
            if (h_Instance == null)
                h_Instance = new GameTime();

            return h_Instance;
        }
    }
    public double Time
    {
        get
        {
            if (PhotonNetwork.inRoom)
                return PhotonNetwork.time;
            else return UnityEngine.Time.time;
        }
    }
}

