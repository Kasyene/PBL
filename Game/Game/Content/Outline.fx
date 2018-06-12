texture2D ScreenTexture; 
sampler screenTextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float2 ScreenSize = float2(600, 600);

float Thickness = 0.3f;
float Threshold = 0.2f;

float FadeAmount = 0.0f;
float GammaValue = 1.0f;

bool TimeStop = true;

float GetGray(float4 c)
{
	return(dot(c.rgb,((0.33333).xxx)));
}

float4 PixelShaderOutline(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = ScreenTexture.Sample(screenTextureSampler, texCoord.xy); //tu samplujemy color textury w danym punkcie
	float2 uv = texCoord.xy;

	float g11 = GetGray(color);//uzyskujemy odcieñ szaroœci

	if (g11 < 0.3) //sprawdzamy czy dany punkt nie jest w cieniu, jeœli jest to zmiejszamy threshold czyli warunek kiedy bêdzie outline
	{
		Threshold /= 5;
		Thickness *= 2;
	}

	float2 ox = float2(Thickness / ScreenSize.x, 0.0); //odleg³oœæ próbkowania w x
	float2 oy = float2(0.0, Thickness / ScreenSize.y); //odleg³oœæ próbkowania w y

	float2 pointPosition = uv - oy; //ustawiamy nasz¹ pozycjê na pierwszy rz¹d kernela 
	//nastêpnie próbkujemy kolejne punkty przesuwaj¹c siê od do³u do góry i z ka¿dej próbki uzyskujemy odcieñ szaroœci
	float4 actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition -ox);		float g00 = GetGray(actualSample);
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition);			float g01 = GetGray(actualSample);
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition +ox);		float g02 = GetGray(actualSample);

	pointPosition = uv; //œrodkowy rz¹d kernela
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition -ox);		float g10 = GetGray(actualSample);
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition +ox);		float g12 = GetGray(actualSample);

	pointPosition = uv + oy;//ostatni rz¹d kernela
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition -ox);		float g20 = GetGray(actualSample);
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition);			float g21 = GetGray(actualSample);
	actualSample = ScreenTexture.Sample(screenTextureSampler, pointPosition +ox);		float g22 = GetGray(actualSample);

	/*
	Kernel
	-1 0 1
	-2 0 2
	-1 0 1
	tzw. Operator Sobela
	*/
	float K00 = -1;
	float K01 = -2;
	float K02 = -1;
	float K10 = 0;
	float K11 = 0;
	float K12 = 0;
	float K20 = 1;
	float K21 = 2;
	float K22 = 1;

	//mno¿ymy nasze próbki odcieni szaroœci przez kernel i sumujemy
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

	//mno¿ymy nasze próbki odcieni szaroœci przez kernel obrócony o 90 stopni i sumujemy
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

	//liczymy kontrast jako pierwiastek z sumy kwadratów wyników poszczególnych kerneli
	float contrast = sqrt(sx*sx + sy*sy);
	
	// wartoœæ domyœlna koloru outline
	float result = 0.8;

	//je¿eli otrzymany kontrast jest wiêkszy ni¿ za³o¿ony próg (threshold) to wartoœæ dla outline ustawiamy na zero (czarny) 
	if (contrast > Threshold)
	{
		result = 0;
	}
	
	//je¿eli czas nie jest zatrzymany to mno¿ymy 2 razy kolor punktu razy wynik outline
	if (!TimeStop)
	{
		color = saturate(color * color * float4(result.xxxx));
	}
	else
	//je¿eli czas jest zatrzymany to mno¿ymy 2 razy kolor punktu razy wynik outline i mnozymy razy 0.5 by unikn¹æ przejaskrawienia przy efekcie sepii
	{
		color = saturate(color * color * float4(result.xxxx)) * 0.5;
	}
	return pow(color, float4((1 / GammaValue).xxxx)); //gamma correction
}

float4 PixelShaderSepia(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	if (TimeStop)
	{
		float4 color = ScreenTexture.Sample(screenTextureSampler, texCoord.xy);

		float4 outputColor = color;
		outputColor.r = (color.r * 0.393) + (color.g * 0.769) + (color.b * 0.189);
		outputColor.g = (color.r * 0.349) + (color.g * 0.686) + (color.b * 0.168);
		outputColor.b = (color.r * 0.272) + (color.g * 0.534) + (color.b * 0.131);

		return pow(outputColor * 0.5, float4((1 / GammaValue).xxxx));
	}
	else
	{
		return 0;
	}
}

float4 PixelShaderFade(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	if (FadeAmount < 0.01)
	{
		return 0;
	}
	else
	{
	float4 color = tex2D(screenTextureSampler, texCoord.xy);
	float4 color2 = float4(1,1,1,1);

	float4 finalColor = lerp(color,color2,FadeAmount);

	finalColor.a = 1;

	return finalColor;
	}
}

technique PostOutline
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderSepia();
	}

    pass Pass2
    {
        PixelShader = compile ps_4_0 PixelShaderOutline();
    }

	pass Pass3
	{
		PixelShader = compile ps_4_0 PixelShaderFade();
	}
}
