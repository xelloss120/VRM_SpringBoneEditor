Shader "Unlit/Bone"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        // �Q�l
        // [Unity] 3D�I�u�W�F�N�g����ɑO�ʂɕ\������V�F�[�_
        // https://qiita.com/edo_m18/items/c4949201773d65d6bde1
        // Unity�ɗp�ӂ���Ă���T�[�t�F�[�X�V�F�[�_�[�v���O�����̒��g�͂ǂ��Ȃ��Ă���̂�
        // https://atmarkit.itmedia.co.jp/ait/articles/1801/05/news010.html
        // Writing Surface Shaders
        // https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
        // Unity��Unlit Shader���쐬���Ĕ������ȃI�u�W�F�N�g��`�悵�Ă݂悤�I
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
