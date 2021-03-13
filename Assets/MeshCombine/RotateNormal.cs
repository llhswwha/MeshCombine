using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNormal : MonoBehaviour
{
    public float speed = 100.0f;

    public Vector3 dir=Vector3.up;

    public MeshFilter meshFilter=null;

    public Quaternion rotation;

    void Start()
    {
        meshFilter=GetComponent<MeshFilter>();
    }

    public Vector3[] normals;

    // Update is called once per frame
    void Update()
    {
        // obtain the normals from the Mesh
        Mesh mesh = meshFilter.mesh;
        //Vector3[] normals = mesh.normals;
        normals = mesh.normals;
        // edit the normals in an external array
        rotation = Quaternion.AngleAxis(Time.deltaTime * speed, dir);

        for (int i = 0; i < normals.Length; i++)
            normals[i] = rotation * normals[i];

        // assign the array of normals to the mesh
        mesh.normals = normals;
    }
}
