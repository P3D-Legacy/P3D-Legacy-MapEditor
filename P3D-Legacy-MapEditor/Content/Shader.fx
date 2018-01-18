float4x4	View;
float4x4	Projection;

float3		DiffuseColor;
float		Alpha;

struct VSInput
{
	float4x4 World : BLENDWEIGHT;
	
    //float4 World1 : TEXCOORD0;
	//float4 World2 : TEXCOORD1;
	//float4 World3 : TEXCOORD2;
	//float4 World4 : TEXCOORD3;
    //float3 Normal   : NORMAL;
    //float2 TexCoord : TEXCOORD0;
};

struct VSOutputTxNoFog
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    //float2 TexCoord   : TEXCOORD0;
};


VSOutputTxNoFog VSBasic(VSInput vin)
{
    VSOutputTxNoFog vout;
	
	//float4x4 world				=	float4x4(
	//vin.World1.x, vin.World1.y, vin.World1.z, vin.World1.w,
	//vin.World2.x, vin.World2.y, vin.World2.z, vin.World2.w,
	//vin.World3.x, vin.World3.y, vin.World3.z, vin.World3.w,
	//vin.World4.x, vin.World4.y, vin.World4.z, vin.World4.w);
	
	//float4x4 world				=	float4x4(
	//vin.World1.x, vin.World2.x, vin.World3.x, vin.World4.x,
	//vin.World1.y, vin.World2.y, vin.World3.y, vin.World4.y,
	//vin.World1.z, vin.World2.z, vin.World3.z, vin.World4.z,
	//vin.World1.w, vin.World2.w, vin.World3.w, vin.World4.w);
	
	//float4 worldPosition		=	mul(vin.Position, World);
    //float4 viewPosition			=	mul(worldPosition, View);
    //vout.PositionPS				=	mul(viewPosition, Projection);
	
	float4x4 worldView			=	mul(vin.World, View);
	float4x4 worldViewProj		=	mul(worldView, Projection);
	vout.PositionPS				=	mul(float4(0.0f, 0.0f, 0.0f, 1.0f), worldViewProj);
    vout.Diffuse 				= 	float4(DiffuseColor, Alpha);
    
    return vout;
}

float4 PSBasicTxNoFog(VSOutputTxNoFog pin) : SV_Target0
{
	return pin.Diffuse;
    //return SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
}

technique BasicEffect { pass { VertexShader = compile vs_4_0_level_9_1 VSBasic (); PixelShader = compile ps_4_0_level_9_1 PSBasicTxNoFog(); } }
