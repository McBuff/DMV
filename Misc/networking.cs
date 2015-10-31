// network pred algorithms and such


public float TrueGameTime;
public float ArticifialDelay;
public float ActualNetDelay;

public List<deathBallNetData> m_NetPackagesList;

void Start(){
	
	float netDelay = servertime - clienttime;
	
	TrueGameTime = servertime;
	ArticifialDelay <-- set
}

void Update(){
	
	TrueGameTime += Time.deltaTime; // TRUE SERVER TIME
	float lastReceivedTime = <-- last packet time
	
	float predictDT = TrueGameTime - lastReceivedTime - ArticifialDelay;
	
	Vector3 predictedNextPos = CalculateDeathBallMove( lastReceivedPos , LastReceivedDir, predictDT );
	
}

void CalculateDeathBallMove( Vector3 startpos, Vector3 startdir , float dTime){
	
	// check for future collisions using raycasts
	
	rc_resultLeft <-- check raycast
	rc_resultRight <-- check raycast
	
	
	
}




public class predictedCollision(){
	
	
}