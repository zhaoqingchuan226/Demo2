#ifndef _BSDF_Library_
#define _BSDF_Library_

#include "Common.hlsl"
#include "Montcalo_Library.hlsl"
//#include "ImageBasedLighting.hlsl"

#define TRANSMISSION_WRAP_ANGLE (PI/12)             
#define TRANSMISSION_WRAP_LIGHT cos(PI/2 - TRANSMISSION_WRAP_ANGLE)


struct BSDFContext
{
	float NoL;
	float NoV;
	float NoH;
	float LoH;
	float VoL;
	float VoH;
};

void Init(inout BSDFContext LightData, half3 N, half3 V, half3 L, half3 H)
{
	LightData.NoL = max(dot(N, L), 0);
	LightData.NoV = max(dot(N, V), 0);
	LightData.NoH = max(dot(N, H), 0);
	LightData.LoH = max(dot(L, H), 0);
	LightData.VoL = max(dot(V, L), 0);
	LightData.VoH = max(dot(V, H), 0);
}

struct AnisoBSDFContext
{
    float ToH;
    float ToL; 
    float ToV; 
    float BoH;
    float BoL;
    float BoV; 
};

void Init_Aniso(inout AnisoBSDFContext LightData, half3 Tangent, half3 Bitangent, half3 H, half3 L, half3 V)
{
    LightData.ToH = dot(Tangent, H);
    LightData.ToL = dot(Tangent, L); 
    LightData.ToV = dot(Tangent, V); 

    LightData.BoH = dot(Bitangent, H);
    LightData.BoL = dot(Bitangent, L);
	LightData.BoV = dot(Bitangent, V);
}


/////////////////////////////////////////////////////////////////Fresnel
float IorToFresnel(float transmittedIor, float incidentIor)
{
	return pow2(transmittedIor - incidentIor) / pow2(transmittedIor + incidentIor);
}

float3 IorToFresnel(float3 transmittedIor, float3 incidentIor)
{
	return pow2(transmittedIor - incidentIor) / pow2(transmittedIor + incidentIor);
}

float FresnelToIOR(float fresnel0)
{
	return ( 1 + pow2(fresnel0) ) / ( 1 - pow2(fresnel0) );
}

float3 FresnelToIOR(float3 fresnel0)
{
	return ( 1 + pow2(fresnel0) ) / ( 1 - pow2(fresnel0) );
}

float F_Schlick(float F0, float F90, float HoV)
{
	return F0 + (F90 - F0) * pow5(1 - HoV);
}

float3 F_Schlick(float3 F0, float3 F90, float HoV)
{
	float Fc = pow5( 1 - HoV );				
	return saturate( 50 * F0.g ) * Fc + (1 - Fc) * F0;
}

float3 F_Fresnel(float3 F0, float HoV)
{
	float3 F0Sqrt = sqrt(clamp(float3(0, 0, 0), float3(0.99, 0.99, 0.99), F0));
	float3 n = (1 + F0Sqrt) / (1 - F0Sqrt);
	float3 g = sqrt(n * n + HoV * HoV - 1);
	return 0.5 * Square((g - HoV) / (g + HoV)) * (1 + Square(((g + HoV) * HoV - 1) / ((g - HoV) * HoV + 1)));
}

float F_Hair(float CosTheta) {
	const float n = 1.55;
	const float F0 = Square((1 - n) / (1 + n));
	return F0 + (1 - F0) * pow5(1 - CosTheta);
}

float3 EvalSensitivity(float opd, float shift) {
    float phase = 2 * PI * opd * 1e-6;
    float3 val = float3(5.4856e-13, 4.4201e-13, 5.2481e-13);
    float3 pos = float3(1.6810e+06, 1.7953e+06, 2.2084e+06);
    float3 var = float3(4.3278e+09, 9.3046e+09, 6.6121e+09);
    float3 xyz = val * sqrt(2 * PI * var) * cos(pos * phase + shift) * exp(-var * phase * phase);
    xyz.x += 9.7470e-14 * sqrt(2 * PI * 4.5282e+09) * cos(2.2399e+06 * phase + shift) * exp(-4.5282e+09 * phase * phase);
    return xyz / 1.0685e-7;
}

float3 Flim_Iridescence(float eta_1, float cosTheta1, float iridescenceThickness, float3 baseLayerFresnel0, float iorOverBaseLayer = 0) {
    float Dinc = 3 * iridescenceThickness;
    float eta_2 = lerp(2, 1, iridescenceThickness);
    float sinTheta2 = Square(eta_1 / eta_2) * (1 - Square(cosTheta1));
    float cosTheta2 = sqrt(1 - sinTheta2);

    float R0 = IorToFresnel(eta_2, eta_1); 
	float R12 = F_Schlick(R0, 1, cosTheta1);
    float R21 = R12; float T121 = 1 - R12; 

    float OPD = Dinc * cosTheta2;
	float3 R23 = F_Schlick(baseLayerFresnel0, 1, cosTheta2);
	float3 R123 = R12 * R23; float3 r123 = sqrt(R123);
    float3 Rs = Square(T121) * R23 / (1 - R123);
    float3 C0 = R12 + Rs; 
	float3 I = C0;
    float3 Cm = Rs - T121;
	
    UNITY_UNROLL
    for (int m = 1; m <= 2; m++) {
        Cm *= r123;
		float3 Sm = 2 * EvalSensitivity(m * OPD, m * PI);
		I += Cm * Sm;
    }
    return I;
}



//////////Refraction Transmission
void RefractionSphere(float3 V, float3 positionWS, float3 normalWS, float ior, float thickness, out float dist, out float3 position, out float3 rayWS) {
    float3 R1 = refract(-V, normalWS, 1 / ior);
    float3 C = positionWS - normalWS * thickness * 0.5;

    float NoR1 = dot(normalWS, R1);
    float distance = -NoR1 * thickness;
    float3 P1 = positionWS + R1 * distance;
    float3 N1 = normalize(C - P1);
    float3 R2 = refract(R1, N1, ior);
    float N1oR2 = dot(N1, R2);
    float VoR1 = dot(V, R1);

    dist = distance;
    position = P1;
    rayWS = R2;
}

void RefractionPlane(float3 V, float3 positionWS, float3 normalWS, float ior, float thickness, out float dist, out float3 position, out float3 rayWS) {
    float3 R = refract(-V, normalWS, 1 / ior);
    float distance = thickness / dot(R, -normalWS);

    dist = distance;
    position = positionWS + R * dist;
    rayWS = -V;
}



/////////////////////////////////////////////////////////////////Diffuse
float3 Diffuse_Lambert(float3 DiffuseColor)
{
	return DiffuseColor * Inv_PI;
}

half3 Diffuse_Fabric(half3 DiffuseColor, half Roughness)
{
    return Diffuse_Lambert(DiffuseColor) * lerp(1, 0.5, Roughness);
}

half Diffuse_Burley_NoPi(half LoH, half NoL, half NoV, half Roughness)
{
	Roughness = pow4(Roughness);
	half FD90 = 0.5 + 2 * pow2(LoH) * Roughness;
	half ViewScatter = 1 + (FD90 - 1) * pow5(1 - NoL);
	half LightScatter = 1 + (FD90 - 1) * pow5(1 - NoV);
	return ViewScatter * LightScatter;
}

half3 Diffuse_Burley(half LoH, half NoL, half NoV, half3 DiffuseColor, half Roughness)
{
	return Diffuse_Burley_NoPi(LoH, NoL, NoV, Roughness) * DiffuseColor * Inv_PI;
}

half Diffuse_RenormalizeBurley_NoPi(half LoH, half NoL, half NoV, half Roughness)
{
	Roughness = pow4(Roughness);
	half EnergyBias = lerp(0, 0.5, Roughness);
	half EnergyFactor = lerp(1, 1 / 0.662, Roughness);
	half F90 = EnergyBias + 2 * pow2(LoH) * Roughness;
	half LightScatter = F_Schlick(1, F90, NoL);
	half ViewScatter = F_Schlick(1, F90, NoV);
	return LightScatter * ViewScatter * EnergyFactor;
}

half3 Diffuse_RenormalizeBurley(half LoH, half NoL, half NoV, half3 DiffuseColor, half Roughness)
{
	return Diffuse_RenormalizeBurley_NoPi(LoH, NoL, NoV, Roughness) * DiffuseColor * Inv_PI;
}

half Diffuse_OrenNayar_NoPi(half VoH, half NoL, half NoV, half Roughness)
{
	half a = pow2(Roughness);
	half s = a;
	half s2 = s * s;
	half VoL = 2 * VoH * VoH - 1;		
	half Cosri = VoL - NoV * NoL;
	half C1 = 1 - 0.5 * s2 / (s2 + 0.33);
	half C2 = 0.45 * s2 / (s2 + 0.09) * Cosri * ( Cosri >= 0 ? rcp( max( NoL, NoV ) ) : 1 );
	return (C1 + C2) * (1 + Roughness * 0.5);
}

half3 Diffuse_OrenNayar(half VoH, half NoL, half NoV, half3 DiffuseColor, half Roughness)
{
	return Diffuse_OrenNayar_NoPi(VoH, NoL, NoV, Roughness) * DiffuseColor * Inv_PI;
}

half TransmissionBRDF_Wrap(half3 L, half3 N, half W) {
    return saturate((dot(L, N) + W) / ((1 + W) * (1 + W)));
}

half3 TransmissionBRDF_UE4(half3 L, half3 V, half3 N, half3 H, half3 SSS_Color, half AO, half SSS_Thickness) {
	half InScatter = pow(saturate(dot(L, -V)), 12) * lerp(3, 0.1, SSS_Thickness);
	half NormalContribution = saturate(dot(N, H) * SSS_Thickness + 1 - SSS_Thickness);
	half BackScatter = AO * NormalContribution / (PI * 2);
	return SSS_Color * lerp(BackScatter, 1, InScatter);
}

half3 TransmissionBRDF_Foliage(half3 SSS_Color, half3 L, half3 V, half3 N)
{
	half Wrap = 0.5;
	half NoL = saturate((dot(-N, L) + Wrap) / Square(1 + Wrap));

	half VoL = saturate(dot(V, -L));
	half a = 0.6;
	half a2 = a * a;
	half d = ( VoL * a2 - VoL ) * VoL + 1;	
	half GGX = (a2 / PI) / (d * d);		
	return NoL * GGX * SSS_Color;
}

half3 TransmissionBRDF_Frostbite(half3 L, half3 V, half3 N, half3 SSS_Color, half AO, half SSS_AmbientIntensity, half SSS_Distortion, half SSS_Power, half SSS_Scale, half SSS_Thickness) {
	half3 newLight = L + N * SSS_Distortion;
	half newNoL = pow(saturate(dot(V, -newLight)), SSS_Power) * SSS_Scale;
	half newAtten = (newNoL + (SSS_Color * SSS_AmbientIntensity)) * SSS_Thickness;
	return SSS_Color * newAtten * AO;
}



/////////////////////////////////////////////////////////////////Specular
half D_GGX(half NoH, half Roughness)
{
	Roughness = pow4(Roughness);
	half D = (NoH * Roughness - NoH) * NoH + 1;
	return Roughness / (PI * pow2(D));
}

float D_Beckmann(half NoH, half Roughness)
{
	Roughness = pow4(clamp(Roughness, 0.08, 1));
	NoH = pow2(NoH);
	return exp((NoH - 1) / (Roughness * NoH)) / (PI * Roughness * NoH);
}

float D_AnisotropyGGX(float ToH, float BoH, float NoH, float RoughnessT, float RoughnessB) {
    float D = ToH * ToH / pow2(RoughnessT) + BoH * BoH / pow2(RoughnessB) + pow2(NoH);
    return 1 / (RoughnessT * RoughnessB * pow2(D));
}

float D_InvBlinn(float NoH, float Roughness)
{
	float m2 = pow4(Roughness);
	float A = 4;
	float Cos2h = NoH * NoH;
	float Sin2h = 1 - Cos2h;
	return rcp(PI * (1 + A * m2)) * (1 + A * exp(-Cos2h / m2));
}

float D_InvBeckmann(float NoH, float Roughness)
{
	float m2 = pow4(Roughness);
	float A = 4;
	float Cos2h = NoH * NoH;
	float Sin2h = 1 - Cos2h;
	float Sin4h = Sin2h * Sin2h;
	return rcp(PI * (1 + A*m2) * Sin4h) * (Sin4h + A * exp(-Cos2h / (m2 * Sin2h)));
}

float D_InverseGGX_Ashikhmin(float NoH, float Roughness) {
	float a2 = pow4(Roughness);
	float d = (NoH - a2 * NoH) * NoH + a2;
	return rcp(PI * (1 + 4 * a2)) * (1 + 4 * a2 * a2 / (d * d));
}

float D_InverseGGX_Charlie(float NoH, float Roughness)
{
    float invR = 1 / Roughness;
    float cos2h = pow2(NoH);
    float sin2h = 1 - cos2h;
    return ((2 + invR) * pow(sin2h, invR * 0.5) / 2) * Inv_PI;
}



/////////////////////////////////////////////////////////////////GeomtryVis
float Vis_Neumann(half NoL, half NoV)
{
	return 1 / (4 * max(NoL, NoV));
}

float Vis_Kelemen(float VoH)
{
	return rcp( 4 * VoH * VoH + 1e-5);
}

float Vis_Schlick(half NoL, half NoV, half Roughness)
{
	float k = pow2(Roughness) * 0.5;
	float Vis_SchlickV = NoV * (1 - k) + k;
	float Vis_SchlickL = NoL * (1 - k) + k;
	return 0.25 / (Vis_SchlickV * Vis_SchlickL);
}

half Vis_SmithGGX(half NoL, half NoV, half Roughness)
{
	half a = pow2(Roughness);
	half LambdaL = NoV * (NoL * (1 - a) + a);
	half LambdaV = NoL * (NoV * (1 - a) + a);
	return (0.5 * rcp(LambdaV + LambdaL)) / PI;
}

float Vis_SmithGGXCorrelated(half NoL, half NoV, half Roughness)
{
	float a = pow2(Roughness);
	float LambdaV = NoV * sqrt((-NoL * a + NoL) * NoL + a);
	float LambdaL = NoL * sqrt((-NoV * a + NoV) * NoV + a);
	return (0.5 / (LambdaL + LambdaV)) / PI;
}

float Vis_InverseGGX_Ashikhmin(half NoL, half NoV) {
	return rcp(4 * (NoL + NoV - NoL * NoV));
}

float Vis_InverseGGX_Charlie(half NoL, half NoV, half Roughness)
{
    float lambdaV = NoV < 0.5 ? exp(CharlieL(NoV, Roughness)) : exp(2 * CharlieL(0.5, Roughness) - CharlieL(1 - NoV, Roughness));
    float lambdaL = NoL < 0.5 ? exp(CharlieL(NoL, Roughness)) : exp(2 * CharlieL(0.5, Roughness) - CharlieL(1 - NoL, Roughness));

    return 1 / ((1 + lambdaV + lambdaL) * (4 * NoV * NoL));
}

float Vis_AnisotropyGGX(float ToV, float BoV, float NoV, float ToL, float BoL, float NoL, float RoughnessT, float RoughnessB) {
	RoughnessT = pow2(RoughnessT);
	RoughnessB = pow2(RoughnessB);

	float LambdaV = NoL * sqrt(RoughnessT * pow2(ToV) + RoughnessB * pow2(BoV) + pow2(NoV));
	float LambdaL = NoV * sqrt(RoughnessT * pow2(ToL) + RoughnessB * pow2(BoL) + pow2(NoL));

    return (0.5 / (LambdaV + LambdaL)) / PI;
}

float Vis_Hair(float B, float Theta) {
	return exp(-0.5 * Square(Theta) / (B * B)) / (sqrt(2 * PI) * B);
}

#endif