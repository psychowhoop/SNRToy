using UniFramework.Event;
using UnityEngine;


namespace DefEventDefine
{
    public class EvtYooPackageInitComplete : IEventMessage
    {
        public System.Object pasData { get; set; }
        public string packageName = "";
        public static void Send(string packageName = "")
        {
            var msg = new EvtYooPackageInitComplete();
            msg.packageName = packageName;
            UniEvent.SendMessage(msg);
        }
    }

    public class EvtChangeToHallScene : IEventMessage
    {
        public System.Object pasData { get; set; }
        public static void Send()
        {
            var msg = new EvtChangeToHallScene();
            UniEvent.SendMessage(msg);
        }
    }


    public class EvtMsg : IEventMessage
    {
        public System.Object pasData { get; set; }

    }

    // public class EvtSetGManagerBehavior : EvtMsg
    // {
    //     public static void Send(MonoBehaviour sData)
    //     {
    //         var msg = new EvtSetGManagerBehavior();
    //         msg.pasData = sData;
    //         UniEvent.SendMessage(msg);
    //     }
    // }

}

