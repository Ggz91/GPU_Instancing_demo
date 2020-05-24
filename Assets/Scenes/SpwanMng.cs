using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanMng : MonoBehaviour
{
    public GameObject Prefab;
    public int width = 100;
    public int height = 100;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i< width; ++i)
        {
            for(int j = 0; j<height; ++j)
            {
                GameObject.Instantiate(Prefab, new Vector3(2*i, 0, 2*j), Quaternion.identity);
            }
        }
    }

}
