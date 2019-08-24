
#define MAX_SHADER_MATRICES 50


float4x4 vp : VIEWPROJECTION;

float4 materialDiffuse : MATERIAL_DIFFUSE;

float4x4 instanceData[MAX_SHADER_MATRICES] : INSTANCE_DATA;




struct VIN {
	float4 position : POSITION;
	float instanceId : TEXCOORD0;
};




/**********************************************
	color
**********************************************/
struct VOUT0 {
	float4 position : POSITION;
};
VOUT0 Vertex0(VIN IN) {
	VOUT0 OUT;
	
	float4x4 data = instanceData[IN.instanceId];
	
	if(data._41 == 1.0)
	{
		float3x4 m = float3x4(data._11_12_13_14,data._21_22_23_24,data._31_32_33_34);
		float3 wpos = mul(m, IN.position);
		OUT.position = mul(float4(wpos,1), vp);
	}
	else
	{
		OUT.position = float4(0,0,0,1);
	}
	
	return OUT;
}
float4 Fragment0() : COLOR {
	return materialDiffuse;
}


technique t_basic_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex0();
		PixelShader = compile ps_2_0 Fragment0();
	}
}

technique t_constantsinstancing_color_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex0();
		PixelShader = compile ps_2_0 Fragment0();
	}
}
