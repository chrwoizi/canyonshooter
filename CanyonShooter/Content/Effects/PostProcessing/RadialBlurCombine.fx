// Pixel shader combines the blur image with the original scene

sampler BlurSampler : register(s0);
sampler BaseSampler : register(s1);

float Power = 1.0;

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}

float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 base = tex2D(BaseSampler, texCoord);
    float4 blur = tex2D(BlurSampler, texCoord);
    
    float distance = 2*length(texCoord - float2(0.5,0.5));
    
    float blurAmount = min(pow(distance, 0.5), 1);
    float4 result = lerp(base, blur, blurAmount);
    
    float saturation = max(0, 1 - pow(distance,2));
    result = AdjustSaturation(result, saturation);
    
    return lerp(base, result, Power);
}


technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
