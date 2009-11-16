texture2D chrRam;

SamplerState pointSampler
{
    Filter = MIN_MAG_MIP_POINT;

};

/*

% Description of my shader.
% Second line of description for my shader.

keywords: material classic

date: YYMMDD

*/

struct VS_IN
{
	float4 position : POSITION;
	float4 color : COLOR;
	float2 UV: TEXCOORD0;
};

struct PS_IN
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 UV: TEXCOORD0;
};

float4x4 WorldViewProj : WorldViewProjection;

PS_IN mainVS(VS_IN vs) 
{
	PS_IN ps = (PS_IN) vs;
	ps.position =  vs.position; // mul(float4(vs.position.xyz, 1.0), WorldViewProj);
	ps.color = vs.color;
	ps.UV = vs.UV;
	return ps;
}

float ramStart = 16384.0;

float4 mainPS(PS_IN ps) : SV_Target {
	
	float y = ramStart / 1024.0; // 4 bytes per pixel
	float yEnd = y  + 16;
	
	
	int i = (int)(ps.UV.x * 1024) & 3;
	
	// y should go from ramstart to ramstart + 0x4000
	float4 ram = chrRam.Load(int3( ps.UV.x * 255,  
	   (ps.UV.y * (yEnd - y))  , 0)) ;
	
	if (ram[i] )
	{
		float4 result = ram;
		switch (i)
		{
			case 0:
				result = float4(ram.xyz, 1.0);
				break;
			case 1:
				result = float4(ram.yxz, 1.0);
				break;
			case 2:
				result = float4(ram.zxy, 1.0);
				break;
			case 3:
				result = float4(ram.yzx, 1.0);
				break;
		}
		return (result + float4(0.25,0.35, 0.25, 1))/2;

	} else
	{
		return float4(0,0,0,0);
	}
}

technique10 Doodle {
	pass p0 {
		VertexShader = compile vs_4_0 mainVS();
		PixelShader = compile ps_4_0 mainPS();
	}
}
