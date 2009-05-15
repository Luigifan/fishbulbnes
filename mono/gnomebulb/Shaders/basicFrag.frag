varying vec2 myTexCoord;
uniform sampler2D myTexture;
void main()
{
	gl_FragColor= texture2D(myTexture,myTexCoord);
}
