#define NUM_LIGHTS 8

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 ViewVector;
texture ModelTexture;
texture NormalMap;
texture RefractionMap;

//General Light Values
float AmbientIntensity = 0.5;
float SpecularIntensity = 0.1;
float Shininess = 200;
float BumpConstant = 1;
int PointLightNumber = 0;
float bias = 0.005;

//Directional Light
float3 DirectionalLightDirection;
float4 DirectionalAmbientColor;
float4 DirectionalSpecularColor;
float4x4 DirectionalLightViewProj;
texture DirectionalShadowMap;

//Point Lights
float3 PointLightPosition[NUM_LIGHTS];
float3 PointLightAttenuation[NUM_LIGHTS];
float4 PointAmbientColor[NUM_LIGHTS];
float4 PointSpecularColor[NUM_LIGHTS];
float4x4 PointLightViewProj[NUM_LIGHTS];

sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

sampler2D normalSampler = sampler_state {
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler2D shadowMapSampler = sampler_state {
	Texture = <DirectionalShadowMap>;
};

samplerCUBE refractionCubeSampler = sampler_state
{
	texture = <RefractionMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 Tangent : TEXCOORD2;
	float3 Binormal : TEXCOORD3;
	float4 WorldPos : TEXCOORD4;
};

struct CreateShadowMap_VertexShaderOutput
{
	float4 Position : POSITION;
	float Depth : TEXCOORD0;
};

CreateShadowMap_VertexShaderOutput CreateShadowMap_VertexShader(float4 Position: POSITION)
{
	CreateShadowMap_VertexShaderOutput output;
	output.Position = mul(Position, mul(World, DirectionalLightViewProj));
	output.Depth = output.Position.z / output.Position.w;
	return output;
}

float4 CreateShadowMap_PixelShader(CreateShadowMap_VertexShaderOutput input) : COLOR
{
	return float4(input.Depth, 0, 0, 0);
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPos = worldPosition;
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Normal = normalize(mul(input.Normal, World));
	output.Tangent = normalize(mul(input.Tangent, World));
	output.Binormal = normalize(mul(input.Binormal, World));

	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float Toonify(float intensity)
{
	float lightIntensity;
	if (intensity > 0.95) lightIntensity = 1.0f;
	else if (intensity > 0.5) lightIntensity = 0.7f;
	else if (intensity > 0.05) lightIntensity = 0.3f;
	else intensity = 0.0f;

	lightIntensity = max(0.02, lightIntensity);
	return lightIntensity;
}

float4 DirectionalLightCalculation(VertexShaderOutput input)
{
	float3 bump = BumpConstant * (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
	bumpNormal = normalize(bumpNormal);

	float lightIntensity = dot(normalize(DirectionalLightDirection), bumpNormal);
	//lightIntensity = Toonify(lightIntensity);

	float3 light = normalize(DirectionalLightDirection);
	float dotProduct = dot(normalize(2 * dot(light, bumpNormal) * bumpNormal - light), normalize(mul(normalize(ViewVector), World)));
	float4 diffuseColor = tex2D(textureSampler, input.TextureCoordinate);

	float4 ambient = diffuseColor * DirectionalAmbientColor * AmbientIntensity;
	float4 specular = SpecularIntensity * DirectionalSpecularColor * max(pow(dotProduct, Shininess), 0);
	specular = saturate(specular);
	float4 diffuse = diffuseColor * lightIntensity;

	float4 lightingPosition = mul(input.WorldPos, DirectionalLightViewProj);

	float4 finalColor = ambient + diffuse + specular;
	finalColor = max(0, finalColor);
	return finalColor;
}

float4 PointLightCalculation(VertexShaderOutput input)
{
	float4 ambient;
	float4 specular;
	float4 diffuse;
	for (int i = 0; i < PointLightNumber; i++)
	{
		float3 bump = BumpConstant * (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
		float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
		bumpNormal = normalize(bumpNormal);

		float lightIntensity = dot(normalize(PointLightPosition[i] - input.WorldPos), bumpNormal);
		//lightIntensity = Toonify(lightIntensity);

		float3 light = normalize(PointLightPosition[i] - input.WorldPos);

		float dist = length(PointLightPosition[i] - input.WorldPos);
		float att = 1.0 / (PointLightAttenuation[i].x + PointLightAttenuation[i].y * dist + PointLightAttenuation[i].z * dist * dist);

		float dotProduct = dot(normalize(2 * dot(light, bumpNormal) * bumpNormal - light), normalize(mul(normalize(ViewVector), World)));
		float4 diffuseColor = tex2D(textureSampler, input.TextureCoordinate);

		ambient += diffuseColor * PointAmbientColor[i] * AmbientIntensity * att;
		specular += SpecularIntensity * PointSpecularColor[i] * max(pow(dotProduct, Shininess), 0) * att;
		diffuse += diffuseColor * att * lightIntensity;
	}
	specular = saturate(specular);
	float4 finalColor = ambient + diffuse + specular;
	finalColor = max(0, finalColor);
	return finalColor;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 directionalLight = DirectionalLightCalculation(input);
	float4 pointLight = PointLightCalculation(input);

	float4 light = directionalLight + pointLight;

	float3 bump = BumpConstant * (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
	bumpNormal = normalize(bumpNormal);

	float3 Refract = refract(ViewVector, bumpNormal, 0.66);
	float3 RefractColor = texCUBE(refractionCubeSampler, Refract * float3(1, 1, -1));

	return saturate(light * float4(RefractColor, 1));
}

technique Draw
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}

technique CreateShadowMap
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 CreateShadowMap_VertexShader();
		PixelShader = compile ps_4_0 CreateShadowMap_PixelShader();
	}
}