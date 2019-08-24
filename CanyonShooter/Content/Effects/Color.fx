


float4x4 mvp : WORLDVIEWPROJECTION;

float4 materialDiffuse : MATERIAL_DIFFUSE;




struct VIN {
	float4 position : POSITION;
};



/**********************************************
	color
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
	return materialDiffuse;
}



technique t_basic_2_0
{
	pass p0
	{
		VertexShader = compile vs_1_1 Vertex0();
		PixelShader = compile ps_1_1 Fragment0();
	}
}

technique t_color_2_0
{
	pass p0
	{
		VertexShader = compile vs_1_1 Vertex0();
		PixelShader = compile ps_1_1 Fragment0();
	}
}
