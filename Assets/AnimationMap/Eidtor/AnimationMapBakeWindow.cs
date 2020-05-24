using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationMapBakeWindow : EditorWindow
{
    #region 变量
    public GameObject TargetObj;
    public Shader AniShader;
    public Texture2D MainTex;
    const int AniTexWidth = 1024;
    const int AniTexHeight = 768;
    public float AnimationLength = 1.0f;
    const string AniMatPath = "Assets/Shaders/AnimShader.mat";
    const string AniTexPath = "Assets/Shaders/AnimShader.asset";
    const string AniPrefabPath = "Assets/Shaders/AnimShader.prefab";

    AnimMapBakeImp m_ani_map_bake_imp = null;
    #endregion 

    #region 方法
    [MenuItem("Window/AnimationMapBakeWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<AnimationMapBakeWindow>();
    }
    byte[] m_raw_tex_data;
    private void OnGUI()
    {
        TargetObj = EditorGUILayout.ObjectField(TargetObj, typeof(GameObject), true) as GameObject;
        AniShader = EditorGUILayout.ObjectField(AniShader, typeof(Shader), true) as Shader;
        if(null == m_ani_map_bake_imp)
        {
            m_ani_map_bake_imp = new AnimMapBakeImp();
        }
        m_ani_map_bake_imp.SetTargetObj(TargetObj);
        if (GUILayout.Button("Bake"))
        {
            //存成材质
            BakedData baked_data = m_ani_map_bake_imp.GenRawTexData();
            Material mat = SaveAsMat(baked_data);
            SaveAsPrefab(baked_data, mat);
        }
        
    }

    void SaveAsPrefab(BakedData baked_data, Material mat)
    {
        GameObject go = new GameObject();
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshFilter>().sharedMesh = TargetObj.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        PrefabUtility.CreatePrefab(AniPrefabPath, go);
    }

    //生成tex
    Texture2D GenTextureFromRawData(BakedData baked_data)
    {
        Texture2D ani_tex = new Texture2D(baked_data.m_width, baked_data.m_height, TextureFormat.RGBAHalf, false);
        ani_tex.LoadRawTextureData(baked_data.m_tex_raw_data);
        return ani_tex;
    }
    Material SaveAsMat(BakedData baked_data)
    {
        Material mat = new Material(AniShader);
        Texture2D tex = GenTextureFromRawData(baked_data);
        mat.SetTexture("_AnimationTex", tex);
        mat.SetTexture("_MaxTex", MainTex);
        mat.SetFloat("_AnimationLen", baked_data.m_lenght);
        AssetDatabase.CreateAsset(mat, AniMatPath);
        AssetDatabase.CreateAsset(tex, AniTexPath);
        return mat;
    }
    #endregion
}
