varying vec2 myTexCoord;
void main()
{
	gl_Position = ftransform();
	myTexCoord=vec2(gl_MultiTexCoord0);
}
