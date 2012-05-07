// water.fx
sampler samplerState; 

float horizon = 0.5;
float delta;
float theta;

struct PS_INPUT 
{ 
	float2 TexCoord : TEXCOORD0; 
}; 

float4 Water(PS_INPUT Input) : COLOR0 
{
	float4 sum = 0;
	float2 tex = Input.TexCoord;
	
	if (tex.y < 1.0) 
	{
		tex.y = horizon - tex.y;
		tex.y += (cos(tex.x * 50.0 + delta) / 500.0f);
		tex.y += (sin(tex.y * 250.0 + theta) / 120.0f);
		tex.x += (tex.y * (sin(tex.y * 750.0 + theta) / 250.0f));
		
		sum = tex2D(samplerState, tex);
	}
	
	sum *= 0.35f;
	
	if (Input.TexCoord.y < 0.2f)
		sum.a = 0.0;
	else if (Input.TexCoord.y < 0.25f)
		sum.a = (Input.TexCoord.y - 0.2) * 20.0;
	else sum.a = 1.0;
    
    return sum;
}

technique Water2 
{ 
	pass P0
	{ 
		PixelShader = compile ps_2_0 Water(); 
	} 
}