Shader "CustomShader/RoomFogShader"
{
	Properties
	{
		_MainTex("Visual Fog", 2D) = "white" {}
		_AlphaMask("Alpha Mask Fog (Uses R Channel)", 2D) = "white" {}
		_AlphaMaskScale("Alpha Mask Scale", Range(0, 5)) = 1.0
		_ScrollSpeedX("Scroll Speed X", Float) = 1.0
		_ScrollSpeedY("Scroll Speed Y", Float) = 1.0
	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "Render" = "Transparent" "IgnoreProjector" = "True" }
			LOD 100

			//ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;//for the alpha mask
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _AlphaMask;

				uniform float _ScrollSpeedX;//scale for the scrollSpeed in X direction
				uniform float _ScrollSpeedY;//scale for the scrollspeed in Y direction
				uniform float _InRoom = 1;//basic a bool to know if the player is in this room
				uniform float _UnscaledTime;//a unscaled version of time

				uniform float _AlphaMaskScale = 1;//scale for the alpha mask

				float4 _MainTex_ST;
				float4 _AlphaMask_ST;

				v2f vert(appdata v)
				{
					v2f o;
					//get world space of vertex
					o.vertex = UnityObjectToClipPos(v.vertex);
					//uv from vertex position in world space
					//o.uv = TRANSFORM_TEX(UnityObjectToViewPos(o.vertex), _MainTex);
					o.uv = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex), _MainTex);
					o.uv2 = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex), _AlphaMask);
					//o.uv = float2(0, 0);
					//fog?
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//uv offset of
					float2 uv = i.uv;
					uv.x += _UnscaledTime * _ScrollSpeedX;
					uv.y += _UnscaledTime * _ScrollSpeedY;

					float2 uv2 = i.uv2;
					uv2.x += _UnscaledTime * _ScrollSpeedX;
					uv2.y += _UnscaledTime * _ScrollSpeedY;

					// sample the texture
					fixed4 col = tex2D(_MainTex, uv);
					fixed4 alphaMask = tex2D(_AlphaMask, uv2);

					float colorMask = min(alphaMask.r * _AlphaMaskScale, 1);
					float alpha = colorMask * 1 - (1.0f *  _InRoom);

					col.a = alpha;

					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);


					//fixed4 col = fixed4(1,1,1,1 - _InRoom);


					return col;
				}
				ENDCG
			}
		}
}

					//1 being no alpha
					//we subtract 1.0 * (if _InRoom is above 0.5) //(OLD CODE FOR WHEN _InRoom was a int)
					//float alpha = 1 - (1.0f * step(0.5f, _InRoom));

