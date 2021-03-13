using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerSingletonComponent : SingletonComponent<PlayerManagerSingletonComponent>
{
    public static PlayerManagerSingletonComponent Instance{
        get{
            return ((PlayerManagerSingletonComponent)_Instance);
        }
        set { _Instance =value;}
    }

    private void Awake(){
        DontDestroyOnLoad(this.gameObject);
    }

    public void CreateEnemy(GameObject prefab){
        //same as StaticEnemyManager
        Debug.Log("CreateEnemy:"+prefab);
    }

    public void KillAll(){
        //same as StaticEnemyManager
        Debug.Log("KillAll");
    }
}
