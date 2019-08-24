
#include "includes/light.fxh"

#include "includes/specialEffects.fxh"

/**********************************************
	globals
**********************************************/

float4x4 mvp : WORLDVIEWPROJECTION;
float4x4 m : WORLD;
float4x4 v : VIEW;

float3 viewerPosition : CAMERA_POSITION;

texture texture0 : TEXTURE0;

float3 materialEmissive : MATERIAL_EMISSIVE;
float4 materialDiffuse : MATERIAL_DIFFUSE;
float4 materialSpecularAndShininess : MATERIAL_SPECULAR_AND_SHININESS;


sampler2D texsampler = sampler_state
{
	Texture = <texture0>;
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
	color, sunlight
**********************************************/
struct VOUT_C_SL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
};
struct FIN_C_SL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
};
VOUT_C_SL Vertex_C_SL(VIN IN) {
	VOUT_C_SL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;	
	return OUT;
}
float4 Fragment_C_SL(FIN_C_SL IN) : COLOR {
	float3 color = computeSunlight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, materialDiffuse.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	return float4(color, materialDiffuse.a);
}


/**********************************************
	color, vertex pointlight, fragment sunlight
**********************************************/
struct VOUT_C_VL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float3 lightd		: TEXCOORD3;
	float3 lights		: TEXCOORD4;
};
struct FIN_C_VL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float3 lightd		: TEXCOORD3;
	float3 lights		: TEXCOORD4;
};
VOUT_C_VL Vertex_C_VL(VIN IN) {
	VOUT_C_VL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;	
	
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
float4 Fragment_C_VL(FIN_C_VL IN) : COLOR {
	float3 color = min(materialEmissive + (ambientLight + IN.lightd)*materialDiffuse.rgb + IN.lights*materialSpecularAndShininess.rgb, float3(1,1,1));
	return float4(color, materialDiffuse.a);
}


/**********************************************
	color, all lights
**********************************************/
struct VOUT_C_AL {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
};
struct FIN_C_AL {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
};
VOUT_C_AL Vertex_C_AL(VIN IN) {
	VOUT_C_AL OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;	
	return OUT;
}
float4 Fragment_C_AL(FIN_C_AL IN) : COLOR {
	float3 color = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, materialDiffuse.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	return specialEffects(float4(color, materialDiffuse.a), IN.wposition, viewerPosition, IN.wview);
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
	float4 texcolor = tex2D(texsampler, IN.texcoord);
	float3 color = computeSunlight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	return float4(color, texcolor.a);
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
	float4 texcolor = tex2D(texsampler, IN.texcoord);
	float3 color = min(materialEmissive + (ambientLight + IN.lightd)*texcolor.rgb + IN.lights*materialSpecularAndShininess.rgb, float3(1,1,1));
	return float4(color, texcolor.a);
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
	float4 texcolor = tex2D(texsampler, IN.texcoord);
	float3 color = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	return specialEffects(float4(color, texcolor.a), IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	wireframe
**********************************************/
struct VOUT_W {
	float4 position : POSITION;
};
VOUT_W Vertex_W(VIN IN) {
	VOUT_W OUT;
	OUT.position = mul(IN.position, mvp);
	return OUT;
}
float4 Fragment_W() : COLOR {
	return float4(materialDiffuse.rgb,1);
}



/**********************************************
	techniques
**********************************************/

technique t_color_light_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_C_AL();
		PixelShader = compile ps_3_0 Fragment_C_AL();
	}
}
technique t_color_light_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex_C_VL();
		PixelShader = compile ps_2_0 Fragment_C_VL();
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


technique t_texture_light_wireframeoverlay_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL();
		PixelShader = compile ps_3_0 Fragment_T_AL();
	}
	pass p1
	{
		FillMode = WIREFRAME;
		DepthBias = -0.000001f;
		ZWriteEnable = false;
		VertexShader = compile vs_2_0 Vertex_W();
		PixelShader = compile ps_2_0 Fragment_W();
	}
}
technique t_texture_light_wireframeoverlay_2_0
{
	pass p0
	{
		VertexShader = compile vs_2_0 Vertex_T_VL();
		PixelShader = compile ps_2_0 Fragment_T_VL();
	}
	pass p1
	{
		FillMode = WIREFRAME;
		DepthBias = -0.000001f;
		ZWriteEnable = false;
		VertexShader = compile vs_2_0 Vertex_W();
		PixelShader = compile ps_2_0 Fragment_W();
	}
}

