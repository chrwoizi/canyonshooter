


float4x4 mvp : WORLDVIEWPROJECTION;
texture texture0 : TEXTURE0;
texture texture1 : TEXTURE1;
float power : POWER;

sampler2D texsampler1 = sampler_state
{
	Texture = <texture0>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};
sampler2D texsampler2 = sampler_state
{
	Texture = <texture1>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VIN {
	float4 position : POSITION;
	float2 texcoord : TEXCOORD0;
};


/**********************************************
	white
**********************************************/
struct VOUT0 {
	float4 position : POSITION;
};
VOUT0 Vertex0(VIN IN) {
	VOUT0 OUT;
	OUT.position = mul(IN.position, mvp);
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
	OUT.position = mul(IN.position, mvp);
	OUT.texcoord = IN.texcoord;
	return OUT;
}
float4 Fragment1(FIN1 IN) : COLOR {
	float4 color1 = tex2D(texsampler1, IN.texcoord);
	float4 color2 = tex2D(texsampler2, IN.texcoord);
	return lerp(color1, color2, power);
}


/**********************************************
	wireframe
**********************************************/
struct VOUT2 {
	float4 position : POSITION;
};
VOUT2 Vertex2(VIN IN) {
	VOUT2 OUT;
	OUT.position = mul(IN.position, mvp);
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

technique t_texture_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex1();
		PixelShader = compile ps_2_0 Fragment1();
	}
}

technique t_texture_3_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex1();
		PixelShader = compile ps_2_0 Fragment1();
	}
}

technique t_texture_wireframeoverlay_2_0
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




