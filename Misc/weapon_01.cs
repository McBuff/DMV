using UnityEngine;
using System.Collections;

public class Weapon : Photon.MonoBehaviour
{

	public float WeaponAttackTime = .3f; // in seconds	
	public float WeaponSwingArc = 120; // in degrees
	public float WeaponArcOuter = 1.3f;
	public float WeaponArcInner = .5f; // distance from player to weapon arc
	
	
	private float m_AttackStartTime; // ingame time when the attack was called
	private float m_PlayerAttackAngle = 0;
	
	private Player m_Owner;
	
	void Start(){		
		m_Owner = <- get player ( parent )
	}
	
	public void DoAttack( float playerAngle, float startTime = 0 ){
		
		m_AttackStartTime = Time.time;
		if( startTime != 0 ) m_AttackStartTime = startTime;		
		
	}
	
	public void Update(){
	
		float currentTime = Time.time;
		float attackEndTime = m_AttackStartTime + WeaponAttackTime;
		
		// while in attack period
		if( currentTime < attackEndTime ){
			
			//Check area for balls and players
		}
		
	}
	public bool CheckHit( Vector3 position ){
		
		
	}
	
	public void DoPlayerHit(Player player){
		
		// check if playuer is hit
		if( not self ){
			
			player.
		}
		
	}
	public void DoBallhit( DeathBall deathball ){
		
		// send ball flying in players look direction
		
		deathball.changedir
		deathball.increasespeed;
		
	}
	
	
	
	

}