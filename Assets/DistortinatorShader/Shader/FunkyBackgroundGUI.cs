#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CanEditMultipleObjects]
public class FunkyBackgroundGUI : ShaderGUI
{
    public BlendMode bm;
    public BlendMode bm2;
    public int interlaceSize;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // render the default gui
        base.OnGUI(materialEditor, properties);

        var targetMat = materialEditor.target as Material;
        bm = (BlendMode) targetMat.GetInt("_MySrcMode");
        bm2 = (BlendMode) targetMat.GetInt("_MyDstMode");
        interlaceSize = targetMat.GetInt("_InterlaceSize");

        bm = (BlendMode) EditorGUILayout.EnumPopup("Source Blend Mode", bm);
        targetMat.SetFloat("_MySrcMode", (float) bm);
        bm2 = (BlendMode) EditorGUILayout.EnumPopup("Destiny Blend Mode", bm2);
        targetMat.SetFloat("_MyDstMode", (float) bm2);

        targetMat.SetFloat("_Interlace2", interlaceSize * 2);

        //bool interlace = Array.IndexOf(targetMat.shaderKeywords, "INTERLACING_ON") != -1;
    }
}
#endif