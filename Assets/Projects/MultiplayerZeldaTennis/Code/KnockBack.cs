using UnityEngine;
using System.Collections;

public class KnockBack : MonoBehaviour {

    public Vector3 Direction;

    public float Distance = 1.3f;

    private float StartTime = 0;
    private float EndTime = -1;
    public float Duration = .35f;
    private Vector3 PreviousMove;

	// Use this for initialization
	void Start () {
	
	}

    public bool isFinished() {
        return ((StartTime +Duration) > EndTime);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            StartKnockBack(Vector3.forward, Distance, Duration);

        ApplyKnockBack();
	}

    /// <summary>
    /// calculates and applies the force to given object ( or SELF if not specified )
    /// </summary>
    public void ApplyKnockBack() {

        float currentTime = Time.time- StartTime;

        float progress = currentTime / (EndTime - StartTime);

        progress = 1f - progress;

        float currentDistance = (  InterpolateCosClamped(progress)) * Distance;

        Vector3 currentMove = currentDistance * Direction;

        transform.position += currentMove - PreviousMove; 

        PreviousMove = currentMove;

    }

    public void StartKnockBack( Vector3 dir, float dist, float dur ) {
        StartTime = Time.time;
        Duration = dur;
        Distance = dist;
        EndTime = Duration + StartTime;
        Direction = dir;
    }


    /// <summary>
    /// returns a point on a cosine path
    /// </summary>
    /// <param name="lineairValue">scalar value between 0-1 , 1 = the point where cosine hits x</param>
    /// <returns></returns>
    public float InterpolateCos( float lineairValue)
    {
        // f(x) = cos ( currentTime / totaltime * 90g ) * F
        float outValue = Mathf.Cos( Mathf.Deg2Rad * 90f * lineairValue);
        return outValue;
    }
    /// <summary>
    /// returns a point on a cosine path
    /// </summary>
    /// <param name="lineairValue">scalar value between 0-1 , 1 = the point where cosine hits x</param>
    /// <returns></returns>
    public float InterpolateCosClamped(float lineairValue)
    {
        return InterpolateCos(Mathf.Clamp01(lineairValue));
    }
}
