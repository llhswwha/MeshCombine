using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTest : MonoBehaviour
{
    public bool IsUpdate=false;

    public int scriptCount=1000;

    public int objCount=100000;

    public static int ObjectCount=0;

    void Start()
    {
        ObjectCount=objCount;
        if(IsUpdate)
        {
            for(int i=0;i<scriptCount;i++)
            {
                this.gameObject.AddComponent<UpdateComponent>();
            }
        }
        else{
            for(int i=0;i<scriptCount;i++)
            {
                this.gameObject.AddComponent<OnUpdateableComponent>();
            }
        }
    }

}
