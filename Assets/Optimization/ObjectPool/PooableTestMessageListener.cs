using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooableTestMessageListener : MonoBehaviour,IPoolableComponent
{
  public void Spawned()
  {
    MessagingSystem.Instance.AttachListener(typeof(MyCustomMessage),this.HandleMyCustomMessage);
  }

  bool HandleMyCustomMessage(Message msg){
      MyCustomMessage castMsg=msg as MyCustomMessage;
      Debug.Log(string.Format("Got the message! {0},{1}",castMsg._intValue,castMsg._floatValue));
      return true;
  }

  public void Despawned()
  {
    MessagingSystem.Instance.DetachListener(typeof(MyCustomMessage),this.HandleMyCustomMessage);
  }
}


