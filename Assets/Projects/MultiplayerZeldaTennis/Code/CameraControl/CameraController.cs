using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public Transform targetCameraLookAtObject;
    public Transform targetCameraPositionObject;

    public Vector3 CameraOffset;
    public Vector3 CameraLookatOffset;

    private Unity_CameraControl.CalculateCameraBehaviour m_CamController;

    private float cameraSpeed;

    // Use this for initialization
    void Start()
    {
        CameraOffset = new Vector3(0, 15.0f, -2.0f);
        cameraSpeed = .2f;

        // Set camera's strategy to Idle
        SetCameraType(new Unity_CameraControl.Lerp());
        CameraPresetData presetData = CameraManager.GetInstance().GetCameraPresetPosition("CameraPreset_Lobby");
        CameraManager.GetInstance().LerpTo(presetData.CameraPos , presetData.CameraTarget);

    }

    // Update is called once per frame
    void Update()
    {
        if( targetCameraLookAtObject != null && targetCameraPositionObject != null)
               m_CamController.Execute(GetComponent<Camera>(), targetCameraPositionObject.position, targetCameraLookAtObject.position);
        /*

        if(targetObject != null) { 
        // interpolate camera pos with previous pos
        Vector3 currentpos, nextpos;
                
        currentpos = transform.position;
        nextpos = targetObject.position + CameraOffset;

        nextpos = Vector3.Lerp(currentpos , nextpos , cameraSpeed);

        transform.position = nextpos;
        */


    }

    /// <summary>
    /// Set the strategy pattern for this camera
    /// </summary>
    /// <param name="strategy"></param>
    public void SetCameraType(Unity_CameraControl.ICameraBehaviour strategy)
    {
        m_CamController = new Unity_CameraControl.CalculateCameraBehaviour(strategy);
    }

}
