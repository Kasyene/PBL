#define NUM_LIGHTS 8

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 ViewVector;
int pointLightNumber = 0;
texture ModelTexture;
texture ShadowMap;

//General Light Values
float AmbientIntensity = 0.1;
float SpecularIntensity = 0.1;
float Shininess = 200;

//Directional Light
float3 DirectionalLightDirection;
float4 DirectionalAmbientColor;
float4 DirectionalDiffuseColor;
float4 DirectionalSpecularColor;
float4x4 DirectionalLightViewProj;

//Point Lights
float3 PointLightPosition[NUM_LIGHTS];
float4 PointAmbientColor[NUM_LIGHTS];
float4 PointDiffuseColor[NUM_LIGHTS];
float4 PointSpecularColor[NUM_LIGHTS];
float4x4 PointLightViewProj[NUM_LIGHTS];


sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

sampler2D shadowMapSampler = sampler_state {
	Texture = <ShadowMap>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
	float4 WorldPos : TEXCOORD2;
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
	float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Normal = normal;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 DirectionalLightCalculation(VertexShaderOutput input)
{
	float3 light = normalize(DirectionalLightDirection);
	float3 normal = normalize(input.Normal);
	float dotProduct = dot(normalize(2 * dot(light, normal) * normal - light), normalize(mul(normalize(ViewVector), World)));
	float lightIntensity = dot(DirectionalLightDirection, normal);
	float4 diffuseColor = tex2D(textureSampler, input.TextureCoordinate);

	float4 ambient = DirectionalAmbientColor * AmbientIntensity;
	float4 specular = SpecularIntensity * DirectionalSpecularColor * max(pow(dotProduct, Shininess), 0);
	float4 diffuse = diffuseColor * lightIntensity;

	float4 lightingPosition = mul(input.WorldPos, DirectionalLightViewProj);

	float2 shadowTexCoord = 0.5 * lightingPosition.xy /lightingPosition.w + float2(0.5, 0.5);
	shadowTexCoord.y = 1.0f - shadowTexCoord.y;
	float shadowdepth = tex2D(shadowMapSampler, shadowTexCoord).r;
	float ourdepth = (lightingPosition.z / lightingPosition.w) - 0.001f;

	if (shadowdepth < ourdepth)
	{
		diffuse *= float4(0.5, 0.5, 0.5, 0);
	};

	return saturate(diffuse + ambient + specular);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 directionalLight = DirectionalLightCalculation(input);

	return directionalLight;
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