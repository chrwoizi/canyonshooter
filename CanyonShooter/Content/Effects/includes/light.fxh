/**********************************************
	Defines
**********************************************/
#ifdef SHADOWS
	#define MAX_SHADOWMAPS 3
#endif


/**********************************************
	Types
**********************************************/

#define LIGHT_FLOAT float
#define LIGHT_FLOAT2 float2
#define LIGHT_FLOAT3 float3
#define LIGHT_FLOAT4 float4

/*#define LIGHT_FLOAT half
#define LIGHT_FLOAT2 half2
#define LIGHT_FLOAT3 half3
#define LIGHT_FLOAT4 half4*/

#ifdef SHADOWS
	#define SHADOW_FLOAT float
	#define SHADOW_FLOAT2 float2
	#define SHADOW_FLOAT3 float3
	#define SHADOW_FLOAT4 float4

	/*#define SHADOW_FLOAT half
	#define SHADOW_FLOAT2 half2
	#define SHADOW_FLOAT3 half3
	#define SHADOW_FLOAT4 half4*/
#endif


/**********************************************
	Globals
**********************************************/

#define MAX_LIGHTS 5

float3 sunDirection							: SUNLIGHT_DIRECTION		= float3(0, -1, 0);		// in which direction does the sunlight go
float3 sunColor								: SUNLIGHT_COLOR			= float3(1, 1, 1);

int pointLightCount							: POINTLIGHT_COUNT			= 0;
float3 pointLightPositions[MAX_LIGHTS]		: POINTLIGHT_POSITIONS;
float3 pointLightColors[MAX_LIGHTS]			: POINTLIGHT_COLORS;
float2 pointLightAttenuations[MAX_LIGHTS]	: POINTLIGHT_ATTENUATIONS;

float3 ambientLight							: AMBIENT_LIGHT				= float3(0.1, 0.1, 0.1);

#ifdef SHADOWS
	float shadowmapDepthbias = 0.001f;

	float pointlightShadowmapCount : POINTLIGHT_SHADOWMAP_COUNT;

	float4x4 sunlightShadowmapLowMatrix : SUNLIGHT_SHADOWMAP_LOW_MATRIX;
	float4x4 sunlightShadowmapHighMatrix : SUNLIGHT_SHADOWMAP_HIGH_MATRIX;
	float2 sunlightShadowmapLowTexelsize : SUNLIGHT_SHADOWMAP_LOW_TEXELSIZE;
	float2 sunlightShadowmapHighTexelsize : SUNLIGHT_SHADOWMAP_HIGH_TEXELSIZE;

	float4x4 pointlightShadowmapMatrices[MAX_SHADOWMAPS] : POINTLIGHT_SHADOWMAP_MATRICES;
	float2 pointlightShadowmapTexelsizes[MAX_SHADOWMAPS] : POINTLIGHT_SHADOWMAP_TEXELSIZES;
	
	texture sunlightShadowmapLowTexture : SUNLIGHT_SHADOWMAP_LOW;
	texture sunlightShadowmapHighTexture : SUNLIGHT_SHADOWMAP_HIGH;

	texture pointlightShadowmapTexture0 : POINTLIGHT_SHADOWMAP0;
	texture pointlightShadowmapTexture1 : POINTLIGHT_SHADOWMAP1;
	texture pointlightShadowmapTexture2 : POINTLIGHT_SHADOWMAP2;

	sampler2D sunlightShadowmapLowSampler =
	sampler_state
	{
		Texture = <sunlightShadowmapLowTexture>;
	};
	sampler2D sunlightShadowmapHighSampler =
	sampler_state
	{
		Texture = <sunlightShadowmapHighTexture>;
	};

	sampler2D pointlightShadowmapSamplers[MAX_SHADOWMAPS] = {
		sampler_state
		{
			Texture = <pointlightShadowmapTexture0>;
		},
		sampler_state
		{
			Texture = <pointlightShadowmapTexture1>;
		},
		sampler_state
		{
			Texture = <pointlightShadowmapTexture2>;
		}
	};
#endif



/**********************************************
	Functions
**********************************************/

LIGHT_FLOAT pointLightAttenuation(LIGHT_FLOAT distance, int id)
{
	return max(0, 1 - distance*(LIGHT_FLOAT(pointLightAttenuations[id].y) + distance*LIGHT_FLOAT(pointLightAttenuations[id].x)));
}

LIGHT_FLOAT3 computeDiffuseSunlight(LIGHT_FLOAT3 normal)
{
	return max(0, LIGHT_FLOAT3(sunColor) * dot(-LIGHT_FLOAT3(sunDirection), normal));
}

LIGHT_FLOAT3 computeSpecularSunlight(LIGHT_FLOAT3 viewerDirection, LIGHT_FLOAT3 normal, LIGHT_FLOAT shininess)
{
	// Blinn-Phong: color * dot((L+V)/|L+H|, N)^shininess 
	return max(0, LIGHT_FLOAT3(sunColor) * pow(dot(normalize(viewerDirection-LIGHT_FLOAT3(sunDirection)), normal), shininess));
}

LIGHT_FLOAT3 computeDiffusePointLight(LIGHT_FLOAT3 lightDir, LIGHT_FLOAT distance, LIGHT_FLOAT3 normal, int id)
{	
	LIGHT_FLOAT3 result;
	if(dot(lightDir, normal) <= 0) 
	{
		result = LIGHT_FLOAT3(0,0,0);
	}
	else
	{
		result = pointLightAttenuation(distance, id) * max(0, LIGHT_FLOAT3(pointLightColors[id]) * dot(lightDir, normal));
	}
	
	return result;
}

LIGHT_FLOAT3 computeSpecularPointLight(LIGHT_FLOAT3 viewerDirection, LIGHT_FLOAT3 lightDir, LIGHT_FLOAT distance, LIGHT_FLOAT3 normal, LIGHT_FLOAT shininess, int id)
{
	LIGHT_FLOAT3 result;
	if(dot(lightDir, normal) <= 0) 
	{
		result = LIGHT_FLOAT3(0,0,0);
	}
	else
	{
		// Blinn-Phong: color * dot((L+V)/|L+H|, N)^shininess 
		result = pointLightAttenuation(distance, id) * max(0, LIGHT_FLOAT3(pointLightColors[id]) * pow(dot(normalize(viewerDirection+lightDir), normal), shininess));
	}
	
	return LIGHT_FLOAT3(0,0,0);
	return result;
}

#ifdef SHADOWS
	// default shadow mapping 
	// or 
	// 2x2 percentage closer filtering
	// or
	// 2x2 bilinear percentage closer filtering
	//
	//  OPTIMIZED  -  2 shadows: 170 fps
	//
	// uncomment the following defines to set the shadowmap technique
	#define PCF
	#define BILINEAR
	SHADOW_FLOAT unshadowedFromPointlight(SHADOW_FLOAT3 shadowCoord, int shadowmapId)
	{
		SHADOW_FLOAT result = 1;
		if(shadowmapId < pointlightShadowmapCount)
		{
			if(shadowCoord.x>=0 && shadowCoord.x<=1 && shadowCoord.y>=0 && shadowCoord.y<=1)
			{
				#ifdef PCF
					SHADOW_FLOAT4 texcoords = SHADOW_FLOAT4(
						shadowCoord.xy,
						shadowCoord.xy+20*pointlightShadowmapTexelsizes[shadowmapId]
					);
				    
					// grab the samples
					SHADOW_FLOAT4 depth = SHADOW_FLOAT4(
						tex2D(pointlightShadowmapSamplers[shadowmapId], texcoords.xy).x,
						tex2D(pointlightShadowmapSamplers[shadowmapId], texcoords.xw).x,
						tex2D(pointlightShadowmapSamplers[shadowmapId], texcoords.zy).x,
						tex2D(pointlightShadowmapSamplers[shadowmapId], texcoords.zw).x
					);
					
					// depth test
					SHADOW_FLOAT4 viz = (depth > shadowCoord.z-shadowmapDepthbias);

					#ifdef BILINEAR
						// bilinear filter test results
						SHADOW_FLOAT2 tcoord = frac(shadowCoord.xy / pointlightShadowmapTexelsizes[shadowmapId]);
						SHADOW_FLOAT2 v = lerp(viz.xy, viz.zw, tcoord.x);
						result = lerp(v.x, v.y, tcoord.y);
					#else
						result = dot(viz, SHADOW_FLOAT4(0.25,0.25,0.25,0.25));
					#endif
				#else
					// grab the sample
					SHADOW_FLOAT depth = tex2D(pointlightShadowmapSamplers[shadowmapId], shadowCoord.xy).x;
					
					// depth test
					result = depth > shadowCoord.z-shadowmapDepthbias;
				#endif
			}
		}
		return result;
	}
	SHADOW_FLOAT unshadowedFromSunlightLow(SHADOW_FLOAT3 shadowCoord)
	{
		SHADOW_FLOAT result = 1;
		if(shadowCoord.x>=0 && shadowCoord.x<=1 && shadowCoord.y>=0 && shadowCoord.y<=1)
		{
			#ifdef PCF
				SHADOW_FLOAT4 texcoords = SHADOW_FLOAT4(
					shadowCoord.xy,
					shadowCoord.xy+sunlightShadowmapLowTexelsize
				);
			    
				// grab the samples
				SHADOW_FLOAT4 depth = SHADOW_FLOAT4(
					tex2D(sunlightShadowmapLowSampler, texcoords.xy).x,
					tex2D(sunlightShadowmapLowSampler, texcoords.xw).x,
					tex2D(sunlightShadowmapLowSampler, texcoords.zy).x,
					tex2D(sunlightShadowmapLowSampler, texcoords.zw).x
				);
				
				// depth test
				SHADOW_FLOAT4 viz = (depth > shadowCoord.z-shadowmapDepthbias);

				#ifdef BILINEAR
					// bilinear filter test results
					SHADOW_FLOAT2 tcoord = frac(shadowCoord.xy / sunlightShadowmapLowTexelsize);
					SHADOW_FLOAT2 v = lerp(viz.xy, viz.zw, tcoord.x);
					result = lerp(v.x, v.y, tcoord.y);
				#else
					result = dot(viz, SHADOW_FLOAT4(0.25,0.25,0.25,0.25));
				#endif
			#else
				// grab the sample
				SHADOW_FLOAT depth = tex2D(sunlightShadowmapLowSampler, shadowCoord.xy).x;
				
				// depth test
				result = depth > shadowCoord.z-shadowmapDepthbias;
			#endif
		}
		return result;
	}
	SHADOW_FLOAT unshadowedFromSunlightHigh(SHADOW_FLOAT3 shadowCoord)
	{
		SHADOW_FLOAT result = 1;
		if(shadowCoord.x>=0 && shadowCoord.x<=1 && shadowCoord.y>=0 && shadowCoord.y<=1)
		{
			#ifdef PCF
				SHADOW_FLOAT4 texcoords = SHADOW_FLOAT4(
					shadowCoord.xy,
					shadowCoord.xy+sunlightShadowmapHighTexelsize
				);
			    
				// grab the samples
				SHADOW_FLOAT4 depth = SHADOW_FLOAT4(
					tex2D(sunlightShadowmapHighSampler, texcoords.xy).x,
					tex2D(sunlightShadowmapHighSampler, texcoords.xw).x,
					tex2D(sunlightShadowmapHighSampler, texcoords.zy).x,
					tex2D(sunlightShadowmapHighSampler, texcoords.zw).x
				);
				
				// depth test
				SHADOW_FLOAT4 viz = (depth > shadowCoord.z-shadowmapDepthbias);

				#ifdef BILINEAR
					// bilinear filter test results
					SHADOW_FLOAT2 tcoord = frac(shadowCoord.xy / sunlightShadowmapHighTexelsize);
					SHADOW_FLOAT2 v = lerp(viz.xy, viz.zw, tcoord.x);
					result = lerp(v.x, v.y, tcoord.y);
				#else
					result = dot(viz, SHADOW_FLOAT4(0.25,0.25,0.25,0.25));
				#endif
			#else
				// grab the sample
				SHADOW_FLOAT depth = tex2D(sunlightShadowmapHighSampler, shadowCoord.xy).x;
				
				// depth test
				result = depth > shadowCoord.z-shadowmapDepthbias;
			#endif
		}
		return result;
	}
#endif

#ifdef SHADOWS
	/**
	 *	all values in world coordinates
	 */
	void computeOnlyPointlights(
		float3 position, 
		float3 normal, 
		float3 viewDirection, 
		SHADOW_FLOAT3 shadowCoord0, 
		SHADOW_FLOAT3 shadowCoord1, 
		SHADOW_FLOAT3 shadowCoord2, 
		float3 materialEmissiveColor, 
		float3 materialDiffuseColor, 
		float3 materialSpecularColor, 
		float materialShininess,
		out float3 diffuse,
		out float3 specular
	)
	{	
		LIGHT_FLOAT3 _viewerDirection		= -LIGHT_FLOAT3(viewDirection);
		LIGHT_FLOAT3 _normal				= LIGHT_FLOAT3(normal);
		LIGHT_FLOAT _materialShininess		= LIGHT_FLOAT(materialShininess);
		
		bool computeDiffuse = any(materialDiffuseColor);
		bool computeSpecular = any(materialSpecularColor);
		
		LIGHT_FLOAT3 specularResult = LIGHT_FLOAT3(0,0,0);
		LIGHT_FLOAT3 diffuseResult = LIGHT_FLOAT3(0,0,0);
		int i = 0;
		
		while(i < pointLightCount)
		{
			float3 localLightPosition = pointLightPositions[i] - position;
			
			LIGHT_FLOAT3 _localLightPosition = LIGHT_FLOAT3(localLightPosition);
			LIGHT_FLOAT distance = length(_localLightPosition);
			LIGHT_FLOAT3 lightDir = _localLightPosition/distance;
			
			bool facingLight = dot(normal, localLightPosition) > 0;
			
			SHADOW_FLOAT3 shadowCoord = SHADOW_FLOAT3(0,0,0);
			if(i == 0) shadowCoord = shadowCoord0;
			else 
			{
				if(i == 1) shadowCoord = shadowCoord1;
				else shadowCoord = shadowCoord2;
			}
			
			SHADOW_FLOAT unshadowed = facingLight ? unshadowedFromPointlight(shadowCoord, 0) : 0;
			
			if(any(unshadowed))
			{
				if(computeDiffuse) diffuseResult += unshadowed * computeDiffusePointLight(lightDir, distance, _normal, i);
				if(computeSpecular) specularResult += unshadowed * computeSpecularPointLight(_viewerDirection, lightDir, distance, _normal, _materialShininess, i);
			}
		
			i++;
		}

		diffuse = float3(diffuseResult);
		specular = float3(specularResult);
	}
#endif

/**
 *	all values in world coordinates
 */
void computeOnlyPointlights(
	float3 position, 
	float3 normal, 
	float3 viewDirection, 
	float3 materialEmissiveColor, 
	float3 materialDiffuseColor, 
	float3 materialSpecularColor, 
	float materialShininess,
	out float3 diffuse,
	out float3 specular
)
{	
	LIGHT_FLOAT3 _viewerDirection		= -LIGHT_FLOAT3(viewDirection);
	LIGHT_FLOAT3 _normal				= LIGHT_FLOAT3(normal);
	LIGHT_FLOAT _materialShininess		= LIGHT_FLOAT(materialShininess);
	
	bool computeDiffuse = any(materialDiffuseColor);
	bool computeSpecular = any(materialSpecularColor);
	
	LIGHT_FLOAT3 specularResult = LIGHT_FLOAT3(0,0,0);
	LIGHT_FLOAT3 diffuseResult = LIGHT_FLOAT3(0,0,0);
	int i = 0;
	
	while(i < pointLightCount)
	{
		float3 localLightPosition = pointLightPositions[i] - position;
		
		LIGHT_FLOAT3 _localLightPosition = LIGHT_FLOAT3(localLightPosition);
		LIGHT_FLOAT distance = length(_localLightPosition);
		LIGHT_FLOAT3 lightDir = _localLightPosition/distance;
		
		if(computeDiffuse) diffuseResult += computeDiffusePointLight(lightDir, distance, _normal, i);
		if(computeSpecular) specularResult += computeSpecularPointLight(_viewerDirection, lightDir, distance, _normal, _materialShininess, i);
	
		i++;
	}

	diffuse = float3(diffuseResult);
	specular = float3(specularResult);
}

#ifdef SHADOWS
	/**
	 *	all values in world coordinates
	 */
	void computeOnlySunlight(
		float3 position, 
		float3 normal, 
		float3 viewDirection, 
		SHADOW_FLOAT3 shadowCoordLow,
		SHADOW_FLOAT3 shadowCoordHigh,
		float3 materialEmissiveColor, 
		float3 materialDiffuseColor, 
		float3 materialSpecularColor, 
		float materialShininess,
		out float3 diffuse,
		out float3 specular
	)
	{	
		LIGHT_FLOAT3 _viewerDirection		= -LIGHT_FLOAT3(viewDirection);
		LIGHT_FLOAT3 _normal				= LIGHT_FLOAT3(normal);
		LIGHT_FLOAT _materialShininess		= LIGHT_FLOAT(materialShininess);
		
		bool facingLight = dot(normal, -sunDirection) > 0;
		
		diffuse = float3(0,0,0);
		specular = float3(0,0,0);
		
		SHADOW_FLOAT unshadowed = 1;
		if(shadowCoordHigh.x>=0 && shadowCoordHigh.x<=1 && shadowCoordHigh.y>=0 && shadowCoordHigh.y<=1)
		{
			float smoothDistance = 0.2;
			if(shadowCoordHigh.x>=smoothDistance && shadowCoordHigh.x<=1-smoothDistance && shadowCoordHigh.y>=smoothDistance && shadowCoordHigh.y<=1-smoothDistance)
			{
				SHADOW_FLOAT unshadowedHigh = facingLight ? unshadowedFromSunlightHigh(shadowCoordHigh) : 0;
				unshadowed = unshadowedHigh;
			}
			else
			{
				SHADOW_FLOAT unshadowedLow = facingLight ? unshadowedFromSunlightLow(shadowCoordLow) : 0;
				SHADOW_FLOAT unshadowedHigh = facingLight ? unshadowedFromSunlightHigh(shadowCoordHigh) : 0;
				
				float x = min(shadowCoordHigh.x, 1-shadowCoordHigh.x);
				float y = min(shadowCoordHigh.y, 1-shadowCoordHigh.y);
				
				float l = min(x, y)/smoothDistance;
				unshadowed = lerp(unshadowedLow, unshadowedHigh, l);
			}
		}
		else if(shadowCoordLow.x>=0 && shadowCoordLow.x<=1 && shadowCoordLow.y>=0 && shadowCoordLow.y<=1)
		{
			SHADOW_FLOAT unshadowedLow = facingLight ? unshadowedFromSunlightLow(shadowCoordLow) : 0;
			unshadowed = unshadowedLow;
		}
		
		/*SHADOW_FLOAT unshadowedLow = facingLight ? unshadowedFromSunlightLow(shadowCoordLow) : 0;
		unshadowed = unshadowedLow;*/
		
		if(any(unshadowed))
		{
			diffuse = any(materialDiffuseColor) ? unshadowed * computeDiffuseSunlight(_normal) : float3(0,0,0);
			specular = any(materialSpecularColor) ? unshadowed * computeSpecularSunlight(_viewerDirection, _normal, _materialShininess) : float3(0,0,0);
		}
	}
#endif

/**
 *	all values in world coordinates
 */
void computeOnlySunlight(
	float3 position, 
	float3 normal, 
	float3 viewDirection, 
	float3 materialEmissiveColor, 
	float3 materialDiffuseColor, 
	float3 materialSpecularColor, 
	float materialShininess,
	out float3 diffuse,
	out float3 specular
)
{	
	LIGHT_FLOAT3 _viewerDirection		= -LIGHT_FLOAT3(viewDirection);
	LIGHT_FLOAT3 _normal				= LIGHT_FLOAT3(normal);
	LIGHT_FLOAT _materialShininess		= LIGHT_FLOAT(materialShininess);

	// TODO use "any" for diffuse too. removed initially because of shader model 2 restrictions
	//diffuse = any(materialDiffuseColor) ? computeDiffuseSunlight(_normal) : float3(0,0,0);
	diffuse = computeDiffuseSunlight(_normal);
	specular = any(materialSpecularColor) ? computeSpecularSunlight(_viewerDirection, _normal, _materialShininess) : float3(0,0,0);
}

#ifdef SHADOWS
	/**
	 *	all values in world coordinates
	 */
	float3 computeSunlight(
		float3 position, 
		float3 normal, 
		float3 viewDirection, 
		SHADOW_FLOAT3 sunlightLowShadowCoord,
		SHADOW_FLOAT3 sunlightHighShadowCoord,
		float3 materialEmissiveColor, 
		float3 materialDiffuseColor, 
		float3 materialSpecularColor, 
		float materialShininess
	)
	{	
		float3 sunlightDiffuse;
		float3 sunlightSpecular;
		computeOnlySunlight(
			position, 
			normal, 
			viewDirection, 
			sunlightLowShadowCoord,
			sunlightHighShadowCoord,
			materialEmissiveColor, 
			materialDiffuseColor, 
			materialSpecularColor, 
			materialShininess,
			sunlightDiffuse,
			sunlightSpecular
		); 
		
		return min(
			 (sunlightDiffuse + ambientLight) * materialDiffuseColor 
				+ sunlightSpecular * materialSpecularColor
				+ materialEmissiveColor,
			 float3(1,1,1)
		);
	}
#endif

/**
 *	all values in world coordinates
 */
float3 computeSunlight(
	float3 position, 
	float3 normal, 
	float3 viewDirection, 
	float3 materialEmissiveColor, 
	float3 materialDiffuseColor, 
	float3 materialSpecularColor, 
	float materialShininess
)
{	
	float3 sunlightDiffuse;
	float3 sunlightSpecular;
	computeOnlySunlight(
		position, 
		normal, 
		viewDirection, 
		materialEmissiveColor, 
		materialDiffuseColor, 
		materialSpecularColor, 
		materialShininess,
		sunlightDiffuse,
		sunlightSpecular
	); 
	
    return min(
		 (sunlightDiffuse + ambientLight) * materialDiffuseColor 
			+ sunlightSpecular * materialSpecularColor
			+ materialEmissiveColor,
		 float3(1,1,1)
	);
}

#ifdef SHADOWS
	/**
	 *	all values in world coordinates
	 */
	float3 computeLight(
		float3 position, 
		float3 normal, 
		float3 viewDirection, 
		SHADOW_FLOAT3 sunlightLowShadowCoord, 
		SHADOW_FLOAT3 sunlightHighShadowCoord, 
		SHADOW_FLOAT3 pointLightShadowCoord0, 
		SHADOW_FLOAT3 pointLightShadowCoord1, 
		SHADOW_FLOAT3 pointLightShadowCoord2, 
		float3 materialEmissiveColor, 
		float3 materialDiffuseColor, 
		float3 materialSpecularColor, 
		float materialShininess
	)
	{	
		float3 sunlightDiffuse;
		float3 sunlightSpecular;
		computeOnlySunlight(
			position, 
			normal, 
			viewDirection, 
			sunlightLowShadowCoord,
			sunlightHighShadowCoord,
			materialEmissiveColor, 
			materialDiffuseColor, 
			materialSpecularColor, 
			materialShininess,
			sunlightDiffuse,
			sunlightSpecular
		); 
		
		float3 pointlightDiffuse;
		float3 pointlightSpecular;
		computeOnlyPointlights(
			position, 
			normal, 
			viewDirection, 
			pointLightShadowCoord0, 
			pointLightShadowCoord1, 
			pointLightShadowCoord2, 
			materialEmissiveColor, 
			materialDiffuseColor, 
			materialSpecularColor, 
			materialShininess,
			pointlightDiffuse,
			pointlightSpecular
		);
		
		return min(
			 (sunlightDiffuse + pointlightDiffuse + LIGHT_FLOAT3(ambientLight)) * materialDiffuseColor 
				+ (sunlightSpecular + pointlightSpecular) * materialSpecularColor
				+ materialEmissiveColor,
			 float3(1,1,1)
		);
	}
	/**
	 *	all values in world coordinates
	 */
	float3 computeLight(
		float3 position, 
		float3 normal, 
		float3 viewDirection, 
		SHADOW_FLOAT3 sunlightLowShadowCoord, 
		SHADOW_FLOAT3 sunlightHighShadowCoord, 
		float3 materialEmissiveColor, 
		float3 materialDiffuseColor, 
		float3 materialSpecularColor, 
		float materialShininess
	)
	{	
		float3 sunlightDiffuse;
		float3 sunlightSpecular;
		computeOnlySunlight(
			position, 
			normal, 
			viewDirection, 
			sunlightLowShadowCoord,
			sunlightHighShadowCoord,
			materialEmissiveColor, 
			materialDiffuseColor, 
			materialSpecularColor, 
			materialShininess,
			sunlightDiffuse,
			sunlightSpecular
		); 
		
		float3 pointlightDiffuse;
		float3 pointlightSpecular;
		computeOnlyPointlights(
			position, 
			normal, 
			viewDirection, 
			materialEmissiveColor, 
			materialDiffuseColor, 
			materialSpecularColor, 
			materialShininess,
			pointlightDiffuse,
			pointlightSpecular
		);
		
		return min(
			 (sunlightDiffuse + pointlightDiffuse + LIGHT_FLOAT3(ambientLight)) * materialDiffuseColor 
				+ (sunlightSpecular + pointlightSpecular) * materialSpecularColor
				+ materialEmissiveColor,
			 float3(1,1,1)
		);
	}
#endif
	
/**
 *	all values in world coordinates
 */
float3 computeLight(
	float3 position, 
	float3 normal, 
	float3 viewDirection, 
	float3 materialEmissiveColor, 
	float3 materialDiffuseColor, 
	float3 materialSpecularColor, 
	float materialShininess
)
{	
	float3 sunlightDiffuse;
	float3 sunlightSpecular;
	computeOnlySunlight(
		position, 
		normal, 
		viewDirection, 
		materialEmissiveColor, 
		materialDiffuseColor, 
		materialSpecularColor, 
		materialShininess,
		sunlightDiffuse,
		sunlightSpecular
	); 
	
	float3 pointlightDiffuse;
	float3 pointlightSpecular;
	computeOnlyPointlights(
		position, 
		normal, 
		viewDirection, 
		materialEmissiveColor, 
		materialDiffuseColor, 
		materialSpecularColor, 
		materialShininess,
		pointlightDiffuse,
		pointlightSpecular
	);
	
	return min(
		 (sunlightDiffuse + pointlightDiffuse + LIGHT_FLOAT3(ambientLight)) * materialDiffuseColor 
			+ (sunlightSpecular + pointlightSpecular) * materialSpecularColor
			+ materialEmissiveColor,
		 float3(1,1,1)
	);
}


#ifdef SHADOWS
	SHADOW_FLOAT3 makePointlightShadowCoord(float4 wposition, int shadowmapId)
	{		
		SHADOW_FLOAT4 shadowCoord = mul(wposition, pointlightShadowmapMatrices[shadowmapId]);
		
		return shadowCoord.xyz;
	}

	void computePointlightShadowCoords(float3 wposition, out SHADOW_FLOAT3 shadowCoord0, out SHADOW_FLOAT3 shadowCoord1, out SHADOW_FLOAT3 shadowCoord2)
	{
		float4 wposition4 = float4(wposition, 1);
		
		if(pointlightShadowmapCount == 0)
		{
			shadowCoord0 = SHADOW_FLOAT3(0,0,0);
			shadowCoord1 = SHADOW_FLOAT3(0,0,0);
			shadowCoord2 = SHADOW_FLOAT3(0,0,0);
		}
		else if(pointlightShadowmapCount == 1)
		{
			shadowCoord0 = makePointlightShadowCoord(wposition4, 0);
			shadowCoord1 = SHADOW_FLOAT3(0,0,0);
			shadowCoord2 = SHADOW_FLOAT3(0,0,0);
		}
		else if(pointlightShadowmapCount == 2)
		{
			shadowCoord0 = makePointlightShadowCoord(wposition4, 0);
			shadowCoord1 = makePointlightShadowCoord(wposition4, 1);
			shadowCoord2 = SHADOW_FLOAT3(0,0,0);
		}
		else
		{
			shadowCoord0 = makePointlightShadowCoord(wposition4, 0);
			shadowCoord1 = makePointlightShadowCoord(wposition4, 1);
			shadowCoord2 = makePointlightShadowCoord(wposition4, 2);
		}
	}
	void computeSunlightShadowCoords(float3 wposition, out SHADOW_FLOAT3 shadowCoordLow, out SHADOW_FLOAT3 shadowCoordHigh)
	{
		shadowCoordLow = mul(float4(wposition, 1), sunlightShadowmapLowMatrix);
		shadowCoordHigh = mul(float4(wposition, 1), sunlightShadowmapHighMatrix);
	}
#endif