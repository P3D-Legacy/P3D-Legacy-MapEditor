#define FXAA_PC 1
#define FXAA_HLSL_4 1
#define FXAA_QUALITY__PRESET 12
#define FXAA_GREEN_AS_LUMA 1
#include "Fxaa3_11.h"

float SubpixelQuality;
float EdgeThreshold;
float EdgeThresholdMin;
float2 InverseDimensions;

texture2D Texture;
sampler TextureSampler : register(s0) = sampler_state
{
	Texture = <Texture>;
};

float4 PS(float4 position : SV_Position, float4 color : COLOR0, float2 texCoords : TEXCOORD0) : SV_Target0
{
	FxaaTex tex;
	tex.smpl = TextureSampler;
	tex.tex = Texture;

	return FxaaPixelShader(
		texCoords,
		tex,
		InverseDimensions,
		SubpixelQuality,
		EdgeThreshold,
		EdgeThresholdMin
	);
}

Technique T
{
	Pass P
	{
		PixelShader = compile ps_4_0 PS();
	}
}