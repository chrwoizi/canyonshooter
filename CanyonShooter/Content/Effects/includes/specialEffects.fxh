

bool useFog : FOG = true;

float FogPlaneHeight = -80;

float4 fogged(float4 pixelColor, float3 wposition, float3 viewerPosition, float3 wview, float height)
{
	float planeFogDepth = 0;
	
	if(viewerPosition.y < height)
	{
		if(wposition.y < height)
		{
			planeFogDepth = length(wview);
		}
		else
		{
			float3 n = float3(0,1,0);
			float3 d = normalize(wview);
			float3 p = viewerPosition;
			planeFogDepth = (height - dot(p,n)) / dot(d,n);
		}
	}
	else
	{
		if(wposition.y < height)
		{		
			float3 n = float3(0,1,0);
			float3 d = normalize(wview);
			float3 p = viewerPosition;
			float l = (height - dot(p,n)) / dot(d,n);
			planeFogDepth = length(wview) - l;
		}
	}
			
	float defaultDistance = length(wview);
	float defaultFog = min(defaultDistance/6000, 1);
	float planeFog = min(planeFogDepth / 3000, 1);
	
	float fog = max(defaultFog*defaultFog, lerp(planeFog*planeFog,planeFog,0.5));
	fog = min(fog, 0.5);
	
	float4 fogColor = float4(1,1,1,1);
	
	return float4(lerp(pixelColor.rgb, fogColor.rgb, fog), pixelColor.a);
}

float4 specialEffects(float4 color, float3 wposition, float3 viewerPosition, float3 wview)
{
	float4 result = color;
	
	if(useFog)
	{
		result = fogged(color, wposition, viewerPosition, wview, FogPlaneHeight);
	}
	
	return result;
}