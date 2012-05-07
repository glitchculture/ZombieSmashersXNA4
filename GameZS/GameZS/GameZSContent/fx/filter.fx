//filter.fx
sampler samplerState;
sampler refractSampler;

float burn = 0.01f;
float saturation = 1.0f;
float r = 1.0f;
float g = 1.0f;
float b = 1.0f;
float brite = 0.0f;

struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
};

float2 GetDif(float2 _tex) 
{
	float2 dif;
	
	float2 tex = _tex;
	float2 btex = _tex;
	tex.x -= 0.003;
	btex.x += 0.003;
	dif.x = tex2D(refractSampler, tex).r - tex2D(refractSampler, btex).r;
	
	tex = _tex;
	btex = _tex;
	tex.y -= 0.003;
	btex.y += 0.003;
	
	dif.y = tex2D(refractSampler, tex).r - tex2D(refractSampler, btex).r;
	
	tex = _tex;
	dif *= (1.5 - tex2D(refractSampler, tex).r);
	
	return dif;
}

float4 Filter(PS_INPUT Input) : COLOR0
{
	
	float2 tex = Input.TexCoord + GetDif(Input.TexCoord) * 0.1f;
	float4 col = tex2D(samplerState, tex);
	float d = sqrt(pow((tex.x - 0.5), 2) + pow((tex.y - 0.5), 2));
	
	col.rgb -= d * burn;
	
	float a = col.r + col.g + col.b;
	a /= 3.0f;
	
	a *= 1.0f - saturation;
	
	col.r = (col.r * saturation + a) * r;
	col.g = (col.g * saturation + a) * g;
	col.b = (col.b * saturation + a) * b;
	
	col.rgb += brite;
	
	return col;
}

technique Filter2
{
    pass P0
    {
        PixelShader = compile ps_2_0 Filter();
    }
}
