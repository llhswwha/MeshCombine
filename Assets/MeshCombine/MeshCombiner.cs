using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
//[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{
    public int CombineMode=0;//0:sourceRoot合并为一个模型，1:sourceRoot的子物体分别合并为一个模型

    public GameObject sourceRoot;

    public List<GameObject> sourceList=new List<GameObject>();

    public List<GameObject> resultList=new List<GameObject>();

    public bool Auto=false;

    public bool IsCombine=false;

    public MeshCombinerSetting Setting;

    // public bool IsDestroySource=false;
    // public bool IsCoroutine=false;

    // public int WaitCount=10000;


    // Start is called before the first frame update
    void Start()
    {
        if(Auto)
        {
            CombineByMaterial();
        }

        LoadScenes();
    }

    private void InitSetting(){
        if(Setting==null){
            Setting=MeshCombinerSetting.Instance;
        }
        if(Setting==null){
            Setting=this.gameObject.AddComponent<MeshCombinerSetting>();
        }
    }

    // public  CombinedMesh combinedMesh;


    [ContextMenu("Combine")]

    public void Combine(){
        CombineEx(2);
    }

    public bool CombineBySub=false;

    public List<MeshCombiner> SubCombiners=new List<MeshCombiner>();

    public bool IsSaveToScene=false;

    public List<string> modelScenes=new List<string>();

    private void CombineEx(int mode)
    {
        InitSetting();

        Debug.LogError("Start CombineEx mode:"+mode+"|"+gameObject);
        resultList=new List<GameObject>();
        string sourceName="";
        // if((sourceList.Count==0 || CombineMode == 0)&&sourceRoot!=null){
        //     sourceList=new List<GameObject>();
        //     if(CombineMode==0){
        //         sourceList.Add(sourceRoot);
        //         sourceName+=sourceRoot.name;
        //     }
        //     else{
        //         for(int i=0;i<sourceRoot.transform.childCount;i++){
        //             GameObject child=sourceRoot.transform.GetChild(i).gameObject;
        //             sourceList.Add(child);
        //             sourceName+=child.name+";";
        //         }
        //     }
        // }

        if(CombineMode==1 && sourceRoot!=null){
            sourceList=new List<GameObject>();
            for(int i=0;i<sourceRoot.transform.childCount;i++){
                    GameObject child=sourceRoot.transform.GetChild(i).gameObject;
                    sourceList.Add(child);
                    sourceName+=child.name+";";
            }
        }
        else if(CombineMode==0 && sourceRoot!=null){
            sourceList=new List<GameObject>();
            if(CombineMode==0){
                sourceList.Add(sourceRoot);
                sourceName+=sourceRoot.name;
            }
        }

        Debug.LogError("sourceList:"+sourceName);
        // foreach(var source in sourceList){
        //     if(source==null)continue;

        //     if(CombineBySub){
        //         MeshCombiner subCombiner=gameObject.AddComponent<MeshCombiner>();
        //         subCombiner.Auto=true;
        //         //subCombiner.IsCoroutine=this.IsCoroutine;
        //         subCombiner.CombineBySub=false;
        //         //subCombiner.WaitCount=this.WaitCount;
        //         //subCombiner.IsDestroySource=false;
        //         subCombiner.sourceList.Add(source);
        //         SubCombiners.Add(subCombiner);
        //     }
        //     else{
        //         if(Setting.IsCoroutine){
        //             StartCoroutine(MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,mode,Setting.WaitCount));
        //         }
        //         else{
        //             GameObject target=MeshCombineHelper.CombineEx(source,mode);
        //             resultList.Add(target);
        //             Debug.Log("Combine:"+source+"->"+target);
        //             if(Setting.IsDestroySource){
        //                 GameObject.Destroy(source);
        //             }
        //         }

        //     }
        // }

        if(Setting.IsCoroutine){
            //合并
            StartCoroutine(CombineEx_Coroutine(mode,t=>{
                Debug.Log("Result:"+t);
                if(IsSaveToScene){
                    SaveToScene(t);
                }
            }));
        }
        else{
            for (int i = 0; i < sourceList.Count; i++)
            {
                GameObject source = sourceList[i];
                Debug.LogWarning(string.Format("CombineEx {0} ({1}/{2})",source,i+1,sourceList.Count));
                if(source==null)continue;

                if(CombineBySub){
                    MeshCombiner subCombiner=gameObject.AddComponent<MeshCombiner>();
                    subCombiner.Auto=true;
                    //subCombiner.IsCoroutine=this.IsCoroutine;
                    subCombiner.CombineBySub=false;
                    //subCombiner.WaitCount=this.WaitCount;
                    //subCombiner.IsDestroySource=false;
                    subCombiner.sourceList.Add(source);
                    SubCombiners.Add(subCombiner);
                }
                else{
                    // if(Setting.IsCoroutine){
                    //     StartCoroutine(MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,mode,Setting.WaitCount));
                    // }
                    // else
                    {
                        GameObject target=MeshCombineHelper.CombineEx(source,mode);//合并
                        resultList.Add(target);
                        Debug.Log("Combine:"+source+"->"+target);
                        if(Setting.IsDestroySource){
                            GameObject.Destroy(source);
                        }
                        // Scene scene=SceneManager.CreateScene(target.name);
                        // SceneManager.MoveGameObjectToScene(target, scene);
                        if(IsSaveToScene){
                            SaveToScene(target);
                        }
                        
                    }

                }
            }
        }
    }

    private void SaveToScene(GameObject target){
        #if UNITY_EDITOR
        Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name=target.name;
        target.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(target, scene);
        Debug.LogError("path:"+(EditorSceneManager.GetActiveScene().path));
        //string[] path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
        //path[path.Length - 1] = "AutoSave_" + path[path.Length - 1];
        //bool saveOK = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), string.Join("/", path));
        string path="Assets\\CombinedScene\\"+target.name+".unity";
        EditorSceneManager.SaveScene(scene,path);  
        modelScenes.Add(path);   
        #endif
    }

    private void LoadScenes(){
        StartCoroutine(LoadYourAsyncScenes());
    }

    IEnumerator LoadYourAsyncScenes()
    {
        foreach(string path in modelScenes)
        {
            if(string.IsNullOrEmpty(path))continue;
            Debug.Log("path:"+path);
            Scene scene=SceneManager.GetSceneByPath(path);
            if(scene==null)continue;
            string sceneName=path.Replace("Assets\\","");//Assets\CombinedScene\Group1 (1)_Combined.unity
            sceneName=sceneName.Replace("\\","/");
            sceneName=sceneName.Replace(".unity","");
            //SceneManager.LoadSceneAsync(scene.name);
            Debug.Log("load:"+scene.name+"|"+sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.


    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator CombineEx_Coroutine(int mode,Action<GameObject> callback)
    {
        DateTime start=DateTime.Now;
        for (int i = 0; i < sourceList.Count; i++)
        {
            GameObject source = (GameObject)sourceList[i];
             Debug.LogError(string.Format("CombineEx {0} ({1}/{2})",source,i+1,sourceList.Count));
            if (source!=null)
            {
                yield return MeshCombineHelper.CombineEx_Coroutine(source,Setting.IsDestroySource,Setting.WaitCount,mode,callback);
            }
        }
        Setting.WriteLog("完成合并 用时:"+(DateTime.Now-start));
        yield return null;
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
