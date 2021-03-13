using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilerTest : MonoBehaviour
{
    public int count1=10000;
    public int count2=1000;

    public int numTests=1000;

    public bool IsCoroutine=false;
    void Start()
    {
        // using(new CustomTimer("MyTest",numTests))
        // {
        //     for(int i=0;i<numTests;++i){
        //         DoSomethingCompletelyStupid(count2);
        //     }
        // }

        if(IsCoroutine)
            StartCoroutine(DoCoroutine());

        if(IsRepeat){
            InvokeRepeating("DoTest",0f,delay);
        }
    }

    public bool IsRepeat=false;

    public float delay=0.2f;

    private IEnumerator DoCoroutine()
    {
        DoSomethingCompletelyStupid(count2);
        yield return new WaitForSeconds(delay);
    }

    public bool IsUpdate=false;

    // Update is called once per frame
    void Update()
    {
        if(IsUpdate)
            DoSomethingCompletelyStupid(count2);
    }

    [ContextMenu("DoTest")]
    private void DoTest(){
        DoSomethingCompletelyStupid(count2);
    }

    public static void DoSomethingCompletelyStupid(int count){
        UnityEngine.Profiling.Profiler.BeginSample("My Profiler Sample");
        List<int> listOfInts=new List<int>();
        //string s="";
        for(int i=0;i<count;i++)
        {
            listOfInts.Add(i);
            //s+=i+",";
            //DoNone(10);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private static void DoNone(int count){
        //UnityEngine.Profiling.Profiler.BeginSample("DoNone");
        int s=0;
        for(int i=0;i<count;i++)
        {
            s++;
        }
        //UnityEngine.Profiling.Profiler.EndSample();
    }
}
