// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

shader "MaskingShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 1
        _MaskTex ("Masking Texture", 2D) = "white"{}
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent-1"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ColorMask RGBA
        ZWrite Off
        Blend One OneMinusSrcAlpha
 
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.color = IN.color;
                OUT.texcoord = IN.texcoord;
 
                return OUT;
            }
 
            sampler2D _MainTex;
            sampler2D _MaskTex;
            Float _Alpha;
 
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, IN.texcoord);
                fixed4 masked = tex2D(_MaskTex, IN.texcoord);
                return fixed4(original.r * masked.a * _Alpha, original.g * masked.a * _Alpha,original.b * masked.a * _Alpha, masked.a * _Alpha);
            }
        ENDCG
        }
    }
}