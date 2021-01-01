using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{
    public int CombineMode=0;//0:sourceRoot合并为一个模型，1:sourceRoot的子物体分别合并为一个模型

    public GameObject sourceRoot;

    public List<GameObject> sourceList=new List<GameObject>();

    public List<GameObject> resultList=new List<GameObject>();

    public bool Auto=false;

    public bool IsDestroySource=false;

    public bool IsCombine=false;

    public int MaxVertex=65535;

    public UnityEngine.Rendering.IndexFormat indexFormat=UnityEngine.Rendering.IndexFormat.UInt16;

    public bool NoLimit=false;

    // Start is called before the first frame update
    void Start()
    {
        if(NoLimit){
            indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            MaxVertex=int.MaxValue;
        }
        else{
            if(MaxVertex>65535){
                indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            }
        }
        CombinedMesh.indexFormat=indexFormat;
        CombinedMesh.MaxVertex=MaxVertex;

        if(Auto)
        {
            CombineByMaterial();
        }
    }

    public  CombinedMesh combinedMesh;


    [ContextMenu("Combine")]

    public void Combine(){
        CombineEx(2);
    }

    public bool CombineBySub=false;

    public List<MeshCombiner> SubCombiners=new List<MeshCombiner>();

    public bool IsCoroutine=false;

    public int WaitCount=10000;

    private void CombineEx(int mode){
        Debug.Log("CombineEx:"+mode+"|"+gameObject);
        resultList=new List<GameObject>();
        string sourceName="";
        if((sourceList.Count==0 || CombineMode == 0)&&sourceRoot!=null){
            sourceList=new List<GameObject>();
            if(CombineMode==0){
                sourceList.Add(sourceRoot);
                sourceName+=sourceRoot.name;
            }
            else{
                for(int i=0;i<sourceRoot.transform.childCount;i++){
                    GameObject child=sourceRoot.transform.GetChild(i).gameObject;
                    sourceList.Add(child);
                    sourceName+=child.name+";";
                }
            }
        }
        Debug.LogError("sourceList:"+sourceName);
        foreach(var source in sourceList){
            if(source==null)continue;

            if(CombineBySub){
                MeshCombiner subCombiner=gameObject.AddComponent<MeshCombiner>();
                subCombiner.Auto=true;
                subCombiner.IsCoroutine=this.IsCoroutine;
                subCombiner.CombineBySub=false;
                subCombiner.WaitCount=this.WaitCount;
                subCombiner.IsDestroySource=false;
                subCombiner.sourceList.Add(source);
                SubCombiners.Add(subCombiner);
            }
            else{
                if(IsCoroutine){
                    StartCoroutine(MeshCombineHelper.CombineEx_Coroutine(source,IsDestroySource,mode,WaitCount));
                }
                else{
                    GameObject target=MeshCombineHelper.CombineEx(source,mode);
                    resultList.Add(target);
                    Debug.Log("Combine:"+source+"->"+target);
                    if(IsDestroySource){
                        GameObject.Destroy(source);
                    }
                }

            }
        }
    }

    [ContextMenu("CombineByMaterial")]

    public void CombineByMaterial(){
        CombineEx(1);
    }


    [ContextMenu("CombineEx")]

    public void CombineEx(){
        CombineEx(0);
    }

    public bool AutoAdd;

    public List<GameObject> mergedObjs=new List<GameObject>();

    public GameObject mergedObjRoot=null;

    public GameObject mergedObj=null;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)){
            Debug.Log("Click");
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                GameObject go=hit.collider.gameObject;
                Debug.Log("Hit:"+go);
                if(AutoAdd){
                    if(!mergedObjs.Contains(go)){
                        if(mergedObjRoot==null){
                            mergedObjRoot=new GameObject();
                            mergedObjRoot.name="mergedObjRoot";
                        }

                        mergedObjs.Add(go);
                        go.transform.SetParent(mergedObjRoot.transform);

                        mergedObj=MeshCombineHelper.SimpleCombine(mergedObjRoot,mergedObj);
                        mergedObj.transform.SetParent(this.transform);
                    }
                }
                if(AutoRemove){
                    MeshCombineHelper.RemveGo(go);
                }
            }
        }
    }

    
    public bool AutoRemove;

}
