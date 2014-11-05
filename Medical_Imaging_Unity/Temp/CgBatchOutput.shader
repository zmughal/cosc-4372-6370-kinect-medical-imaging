Shader "Hidden/GlowConeTap" {

Properties {
	_Color ("Color", color) = (1,1,1,0)
	_MainTex ("", 2D) = "white" {}
}

Category {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	Subshader {
		Pass {
			// shader program with errors was here
Program "err" { SubProgram { "!!error" } }

#LINE 57

		}
	}

	Subshader {
		Pass {
			SetTexture [_MainTex] {constantColor [_Color] combine texture * constant alpha}
			SetTexture [_MainTex] {constantColor [_Color] combine texture * constant + previous}
			SetTexture [_MainTex] {constantColor [_Color] combine texture * constant + previous}
			SetTexture [_MainTex] {constantColor [_Color] combine texture * constant + previous}		
		}

	}
}

Fallback off

}
