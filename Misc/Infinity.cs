#Infinity

public class iPixel(){

	public RGBA value;
		
	public iPixel(RGBA rgba){}
	public iPixel(double r, double, g, double b, double a = 1d; ); // double from 0 to 1 ? intensityL

public void Contract( RGB px ){
	
	
}

public void Enhance(){}

};


/*
a 2x2 iPixel

randomzes neighbour pixels with given KEY 

? what is key made out off? RGBA region? Zoom? Pattern?
	? RGBA Region is an AREA on the map
	? Zoom is the levels of ENHANCE applied to the camera
	? Pattern is a specific continent on the map, this determines randomisation
	
	Keep track of WHICH PIXELS are made, PIXELS CREATED can generate ANY pixel pattern between 0,0,0 and 1,1,1 , and ANY combination of these 3 neighbouring pixels are possible.
	
	HASHCODE: Should contain an instruction set for refinement of image , BIGGEST TARGET
	
	Algorithm:
		*/

public class HashCode hashcode{
	
	string codeString = "";
	
	
	public HashCode GenerateHashCodeForEnhance( RGBA rgba, Long zoom, HashCode parent ){
		
		
	}
	public HashCode GenerateHashCodeForCurrent( HashCode current ){
		
	}
	public AlgorithmZoom( HashCode code){
		
		iPixel R, D, B; // right, diagonal, bottom
		Long 
		
	}
}		
		/*
	
	
	
? How does ENHANCE/CONTRACT work?
	? CONTRACT
		Build Key: , or be given key.
			BUILD: // get key from iPixel RGBA ( doubles ) + Zoom ( Long ) + Pattern ( HashCode ?)
			Example:
				0.5d,0.0d,0.5d, 50.000, ScYN1BOkQ7wJhY2zf7mCP11JsRPdUzhh0vBqv4xqef67i6
				
				
			
		Read Key:
			
			Example:
				0.5d,0.0d,0.5d, 50.000, ScYN1BOkQ7wJhY2zf7mCP11JsRPdUzhh0vBqv4xqef67i6

*/