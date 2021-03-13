using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagingSystem : SingletonComponent<MessagingSystem>
{
    public delegate bool MessageHandlerDelegate(Message message);
    public static MessagingSystem Instance{
        get{
            return ((MessagingSystem)_Instance);
        }
        set{
            _Instance=value;
        }
    }
    private Dictionary<string,List<MessageHandlerDelegate>> _listenerDict=new Dictionary<string, List<MessageHandlerDelegate>>();

    public bool AttachListener(System.Type type,MessageHandlerDelegate handler){
        if(type==null){
            Debug.LogError("MessagingSystem:AttachListener failed due to having no message type specified");
            return false;
        }
        string msgType=type.Name;
        if(!_listenerDict.ContainsKey(msgType)){
            _listenerDict.Add(msgType,new List<MessageHandlerDelegate>());
        }
        List<MessageHandlerDelegate> listenerList=_listenerDict[msgType];
        if(listenerList.Contains(handler)){
            return false;//already in list
        }
        listenerList.Add(handler);
        return true;
    }

    private Queue<Message> _messageQueue=new Queue<Message>();

    public bool QueueMessage(Message msg)
    {
        if(!_listenerDict.ContainsKey(msg.type)){
            return false;//没有处理消息的对象
        }
        _messageQueue.Enqueue(msg);
        return true;
    }

    private const int _maxQueueProcessingTime=16667;
    private System.Diagnostics.Stopwatch timer=new System.Diagnostics.Stopwatch();

    void Update(){
        timer.Start();
        while(_messageQueue.Count>0){
            if(_maxQueueProcessingTime>0.0f){
                if(timer.Elapsed.Milliseconds>_maxQueueProcessingTime)//基于时间的保护措施
                {
                    timer.Stop();
                    return;
                }
            }
            Message msg=_messageQueue.Dequeue();
            if(!TriggerMessage(msg)){
                Debug.LogError("Error when processing message:"+msg.type);
            }
        }
    }

    public bool TriggerMessage(Message msg){
        string msgType=msg.type;
        if(!_listenerDict.ContainsKey(msgType)){
            Debug.Log(" MessagingSystem:Message \""+msgType+"\" has no listeners!");
            return false;
        }
        List<MessageHandlerDelegate> listenerList=_listenerDict[msgType];
        for(int i=0;i<listenerList.Count;++i){
            if(listenerList[i](msg))
                return true;//message consumed by the delegate
        }
        return true;
    }

    public bool DetachListener(System.Type type,MessageHandlerDelegate handler)
    {
        if(type==null){
            Debug.LogError("MessagingSystem:DetachListener failed due to having no message type specified");
            return false;
        }
        string msgType=type.Name;
        if(!_listenerDict.ContainsKey(msgType)){
            return false;
        }
        List<MessageHandlerDelegate> listenerList=_listenerDict[msgType];
        if(!listenerList.Contains(handler)){
            return false;//not in list
        }
        listenerList.Remove(handler);
        return true;
    }
}