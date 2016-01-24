using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Player;

public class ScoreBoard_Player : MonoBehaviour {

    public PhotonPlayer AssignedPlayer;
    public int SlotIndex;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (AssignedPlayer == null)
        {
            // set blanks for everything
            this.transform.GetChild(0).GetComponent<Text>().text = "---";
            this.transform.GetChild(1).GetComponent<Text>().text = "-----------";
            this.transform.GetChild(2).GetComponent<Text>().text = "---";
            this.transform.GetChild(3).GetComponent<Text>().text = "---";
            this.transform.GetChild(4).GetComponent<Text>().text = "---";
        }
        else {
            // Fill in RICH text
            UpdateColorField();
            UpdateNameField();
            UpdateClientTypeField();
            updateScoreField();
            UpdatePingField();

            // make this panel slightly more bright if this is the local player's box
            if( AssignedPlayer.ID == PhotonNetwork.player.ID)
                GetComponent<Image>().color = new Color(1, 1, 1, .70f);
        }

	}

    private void UpdateColorField()
    {
        string hexColor = "";
        Color col = PlayerManager.GetInstance().GetPlayerSlotColor(SlotIndex);
        hexColor = ColorUtility.ColorToHex(col);
        this.transform.GetChild(0).GetComponent<Text>().text = "<color=" + hexColor + ">" + "---" + "</color>";

    }
    private void UpdateNameField()
    {
        this.transform.GetChild(1).GetComponent<Text>().text = AssignedPlayer.name;
    }
    private void UpdateClientTypeField() {
        string newText = "N/A";

        if (AssignedPlayer.isMasterClient)
            newText = "Host";
        else newText = "Client";

        this.transform.GetChild(2).GetComponent<Text>().text = newText;
    }
    private void updateScoreField() {

        this.transform.GetChild(3).GetComponent<Text>().text = AssignedPlayer.GetScore().ToString();

    }
    private void UpdatePingField() {

        // green/yellow/red
        // 35 > ??  > 65 
        int pingValueMS = PhotonNetwork.GetPing();
        string pingColor = "#ff0000ff"; /// red

        if (pingValueMS < 65)
            pingColor = "#ffff00ff";  // yellow

        if (pingValueMS < 35)
            pingColor = "#008000ff"; // green

        this.transform.GetChild(4).GetComponent<Text>().text = "<color=" + pingColor + ">" + pingValueMS + "</color>";
    }
    
}
