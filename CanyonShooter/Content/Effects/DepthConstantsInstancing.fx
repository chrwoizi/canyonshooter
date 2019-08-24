
#define MAX_SHADER_MATRICES 50


float4x4 vp : VIEWPROJECTION;

float4x4 instanceData[MAX_SHADER_MATRICES] : INSTANCE_DATA;




struct VIN {
	float4 position : POSITION;
	float instanceId : TEXCOORD0;
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
	
	if(instanceData[IN.instanceId]._41 == 1.0)
	{
		float4x4 data = instanceData[IN.instanceId];
		
		float3 wpos = mul(float3x4(data._11_12_13_14,data._21_22_23_24,data._31_32_33_34), IN.position);
		OUT.position = mul(float4(wpos,1), vp);
	}
	else
	{
		OUT.position = float4(0,0,0,1);
	}
	
	OUT.z = OUT.position.z / OUT.position.w;
	return OUT;
}
float4 FragmentD(FIND IN) : COLOR {
	return IN.z;
}


technique t_depth_constantsinstancing_2_0
{
	pass p0
	{
		CullMode = CCW;
		VertexShader = compile vs_2_0 VertexD();
		PixelShader = compile ps_1_1 FragmentD();
	}
	pass p1
	{
		CullMode = CW;
		VertexShader = compile vs_2_0 VertexD();
		PixelShader = compile ps_1_1 FragmentD();
	}
}
