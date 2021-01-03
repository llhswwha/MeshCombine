using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

 [Serializable]
public class MeshPartInfo{
    public Mesh mesh;

    public Material[] mats;

    public List<MeshFilter> meshFilters;

    public MeshPartInfo(List<MeshFilter> mfs){
        meshFilters=mfs;
    }
}

 [Serializable]
public class CombinedMesh{

    public string name;
    //public Mesh mesh;

    public List<MeshPartInfo> meshes;

    public Material mat;

    public Transform source;
    public List<MeshFilter> meshFilters;

    public int VertexCount=0;

    public static int MaxVertex=65535;

    public static UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public CombinedMesh(Transform source,List<MeshFilter> mfs, Material mat){
        this.source=source;

        if(mfs==null || mfs.Count==0){
            this.meshFilters=source.GetComponentsInChildren<MeshFilter>().ToList();
        }
        else{
            this.meshFilters=new List<MeshFilter>(mfs);
        }
        this.mat=mat;
        //SimpleCombine();

        //Debug.LogError("CombinedMesh Read/Write Enabled = True !! :"+ms);
    }

    private MeshPartInfo InnerDoCombine(MeshPartInfo info,int id){

        List<MeshFilter> mfs=info.meshFilters;
        int count=mfs.Count;
        if(count==0){
            Debug.LogError("meshFiltersToCombined.Count == 0");
            return null;
        }
        CombineInstance[] combines=new CombineInstance[count];
        Material[] mats=null;
        if(mat==null)
            mats=new Material[count];
        Matrix4x4 matrix=source.worldToLocalMatrix;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=mfs[i];
            MeshRenderer mr=mf.GetComponent<MeshRenderer>();
            if(mr==null)continue;
            Mesh ms=mf.sharedMesh;
            if(ms.isReadable==false){
                Debug.LogError("模型必须设置为 Read/Write Enabled = True !! :"+ms);
                continue;
            }
            combines[i].mesh=ms;
            combines[i].transform=matrix*mf.transform.localToWorldMatrix;
            //combines[i].transform=mf.transform;
            mr.enabled=false;//不渲染

            if(mat==null)
                mats[i]=mr.sharedMaterial;

            MeshCombineHelper.AddGo(mf.gameObject,this);
        }
        //source.gameObject.SetActive(false);

        Mesh newMesh=new Mesh();
        newMesh.indexFormat=indexFormat;
        newMesh.name=source.name+"_Combined"+id;
        if(mat!=null){
            newMesh.name=mat.name+"_Combined"+id;
        }
        bool mergeSubMeshes=mat!=null;

        newMesh.CombineMeshes(combines,mergeSubMeshes);//核心 合并网格

        // MeshPartInfo info=new MeshPartInfo();
        info.mesh=newMesh;
        info.mats=mats;
        return info;
    }

    public GameObject Refresh(){
        this.DoCombine(true);
        GameObject target=this.CreateNewGo(false,NewGo);
        return target;
    }

    public void DoCombine(bool logTime){

        List<MeshPartInfo> allList=new List<MeshPartInfo>();
        List<MeshFilter> list=new List<MeshFilter>();
        allList.Add(new MeshPartInfo(list));

        VertexCount=0;
        int vcSum=0;
        int count=meshFilters.Count;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=meshFilters[i];
            Mesh ms=mf.sharedMesh;
            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            int vc=ms.vertexCount;
            VertexCount+=vc;

            if(vcSum+vc>MaxVertex)
            {
                vcSum=vc;
                list=new List<MeshFilter>();
                list.Add(mf);
                allList.Add(new MeshPartInfo(list));
            }
            else{
                vcSum+=vc;
                list.Add(mf);
            }
        }
        meshes=new List<MeshPartInfo>();

        Debug.Log(string.Format("DoCombine allList.Count:",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            Debug.LogWarning(string.Format("DoCombine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));
            var newMesh=InnerDoCombine(allList[i],i);
            meshes.Add(newMesh);
        }

        DateTime start=DateTime.Now;
        Debug.LogWarning(
            string.Format("CombinedMesh 用时:{1}ms,Mesh数量:{1} 子模型数:{2},VertexCount:{3},Mat:{4}"
            ,(DateTime.Now-start).TotalMilliseconds,count,allList.Count,VertexCount,mat)
            );
    }

    public IEnumerator DoCombine_Coroutine(bool logTime,int waitCount){

        List<MeshPartInfo> allList=new List<MeshPartInfo>();
        List<MeshFilter> list=new List<MeshFilter>();
        allList.Add(new MeshPartInfo(list));

        VertexCount=0;
        int vcSum=0;
        int count=meshFilters.Count;
        for(int i=0;i<count;i++)
        {
            MeshFilter mf=meshFilters[i];
            Mesh ms=mf.sharedMesh;
            //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
            int vc=ms.vertexCount;
            VertexCount+=vc;

            if(vcSum+vc>MaxVertex)
            {
                vcSum=vc;
                list=new List<MeshFilter>();
                list.Add(mf);
                allList.Add(new MeshPartInfo(list));
            }
            else{
                vcSum+=vc;
                list.Add(mf);
            }
        }
        meshes=new List<MeshPartInfo>();
        //Debug.Log(string.Format("DoCombine[{0}/{1}]:{2}",i,count,VertexCount));
        Debug.Log(string.Format("DoCombine allList.Count:{0}",allList.Count));
        for(int i=0;i<allList.Count;i++)
        {
            Debug.LogWarning(string.Format("DoCombine_Coroutine {0} ({1}/{2})",allList[i].mesh,i+1,allList.Count));                                                                                                                                                                                                                                                                                                                                                                                                                            
            if(i % waitCount == 0){
                yield return null;
            }
            var newMesh=InnerDoCombine(allList[i],i);
            meshes.Add(newMesh);
        }

        DateTime start=DateTime.Now;
        Debug.LogWarning(
            string.Format("CombinedMesh 用时:{1},Mesh数量:{1} 子模型数:{2},VertexCount:{3},Mat:{4}"
            ,(DateTime.Now-start),count,allList.Count,VertexCount,mat)
            );
        yield return null;
    }

    public void SetRendererAndFilter(GameObject go,MeshPartInfo ms){
        MeshFilter meshFilter=go.GetComponent<MeshFilter>();
        if(meshFilter==null){
            meshFilter=go.AddComponent<MeshFilter>();
        }
        meshFilter.mesh=ms.mesh;
        MeshRenderer meshRenderer=go.GetComponent<MeshRenderer>();
        if(meshRenderer==null){
            meshRenderer=go.AddComponent<MeshRenderer>();
        }

        if(mat!=null){
            meshRenderer.sharedMaterial=mat;
        }
        else
        {
            meshRenderer.sharedMaterials=ms.mats;
        }
        
        meshRenderer.enabled=true;
    }
    
    public void SetCollider(GameObject go,MeshPartInfo ms){
        Collider collider=go.GetComponent<Collider>();
        if(collider!=null){
            GameObject.DestroyImmediate(collider);
        }
        MeshCollider meshCollider=go.AddComponent<MeshCollider>();
        meshCollider.sharedMesh=ms.mesh;
    }

    public GameObject NewGo;

    public GameObject CreateNewGo(bool enableCollider,GameObject target){
        if(target==null){
            target=new GameObject();
            target.name=source.name+"_Combined";
        }
        if(meshes.Count==1){
            this.SetRendererAndFilter(target,meshes[0]);
            if(enableCollider)
                this.SetCollider(target,meshes[0]);
        }
        else{
            for(int i=0;i<meshes.Count;i++){
                var info=meshes[i];
                GameObject subObj=new GameObject();
                subObj.name=info.mesh.name;
                subObj.transform.SetParent(target.transform);

                this.SetRendererAndFilter(subObj,info);
                if(enableCollider)
                    this.SetCollider(subObj,info);
            }
        }
        
        target.transform.position=source.transform.position;//坐标一致,不设置的话，就是按照新的target的坐标来
        target.transform.localRotation=source.transform.localRotation;//坐标一致,不设置的话，就是按照新的target的坐标来
        target.transform.localScale=source.transform.localScale;//坐标一致,不设置的话，就是按照新的target的坐标来
        CombinedMeshInfo meshInfo=target.AddComponent<CombinedMeshInfo>();
        meshInfo.combinedMesh=this;

        name=target.name;
        NewGo=target;

        target.transform.SetParent(source.transform.parent);
        return target;
    }
}