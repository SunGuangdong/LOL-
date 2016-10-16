using UnityEngine;
using System.Collections;

public class WarFogTest : MonoBehaviour {

    [SerializeField]
    private RenderTexture mask;
    [SerializeField]
    private Material mat;

    public void OnRenderImage(RenderTexture source,RenderTexture des) {
        mat.SetTexture("_MaskTex", mask);
        mat.SetFloat("_Dead",FightScene.instance.dead?0:1);
        Graphics.Blit(source, des, mat);
    }
	
}
