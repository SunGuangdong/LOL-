﻿Shader "Custom/WarFog" {  
    Properties {  
        _MainTex ("_MainTex", 2D) = "white" {}  
		_MaskTex ("_MaskTex", 2D) = "white" {}
		_Dead ("_Dead", Float) = 0
    }  
    SubShader {  
        Pass {  
            CGPROGRAM  
            #pragma vertex m_vert_img  
            #pragma fragment frag  
               
            #include "UnityCG.cginc"  
               
            uniform sampler2D _MainTex;  
			uniform sampler2D _MaskTex; 
            uniform float _Dead; 
			   struct m_appdata_img {
    float4 vertex : POSITION;
    half2 texcoord : TEXCOORD0;
	half2 texcoord1 : TEXCOORD1;
};
struct m_v2f_img {
	float4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half2 uv1 : TEXCOORD1;
};

            fixed4 frag(m_v2f_img i) : COLOR  
            {  
                fixed4 renderTex = tex2D(_MainTex, i.uv);  
                 fixed4 renderTex1 = tex2D(_MaskTex, i.uv1); 
                fixed4 finalColor;
				if(_Dead<0.5){
					float color=dot(renderTex.rgb,fixed3(0.22,0.707,0.071));
					finalColor=fixed4(color,color,color,1);
				}else
				if(renderTex1.r<.3){
					finalColor = renderTex.rgba;
				}else{
					finalColor = renderTex.rgba;
				} 
                   
                return finalColor;  
            }  

	float2 m_MultiplyUV (float4x4 mat, float2 inUV) {
	float4 temp = float4 (inUV.x, 1-inUV.y, 0, 0);
	temp = mul (mat, temp);
	return temp.xy;
	}


m_v2f_img m_vert_img( m_appdata_img v )
{
	m_v2f_img o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
	o.uv1 = m_MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord1 );
	return o;
}
            ENDCG  
        }  
    }  
    FallBack "Diffuse"  
}
