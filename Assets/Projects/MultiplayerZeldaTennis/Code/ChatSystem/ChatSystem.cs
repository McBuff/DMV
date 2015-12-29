using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The obligatory chatbox and messaging system. Comes with customisable hotkeys
/// </summary>
public class ChatSystem : MonoBehaviour {

    public bool isChatShowing = false;
    protected List<string> m_Chatlog;

	// Use this for initialization
	void Start () {
        m_Chatlog = new List<string>();
    }

    // Update is called once per frame
    void Update() {

        KeyCode openChatKey = KeyCode.T;
        //todo: opening a chat windows should surpress all player abilities, sot hat they don't accidentally endanger them.



        if (Input.GetKeyDown(openChatKey) && !isChatShowing) {
            
            ToggleChat();
        }
    }

    public void ToggleChat() {

        isChatShowing = !isChatShowing;
        if (isChatShowing)
        {
            
            // show chat windows and commands
            CanvasRenderer inputFieldCanvas = GetComponentInChildren<CanvasRenderer>();
            inputFieldCanvas.SetAlpha(0);

            
        }
        else
        {
            // show chat windows and commands
            CanvasRenderer inputFieldCanvas = GetComponentInChildren<CanvasRenderer>();
            inputFieldCanvas.SetAlpha(1f);


        }
    }

    void OnGUI()
    {
        
    }

}

