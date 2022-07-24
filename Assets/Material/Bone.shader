Shader "Unlit/Bone"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        // 参考
        // [Unity] 3Dオブジェクトを常に前面に表示するシェーダ
        // https://qiita.com/edo_m18/items/c4949201773d65d6bde1
        // Unityに用意されているサーフェースシェーダープログラムの中身はどうなっているのか
        // https://atmarkit.itmedia.co.jp/ait/articles/1801/05/news010.html
        // Writing Surface Shaders
        // https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
        // UnityでUnlit Shaderを作成して半透明なオブジェクトを描画してみよう！
        // https://madai21.hatenablog.com/entry/unity-script-draw-object-transparent-unlit-shader

        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            
            ENDCG
        }
    }
}
