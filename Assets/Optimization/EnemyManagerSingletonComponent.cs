using UnityEngine;

public class EnemyManagerSingletonComponent : SingletonComponent<EnemyManagerSingletonComponent>
{
    public static EnemyManagerSingletonComponent Instance{
        get{
            return ((EnemyManagerSingletonComponent)_Instance);
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
