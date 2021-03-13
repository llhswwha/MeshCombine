using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh=new Mesh();
        mesh.vertices=new Vector3[]{new Vector3(-1,0,0),new Vector3(1,0,0),new Vector3(0,2,0)};
        mesh.triangles=new int[]{0,1,2};
        mesh.RecalculateNormals();
        MeshFilter meshFilter=this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh=mesh;
        MeshRenderer renderer=this.gameObject.AddComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
