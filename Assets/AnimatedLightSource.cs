using UnityEngine;
using System.Collections;

public class AnimatedLightSource : MonoBehaviour {

    public Color BrightColor;
    public Color MainColor;
    public Color DarkColor;

    public Light LightSource;
    public float FlickerSpeed;

    public float MaxIntensity;
    public float Minintensity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // update light intensity depending on data set
        Random.seed = Time.frameCount%50;
        float randomNumber = Random.Range(Minintensity, MaxIntensity);

        LightSource.color = Color.Lerp(DarkColor, BrightColor, randomNumber / (MaxIntensity-Minintensity));
        LightSource.intensity = randomNumber;
    }
}
