// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//波动效果
Shader "MiniFramework/2D Effect/Wave"
{
	Properties 
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Amplitude ("振幅", Range(0, 1)) = 0.3     
        _AngularVelocity ("角速度(圈数)", Range(0, 50)) = 10     
        _Speed ("移动速度", Range(0, 30)) = 10  
	}
    
	SubShader 
	{
		LOD 200
		Tags 
		{
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"LightMode" = "ForwardBase"
		}
		Pass 
		{

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Amplitude;     
            float _AngularVelocity;     
            float _Speed;   

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0; 
			};

			v2f vert(appdata_full  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				i.uv.y += _Amplitude * sin(_AngularVelocity * i.uv.x + _Speed * _Time.y);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
