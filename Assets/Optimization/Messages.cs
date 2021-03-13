using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string type;

    public Message(){
        type=this.GetType().Name;
    }
}

public class CreateEnemyMessage : Message
{

}

public class EnemyCreatedMessage : Message
{
    public readonly GameObject enemyObject;

    public readonly string enemyName;

    public EnemyCreatedMessage(GameObject enemyObject,string enemyName)
    {
        this.enemyObject=enemyObject;
        this.enemyName=enemyName;
    }
}
