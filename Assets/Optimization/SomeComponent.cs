using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeComponent : MonoBehaviour
{
    void OnDestroy(){
        Debug.Log("SomeComponent.OnDestroy");
        if(EnemyManagerSingletonComponent.IsAlive){
            EnemyManagerSingletonComponent.Instance.KillAll();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        EnemyManagerSingletonComponent.Instance.KillAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
