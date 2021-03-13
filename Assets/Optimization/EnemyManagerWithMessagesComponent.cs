using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerWithMessagesComponent : MonoBehaviour
{
    private List<GameObject> _enemies=new List<GameObject>();
    [SerializeField] private GameObject _enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        MessagingSystem.Instance.AttachListener(typeof(CreateEnemyMessage),this.HandleCreateEnemy);
    }

    public float Radio=10.0f;
    public int Count=0;

    bool HandleCreateEnemy(Message msg){
        Count++;
        CreateEnemyMessage castMsg=msg as CreateEnemyMessage;
        string[] names={"Tom","Dick","Harry"};
        GameObject enemy=GameObject.Instantiate(_enemyPrefab,Radio * Random.insideUnitSphere,Quaternion.identity);
        string enemyName=names[Random.Range(0,names.Length)];
        enemy.gameObject.name=enemyName+"_"+Count;
        _enemies.Add(enemy);
        MessagingSystem.Instance.QueueMessage(new EnemyCreatedMessage(enemy,enemyName));
        enemy.AddComponent<DynamicVisibleChanged>();
        return true;
    }

    void OnDestroy(){
        if(MessagingSystem.IsAlive){
            MessagingSystem.Instance.DetachListener(typeof(CreateEnemyMessage),this.HandleCreateEnemy);
        }
    }
}
