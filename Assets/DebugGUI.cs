using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// A GUI entirely used for debugging purposes
/// </summary>
public class DebugGUI : MonoBehaviour {

    public KeyCode ToggleButton;
    private Vector3 m_OffScreenSet;
    private RectTransform m_Rect;
    private bool m_GUIActive = false;


    public RectTransform DeathBallInfoPanel;


    private bool ShowDeathBallInfo;

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

    public void ToggleDeathBallInfo()
    {
        ShowDeathBallInfo = !ShowDeathBallInfo;
        DeathBallInfoPanel.transform.gameObject.SetActive(ShowDeathBallInfo);

    }
}
