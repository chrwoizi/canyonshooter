
float4x4 v : VIEW;
float4x4 p : PROJECTION;

texture text0 : TEXTURE0;

samplerCUBE texsampler0 = sampler_state
{
	Texture = <text0>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VIN {
	float4 position : POSITION;
};

struct VOUT {
	float4 position : POSITION;
	float3 vertexpos : TEXCOORD0;
};

struct FIN {
	float3 fragmentpos : TEXCOORD0;
};

VOUT Vertex0(VIN IN) {
	VOUT OUT;
	
	// move vertex to viewer's location -> vertex position becomes view direction in fragment shader
	OUT.vertexpos = IN.position.xyz;
	
	// rotate vertices by viewer's rotation and project to screen
	OUT.position = mul(
		float4(
			mul(IN.position, (float3x3)v), 
			IN.position.w
		),
		p
	);
  
	return OUT;
}

float4 Fragment0(FIN IN) : COLOR {
	float4 tex = texCUBE(texsampler0, IN.fragmentpos);

	return float4(tex.rgb,1);
}

technique t_basic_2_0
{
	pass p0
	{
		ZWriteEnable = false;
		ClipPlaneEnable = 0; // do not clip the skybox
		VertexShader = compile vs_2_0 Vertex0();
		PixelShader = compile ps_2_0 Fragment0();
	}
}