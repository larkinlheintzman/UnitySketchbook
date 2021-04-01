// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DepthCopy"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MyDepthTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite On ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment CopyDepthBufferFragmentShader

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertex = UnityObjectToClipPos (v.vertex);
				o.scrPos=ComputeScreenPos(o.vertex);
				return o;
			}

			// sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			// sampler2D_float _MyDepthTex;

			// important part: outputs depth from _MyDepthTex to depth buffer
			fixed4 CopyDepthBufferFragmentShader(v2f i) : COLOR
			{
				// float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
				// float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UNITY_PROJ_COORD(i.uv)));
				// half4 depthVect;
				// depthVect.r = depth;
				// depthVect.g = depth;
				// depthVect.b = depth;
				// depthVect.a = 1;
				// return depthVect;

				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
        depth = pow(Linear01Depth(depth), 1);
				return 1 - depth;
				// outDepth = depth;
				// return 0;
			}

			ENDCG
		}
	}
}
