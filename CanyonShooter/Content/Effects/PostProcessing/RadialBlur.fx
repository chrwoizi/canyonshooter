// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

sampler TextureSampler : register(s0);

float InvScreenWidth;

#define SAMPLE_COUNT 10

float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    float2 direction = normalize(texCoord - float2(0.5,0.5));
    
	float weight = 1.0f / float(SAMPLE_COUNT);
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, texCoord + float(i)*5.0*InvScreenWidth*direction) * weight;
    }
    
    return c;
}


technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
