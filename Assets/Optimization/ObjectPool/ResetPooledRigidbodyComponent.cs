using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPooledRigidbodyComponent : MonoBehaviour,IPoolableComponent
{
  [SerializeField] Rigidbody _body;
  
  public void Spawned()
  {
    
  }
  public void Despawned()
  {
    if(_body==null){
        _body=GetComponent<Rigidbody>();
        if(_body==null){
            return;
        }
    }
    _body.velocity=Vector3.zero;
    _body.angularVelocity=Vector3.zero;
  }
}
