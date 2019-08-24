


float4x4 vp : VIEWPROJECTION;

float4 materialDiffuse : MATERIAL_DIFFUSE;




struct VIN {
	float4 position : POSITION;
	float4 worldMatrixRow1 : TEXCOORD0;
	float4 worldMatrixRow2 : TEXCOORD1;
	float4 worldMatrixRow3 : TEXCOORD2;
};



/**********************************************
	color
**********************************************/
struct VOUT0 {
	float4 position : POSITION;
};
VOUT0 Vertex0(VIN IN) {
	VOUT0 OUT;
	
	float3 wpos = mul(float3x4(IN.worldMatrixRow1,IN.worldMatrixRow2,IN.worldMatrixRow3), IN.position);
	OUT.position = mul(float4(wpos,1), vp);

	return OUT;
}
float4 Fragment0() : COLOR {
	return materialDiffuse;
}



technique t_hardwareinstancing_wireframe_2_0
{
	pass p0
	{
		FillMode = WIREFRAME;
		VertexShader = compile vs_2_0 Vertex0();
		PixelShader = compile ps_2_0 Fragment0();
	}
}
