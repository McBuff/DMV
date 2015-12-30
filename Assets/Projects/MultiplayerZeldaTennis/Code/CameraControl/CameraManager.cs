using UnityEngine;
using System.Collections;


    /// <summary>
    /// Utility Class for camera control, it's mostly used to store refferences and links to existing objects related to camera control, such as Stored Locations ( lobby ) and special information
    /// </summary>
    public class CameraManager : MonoBehaviour
{
    public Transform[] CameraPresets;

    private static CameraManager h_instance;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public CameraPresetData GetCameraPresetPosition(string name)
    {
        CameraPresetData presetData = new CameraPresetData();

        for (int i = 0; i < CameraPresets.Length; i++)
        {
            if (CameraPresets[i] == null)
            {
                Debug.LogError("Camera Preset Array has a null element");
                continue;
            }
            if( CameraPresets[i].transform.name == name)
            {
                presetData.CameraPos = CameraPresets[i].transform;
                presetData.CameraTarget = CameraPresets[i].GetChild(0);
            }
        }

        return presetData;
    }

    public void LerpTo(Transform targetCamPos, Transform targetCamLookat)
    {
        CameraController camController = GetComponentInChildren<CameraController>();
        camController.SetCameraType(new Unity_CameraControl.Lerp());
        camController.targetCameraPositionObject = targetCamPos;
        camController.targetCameraLookAtObject = targetCamLookat;
    }

    public void LerpFollow(  Transform  targetCamLookAt)
    {
        CameraController camController = GetComponentInChildren<CameraController>();
        camController.SetCameraType(new Unity_CameraControl.FollowPlayer());
        camController.targetCameraLookAtObject = targetCamLookAt;
    }

    public static CameraManager GetInstance()
    {
        if (h_instance == null)
        {
            GameObject camController = GameObject.Find("_CameraController");
            h_instance = camController.GetComponent<CameraManager>();

            if (h_instance == null)
                Debug.LogError("Camera Controller script can't find itself");
        }

        return h_instance;

    }


    

}

public struct CameraPresetData
{
    // Name of this preset
    string PresetName;

    public Transform CameraPos;

    public Transform CameraTarget;   

}


