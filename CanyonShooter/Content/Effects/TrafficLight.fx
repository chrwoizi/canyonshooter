
#include "includes/light.fxh"


float4x4 mvp : WORLDVIEWPROJECTION;
float4x4 m : WORLD;

float3 viewerPosition : CAMERA_POSITION;

float3 materialEmissive : MATERIAL_EMISSIVE;
float4 materialDiffuse : MATERIAL_DIFFUSE;
float4 materialSpecularAndShininess : MATERIAL_SPECULAR_AND_SHININESS;


texture texture0 : TEXTURE0;
texture texture1 : TEXTURE1;
texture texture2 : TEXTURE2;
texture texture3 : TEXTURE3;
texture texture4 : TEXTURE4;
int texId : TEX_ID;

sampler2D texsampler0 = sampler_state
{
	Texture = <texture0>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};
sampler2D texsampler1 = sampler_state
{
	Texture = <texture1>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};
sampler2D texsampler2 = sampler_state
{
	Texture = <texture2>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};
sampler2D texsampler3 = sampler_state
{
	Texture = <texture3>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};
sampler2D texsampler4 = sampler_state
{
	Texture = <texture4>;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VIN {
	float4 position : POSITION;
	float3 normal : NORMAL;
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
	float4 color = tex2D(texsampler0, IN.texcoord);
	float4 replace;
	if(texId == 0) replace = float4(0,0,0,1);
	else if(texId == 1) replace = tex2D(texsampler1, IN.texcoord);
	else if(texId == 2) replace = tex2D(texsampler2, IN.texcoord);
	else if(texId == 3) replace = tex2D(texsampler3, IN.texcoord);
	else replace = tex2D(texsampler4, IN.texcoord);
	
	if(any(replace.rgb))
	{
		color = replace;
	}
	
	return color;
}


/**********************************************
	textured, sunlight
**********************************************/
struct VOUT_T_SL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
};
struct FIN_T_SL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
};
VOUT_T_SL Vertex_T_SL(VIN IN) {
	VOUT_T_SL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	OUT.texcoord	= IN.texcoord;
	
	return OUT;
}
float4 Fragment_T_SL(FIN_T_SL IN) : COLOR {
	float4 texcolor = tex2D(texsampler0, IN.texcoord);
	float4 replace;
	if(texId == 0) replace = float4(0,0,0,1);
	else if(texId == 1) replace = tex2D(texsampler1, IN.texcoord);
	else if(texId == 2) replace = tex2D(texsampler2, IN.texcoord);
	else if(texId == 3) replace = tex2D(texsampler3, IN.texcoord);
	else replace = tex2D(texsampler4, IN.texcoord);
	
	float4 color;
	
	if(any(replace.rgb))
	{
		color = replace;
	}
	else
	{
		color.rgb = computeSunlight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
		color.a = texcolor.a;
	}
	
	return color;
}


/**********************************************
	textured, vertex pointlight, fragment sunlight
**********************************************/
struct VOUT_T_VL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float3 lightd		: TEXCOORD3;
	float3 lights		: TEXCOORD4;
	float2 texcoord		: TEXCOORD5;
};
struct FIN_T_VL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float3 lightd		: TEXCOORD3;
	float3 lights		: TEXCOORD4;
	float2 texcoord		: TEXCOORD5;
};
VOUT_T_VL Vertex_T_VL(VIN IN) {
	VOUT_T_VL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;	
	OUT.texcoord	= IN.texcoord;
	
	float3 sunlightDiffuse;
	float3 sunlightSpecular;
	computeOnlySunlight(
		OUT.wposition, 
		normalize(OUT.wnormal), 
		normalize(OUT.wview), 
		materialEmissive, 
		float3(1,1,1), 
		float3(1,1,1), 
		materialSpecularAndShininess.a,
		sunlightDiffuse,
		sunlightSpecular
	); 
	
	float3 pointlightDiffuse;
	float3 pointlightSpecular;
	computeOnlyPointlights(
		OUT.wposition, 
		normalize(OUT.wnormal), 
		normalize(OUT.wview), 
		materialEmissive, 
		float3(1,1,1), 
		float3(1,1,1), 
		materialSpecularAndShininess.a,
		pointlightDiffuse,
		pointlightSpecular
	);
	OUT.lightd = min(sunlightDiffuse + pointlightDiffuse, float3(1,1,1));
	OUT.lights = min(sunlightSpecular + pointlightSpecular, float3(1,1,1));
	
	return OUT;
}
float4 Fragment_T_VL(FIN_T_VL IN) : COLOR {
	float4 texcolor = tex2D(texsampler0, IN.texcoord);
	float4 replace;
	if(texId == 0) replace = float4(0,0,0,1);
	else if(texId == 1) replace = tex2D(texsampler1, IN.texcoord);
	else if(texId == 2) replace = tex2D(texsampler2, IN.texcoord);
	else if(texId == 3) replace = tex2D(texsampler3, IN.texcoord);
	else replace = tex2D(texsampler4, IN.texcoord);
	
	float4 color;
	
	if(any(replace.rgb))
	{
		color = replace;
	}
	else
	{
		color.rgb = min(materialEmissive + (ambientLight + IN.lightd)*texcolor.rgb + IN.lights*materialSpecularAndShininess.rgb, float3(1,1,1));
		color.a = texcolor.a;
	}
	
	return color;
}


/**********************************************
	textured, all lights
**********************************************/
struct VOUT_T_AL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
};
struct FIN_T_AL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
};
VOUT_T_AL Vertex_T_AL(VIN IN) {
	VOUT_T_AL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	OUT.texcoord	= IN.texcoord;
	
	return OUT;
}
float4 Fragment_T_AL(FIN_T_AL IN) : COLOR {
	float4 texcolor = tex2D(texsampler0, IN.texcoord);
	float4 replace;
	if(texId == 0) replace = float4(0,0,0,1);
	else if(texId == 1) replace = tex2D(texsampler1, IN.texcoord);
	else if(texId == 2) replace = tex2D(texsampler2, IN.texcoord);
	else if(texId == 3) replace = tex2D(texsampler3, IN.texcoord);
	else replace = tex2D(texsampler4, IN.texcoord);
	
	float4 color;
	
	if(any(replace.rgb))
	{
		color = replace;
	}
	else
	{
		color.rgb = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
		color.a = texcolor.a;
	}
	
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


technique t_texture_light_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL();
		PixelShader = compile ps_3_0 Fragment_T_AL();
	}
}
technique t_texture_light_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex_T_VL();
		PixelShader = compile ps_2_0 Fragment_T_VL();
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




