using UnityEngine;
using System.Collections;

public class ColorUtility : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Converts a UnityEngine Color to a Hex String in format #RRGGBBAA
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex( UnityEngine.Color color)
    {
        string hexColor = "";
        string R = ((int)(color.r * 255f)).ToString("X");
        string G = ((int)(color.g * 255f)).ToString("X");        
        string B = ((int)(color.b * 255f)).ToString("X");
        string A = ((int)(color.a * 255f)).ToString("X");



        if (R.Length == 1)
            R = "0" + R;
        if (G.Length == 1)
            G = "0" + G;
        if (B.Length == 1)
            B = "0" + B;
        if (A.Length == 1)
            A = "0" + A;

        hexColor = "#" + R + G + B + A;
        return hexColor;

    }


    /// <summary>
    /// Returns string with format : <color=#hex>
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    [System.Obsolete(" This method is inpractical, use ColorRichtText instead!")]
    public static string ColorToRichTextTag(UnityEngine.Color color)
    {
        string colorvalue = ColorToHex(color);
        string returnvalue = "<color=" + colorvalue + ">";

        return returnvalue;
    }


    /// <summary>
    /// Encloses given text in Richtext Color tag of chosen Unity Color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ColorRichtText( UnityEngine.Color color, string text)
    {
        string colorvalue = ColorToHex(color);
        return "<color=" + colorvalue + ">" + text + "</color>";
    }
}
