using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreatorComponent : MonoBehaviour
{
    public int InitCount=1000;

    void Start(){
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<InitCount;i++){
                CreateEnemy();
        }
        InitCount=0;
        if(Input.GetKeyDown(KeyCode.Space)){
            CreateEnemy();
        }
    }

    private void CreateEnemy(){
        MessagingSystem.Instance.QueueMessage(new CreateEnemyMessage());
    }
}
