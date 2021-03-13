using UnityEngine;

public class EnemyCreatedListenerComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MessagingSystem.Instance.AttachListener(typeof(EnemyCreatedMessage),this.HandleEnemyCreated);
    }

    bool HandleEnemyCreated(Message msg){
        EnemyCreatedMessage castMsg=msg as EnemyCreatedMessage;
        Debug.Log(string.Format("A new enemy was created ! {0}",castMsg.enemyName));
        return true;
    }
}
