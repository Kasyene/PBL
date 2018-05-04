texture2D ScreenTexture; 
sampler screenTextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float2 ScreenSize = float2(600, 600);

float Thickness = 1.0f;
float Threshold = 0.4f;

float getGray(float4 c)
{
	return(dot(c.rgb,((0.33333).xxx)));
}

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 Color = ScreenTexture.Sample(screenTextureSampler, texCoord.xy);
	float2 ox = float2(Thickness/ScreenSize.x,0.0);
	float2 oy = float2(0.0,Thickness/ScreenSize.y);
	float2 uv = texCoord.xy;

	float2 PP = uv - oy;
	float4 CC = ScreenTexture.Sample(screenTextureSampler, PP-ox);	float g00 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP);			float g01 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP+ox);			float g02 = getGray(CC);

	PP = uv;
	CC = ScreenTexture.Sample(screenTextureSampler, PP-ox);			float g10 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP);			float g11 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP+ox);			float g12 = getGray(CC);

	PP = uv + oy;
	CC = ScreenTexture.Sample(screenTextureSampler, PP-ox);			float g20 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP);			float g21 = getGray(CC);
	CC = ScreenTexture.Sample(screenTextureSampler, PP+ox);			float g22 = getGray(CC);

	float K00 = -1;
	float K01 = -2;
	float K02 = -1;
	float K10 = 0;
	float K11 = 0;
	float K12 = 0;
	float K20 = 1;
	float K21 = 2;
	float K22 = 1;

	float sx = 0;
	sx += g00 * K00;
	sx += g01 * K01;
	sx += g02 * K02;
	sx += g10 * K10;
	sx += g11 * K11;
	sx += g12 * K12;
	sx += g20 * K20;
	sx += g21 * K21;
	sx += g22 * K22;

	float sy = 0;
	sy += g00 * K00;
	sy += g01 * K10;
	sy += g02 * K20;
	sy += g10 * K01;
	sy += g11 * K11;
	sy += g12 * K21;
	sy += g20 * K02;
	sy += g21 * K12;
	sy += g22 * K22;
	
	float contrast = sqrt(sx*sx + sy*sy);
	
	float result = 1;

	if (contrast > Threshold)
	{
		result = 0;
	}
	
	return Color * float4(result.xxx, 1);
}

technique PostOutline
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
