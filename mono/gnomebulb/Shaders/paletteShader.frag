varying vec2 myTexCoord;
uniform sampler2D myTexture;
uniform sampler1D myPalette;
uniform int passNumber = 0;

void main()
{
	if (passNumber ==0)
	{
	
		// gets the index in the nes' palette
		float i = texture2D(myTexture,myTexCoord.xy).r;
		
		// gets the nes palette entry which is in the 255th scanline (first 32 entries)
		float nesColor = texture2D(myTexture, vec2(i,255*256)).r;
		
		// returns the rgb value
		gl_FragColor= texture1D(myPalette, nesColor);
	} 
	else 
	{
		gl_FragColor= texture2D(myTexture,myTexCoord);
		
	}
}
