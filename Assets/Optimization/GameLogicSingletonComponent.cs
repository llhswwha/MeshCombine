﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicSingletonComponent : SingletonComponent<GameLogicSingletonComponent>
{
    public static GameLogicSingletonComponent Instance{
        get{
            return ((GameLogicSingletonComponent)_Instance);
        }
        set{
            _Instance=value;
        }
    }
   
    List<IUpdateable> _updateableObjects=new List<IUpdateable>();
    public void RegisterUpdateableObject(IUpdateable obj){
        if(!_updateableObjects.Contains(obj))
        {
            _updateableObjects.Add(obj);
        }
    }

    public void DeregisterUpdateableObject(IUpdateable obj){
        if(_updateableObjects.Contains(obj)){
            _updateableObjects.Remove(obj);
        }
    }

    void Update()
    {
        float dt=Time.deltaTime;
        for(int i=0;i<_updateableObjects.Count;++i)
        {
            _updateableObjects[i].OnUpdate(dt);
        }
    }
}
