


float4x4 mvp : WORLDVIEWPROJECTION;

float4 materialDiffuse : MATERIAL_DIFFUSE;




struct VIN {
	float4 position : POSITION;
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
	OUT.position = mul(IN.position, mvp);
	OUT.z = OUT.position.z / OUT.position.w;
	return OUT;
}
float4 FragmentD(FIND IN) : COLOR {
	return IN.z;
}


/**********************************************
	techniques
**********************************************/
technique t_depth_2_0
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
