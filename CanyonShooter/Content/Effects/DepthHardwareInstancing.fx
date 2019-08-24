


float4x4 vp : VIEWPROJECTION;


struct VIN {
	float4 position : POSITION;
	float4 worldMatrixRow1 : TEXCOORD0;
	float4 worldMatrixRow2 : TEXCOORD1;
	float4 worldMatrixRow3 : TEXCOORD2;
};



/**********************************************
	depth
**********************************************/
struct VOUTD {
	float4 position : POSITION;
	float z : TEXCOORD0;
};
struct FIND {
	float z : TEXCOORD0;
};
VOUTD VertexD(VIN IN) {
	VOUTD OUT;
	
	float3 wpos = mul(float3x4(IN.worldMatrixRow1,IN.worldMatrixRow2,IN.worldMatrixRow3), IN.position);
	OUT.position = mul(float4(wpos,1), vp);
	
	OUT.z = OUT.position.z / OUT.position.w;
	return OUT;
}
float4 FragmentD(FIND IN) : COLOR {
	return IN.z;
}


technique t_depth_hardwareinstancing_2_0
{
	pass p0
	{
		CullMode = CCW;
		VertexShader = compile vs_1_1 VertexD();
		PixelShader = compile ps_1_1 FragmentD();
	}
	pass p1
	{
		CullMode = CW;
		VertexShader = compile vs_1_1 VertexD();
		PixelShader = compile ps_1_1 FragmentD();
	}
}

