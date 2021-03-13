﻿using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T :SingletonComponent<T>
{
    private static T _instance;

    protected static SingletonComponent<T> _Instance {
        get{
            if(!_instance){
                T[] managers=GameObject.FindObjectsOfType(typeof(T)) as T[];
                if(managers!=null){
                    if(managers.Length==1){
                        _instance=managers[0];
                        return _instance;
                    }
                    else if(managers.Length>1){
                        Debug.LogError("You have more than one "+ typeof(T).Name+" in the Scene. You only need one - it's a singleton!");
                        for(int i=0;i<managers.Length;++i){
                            T manager=managers[i];
                            Destroy(manager.gameObject);
                        }
                    }
                }
                GameObject go=new GameObject(typeof(T).Name,typeof(T));
                _instance=go.GetComponent<T>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
        set{
            _instance=value as T;
        }
    }

    private bool _alive=true;
    void OnDestroy(){
        _alive=false;
    }
    void OnApplicationQuit(){
        _alive=false;
    }

    public static bool IsAlive{
        get{
            if(_instance==null){
                return false;
            }
            return _instance._alive;
        }
    }
}
