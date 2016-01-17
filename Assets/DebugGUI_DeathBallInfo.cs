using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugGUI_DeathBallInfo : MonoBehaviour {

    public RectTransform PositionField;
    public RectTransform DirectionField;
    public RectTransform SpeedField;

    BouncingProjectile m_Deathball;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(m_Deathball == null)
        {            // get deathball from game ( can be NULL )
            m_Deathball = GameObject.FindObjectOfType<BouncingProjectile>();
        }

        UpdatePositionField();
        UpdateDirectionField();
        UpdateSpeedField();
    }

    private void UpdatePositionField()
    {
        if (m_Deathball != null)
            PositionField.GetComponent<Text>().text = m_Deathball.transform.position.ToString();
        else PositionField.GetComponent<Text>().text = "null";


    }

    private void UpdateDirectionField()
    {
        if (m_Deathball != null)
            DirectionField.GetComponent<Text>().text = m_Deathball.Direction.ToString();
        else DirectionField.GetComponent<Text>().text = "null";


    }


    private void UpdateSpeedField()
    {
        if (m_Deathball != null)
            SpeedField.GetComponent<Text>().text = m_Deathball.MovementSpeed.ToString() + "/" + BouncingProjectile.MaxMovementSpeed.ToString();
        else SpeedField.GetComponent<Text>().text = "null";


    }



}
