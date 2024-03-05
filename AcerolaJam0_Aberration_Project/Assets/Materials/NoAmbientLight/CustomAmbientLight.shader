Shader "Custom/CustomAmbientLight"
{
    Properties
    {
         _Color ("Color", Color) = (1,1,1,1)
        _DiffuseTex("Texture", 2D) = "white" {}
    }
 
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }
     
        CGINCLUDE
        #define _GLOSSYENV 1
        ENDCG
     
        CGPROGRAM
        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        //no ambient + handles all light sources shadows
        #pragma surface surf Standard noambient fullforwardshadows

        #pragma exclude_renderers gles
 
        struct Input
        {
            float2 uv_DiffuseTex;
        };
 
        fixed4 _Color;
        sampler2D _DiffuseTex;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 albedo = tex2D(_DiffuseTex, IN.uv_DiffuseTex) * _Color;
     
            o.Albedo = albedo.rgb;
            o.Alpha = albedo.a;
        }
        ENDCG
    }
 
    FallBack "Diffuse"
}