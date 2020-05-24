using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Animations;
using UnityEditor.Animations;

public struct BakedData
{
    public byte[] m_tex_raw_data;
    public float m_lenght;
    public int m_width;
    public int m_height;
}

public class AnimMapBakeImp 
{
    int VertexHigh;
    List<AnimationState> m_list_anim_clip;
    Animation m_anim;
    SkinnedMeshRenderer m_skinned_renderer;
    Mesh m_skin_mesh;
    BakedData m_baked_data;
    public void SetTargetObj(GameObject obj)
    {
        if (null == obj)
        {
            return;
        }
        m_skinned_renderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        m_anim = obj.GetComponent<Animation>();
        Debug.Assert(m_skinned_renderer);
        Debug.Assert(m_anim);
        m_list_anim_clip = new List<AnimationState>(m_anim.Cast<AnimationState>());
        m_skin_mesh = new Mesh();
        m_baked_data = new BakedData();
        m_baked_data.m_width = Mathf.ClosestPowerOfTwo(m_skinned_renderer.sharedMesh.vertexCount);
    }

    void RecordCurTimeAnimMap(AnimationState ami, ref Texture2D cur_ami_tex, int cur_frame)
    {
        m_anim.Sample();
        m_skinned_renderer.BakeMesh(m_skin_mesh);
        for(int pos = 0; pos < m_skin_mesh.vertexCount; ++pos)
        {
            Color cur_pos = new Color(m_skin_mesh.vertices[pos].x, m_skin_mesh.vertices[pos].y, m_skin_mesh.vertices[pos].z);
            cur_ami_tex.SetPixel(pos, cur_frame, cur_pos);
        }
    }

    List<byte> BakeSingleAnimClip(AnimationState ami)
    {
        AnimationClip clip = ami.clip;
        int total_frame = Mathf.ClosestPowerOfTwo((int)(clip.frameRate * clip.length));
        float sample_gap = clip.length / total_frame;

        //播放动画
        m_anim.Play(ami.name);
        float acc_time = 0.0f;
        int acc_count = 0;
        Texture2D cur_ami_tex = new Texture2D(m_baked_data.m_width, total_frame, TextureFormat.RGBAHalf, false);
        while(acc_count < total_frame)
        {
            ami.time = acc_time;
            //把这个时刻的顶点信息记录下来
            RecordCurTimeAnimMap(ami, ref cur_ami_tex, acc_count);
            acc_time += sample_gap;
            ++acc_count;
        }
        cur_ami_tex.Apply();
        m_baked_data.m_height += total_frame;
        m_baked_data.m_lenght += clip.length;
        //把这个合并到最终的raw_tex_data中
        byte[] cur_tex_raw_data = cur_ami_tex.GetRawTextureData();
        List<byte> res_list = new List<byte>(cur_tex_raw_data);
        return res_list;
    }

    public BakedData GenRawTexData()
    {
        List<byte> res_list = new List<byte>();
        foreach(var ami in m_list_anim_clip)
        {
            res_list.AddRange(BakeSingleAnimClip(ami));
        }
        
        m_baked_data.m_tex_raw_data = res_list.ToArray();
        return m_baked_data;
    }
}
