using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnscreenConsole : MonoBehaviour
{
    class Log
    {
        public string logString { get; set; }
        public string stackTrace { get; set; }
        public LogType type { get; set; }
        public Log(string logString, string stackTrace, LogType type)
        {
            this.logString = logString;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }
    object queueLock = new object();
    Queue<Log> queue = new Queue<Log>();

    public RectTransform logParent;
    public Text logItem;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        logParent.gameObject.SetActive(true);
        //loop a completely arbitary amount to prevent infinite loops
        for (int i = 0; i < 100; i++)
        {
            Log log;
            lock (queueLock)
            {
                if (queue.Count <= 0)
                    break;
                log = queue.Dequeue();
            }
            if (string.IsNullOrEmpty(log.logString))
                continue;
            var item = Instantiate(logItem, logParent);
            item.text = log.logString;
            item.rectTransform.SetAsLastSibling();
            switch (log.type)
            {
                case LogType.Error:
                    item.color = Color.red;
                    item.GetComponent<FadeAndDie>().startfade = 20;
                    break;
                case LogType.Assert:
                    item.color = Color.magenta;
                    item.GetComponent<FadeAndDie>().startfade = 20;
                    break;
                case LogType.Warning:
                    item.color = Color.yellow;
                    item.GetComponent<FadeAndDie>().startfade = 10;
                    break;
                case LogType.Log:
                    item.color = Color.white;
                    break;
                case LogType.Exception:
                    item.color = new Color(1, 0.5f, 0);
                    item.GetComponent<FadeAndDie>().startfade = 20;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        lock(queueLock)
        {
            queue.Enqueue(new Log(logString, stackTrace, type));
        }
    }
}
