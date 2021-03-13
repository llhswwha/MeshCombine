using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicVisibleChanged : MonoBehaviour
{
    void OnBecameVisible(){
        Debug.Log("OnBecameVisible:"+gameObject.name);
        enabled=true;
    }
    void OnBecameInvisible(){
        Debug.LogError("OnBecameInvisible"+gameObject.name);
        enabled=false;
    }
}
