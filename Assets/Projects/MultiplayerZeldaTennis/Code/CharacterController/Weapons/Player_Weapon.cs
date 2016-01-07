using UnityEngine;
using System.Collections;

public class Player_Weapon : MonoBehaviour {

    /// <summary>
    /// Duration of this attack
    /// </summary>
    public float Duration;

    /// <summary>
    /// GameTime at which attack was issued
    /// </summary>
    protected float AttackStartTime = 0;

    /// <summary>
    /// Direction the player was facing when the attack was issued
    /// </summary>
    protected float m_PlayerAttackDirection;

    protected Collider m_MyCollider;


    // Use this for initialization
    void Start () {
        m_MyCollider = GetComponentInChildren<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    

    public virtual void Attack(float direction, float startTime = 0)
    {
        if (startTime == 0)
            AttackStartTime = Time.time;
        else AttackStartTime = startTime;

        m_PlayerAttackDirection = direction;
        
    }


    public bool isAttacking()
    {
        float endTime = Duration + AttackStartTime;
        if (Time.time <= endTime && AttackStartTime != 0)
            return true;
        return false;
    }

    /// <summary>
    /// Returns a float 0-1 on how far the attack has progressed
    /// </summary>
    /// <returns></returns>
    protected float GetAttackProgress()
    {
        float endTime = Duration + AttackStartTime;
        float attackProgress = (Time.time - AttackStartTime) / (endTime - AttackStartTime);
        return attackProgress;
    }
}
