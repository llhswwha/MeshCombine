using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCreator : MonoBehaviour
{
    public int width=10;
    public int height=10;
    public float scale=1;//密度

    public Color color=Color.green;
    public Material mat;
    public GameObject result;
    public const string DefaultShader="HDRP/Lit";//"Standard","HDRP/Lit"

    public bool TestNormal;
    public Vector3 DefaultNormal=new Vector3(0,1,0);
    

    // Start is called before the first frame update
    void Start()
    {
        result=GenerateGO();
    }

    private Mesh GenerateMesh(){

        int h=height+1;
        int w=width+1;

        Mesh mesh=new Mesh();
        int x=0;
        int y=0;

        //创建顶点和UV
        Vector3[] vertices=new Vector3[h*w];
        Vector2[] uv=new Vector2[h*w];
        Vector3[] normals=new Vector3[h*w];

        //把uv缩放到0-1
        Vector2 uvScale=new Vector2(1.0f/(w-1),1.0f/(h-1));
        for(y=0;y<h;y++)
        {
            for(x=0;x<w;x++)
            {
                int i=y*w+x;
                vertices[y*w+x]=new Vector3(x*scale,0,y*scale);
                uv[i]=Vector2.Scale(new Vector2(x,y),uvScale);
                normals[i]=DefaultNormal;
            }
        }

        mesh.vertices=vertices;
        mesh.uv=uv;

        //三角形index
        int[] triangles=new int[(h-1)*(w-1)*6];
        int index=0;
        for(y=0;y<h-1;y++)
        {
            for(x=0;x<w-1;x++)
            {
                //每个格子2个三角形，总共6个index
                triangles[index++]=(y*w)+x;
                triangles[index++]=((y+1)*w)+x;
                triangles[index++]=(y*w)+x+1;

                triangles[index++]=((y+1)*w)+x;
                triangles[index++]=((y+1)*w)+x+1;
                triangles[index++]=(y*w)+x+1;
            }
        }
        mesh.triangles=triangles;

        if(TestNormal)
            mesh.normals=normals;
        //mesh.RecalculateNormals();
        return mesh;
    }

    private GameObject GenerateGO(){
        GameObject obj=new GameObject("Mesh_Plane");
        MeshFilter meshFilter=obj.AddComponent<MeshFilter>();
        //创建mesh
        Mesh mesh=GenerateMesh();
        meshFilter.mesh=mesh;
        MeshRenderer renderer=obj.AddComponent<MeshRenderer>();
        //创建材质
        if(mat==null){
            Material mat=new Material(Shader.Find(DefaultShader));
            //mat.color=Color.green;
            mat.SetColor("_BaseColor",color);
        }
        renderer.material=mat;
        obj.AddComponent<MeshInfo>();
        return obj;
    }

    [ContextMenu("Recreate")]
    private void Recreate(){
        if(result)
            GameObject.DestroyImmediate(result);
        result=GenerateGO();
    }
}
