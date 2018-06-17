using System;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A collection of whatever status numbers we might need. It should be writable from any thread.
/// </summary>
[RequireComponent(typeof(Text))]
public class StatsReadout : MonoBehaviour
{
    private static int _blocksDrawn;
    public static int BlocksDrawn
    {
        get { return _blocksDrawn; }
        set { Interlocked.Exchange(ref _blocksDrawn, value); }
    }
    private static int _queueLength;
    public static int QueueLength
    {
        get { return _queueLength; }
        set { Interlocked.Exchange(ref _queueLength, value); }
    }
    private static long _blockProcessTime;
    public static TimeSpan BlockProcessTime
    {
        get { return new TimeSpan(_blockProcessTime); }
        set { Interlocked.Exchange(ref _blockProcessTime, value.Ticks); }
    }



    private Text text;

    public void Awake()
    {
        text = GetComponent<Text>();
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    public void LateUpdate()
    {
        StringBuilder updateBuilder = new StringBuilder();
        updateBuilder.Append("Blocks drawn: ").Append(BlocksDrawn).AppendLine();
        updateBuilder.Append("Blocks waiting to be processed: ").Append(QueueLength).AppendLine();
        updateBuilder.Append("Block processing time: ").Append(BlockProcessTime.Milliseconds).Append("ms (").Append((Time.unscaledDeltaTime / BlockProcessTime.TotalSeconds).ToString("F2")).Append(" blocks per frame)").AppendLine();
        text.text = updateBuilder.ToString();
    }
}
