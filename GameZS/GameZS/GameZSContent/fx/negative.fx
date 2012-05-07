//negative.fx
sampler samplerState;

struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
};

float4 Neg(PS_INPUT Input) : COLOR0
{
	float4 col = tex2D(samplerState, Input.TexCoord.xy);
	col.rgb = 1 - col.rgb;
	
	return col;
}

technique Burn
{
    pass P0
    {
        PixelShader = compile ps_2_0 Neg();
    }
}
