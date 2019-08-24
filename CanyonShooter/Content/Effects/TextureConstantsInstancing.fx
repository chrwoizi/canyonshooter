
#define MAX_SHADER_MATRICES 50


float4x4 vp : VIEWPROJECTION;
texture texture0 : TEXTURE0;

float4x4 instanceData[MAX_SHADER_MATRICES] : INSTANCE_DATA;

sampler2D texsampler = sampler_state
{
	Texture = <texture0>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VIN {
	float4 position : POSITION;
	float instanceId : TEXCOORD0;
	float2 texcoord : TEXCOORD1;
};



/**********************************************
	white
**********************************************/
struct VOUT0 {
	float4 position : POSITION;
};
VOUT0 Vertex0(VIN IN) {
	VOUT0 OUT;
	
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
	
	return OUT;
}
float4 Fragment0() : COLOR {
	return float4(1,0,0,1);
}


/**********************************************
	textured
**********************************************/
struct VOUT1 {
	float4 position : POSITION;
	float2 texcoord : TEXCOORD0;
};
struct FIN1 {
	float2 texcoord : TEXCOORD0;
};
VOUT1 Vertex1(VIN IN) {
	VOUT1 OUT;
	
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
	
	OUT.texcoord = IN.texcoord;
	return OUT;
}
float4 Fragment1(FIN1 IN) : COLOR {
	float4 color = tex2D(texsampler, IN.texcoord);
	return color;
}


/**********************************************
	wireframe
**********************************************/
struct VOUT2 {
	float4 position : POSITION;
};
VOUT2 Vertex2(VIN IN) {
	VOUT2 OUT;
	
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
	
	return OUT;
}
float4 Fragment2() : COLOR {
	return float4(1,0,0,1);
}




technique t_basic_2_0
{
	pass p0
	{
		VertexShader = compile vs_1_1 Vertex0();
		PixelShader = compile ps_1_1 Fragment0();
	}
}

technique t_constantsinstancing_texture_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex1();
		PixelShader = compile ps_2_0 Fragment1();
	}
}

technique t_constantsinstancing_texture_wireframeoverlay_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex1();
		PixelShader = compile ps_2_0 Fragment1();
	}
	pass p1
	{
		FillMode = WIREFRAME;
		DepthBias = -0.000001f;
		ZWriteEnable = false;
		VertexShader = compile vs_2_0 Vertex2();
		PixelShader = compile ps_2_0 Fragment2();
	}
}




