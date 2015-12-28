using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EventLog : MonoBehaviour {


    // Interface for buffered logs
    public RectTransform EventLog_Item_Prefab;
    public RectTransform EventLog_ContentHolder;

    private List<RectTransform> m_Logs;
    private static EventLog h_Instance;

    // interface for spacing and letter sizes and sumsuch
    private float m_LineSpacing;

    
    private bool m_Initiated = false;

	// Use this for initialization
	void Start () {
        m_Logs = new List<RectTransform>();
        h_Instance = this;
        m_LineSpacing = 21f;
        m_Initiated = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Is this component fully initialised?
    /// </summary>
    /// <returns></returns>
    public bool Initialised()
    {
        return m_Initiated;
    }   

    // Get static instance of the EVENT LOG object
    public static EventLog GetInstance() {

        // Find instance if never looked up, store it in h_Instance for future refference
        if (h_Instance == null)
        {
            RectTransform[] uiElements = RectTransform.FindObjectsOfType<RectTransform>();

            for (int i = 0; i < uiElements.Length; i++)
            {
                EventLog foundLog = uiElements[i].GetComponentInChildren<EventLog>();
                if (foundLog != null)
                {
                    h_Instance = foundLog;
                    break;
                }

            }
        }

        return h_Instance;
    }

    /// <summary>
    /// Log a message in this window
    /// </summary>
    /// <param name="textString"></param>
    public void LogMessage(string textString) {

        // generate new message item and position it under the latest message
        RectTransform newLogMessage = RectTransform.Instantiate(EventLog_Item_Prefab);
        
        newLogMessage.SetParent( EventLog_ContentHolder );

        float newPosX, newPosY, newPosZ = 0f;

        newPosX = 20f;

        float bottomOfContentBox = EventLog_ContentHolder.position.y - EventLog_ContentHolder.rect.height;

        newPosY = bottomOfContentBox - ( m_LineSpacing * m_Logs.Count);

        newLogMessage.position = new Vector3( newPosX, newPosY, newPosZ );

        Text newTextbox =  newLogMessage.GetComponent<Text>();
        newTextbox.text = textString;
        newTextbox.enabled = true;

        m_Logs.Add(newLogMessage);

    }
}
