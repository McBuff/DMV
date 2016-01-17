using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// A GUI entirely used for debugging purposes
/// </summary>
public class DebugGUI : MonoBehaviour {

    //Singleton
    private static DebugGUI h_Instance;

    public KeyCode ToggleButton;
    [System.Obsolete()]
    private Vector3 m_OffScreenSet;
    [System.Obsolete()]
    private RectTransform m_Rect;
    private bool m_GUIActive = false;


    public RectTransform DeathBallInfoPanel;
    public RectTransform GameplayStateInfoPanel;

    private bool ShowDeathBallInfo;
    private bool ShowGameplayStateInfo;

    // Use this for initialization
    void Start () {

        // close menus
        ShowDeathBallInfo = false;
        DeathBallInfoPanel.gameObject.SetActive(ShowDeathBallInfo);

        ShowGameplayStateInfo = false;
        GameplayStateInfoPanel.gameObject.SetActive(ShowGameplayStateInfo);

        // move menu to correct world position

        RectTransform rect = GetComponent<RectTransform>();
        rect.position = new Vector2(Screen.width / 2, rect.position.y);


        SetGUIActive(false);
        //m_Rect = GetComponent<RectTransform>();
        //m_OffScreenSet = m_Rect.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(ToggleButton))
            ToggleGUI();

    }

    public void ToggleGUI()
    {
        SetGUIActive(!m_GUIActive);
        
    }

    /// <summary>
    /// Enable/Disable Debug GUI overlay
    /// </summary>
    /// <param name="active"></param>
    public void SetGUIActive(bool active)
    {
        //toggle
        m_GUIActive = active;

        // set child active/inactive
        transform.GetChild(0).gameObject.SetActive(active);
    }

    public void ToggleGameplayStateInfo()
    {
        ShowGameplayStateInfo = !ShowGameplayStateInfo;
        GameplayStateInfoPanel.transform.gameObject.SetActive(ShowGameplayStateInfo);
    }

    public void ToggleDeathBallInfo()
    {
        ShowDeathBallInfo = !ShowDeathBallInfo;
        DeathBallInfoPanel.transform.gameObject.SetActive(ShowDeathBallInfo);

    }

    public bool GameState_DoManualProgression()
    {
        Transform child = GameplayStateInfoPanel.GetChild(0); // TODO: find actual child

        return child.GetComponent<Toggle>().isOn;
    }

    public void SpawnCharacterController()
    {

    }

    
    public bool GetPlayerHitBoxEnabled()
    {
        return GameplayStateInfoPanel.GetChild(2).GetComponent<Toggle>().isOn;
    }


    /// <summary>
    /// Returns Instance singleton
    /// </summary>
    /// <returns></returns>
    public static DebugGUI GetInstance()
    {
        if(h_Instance == null)
        {
            // find instance
            h_Instance = GameObject.FindObjectOfType<DebugGUI>();
            if (h_Instance == null)
                Debug.LogWarning("DebugGUI is called but no instance was found!");
        }
        return h_Instance;
    }
}
