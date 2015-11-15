
public class GameplayManager : photonBehaviour{
	

	public GameState CurrentState;

void Start(){
	// set ready status	
	CurrentState = starting;
	
}

void Update(){
	
	if(server){
				
	}
	
}

void update_server(){

}

void update_client(){

}

[PhotonRPC]
void ChangeGamestate( GameState newstate, double netTime ){
	CurrentState  = newstate ;
	
	if( server )
	{
		if( pregame )
		{
			create players at start pos ( don't allow move )
			create deathball at center ( don't allow move )
			
		}
		if( starting )
		{

		}
		
	}
}

public enum GameState{
	pregame,
	starting,
	Playing,	
	ending
}