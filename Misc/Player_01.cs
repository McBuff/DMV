using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour
{
	//-----------
	// Public
	//-----------
	
	public PlayerState CurrentState;
	public float m_CurrentStateStartTime; // Lifetime 
	
	//-----------
	// Network
	//-----------
	
	private float m_Lifetime;
	private List<NetworkPackages> m_ReceivedPackages;
	
	//-----------
	// Methods
	//-----------
	public void Start(){}
	public void Update(){}
	
	public void Update_Sync(){
			
			switch ( CurrentState ){
			case Default:

			break;
			case Launched:
			
			break;
			case Attacking:
			
			break;			
		}
		
	}
	
	// Returns direction of movement as unit vecotr ( or less ), multiply with character speed tog et actual distance
	public Vector3 GetMovementInput_Monkey(){
		
		Vector3 direction = Vector3.zero;
		
		if ( A ) add left 
		if ( D ) add right
		if ( W ) add up 
		if ( S ) add down 
		
		// get unit vector and apply wall detection
		if( direction.magnitue > 0.01f )
		{
			direction.normalize();			
			direciton = Wallcheck(direciton , position );
			
			return direction;
		}
		else return Vector3.zero;
		
		
		
		
		
	}
	public Vector3 GetMovemetnInput_Gamepad(){
		
		Vector3 directon = Gamepad.Get Axis X and Y
		
				// get unit vector and apply wall detection
		if( direction.magnitue > 0.01f )
		{
			direction.normalize();			
			direciton = Wallcheck(direciton , position );
			
			return direction;
		}
		else return Vector3.zero;
		
	}
	
	// returns 0-1  floats that determine how far in a specific direction a characetr can run at full speed.
	// takes in account walls
	public Vector3 GetmaxMovement( Vector3 direction, Vector3 startposition );
	
	// calculates player movement
	public Vector3 PlayerMovement( Vector3 direction , Vector3 startposition ){}
	
	// Update code for master of this object
	public void Update_Owner(){
		
		switch ( CurrentState ){
			case Default:
			// do walk and stuff
			Vector3 input = GetplayerInput();
			Vector3 direction = GetPlayerDirection();
			float lookat = GetPlayerLookat();
			
			if( Input.getkeydown( LeftMouse )) RPCCall->SetPlayerState( Attacking, m_Lifetime, pos, rot );
			break;
			case Launched:
			//TODO
			// for now, just exit the state and go to the default state			
			if( time > 1s ) RPCCall->SetPlayerState( Default, m_Lifetime, pos, rot );
			
			break;
			case Attacking:
			
			// while attacking, I cannot move
			float attackTime = Weapon.SwingTime;
			if( attackTime + m_CurrentStateStartTime > m_Lifetime )
				RPCCall->SetPlayerState( Default, m_Lifetime, pos, rot );
			else {
				
				// make sure I stand still till the end of the action				
				
			}
			
			break;			
		}
	}
	
	[RPC] void SetPlayerState( PlayerState state, float lifeTime, Vector3 Position , float Rotation ){
		
	}
	
}

public enum PlayerState{
	Default,
	Launched,
	Attacking
}