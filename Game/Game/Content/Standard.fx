#define NUM_LIGHTS 8

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 ViewVector;
int PointLightNumber = 0;
texture ModelTexture;
texture NormalMap;

//General Light Values
float AmbientIntensity = 0.3;
float SpecularIntensity = 0.1;
float Shininess = 200;
float BumpConstant = 1;

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
texture PointLightShadowCubeMap[NUM_LIGHTS];


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

samplerCUBE shadowCubeMapSampler = sampler_state {
	Texture = <PointLightShadowCubeMap>;
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

float4 DirectionalLightCalculation(VertexShaderOutput input)
{
	float3 bump = BumpConstant * (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
	bumpNormal = normalize(bumpNormal);

	float lightIntensity = dot(normalize(DirectionalLightDirection), bumpNormal);
	if (lightIntensity < 0)
		lightIntensity = 0;

	float3 light = normalize(DirectionalLightDirection);
	float dotProduct = dot(normalize(2 * dot(light, bumpNormal) * bumpNormal - light), normalize(mul(normalize(ViewVector), World)));
	float4 diffuseColor = tex2D(textureSampler, input.TextureCoordinate);

	float4 ambient = diffuseColor * DirectionalAmbientColor * AmbientIntensity;
	float4 specular = SpecularIntensity * DirectionalSpecularColor * max(pow(dotProduct, Shininess), 0);
	float4 diffuse = diffuseColor * lightIntensity;

	float4 lightingPosition = mul(input.WorldPos, DirectionalLightViewProj);

	float2 shadowTexCoord = 0.5 * lightingPosition.xy /lightingPosition.w + float2(0.5, 0.5);
	shadowTexCoord.y = 1.0f - shadowTexCoord.y;
	float shadowdepth = tex2D(shadowMapSampler, shadowTexCoord).r;
	float ourdepth = (lightingPosition.z / lightingPosition.w) - 0.001f;

	if (shadowdepth < ourdepth)
	{
		diffuse *= float4(0.1, 0.1, 0.1, 0);
		specular *= float4(0.1, 0.1, 0.1, 0);
	};

	return saturate(diffuse + ambient + specular);
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
		if (lightIntensity < 0)
			lightIntensity = 0;

		float3 light = normalize(PointLightPosition[i] - input.WorldPos);

		float dist = length(PointLightPosition[i] - input.WorldPos);
		float att = 1.0 / (PointLightAttenuation[i].x + PointLightAttenuation[i].y * dist + PointLightAttenuation[i].z * dist * dist);

		float dotProduct = dot(normalize(2 * dot(light, bumpNormal) * bumpNormal - light), normalize(mul(normalize(ViewVector), World)));
		float4 diffuseColor = tex2D(textureSampler, input.TextureCoordinate);

		ambient += diffuseColor * PointAmbientColor[i] * AmbientIntensity * att;
		specular += SpecularIntensity * PointSpecularColor[i] * max(pow(dotProduct, Shininess), 0) * att;
		diffuse += diffuseColor * att * lightIntensity;
	}
	return saturate(diffuse + ambient + specular);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 directionalLight = DirectionalLightCalculation(input);
	float4 pointLight = PointLightCalculation(input);

	return saturate(directionalLight + pointLight);
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