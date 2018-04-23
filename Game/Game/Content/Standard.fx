float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 ViewVector;
texture ModelTexture;
texture ShadowMap;

//General Light Values
float AmbientIntensity = 0.1;
float DiffuseIntensity = 3.0;
float SpecularIntensity = 1;
float Shininess = 200;

//Directional Light
float3 DirectionalLightDirection;
float4 DirectionalAmbientColor;
float4 DirectionalDiffuseColor;
float4 DirectionalSpecularColor;
float4x4 DirectionalLightViewProj;


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
	float lightIntensity = dot(normal, DirectionalLightDirection);
	float4 diffuseColor = saturate(DirectionalDiffuseColor * DiffuseIntensity * lightIntensity);
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1;

	float4 ambient = textureColor * DirectionalAmbientColor * AmbientIntensity;
	float4 specular = SpecularIntensity * DirectionalSpecularColor * max(pow(dotProduct, Shininess), 0);
	float4 diffuse = textureColor * diffuseColor;

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