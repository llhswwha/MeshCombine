using UnityEngine;

public class OnUpdateableComponent : MonoBehaviour,IUpdateable
{
    void Start()
    {
        GameLogicSingletonComponent.Instance.RegisterUpdateableObject(this);
    }

    void OnDestroy()
    {
        if(GameLogicSingletonComponent.IsAlive){
            GameLogicSingletonComponent.Instance.RegisterUpdateableObject(this);
        }
    }

    public virtual void OnUpdate(float dt)
    {
        ProfilerTest.DoSomethingCompletelyStupid(UpdateTest.ObjectCount);
    }
}
