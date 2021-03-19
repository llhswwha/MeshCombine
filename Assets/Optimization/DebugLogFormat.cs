using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogFormat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("<color=red>[ERROR]</color>This is a <i>very</i><size=14><b>specific</b></size> kind of log message");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
