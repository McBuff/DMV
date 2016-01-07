using UnityEngine;
using System.Collections;
using System.Reflection;


/// <summary>
/// Automatically provides a version number to a project and displays
/// it for 20 seconds at the start of the game.
/// </summary>
/// <remarks>
/// Change the first two number to update the major and minor version number.
/// The following number are the build number (which is increased automatically
///  once a day, and the revision number which is increased every second). 
///  Thank you: https://xeophin.net/en/blog/2014/05/09/simple-version-numbering-unity3d
/// </remarks>
/// 
[assembly: System.Reflection.AssemblyVersion("1.0.*")]
public class VersionNumber : MonoBehaviour {

    public bool ShowVersionInfo = false;

    /// <summary>
    /// Gets the version.
    /// </summary>
    /// <value>The version.</value>
    public string Version
    {
        get
        {
            if (version == null)
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            return version;
        }
    }

    private Rect position = new Rect(0, 0, 200, 20);

    string version;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this); // not really a point tot his yet, but better safe than sorry
	
	}
	
	// Update is called once per frame
	void Update () {

        position.x = Screen.width - position.width - 10f;
        position.y = Screen.height - position.height - 10f;
    }

    void OnGUI()
    {
        // show info in bottom right 
        GUI.contentColor = new Color(.7f, .7f, .7f, .7f);
        GUI.Label(position, "DMV Alpha: " + Version);
    }
}
