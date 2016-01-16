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
    private Vector3 m_OffScreenSet;
    private RectTransform m_Rect;
    private bool m_GUIActive = false;


    public RectTransform DeathBallInfoPanel;
    public RectTransform GameplayStateInfoPanel;

    private bool ShowDeathBallInfo;
    private bool ShowGameplayStateInfo;

    // Use this for initialization
    void Start () {

        ShowDeathBallInfo = false;

        m_Rect = GetComponent<RectTransform>();
        m_OffScreenSet = m_Rect.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(ToggleButton))
            ToggleGUI();

        if (m_GUIActive)
            m_Rect.position = new Vector3(m_Rect.rect.width *.5f, m_OffScreenSet.y, 0);
        else 
            m_Rect.position = Vector3.zero + m_OffScreenSet;

    }

    public void ToggleGUI()
    {
        //toggle
        m_GUIActive = !m_GUIActive;

        // change position
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
