using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class MeshCombineHelper
{

    public static GameObject SimpleCombine(GameObject source,GameObject target){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        combinedMesh.DoCombine(true);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshInfo>();
        Debug.Log("Combine:"+source+"->"+target);
        return target;
    }

    public static IEnumerator SimpleCombine_Coroutine(GameObject source,GameObject target,int waitCount,bool isDestroy,Action<GameObject> callback){
        
        CombinedMesh combinedMesh=new CombinedMesh(source.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(true,waitCount);
        target=combinedMesh.CreateNewGo(false,target);
        target.AddComponent<MeshInfo>();
        Debug.Log("Combine:"+source+"->"+target);
        if(isDestroy){
            GameObject.Destroy(source);
        }
        if(callback!=null){
            callback(source);
        }
        yield return target;
    }
    
    public static GameObject CombineMaterials(GameObject go,out int  count){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined";
        goNew.transform.SetParent(go.transform.parent);
        //int count=0;
        Dictionary<Material,List<MeshFilter>> mat2Filters=GetMatFilters(go,out count);
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            List<MeshFilter> list=item.Value;

            CombinedMesh combinedMesh=new CombinedMesh(go.transform,list,material);
            combinedMesh.DoCombine(true);
            GameObject matGo=combinedMesh.CreateNewGo(false,null);
            matGo.name=material.name;
            matGo.transform.SetParent(goNew.transform);
        }
        Debug.LogError(string.Format("CombineMaterials 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count,count));
        return goNew;
    }

     public static IEnumerator CombineMaterials_Coroutine(GameObject go,int waitCount,bool isDestroy,Action<GameObject> callback){
        DateTime start=DateTime.Now;
        GameObject goNew=new GameObject();
        goNew.name=go.name+"_Combined";
        goNew.transform.SetParent(go.transform.parent);
        int count=0;
        Dictionary<Material,List<MeshFilter>> mat2Filters=GetMatFilters(go,out count);
        yield return null;
        int i=0;
        foreach(var item in mat2Filters)
        {
            Material material=item.Key;
            List<MeshFilter> list=item.Value;
            Debug.LogWarning(string.Format("CombineMaterials_Coroutine {0} ({1}/{2})",material,i+1,mat2Filters.Count));
            CombinedMesh combinedMesh=new CombinedMesh(go.transform,list,material);
            yield return combinedMesh.DoCombine_Coroutine(true,waitCount);
            GameObject matGo=combinedMesh.CreateNewGo(false,null);
            matGo.name=material.name;
            matGo.transform.SetParent(goNew.transform);
            yield return goNew;
            i++;
        }
        if(isDestroy){
            GameObject.Destroy(go);
        }
        Debug.LogError(string.Format("CombineMaterials 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count,count));
        if(callback!=null){
            callback(goNew);
        }
        yield return goNew;
    }
   
    public static Dictionary<Material,List<MeshFilter>> GetMatFilters(GameObject go,out int count){
        DateTime start=DateTime.Now;
        Dictionary<Material,List<MeshFilter>> mat2Filters=new Dictionary<Material, List<MeshFilter>>();
        MeshRenderer[] renderers=go.GetComponentsInChildren<MeshRenderer>();
        count=renderers.Length;
        for(int i=0;i<renderers.Length;i++){
            MeshRenderer renderer=renderers[i];
            if(!mat2Filters.ContainsKey(renderer.sharedMaterial)){
                mat2Filters.Add(renderer.sharedMaterial,new List<MeshFilter>());
            }
            List<MeshFilter> list=mat2Filters[renderer.sharedMaterial];
            MeshFilter filter=renderer.GetComponent<MeshFilter>();
            list.Add(filter);
        }
        Debug.LogError(string.Format("GetMatFilters 用时:{0},Mat数量:{1},Mesh数量:{2}",(DateTime.Now-start),mat2Filters.Count,count));
        return mat2Filters;
    }

    public static GameObject Combine(GameObject source){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        combinedMesh.DoCombine(false);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        return target;
    }

    public static IEnumerator Combine_Coroutine(GameObject source,int waitCount,bool isDestroy,Action<GameObject> callback){
        DateTime start=DateTime.Now;
        int count=0;
        GameObject goNew=CombineMaterials(source,out count);//按材质合并
        CombinedMesh combinedMesh=new CombinedMesh(goNew.transform,null,null);
        yield return combinedMesh.DoCombine_Coroutine(false,waitCount);
        GameObject target=combinedMesh.CreateNewGo(false,null);
        target.name=source.name+"_Combined";
        goNew.transform.SetParent(target.transform);
        GameObject.DestroyImmediate(goNew);
        Debug.LogError(string.Format("CombinedMesh 用时:{0}ms,数量:{1}",(DateTime.Now-start).TotalMilliseconds,count));
        if(isDestroy)
        {
            GameObject.Destroy(source);
        }
        if(callback!=null){
            callback(target);
        }
        yield return target;
    }

    public static GameObject CombineEx(GameObject source,int mode=0){
        GameObject result=null;
        if(mode==0){
            result= Combine(source);
        }
        else if(mode ==1 )
        {
            int count=0;
            result= CombineMaterials(source,out count);
        }
        else{
            result= SimpleCombine(source,null);
        }
        return result;
    }

    public static IEnumerator CombineEx_Coroutine(GameObject source,bool isDestroy,int waitCount,int mode,Action<GameObject> callback){
        if(mode==0){
            yield return Combine_Coroutine(source,waitCount,isDestroy,callback);
        }
        else if(mode ==1 )
        {
            yield return CombineMaterials_Coroutine(source,waitCount,isDestroy,callback);
        }
        else{
           yield return SimpleCombine_Coroutine(source,null,waitCount,isDestroy,callback);
        }
    }

    public static Dictionary<GameObject,CombinedMesh> go2ms=new Dictionary<GameObject, CombinedMesh>();
    public static void AddGo(GameObject gameObject,CombinedMesh mesh){
        if(go2ms.ContainsKey(gameObject)){
            go2ms[gameObject]=mesh;
        }
        else{
            go2ms.Add(gameObject,mesh);
        }
        
    }

    public static CombinedMesh GetMesh(GameObject gameObject){
        if(go2ms.ContainsKey(gameObject)){
            return go2ms[gameObject];
        }
        else{
            return null;
        }
    }

    public static void RemveGo(GameObject gameObject){
        CombinedMesh mesh=GetMesh(gameObject);
        if(mesh!=null){
            Debug.Log("mesh:"+mesh.name);
            Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
            MeshFilter[] meshFilters=gameObject.GetComponentsInChildren<MeshFilter>();
            foreach(var mf in meshFilters){
                mesh.meshFilters.Remove(mf);
            }
            MeshRenderer[] meshRenderers=gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach(var mr in meshRenderers){
                mr.enabled=true;
                mr.material.SetColor("_BaseColor",Color.red);
            }
            Debug.Log("mesh.meshFilters:"+mesh.meshFilters.Count);
            mesh.Refresh();//重新合并
        }
        else{
            Debug.LogError("未找到CombinedMesh:"+gameObject);
        }
    }

}
