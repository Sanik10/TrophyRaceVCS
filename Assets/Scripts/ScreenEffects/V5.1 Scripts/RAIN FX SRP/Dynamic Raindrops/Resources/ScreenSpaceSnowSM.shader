// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/ScreenSpaceSnowSM"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{

			Tags{ "LightMode" = "ForwardBase" "PassFlags" = "OnlyDirectional" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			//v0.1a
			sampler2D _DirectionalShadowMask;
			float shadowPower;float shadowPowerA;
			float screenRainPower; float screenBrightness;
			float objectRainPower;

			//v0.1
			sampler2D _SnowBumpTex;
			float2 SnowBumpPowerScale;
			float Shineness;
			float specularPower;
			/*float3 WorldSpaceViewDir(in float4 v)
			{
				return _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v).xyz;
			}*/
			///////OUTLINE
			float OutlineThickness, DepthSensitivity, NormalsSensitivity, ColorSensitivity;
			float4 OutlineColor, OutlineControls;
			float doOutline;
			///////END OUTLINE

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir: TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				return o;
			}



			sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTextureA;

			//v0.1
			sampler2D _CameraDepthTexture;

			float4x4 _CamToWorld;

			sampler2D _SnowTex;
			float _SnowTexScale;

			half4 _SnowColor;

			fixed _BottomThreshold;
			fixed _TopThreshold;


			////////////////////////////////// OUTLINE
			float4 _MainTex_TexelSize;
			void Outline_float(half3 normal, float4 snow, float2 UV, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity, float ColorSensitivity, float4 OutlineColor, float4 OutlineControls, out float4 Out)
			{
				float halfScaleFloor = floor(OutlineThickness * 0.5);
				float halfScaleCeil = ceil(OutlineThickness * 0.5);
				//float2 Texel = (1.0) / float2(_CameraColorTexture_TexelSize.z, _CameraColorTexture_TexelSize.w);
				float2 Texel = (1.0) / float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);

				float2 uvSamples[4];
				float depthSamples[4];
				float3 normalSamples[4], colorSamples[4];

				uvSamples[0] = UV - float2(Texel.x, Texel.y) * halfScaleFloor;
				uvSamples[1] = UV + float2(Texel.x, Texel.y) * halfScaleCeil;
				uvSamples[2] = UV + float2(Texel.x * halfScaleCeil, -Texel.y * halfScaleFloor);
				uvSamples[3] = UV + float2(-Texel.x * halfScaleFloor, Texel.y * halfScaleCeil);

				for (int i = 0; i < 4; i++)
				{
					/*depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
					normalSamples[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
					colorSamples[i] = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uvSamples[i]);*/
					depthSamples[i] = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvSamples[i]).r;
					normalSamples[i] = UnpackNormal(tex2D(_CameraDepthNormalsTextureA, uvSamples[i])) + normal;
					colorSamples[i] = tex2D(_MainTex, uvSamples[i]);
				}

				// Depth
				float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
				float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
				float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
				float depthThreshold = (1 / DepthSensitivity) * depthSamples[0];
				edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

				// Normals
				float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
				float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
				float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
				edgeNormal = edgeNormal > (1 / NormalsSensitivity) ? 1 : 0;

				// Color
				float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
				float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
				float edgeColor = sqrt(dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1));
				edgeColor = edgeColor > (1 / ColorSensitivity) ? 1 : 0;

				float edge = max(edgeDepth, max(edgeNormal, edgeColor));

				float4 original = tex2D(_MainTex, uvSamples[0]); //snow;// tex2D(_MainTex, uvSamples[0]);
				Out = (1 - edge) * original + edge * lerp(original, OutlineColor, OutlineColor.a);
				//Out = ((1 - edge) * original) + (edge * lerp(original, OutlineColor, OutlineColor.a));
			}
			////////////////////////////////// END OUTLINE //////////////////////////////////


			////////////////////////////////// RAIN START //////////////////////////////////
			//RAIN HEADERS
			#define ftime (_Time.y)
			//#define _LightDir (normalize(_WorldSpaceLightPos0.xyz))	
			#include "ShaderLibs/Noise.cginc"
			#include "ShaderLibs/FBM.cginc"

			float4 interactPointRadius;
			float4 radialControls;
			float4 directionControls;
			float4 wipeControls;
			//MASKED
			float4 mainTexTilingOffset;
			float maskPower;
			//TUTORIAL STAGE 1
			float _Size;
			float _Distortion;
			float _Blur;
			//v0.6
			float4 _TimeOffset;
			float4 _EraseCenterRadius;
			float erasePower;
			//v0.3
			float _TileNumCausticRotMin;
			float4 _RainSmallDirection; 

			float CausticRotateMin(float2 uv, float time) {
				float3x3 mat = float3x3(2, 1, -2, 3, -2, 1, 1, 2, 2);
				float3 vec1 = mul(mat*0.5, float3(uv, time));
				float3 vec2 = mul(mat*0.4, vec1);
				float3 vec3 = mul(mat*0.3, vec2);
				float val = min(length(frac(vec1) - 0.5), length(frac(vec2) - 0.5));
				val = min(val, length(frac(vec3) - 0.5));
				val = pow(val, 7.0)*25.;
				return val;
			}

			float3 CausticTriTwist(float2 uv, float time)
			{
				const int MAX_ITER = 5;
				float2 p = fmod(uv*PI2, PI2) - 250.0;
				float2 i = float2(p);
				float c = 1.0;
				float inten = .005;

				for (int n = 0; n < MAX_ITER; n++)
				{
					float t = time * (1.0 - (3.5 / float(n + 1)));
					i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
					c += 1.0 / length(float2(p.x / (sin(i.x + t) / inten), p.y / (cos(i.y + t) / inten)));
				}
				c /= float(MAX_ITER);
				c = 1.17 - pow(c, 1.4);
				float val = pow(abs(c), 8.0);
				return val;
			}

			float CausticVoronoi(float2 p, float time) {
				float v = 0.0;
				float a = 0.4;
				for (int i = 0; i < 3; i++) {
					v += WNoise(p, time)*a;
					p *= 2.0;
					a *= 0.5;
				}
				v = pow(v, 2.)*5.;
				return v;
			}

			float3 Stars(in float3 rd, float den, float tileNum)
			{
				float3 c = float3(0., 0., 0.);
				float3 p = rd;
				float SIZE = 0.5;
				for (float i = 0.; i < 3.; i++)
				{
					float3 q = frac(p*tileNum) - 0.5;
					float3 id = floor(p*tileNum);
					float2 rn = Hash33(id).xy;

					float size = (Hash13(id)*0.2 + 0.8)*SIZE;
					float demp = pow(1. - size / SIZE, .8)*0.45;
					float val = (sin(_Time.y*31.*size)*demp + 1. - demp) * size;
					float c2 = 1. - smoothstep(0., val, length(q));
					c2 *= step(rn.x, (.0005 + i * i*0.001)*den);
					c += c2 * (lerp(float3(1.0, 0.49, 0.1), float3(0.75, 0.9, 1.), rn.y)*0.25 + 0.75);
					p *= 1.4;
				}
				return c * c*.7;
			}

			float TimeFBM(float2 p, float t)
			{
				float2 f = 0.0;
				float s = 0.5;
				float sum = 0;
				for (int i = 0; i < 5; i++) {
					p += t; t *= 1.5;
					f += s * tex2D(_NoiseTex, p / 256).x; p = mul(float2x2(0.8, -0.6, 0.6, 0.8), p)*2.02;
					sum += s; s *= 0.6;
				}
				return f / sum;
			}

			float3 Cloud(float3 bgCol, float3 ro, float3 rd, float3 cloudCol, float spd, float layer)
			{
				float3 col = bgCol;
				float time = _Time.y*0.05*spd;
				for (int i = 0; i < layer; i++) {
					float2 sc = ro.xz + rd.xz*((i + 3)*40000.0 - ro.y) / rd.y;
					col = lerp(col, cloudCol, 0.5*smoothstep(0.5, 0.8, TimeFBM(0.00002*sc, time*(i + 3))));
				}
				return col;
			}

			float3 Fog(in float3 bgCol, in float3 ro, in float3 rd, in float maxT,
				float3 fogCol, float3 spd, float2 heightRange)
			{
				float d = .4;
				float3 col = bgCol;
				for (int i = 0; i < 7; i++)
				{
					float3  p = ro + rd * d;
					// add some movement at some dir
					p += spd * ftime;
					p.z += sin(p.x*.5);
					// get height desity 
					float hDen = (1. - smoothstep(heightRange.x, heightRange.y, p.y));
					// get final  density
					float den = TNoise(p*2.2 / (d + 20.), ftime, 0.2)* hDen;
					float3 col2 = fogCol * (den *0.5 + 0.5);
					col = lerp(col, col2, clamp(den*smoothstep(d - 0.4, d + 2. + d * .75, maxT), 0., 1.));
					d *= 1.5 + 0.3;
					if (d > maxT)break;
				}
				return col;
			}

			float3 Sky(float3 ro, float3 rd, float3 lightDir) {
				float3 col = float3(0.0, 0.0, 0.0);
				float sundot = clamp(dot(rd, lightDir), 0.0, 1.0);

				// sky      
				col = float3(0.2, 0.5, 0.85)*1.1 - rd.y*rd.y*0.5;
				col = lerp(col, 0.85*float3(0.7, 0.75, 0.85), pow(1.0 - max(rd.y, 0.0), 4.0));
				// sun
				col += 0.25*float3(1.0, 0.7, 0.4)*pow(sundot, 5.0);
				col += 0.25*float3(1.0, 0.8, 0.6)*pow(sundot, 64.0);
				col += 0.4*float3(1.0, 0.8, 0.6)*pow(sundot, 512.0);
				// clouds
				col = Cloud(col, ro, rd, float3(1.0, 0.95, 1.0), 1, 1);
				// . 
				col = lerp(col, 0.68*float3(0.4, 0.65, 1.0), pow(1.0 - max(rd.y, 0.0), 16.0));
				return col;
			}

			// http://iquilezles.org/www/articles/checkerfiltering/checkerfiltering.htm
			float CheckersGradBox(in float2 p)
			{
				// filter kernel
				float2 w = fwidth(p) + 0.001;
				// analytical integral (box filter)
				float2 i = 2.0*(abs(frac((p - 0.5*w)*0.5) - 0.5) - abs(frac((p + 0.5*w)*0.5) - 0.5)) / w;
				// xor pattern
				return 0.5 - 0.5*i.x*i.y;
			}

			float _Ripple(float period, float spreadSpd, float waveGap, float2 uv, float rnd) {
				// sample the texture
				const float WAVE_NUM = 2.;
				const float  CROSS_NUM = 1.0;
				float ww = -WAVE_NUM * .5 * waveGap;
				float hww = ww * 0.5;
				float freq = WAVE_NUM * PI2 / waveGap / (CROSS_NUM + 1.);
				float radius = (float(CROSS_NUM));
				float2 p0 = floor(uv);
				float sum = 0.;

				for (float j = -CROSS_NUM; j <= CROSS_NUM; ++j) {
					for (float i = -CROSS_NUM; i <= CROSS_NUM; ++i) {
						float2 pi = p0 + float2(i, j);
						float2 h22 = Hash23(float3(pi, rnd));
						float h12 = Hash13(float3(pi, rnd));
						float pd = period * (h12 * 1. + 1.);
						float time = ftime + pd * h12;
						float t = fmod(time, pd);
						float spd = spreadSpd * ((1.0 - h12) * 0.2 + 0.8);
						float size = (h12)*0.4 + 0.6;
						float maxt = min(pd*0.6, radius *size / spd);
						float amp = clamp01(1. - t / maxt);
						float2 p = pi + Hash21(h12 + floor(time / pd)) * 0.4;
						float d = (length(p - uv) - spd * t) / radius * 0.5;
						sum -= amp * sin(freq*d) *  smoothstep(ww*size, hww*size, d) *  smoothstep(0., hww*size, d);
					}
				}
				sum /= (CROSS_NUM * 2 + 1)*(CROSS_NUM * 2 + 1);
				return sum;
			}

			float Ripples(float2 uv, float layerNum, float tileNum, float period, float spreadSpd, float waveGap) {
				float sum = 0.;
				for (int i = 0; i < layerNum; i++) {
					sum += _Ripple(period, spreadSpd, waveGap, uv*(1. + i / layerNum) * tileNum, float(i));
				}
				return sum;
			}

			#define Caustic CausticRotateMin

			//RAIN START
			#define SS(a,b,t) smoothstep(a,b,t)
			float2 Rains(float2 uv, float seed, float m, float4 _RainSmallDirection, float4 _TimeOffset) {
				float uvY = (uv.y - _RainSmallDirection.w);
				uv.x = uv.x + _RainSmallDirection.x * 0.5*uvY + _RainSmallDirection.y * 0.6 * uvY * uvY + _RainSmallDirection.z * 0.2 * uvY * uvY * uv.x;

				float period = 5;
				float2 retVal = float2(0.0, 0.0);
				float aspectRatio = 4.0;
				float tileNum = 5;
				float ySpd = 0.1;
				uv.y += ftime * 0.0618* _TimeOffset.x + _TimeOffset.y;
				uv *= float2(tileNum * aspectRatio, tileNum);
				float idRand = Hash12(floor(uv));
				uv = frac(uv);
				float2 gridUV = uv;
				uv -= 0.5;
				float t = (ftime* _TimeOffset.x + _TimeOffset.y) * PI2 / period;
				t += idRand * PI2;
				uv.y += sin(t + sin(t + sin(t)*0.55))*0.45;
				uv.y *= aspectRatio;

				//		uv.x -= 4*sin(t + sin(t + sin(t)*0.55))*0.45;
				//		uv.x *= aspectRatio;

				uv.x += (idRand - .5)*0.6;//0.6
				float r = length(uv);
				r = smoothstep(0.2, 0.1, r);
				float tailTileNum = 3.0;
				float2 tailUV = uv * float2(1.0, tailTileNum);
				tailUV.y = frac(tailUV.y) - 0.5;
				tailUV.x *= tailTileNum;
				float rtail = length(tailUV);
				rtail *= uv.y * 1.5;
				rtail = smoothstep(0.2, 0.1, rtail);
				rtail *= smoothstep(0.3, 0.5, uv.y);
				retVal = float2(rtail*tailUV + r * uv);
				return retVal;
			}
			//https://www.shadertoy.com/view/MlSBzh
			float noise(float t) { return frac(sin(t*100.0)*1000.0); } //fract(sin(t*100.0)*1000.0); }
			float noise2(float2 p) { return noise(p.x + noise(p.y)); }

			float raindot(float2 uv, float2 id, float t) {
				float2 p = 0.1 + 0.8 * float2(noise2(id), noise2(id + float2(1.0, 0.0)));
				float r = clamp(0.5 - fmod(t + noise2(id), 1.0), 0.0, 1.0);
				return smoothstep(0.3 * r, 0.0, length(p - uv));
			}

			float trailDrop(float2 uv, float2 id, float t, float size) {
				float f = size * clamp(noise2(id) - 0.5, 0.0, 1.0);
				// wobbly path
				float wobble = 0.5 + 0.2
					* cos(12.0 * uv.y)
					* sin(50.0 * uv.y);
				float v = 1.0 - 300.0 / f * pow(uv.x - 0.5 + 0.2 * wobble, 2.0);
				// head
				v *= clamp(30.0 * uv.y, 0.0, 1.0);
				v *= clamp(uv.y + 7.0 * t - 0.6, 0.0, 1.0);
				// tail
				v *= clamp(1.0 - uv.y - pow(t, 2.0), 0.0, 1.0);
				//v0.6
				v *= clamp(0.82 - uv.y - pow(t, 1.2), 0.4, 1.0);

				return f * clamp(v * 10.0, 0.0, 1.0);
			}

			float N21(float2 p) {
				p = frac(p*float2(123.34, 345.45));
				p += dot(p, p + 34.345);
				return frac(p.x * p.y);
			}

			float4 dropsLayer(float2 UV, float time, float _Size) {

				float2 aspect = float2(2, 1);
				float2 uvs = UV * _Size * aspect;

				//move only downwards
				uvs.y += time * 0.25;

				float2 gv = frac(uvs) - 0.5;
				float2 id = floor(uvs);

				float n = N21(id);
				time += n * 6.2831;
				float w = UV.y * 10;

				float x = (n - 0.5)*0.8;
				x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6)*0.45;

				float y = -sin(time + sin(time + sin(time)*0.5))*0.45;
				y -= (gv.x - x) * (gv.x - x);

				//move drops
				float2 dropPosition = (gv - float2(x, y)) / aspect;
				float drop = SS(0.05, 0.03, length(dropPosition));

				//TRAILS
				float2 trailPosition = (gv - float2(x, time * 0.25)) / aspect;
				trailPosition.y = (frac(trailPosition.y * 8) - 0.5) / 8;
				float trail = SS(0.03, 0.01, length(trailPosition));
				float fogTrail = SS(-0.05, 0.05, dropPosition.y);
				fogTrail *= SS(0.5, y, gv.y);
				trail *= fogTrail;
				fogTrail *= SS(0.05, 0.04, abs(dropPosition.x));


				float2 offset = drop * dropPosition + trail * trailPosition - fogTrail * 0.005 *n;
				return float4(offset, fogTrail, n * dropPosition.x);
			}
			float4 rain(//float2 uv, float2 uv2, float4 grabUV, float3 worldPos){
				float3 worldPos,
				float2 uv,
				float2 uv2,
				float4 grabUV,
				float4 interactPointRadius,
				float4 radialControls,
				float4 directionControls,
				float4 wipeControls,
				//MASKED
				float4 mainTexTilingOffset,
				float maskPower,
				//TUTORIAL STAGE 1
				float4 _MainTex_ST,
				float _Size,
				float _Distortion,
				float _Blur,
				//v0.6
				float4 _TimeOffset,
				float4 _EraseCenterRadius,
				float erasePower,
				//v0.3
				float _TileNumCausticRotMin,
				float4 _RainSmallDirection
			) {

				float4 colSTART = 0;

				//v0.8
				//uv.x += _WorldSpaceCameraPos.x;
				//uv.y += _WorldSpaceCameraPos.z;

				//v0.6
				float eraser = 1;
				float distErase = sqrt((uv.x - _EraseCenterRadius.x)*(uv.x - _EraseCenterRadius.x) + (uv.y - _EraseCenterRadius.y)*(uv.y - _EraseCenterRadius.y));
				float fadeErase = 1 - saturate(pow((distErase - _EraseCenterRadius.z), erasePower) / _EraseCenterRadius.w);
				if (distErase > _EraseCenterRadius.z) {
					eraser = fadeErase;
				}

				//v0.7 - wiper
				float wipeFactor = 1;
				float wipeDirection = 0;
				float2 direction = float2(abs(cos(2.1*_Time.y)), abs(sin(2.1 * _Time.y)));
				float2 currentPoint = float2(uv.x + wipeControls.x, uv.y - 0.5 + wipeControls.y);
				if (wipeControls.z == 0 || abs(dot(direction, currentPoint)) > 0.75f *(1 - fadeErase)*fadeErase * wipeControls.w) {
					eraser = fadeErase;
				}
				else {
					eraser = 0;
					wipeFactor = 0;
					wipeDirection = dot(direction, currentPoint);
				}
				//v0.6
				float2 center = _TimeOffset.zw;
				float dist = sqrt((uv.x - center.x)*(uv.x - center.x) + (uv.y - center.y)*(uv.y - center.y));



				//v0.7
				float dist3D = sqrt(
					(interactPointRadius.x - worldPos.x)*(interactPointRadius.x - worldPos.x)
					+ (interactPointRadius.y - worldPos.y)*(interactPointRadius.y - worldPos.y)
					+ (interactPointRadius.z - worldPos.z)*(interactPointRadius.z - worldPos.z));
				uv.y = 1 - uv.y;
				if (uv.x > 0.5) {
					//directionControls
					uv.x = uv.x - (uv.x*directionControls.y)*uv.y*directionControls.x;
					if (directionControls.x == 0) {
						uv.x = 1 + radialControls.z * uv.x - radialControls.x * 2 * pow(uv.y, radialControls.y * 4);
					}
					else {
						uv.x = 1 + radialControls.z * uv.x + radialControls.x * 2 * pow(uv.y, radialControls.y * 4);
						uv.x = uv.x + uv.x*uv.y*directionControls.x;
					}
				}
				else {
					uv.x = 1 + radialControls.z * uv.x + radialControls.x * 2 * pow(uv.y, radialControls.y * 4);
					uv.x = uv.x + uv.x*uv.y*directionControls.x;
				}

				float iTime = _Time.y  * _TimeOffset.x + _TimeOffset.y;
				float2 uvA = uv.xy;
				float2 uv1 = float2(uvA.x * 30.0, uvA.y * 4.3 + noise(floor(uvA.x * 20.0)));
				float2 uvi = floor(float2(uv1.x, uv1.y));
				float2 uvf = (uv1 - uvi);
				float v = trailDrop(uvf, uvi, fmod(iTime + noise(floor(uvA.x * 20.0)), 3.0) / 3.0, 12);
				float v2 = trailDrop(uvf + float2(0.056, 0), uvi + float2(0.0156, 0), fmod(iTime * 2 + noise(floor(uvA.x * 10.0)), 11.0) / 3.0, 1);
				v += raindot(frac(uvA * 20.0 + float2(0, 0.1 * iTime)), floor(uvA * 20.0 + float2(0, 0.1 * iTime)), iTime);

				//TUTORIAL STAGE 1
				float time = fmod(_Time.y * _TimeOffset.x + _TimeOffset.y, 7200); //_Time.y;
				float4 col = 0;

				//v0.7
				if (wipeFactor == 0) {
					uv.x = uv.x - 0.5*sign(wipeDirection)* uv.y;// +2 * i.uv.y / 3;
				}

				float4 drops = dropsLayer(uv, time, _Size);
				drops += dropsLayer(uv * 1.25 + 7.45, time, _Size);
				drops += dropsLayer(uv * 1.36 + 1.5, time * 1.2, _Size);
				drops += dropsLayer(uv * 1.57 - 7.45, time * 0.8, _Size);


				//v0.3 - caustic Rot min ------------------------------------ Add extra noise to trails
				float2 uvCaustA = _TileNumCausticRotMin * uv * 15;
				float timeCaustA = _Time.y * _TimeOffset.x + _TimeOffset.y;
				float val = CausticVoronoi(uvCaustA, timeCaustA); //CausticTriTwist //CausticVoronoi
				//_Distortion = _Distortion + 100*val;
				drops *= float4(val, val, val, 1) + float4(val, val, val, 1) * 2;

				//v0.4 - SMALL RAIN
				float baseOffset = 0.1;
				float2 uv4 = uv;
				//uv4 *= float2(_ScreenParams.x / _ScreenParams.y, 1.0);
				uv4 *= 1.5;
				float x4 = (sin((_Time.y)*.1)*.5 + .5)*.3;
				x4 = x4 * x4;
				x4 += baseOffset;
				float s = sin(x4);
				float c = cos(x4);
				float2x2 rot = float2x2(c, -s, s, c);
				uv4 = mul(rot, uv4);
				float moveSpd = 0.1;
				float2 rainUV = float2(0., 0.);
				rainUV += Rains(uv4, 152.12, moveSpd, _RainSmallDirection, _TimeOffset);
				rainUV += Rains(uv4*2.32, 25.23, moveSpd, _RainSmallDirection, _TimeOffset);
				//fixed4 finalColor = tex2D(_MainTex, i.uv + rainUV * 2.);
				drops.xy += rainUV * 0.35;

				//v0.7
				//projUV *= wipeFactor + (1- wipeFactor)*i.uv;
				//v *= wipeFactor;
				if (wipeFactor == 0) {
					//v.x = 0;
					_Distortion = 0.1;
					//rainUV.x = rainUV.x+15;
				}
				drops = drops * wipeFactor + (1 - wipeFactor)*  drops * drops * 1000;// drops * drops*0.2f + drops* wipeFactor;

				//LONG RAIN
				float2 finalUVs = uv * _MainTex_ST.xy + _MainTex_ST.zw + drops.xy * _Distortion;//offset * _Distortion;
				finalUVs = float2(finalUVs + 0.05*(float2(ddx(v), ddy(v)) + float2(ddx(v2), ddy(v2)))); //ADD PREVIOUS RAIN SYSTEM - SMALL DROPS
				finalUVs += 0.5*(float2(ddx(v*0.4), ddy(v * 0.4)) + float2(ddx(v2 * 0.4), ddy(v2 * 0.4))) * drops.w;// n * dropPosition;

				float blur = _Blur * 7 * (1 - drops.z);
				col = colSTART;
				//v0.5
				float fade = 1 - saturate(fwidth(uv) * 50);
				float2 projUV = grabUV.xy / grabUV.w;
				projUV += drops.xy * _Distortion * fade;

				//LONG RAIN
				float halfShift = fwidth(v) / 2;
				float lowEdge = 0.5 - halfShift;
				float upEdge = 0.5 + halfShift;
				float stepAA = (v - lowEdge) / (upEdge - lowEdge);
				stepAA = saturate(stepAA);
				projUV = float2(projUV + 0.012*(float2(abs(ddy(v)) - 0.05 * v, abs(ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.012*(float2((ddy(v)) - 0.05 * v, (ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.012*(float2(abs(ddy(v)) - 0.05 * v, (ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.002*(float2((ddy(v)) - 0.05 * v, abs(ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.002*(float2(((v)) - 0.05 * v, ((v)) - 0.05*v)));
				projUV = float2(projUV + 0.003*(float2((ddy(v)) - 0.05 * v, ((v)) - 0.05*v)));
				blur *= 0.01;
				const float numSamples = 32;
				float a = N21(uv) * 6.2831;
				for (float j = 0; j < numSamples; j++) {
					float2 offs = float2(sin(a), cos(a))*blur;
					float  d = frac(sin((j + 1)*546.0)*5424.0);
					d = sqrt(d);
					offs *= d;
					//col += tex2D(_GrabTexture, projUV + offs * eraser);
					col += tex2D(_MainTex, projUV + offs * eraser);//SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, projUV + offs * eraser);
					a++;
				}
				col /= numSamples;
				//col += tex2D(_MainTex, uv2 * mainTexTilingOffset.xy + mainTexTilingOffset.zw + 0) * maskPower;
				return col;
			}
			//END RAIN
			////////////////////////////////// END RAIN //////////////////////////////////

			///////////////////////////////// RAIN DROPS PUDDLES //////////////////////////
			//RAINDROPS
			sampler2D _Lux_RainRipples;				
			float _Lux_RainIntensity;//in AddWaterFlowRipples and main	
			float _Lux_RippleAnimSpeed;//in AddWaterFlowRipples 
			float _Lux_RippleTiling;//in AddWaterFlowRipples 
			float _Lux_WaterBumpDistance;
			float2 ComputeRipple(float4x4 _CamToWorld, sampler2D _Lux_RainRipples, float4 UV, float CurrentTime, float Weight)
			{
				//float4 Ripple = tex2Dlod(_Lux_RainRipples, UV);
				//// We use multi sampling here in order to improve Sharpness due to the lack of Anisotropic Filtering when using tex2Dlod
				//Ripple += tex2Dlod(_Lux_RainRipples, float4(UV.xy, UV.zw * 0.5));

				//SAMPLE_TEXTURE2D(_SnowTex, sampler_SnowTex, wpos.xz* _SnowTexScale)
				//float4 Ripple = SAMPLE_TEXTURE2D(_Lux_RainRipples, sampler_Lux_RainRipples, UV);
				float4 Ripple = tex2D(_Lux_RainRipples, UV);
				// We use multi sampling here in order to improve Sharpness due to the lack of Anisotropic Filtering when using tex2Dlod
				//Ripple += SAMPLE_TEXTURE2D(_Lux_RainRipples, sampler_Lux_RainRipples, float4(UV.xy, UV.zw * 0.5));
				Ripple += tex2D(_Lux_RainRipples, float4(UV.xy, UV.zw * 0.5));
				Ripple *= 0.5;

				Ripple.yz = Ripple.yz * 2 - 1; // Decompress Normal
				float DropFrac = frac(Ripple.w + CurrentTime); // Apply time shift
				float TimeFrac = DropFrac - 1.0f + Ripple.x;
				float DropFactor = saturate(0.2f + Weight * 0.8f - DropFrac);
				float FinalFactor = DropFactor * Ripple.x * sin(clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.141592653589793);
				return Ripple.yz * FinalFactor * 0.35f;
			}
			//  Add Water Ripples to Waterflow
			float3 AddWaterFlowRipples(float4x4 _CamToWorld, sampler2D _Lux_RainRipples, float2 i_wetFactor, float3 i_worldPos, float2 lambda, float i_worldNormalFaceY, float fadeOutWaterBumps,
				float _Lux_RainIntensity,
				float _Lux_RippleAnimSpeed,
				float _Lux_RippleTiling
			)
			{
				float4 Weights = _Lux_RainIntensity - float4(0, 0.25, 0.5, 0.75);
				Weights = saturate(Weights * 4);
				float animSpeed = _Time.y * _Lux_RippleAnimSpeed;
				//float2 Ripple1 = ComputeRipple(sampler_Lux_RainRipples, _CamToWorld, _Lux_RainRipples, float4(i_worldPos.xz * _Lux_RippleTiling + float2(0.25f, 0.0f), lambda), animSpeed, Weights.x);
				//float2 Ripple2 = ComputeRipple(sampler_Lux_RainRipples, _CamToWorld, _Lux_RainRipples, float4(i_worldPos.xz * _Lux_RippleTiling + float2(-0.55f, 0.3f), lambda), animSpeed * 0.71, Weights.y);
				
				float2 Ripple1 = ComputeRipple(_CamToWorld, _Lux_RainRipples, float4(i_worldPos.xz * _Lux_RippleTiling + float2(0.25f, 0.0f), lambda), animSpeed, Weights.x);
				float2 Ripple2 = ComputeRipple(_CamToWorld, _Lux_RainRipples, float4(i_worldPos.xz * _Lux_RippleTiling + float2(-0.55f, 0.3f), lambda), animSpeed * 0.71, Weights.y);

				
				float3 rippleNormal = float3(Weights.x * Ripple1.xy + Weights.y * Ripple2.xy, 1);
				// Blend and fade out Ripples 
				return lerp(float3(0, 0, 1), rippleNormal, i_wetFactor.y * i_wetFactor.y * fadeOutWaterBumps * i_worldNormalFaceY*i_worldNormalFaceY);
			}
			//END v4.7 Rain ripples
			//void RainDrops_float(
			//	float2 uv,
			//	float4x4 _CamToWorld,
			//	sampler2D _Lux_RainRipples, //UnityTexture2D _Lux_RainRipples,
			//	//UnitySamplerState sampler_Lux_RainRipples,
			//	float _Lux_RainIntensity,//in AddWaterFlowRipples and main	
			//	float _Lux_RippleAnimSpeed,//in AddWaterFlowRipples 
			//	float _Lux_RippleTiling,//in AddWaterFlowRipples 
			//	float _Lux_WaterBumpDistance,
			//	out float3 Out
			//)
			//{
			//	float3 normal = 0;
			//	float depth = 0;
			//	DecodeDepthNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uv), depth, normal);
			//	normal = mul((float3x3)_CamToWorld, normal);
			//	depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
			//	depth = Linear01Depth(depth);
			//	_CamToWorld[0][3] = 0;
			//	_CamToWorld[1][3] = 0;
			//	_CamToWorld[2][3] = 0;
			//	_CamToWorld[3][3] = 0;
			//	// find out snow color
			//	float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
			//	float3 vpos = float3((uv * 2.0 - 1.0) / p11_22, -1.0) * depth;
			//	float4 wpos = mul(_CamToWorld, float4(vpos, 1));
			//	wpos += float4(_WorldSpaceCameraPos, 0) / _ProjectionParams.z;
			//	wpos *= _SnowTexScale * _ProjectionParams.z;


			//	//v4.7 Rain ripples
			//	// Add Water Ripples
			//	float3 rippleNormal = float3(0, 0, 1);
			//	float2 wetFactor = float2(0.1, 0.1);
			//	float2 lambda = float2(1, 1);
			//	float fadeOutWaterBumps = saturate((_Lux_WaterBumpDistance - distance(_WorldSpaceCameraPos, wpos)) / 5);
			//	if (_Lux_RainIntensity > 0) {
			//		rippleNormal = AddWaterFlowRipples(sampler_Lux_RainRipples, _CamToWorld, _Lux_RainRipples, wetFactor, wpos, lambda, 1, fadeOutWaterBumps, _Lux_RainIntensity, _Lux_RippleAnimSpeed, _Lux_RippleTiling);
			//	}
			//	//worldNormal.xyz = worldNormal.xyz + 0.2*saturate(fadeOutWaterBumps*_Lux_RainIntensity*0.5*saturate(1110.5 * rippleNormal.xzy));
			//	float3 worldNormal = 0.2*saturate(fadeOutWaterBumps*_Lux_RainIntensity*0.5*saturate(1110.5 * rippleNormal.xzy));
			//	float dotToVertical = dot(normal, float3(0, 1, 0));
			//	if (dotToVertical > 0.15) {
			//		Out = worldNormal;
			//	}
			//	else {
			//		Out = 0;
			//	}
			//}
			///////////////////////////////// END RAIN DROPS PUDDLES //////////////////////


			float4 _MainTex_ST;


			half4 frag (v2f i) : SV_Target
			{
				half3 normal; half3 normal2;
				float depth;
				float depth2;

				DecodeDepthNormal(tex2D(_CameraDepthNormalsTextureA, i.uv), depth2, normal2);
				normal = mul((float3x3)_CamToWorld, normal2);// +0.9*normal.y;

				//return float4(normal, 1);

				//v0.1
				float2 uv = i.uv.xy / i.uv.w;
				depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);// *length(normal);
				float linearDepth = Linear01Depth(depth);
				depth = linearDepth;
				_CamToWorld[0][3] = 0;
				_CamToWorld[1][3] = 0;
				_CamToWorld[2][3] = 0;
				_CamToWorld[3][3] = 0;


				// find out snow amount
				half snowAmount = normal.g;// *22 + 11111110.1;
				half scale = (_BottomThreshold + 1 - _TopThreshold) / 1 + 1;
				snowAmount = saturate((snowAmount - _BottomThreshold) * scale);


				// find out snow color
				float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
		        float3 vpos = float3((i.uv * 2 - 1) / p11_22, -1) * depth ;
		        float4 wpos = mul(_CamToWorld, float4(vpos, 1));
		        wpos += float4(_WorldSpaceCameraPos, 0) / _ProjectionParams.z;
				wpos *= 1 * _ProjectionParams.z;//_SnowTexScale * _ProjectionParams.z;//v6.1c
				
				//v0.1 - https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
				float3 LightPos = _WorldSpaceLightPos0;
				float3 lightColor = _LightColor0;
				float3 viewDirection = wpos - _WorldSpaceCameraPos; //i.viewDir;
				viewDirection = UNITY_MATRIX_IT_MV[2].xyz;



				//RIPPLES PUDDLES
					//v4.7 Rain ripples
				// Add Water Ripples
				float3 dropsFINAL = 0;
				float3 rippleNormal = float3(0, 0, 1);
				float2 wetFactor = float2(0.1, 0.1);
				float2 lambda = float2(1, 1);
				float fadeOutWaterBumps = saturate((_Lux_WaterBumpDistance - distance(_WorldSpaceCameraPos, wpos)) / 5);
				if (_Lux_RainIntensity > 0) {
					rippleNormal = AddWaterFlowRipples(_CamToWorld, _Lux_RainRipples, wetFactor, wpos, lambda, 1, fadeOutWaterBumps, 
						_Lux_RainIntensity, _Lux_RippleAnimSpeed, _Lux_RippleTiling);
				}
				//worldNormal.xyz = worldNormal.xyz + 0.2*saturate(fadeOutWaterBumps*_Lux_RainIntensity*0.5*saturate(1110.5 * rippleNormal.xzy));
				float3 worldNormal = 0.2*saturate(fadeOutWaterBumps*_Lux_RainIntensity*0.5*saturate(1110.5 * rippleNormal.xzy));
				float dotToVertical = dot(normal, float3(0, 1, 0));
				if (dotToVertical > 0.15) {
					dropsFINAL = worldNormal;
				}
				else {
					dropsFINAL = 0;
				}



				float3 snowBump = UnpackNormal(tex2D(_SnowBumpTex, wpos.xz* _SnowTexScale* 1)) * 5 * SnowBumpPowerScale.y;////v6.1c  DecodeNormal(tex2D(_SnowBumpTex,  wpos.xz* _SnowTexScale*SnowBumpPowerScale.y)) * 5;
				snowBump = mul((float3x3)_CamToWorld, snowBump)* SnowBumpPowerScale.x;
				normal = normal + snowBump + dropsFINAL* _Lux_RainIntensity;
				float3 diffuse = max(0.0, dot(normal, LightPos))*lightColor*Shineness*0.2;
				//https://en.wikibooks.org/wiki/Cg_Programming/Unity/Lighting_of_Bumpy_Surfaces
				//https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
				float3 specularReflection;
				if (dot(normal, LightPos) < 0.0)
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = specularPower * lightColor * pow(max(0.0, dot(reflect(-LightPos, normal),viewDirection)), Shineness);
				}


				
				half4 col = tex2D(_MainTex, i.uv+float2(0,+0.00075));//GIVE SMALL OFFSET TO AVOID BORDERS OVER SNOW
		        half4 snowColor = tex2D(_SnowTex, wpos.xz * _SnowTexScale * 1) * _SnowColor;//v6.1c removed multiple by projection matrix
				//snowColor.rgb += specularReflection + diffuse;
				if (depth > 0.999) {
					specularReflection = diffuse = snowAmount = 0;
				}
				snowColor.rgb += diffuse*0.01 + specularReflection*(col.a)*0.00015;//v0.1a

				//SNOW
				float4 snow = lerp(col, snowColor, snowAmount);

				//ADD OUTLINE
				float4 outline=float4(0,0,0,0);
				if (doOutline) {
					float4 OutOUTLINE;
					/*Outline_float(normal, snow, i.uv, OutlineThickness, DepthSensitivity, NormalsSensitivity,
						ColorSensitivity, OutlineColor, OutlineControls, OutOUTLINE);*/
					Outline_float(normal, col, i.uv, OutlineThickness, DepthSensitivity, NormalsSensitivity,
						ColorSensitivity, OutlineColor, OutlineControls, OutOUTLINE);
					//return OutOUTLINE;
					outline = OutOUTLINE;
				}
				else {
					outline = snow;
				}

				//RAIN
				float4 rainA = rain(
					wpos,// worldPos,
					i.uv, //float2(wpos.x, wpos.z),
					i.uv, //float2(wpos.x, wpos.z),//2,
					i.uv,//grabUV,
					interactPointRadius,
					radialControls,
					directionControls,
					wipeControls,
					mainTexTilingOffset,
					maskPower,
					_MainTex_ST,
					_Size,
					_Distortion,
					_Blur,
					_TimeOffset,
					_EraseCenterRadius,
					erasePower,
					_TileNumCausticRotMin,
					_RainSmallDirection
				);
				float4 rainA3D = rain(
					wpos,// worldPos,
					float2(wpos.x, wpos.z)*0.001,
					float2(wpos.x, wpos.z)*0.001,//2,//v6.1c
					i.uv,//grabUV,
					interactPointRadius,
					radialControls,
					directionControls,
					wipeControls,
					mainTexTilingOffset,
					maskPower,
					_MainTex_ST,
					_Size,
					_Distortion,
					_Blur,
					_TimeOffset,
					_EraseCenterRadius,
					erasePower,
					_TileNumCausticRotMin,
					_RainSmallDirection
				);
				//Out = float4(1, 0, 0, 1);
				dotToVertical = dot( mul((float3x3)_CamToWorld, normal2), float3(0, 1, 0));
				float4 rainFINAL = 0;
				if (dotToVertical > 0.15) {
					rainFINAL = rainA3D * (dotToVertical - 0.15);//v6.1c
				}
				else {
					//rainFINAL = rainA3D;
				}

				outline.rgb +=rainFINAL.rgb * objectRainPower; //v6.1c

				outline = outline * 0.5*screenBrightness + rainA *0.5*screenRainPower;// +rainA * 0.5*(col.a); //v0.1a
				//outline = outline * (1-screenRainPower) + rainA * (screenRainPower);

				outline.rgb = outline.rgb + dropsFINAL.rrr* outline.rgb*_Lux_RainIntensity;

				//v0.1a
				float shadowmask = tex2D(_DirectionalShadowMask, i.uv).b+ shadowPower;
				//return 1-shadowmask;
				//if (shadowmask > 0) {
				//return float4(normal2*100,1);
				//return float4(depth2.rrr, 1);
				if (normal2.r > 0 || normal2.g > 0 || normal2.b > 0){// depth2 > 0.000){//normal.r > 0 || normal.g > 0 || normal.b > 0) {
					outline = outline * shadowmask*1.75*shadowPowerA;// *shadowPower;
				}
				else {
					outline = col * 0.5*screenBrightness + rainA * 0.5*screenRainPower;
					//outline = col * 1 + rainA * 0.5*screenRainPower;
				}
				//}
				if (depth2 - 0.00035 >= depth) {// || (normal.r > 0 || normal.g > 0 || normal.b > 0)) {
					outline = col* 0.5*screenBrightness + rainA *0.5*screenRainPower; //col v0.1b
					///outline = col * (1 - screenRainPower) + rainA * (screenRainPower);
				}

		        // get color and lerp to snow texture
		        //half4 col = tex2D(_MainTex, i.uv);
				return outline;// lerp(col, snowColor, snowAmount);
			}
			ENDCG
		}
	}
}
