//bloom.fx

sampler samplerState;

float v = 0.01f;
float a = 0.2f;

const float2 offsets[12] = {
   -0.326212, -0.405805,
   -0.840144, -0.073580,
   -0.695914,  0.457137,
   -0.203345,  0.620716,
    0.962340, -0.194983,
    0.473434, -0.480026,
    0.519456,  0.767022,
    0.185461, -0.893124,
    0.507431,  0.064425,
    0.896420,  0.412458,
   -0.321940, -0.932615,
   -0.791559, -0.597705,
};

struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
};

float4 Bloom(PS_INPUT Input) : COLOR0
{
	
	float4 col = tex2D(samplerState, Input.TexCoord);
	for (int i = 0; i < 12; i++)
		col += tex2D(samplerState, Input.TexCoord + v * offsets[i]);
		
	
	col /= 13.0f;
	col.a = a;
	
	return col;
}

technique BloomTechnique
{
    pass P0
    {
        PixelShader = compile ps_2_0 Bloom();
    }
}
