
#define SHADOWS
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
texture texture1 : TEXTURE1;

float3 materialEmissive : MATERIAL_EMISSIVE;
float4 materialDiffuse : MATERIAL_DIFFUSE;
float4 materialSpecularAndShininess : MATERIAL_SPECULAR_AND_SHININESS;


sampler2D texsampler = sampler_state
{
	Texture = <texture0>;
	MinFilter = ANISOTROPIC;
	MipFilter = LINEAR;
	MagFilter = ANISOTROPIC;
};

sampler2D normalMapSampler = sampler_state
{
	Texture = <texture1>;
	AddressU = WRAP; 
	AddressV = WRAP;
	MinFilter = ANISOTROPIC;
	MipFilter = LINEAR;
	MagFilter = ANISOTROPIC;
};


struct VIN {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float3 binormal : BINORMAL0;
	float3 tangent : TANGENT0;
	float2 texcoord : TEXCOORD0;
};


/**********************************************
	color, all lights, all shadows
**********************************************/
struct VOUT_C_AL_AS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 pointlightShadowCoord0 : TEXCOORD5;
	SHADOW_FLOAT3 pointlightShadowCoord1 : TEXCOORD6;
	SHADOW_FLOAT3 pointlightShadowCoord2 : TEXCOORD7;
};
struct FIN_C_AL_AS {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 pointlightShadowCoord0 : TEXCOORD5;
	SHADOW_FLOAT3 pointlightShadowCoord1 : TEXCOORD6;
	SHADOW_FLOAT3 pointlightShadowCoord2 : TEXCOORD7;
};
VOUT_C_AL_AS Vertex_C_AL_AS(VIN IN) {
	VOUT_C_AL_AS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	computePointlightShadowCoords(OUT.wposition, OUT.pointlightShadowCoord0, OUT.pointlightShadowCoord1, OUT.pointlightShadowCoord2);
	
	return OUT;
}
float4 Fragment_C_AL_AS(FIN_C_AL_AS IN) : COLOR {
	float3 color = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, IN.pointlightShadowCoord0, IN.pointlightShadowCoord1, IN.pointlightShadowCoord2, materialEmissive, materialDiffuse.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	float4 result = float4(color, materialDiffuse.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	color, all lights, sunlight shadow
**********************************************/
struct VOUT_C_AL_SS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
};
struct FIN_C_AL_SS {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
};
VOUT_C_AL_SS Vertex_C_AL_SS(VIN IN) {
	VOUT_C_AL_SS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	
	return OUT;
}
float4 Fragment_C_AL_SS(FIN_C_AL_SS IN) : COLOR {
	float3 color = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, materialEmissive, materialDiffuse.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	float4 result = float4(color, materialDiffuse.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	color, sunlight, sunlight shadow
**********************************************/
struct VOUT_C_SL_SS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
};
struct FIN_C_SL_SS {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;
};
VOUT_C_SL_SS Vertex_C_SL_SS(VIN IN) {
	VOUT_C_SL_SS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	
	return OUT;
}
float4 Fragment_C_SL_SS(FIN_C_SL_SS IN) : COLOR {
	float3 color = computeSunlight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, materialEmissive, materialDiffuse.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	float4 result = float4(color, materialDiffuse.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	textured, all lights, all shadows
**********************************************/
struct VOUT_T_AL_AS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD5;
	SHADOW_FLOAT3 pointlightShadowCoord0 : TEXCOORD6;
	SHADOW_FLOAT3 pointlightShadowCoord1 : TEXCOORD7;
	SHADOW_FLOAT3 pointlightShadowCoord2 : TEXCOORD8;
};
struct FIN_T_AL_AS {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD5;
	SHADOW_FLOAT3 pointlightShadowCoord0 : TEXCOORD6;
	SHADOW_FLOAT3 pointlightShadowCoord1 : TEXCOORD7;
	SHADOW_FLOAT3 pointlightShadowCoord2 : TEXCOORD8;
};
VOUT_T_AL_AS Vertex_T_AL_AS(VIN IN) {
	VOUT_T_AL_AS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	OUT.texcoord	= IN.texcoord;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	computePointlightShadowCoords(OUT.wposition, OUT.pointlightShadowCoord0, OUT.pointlightShadowCoord1, OUT.pointlightShadowCoord2);
	
	return OUT;
}
float4 Fragment_T_AL_AS(FIN_T_AL_AS IN) : COLOR {
	float4 texcolor = tex2D(texsampler, IN.texcoord);
	float3 color = computeLight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, IN.pointlightShadowCoord0, IN.pointlightShadowCoord1, IN.pointlightShadowCoord2, materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	float4 result = float4(color, materialDiffuse.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	textured, all lights, sunlight shadow
**********************************************/
struct VOUT_T_AL_SS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wview		: TEXCOORD1;
	float2 texcoord		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;  
	float3x3 tangentToWorld    : TEXCOORD5;
};
struct FIN_T_AL_SS {
	float3 wposition	: TEXCOORD0;
	float3 wview		: TEXCOORD1;
	float2 texcoord		: TEXCOORD2;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD3;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD4;  
	float3x3 tangentToWorld    : TEXCOORD5;
};
VOUT_T_AL_SS Vertex_T_AL_SS(VIN IN) {
	VOUT_T_AL_SS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wview		= OUT.wposition - viewerPosition;
	OUT.texcoord	= IN.texcoord;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	
    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors.  the pixel shader will normalize these
    // in case the world matrix has scaling.
    OUT.tangentToWorld[0] = mul(IN.tangent, m);
    OUT.tangentToWorld[1] = mul(IN.normal, m);
    OUT.tangentToWorld[2] = mul(IN.binormal, m);
    	
	return OUT;
}
float4 Fragment_T_AL_SS(FIN_T_AL_SS IN) : COLOR {

    // look up the normal from the normal map, and transform from tangent space
    // into world space using the matrix created above.  normalize the result
    // in case the matrix contains scaling.
    float3 normalFromMap = tex2D(normalMapSampler, IN.texcoord*float2(5,5));
    normalFromMap = normalFromMap * 2 - float3(1,1,1);
    normalFromMap.rgb = normalFromMap.rbg;
    normalFromMap = mul(normalFromMap, IN.tangentToWorld);
    normalFromMap = normalize(normalFromMap);
    
    float4 texcolor = tex2D(texsampler, IN.texcoord*float2(5,5));
	
	float3 color = computeLight(IN.wposition, normalFromMap, normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	
	float4 result = float4(color, texcolor.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
}


/**********************************************
	textured, sunlight, sunlight shadow
**********************************************/
struct VOUT_T_SL_SS {
	float4 position		: POSITION;
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD5;
};
struct FIN_T_SL_SS {
	float3 wposition	: TEXCOORD0;
	float3 wnormal		: TEXCOORD1;
	float3 wview		: TEXCOORD2;
	float2 texcoord		: TEXCOORD3;
	SHADOW_FLOAT3 sunlightLowShadowCoord : TEXCOORD4;
	SHADOW_FLOAT3 sunlightHighShadowCoord : TEXCOORD5;
};
VOUT_T_SL_SS Vertex_T_SL_SS(VIN IN) {
	VOUT_T_SL_SS OUT;
	OUT.position	= mul(IN.position, mvp);
	OUT.wposition	= mul(IN.position, m);
	OUT.wnormal		= mul(IN.normal, (float3x3)m);
	OUT.wview		= OUT.wposition - viewerPosition;
	OUT.texcoord	= IN.texcoord;
	
	computeSunlightShadowCoords(OUT.wposition, OUT.sunlightLowShadowCoord, OUT.sunlightHighShadowCoord);
	
	return OUT;
}
float4 Fragment_T_SL_SS(FIN_T_SL_SS IN) : COLOR {
	float4 texcolor = tex2D(texsampler, IN.texcoord);
	float3 color = computeSunlight(IN.wposition, normalize(IN.wnormal), normalize(IN.wview), IN.sunlightLowShadowCoord, IN.sunlightHighShadowCoord, materialEmissive, texcolor.rgb, materialSpecularAndShininess.rgb, materialSpecularAndShininess.a);
	float4 result = float4(color, texcolor.a);
	return specialEffects(result, IN.wposition, viewerPosition, IN.wview);
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
	return float4(1,0,0,1);
}



/**********************************************
	techniques
**********************************************/

technique t_normalmapping_color_light_shadows_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_C_AL_AS();
		PixelShader = compile ps_3_0 Fragment_C_AL_AS();
	}
}
technique t_normalmapping_color_light_sunlightshadow_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_C_AL_SS();
		PixelShader = compile ps_3_0 Fragment_C_AL_SS();
	}
}
technique t_normalmapping_color_sunlight_sunlightshadow_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_C_SL_SS();
		PixelShader = compile ps_3_0 Fragment_C_SL_SS();
	}
}


technique t_normalmapping_texture_light_shadows_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL_AS();
		PixelShader = compile ps_3_0 Fragment_T_AL_AS();
	}
}
technique t_normalmapping_texture_light_sunlightshadow_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL_SS();
		PixelShader = compile ps_3_0 Fragment_T_AL_SS();
	}
}
technique t_normalmapping_texture_sunlight_sunlightshadow_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_SL_SS();
		PixelShader = compile ps_3_0 Fragment_T_SL_SS();
	}
}


technique t_normalmapping_texture_light_shadows_wireframeoverlay_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL_AS();
		PixelShader = compile ps_3_0 Fragment_T_AL_AS();
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
technique t_normalmapping_texture_light_sunlightshadow_wireframeoverlay_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_AL_SS();
		PixelShader = compile ps_3_0 Fragment_T_AL_SS();
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
technique t_normalmapping_texture_sunlight_sunlightshadow_wireframeoverlay_3_0
{
	pass p0
	{
		VertexShader = compile vs_3_0 Vertex_T_SL_SS();
		PixelShader = compile ps_3_0 Fragment_T_SL_SS();
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

