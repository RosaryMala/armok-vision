using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RemoteFortressReader;
using DF.Enums;

public class ReportManager : MonoBehaviour
{
    public Canvas speechBubble;

    HashSet<int> usedIDs = new HashSet<int>();

    List<Report> condensedReports = new List<Report>();
    // Update is called once per frame
    void Update()
    {
        condensedReports.Clear();
        var status = DFConnection.Instance.PopStatusUpdate();
        if (status != null)
            foreach (var report in status.reports)
            {
                if(usedIDs.Contains(report.id))
                    continue;
                if(report.continuation && condensedReports.Count > 0)
                    condensedReports[condensedReports.Count - 1].text += (" " + report.text);
                else
                    condensedReports.Add(report);
                usedIDs.Add(report.id);
            }
        foreach(var report in condensedReports)
        {
            switch((AnnouncementType)report.type)
            {
                case AnnouncementType.REGULAR_CONVERSATION:
                case AnnouncementType.CONFLICT_CONVERSATION:
                    var bubble = Instantiate(speechBubble, GameMap.DFtoUnityCoord(report.pos), Quaternion.identity, transform);
                    bubble.GetComponentInChildren<Text>().text = report.text;
                    break;
                default:
                    OnscreenConsole.ShowMessage(report.text, 30, new Color32((byte)report.color.red, (byte)report.color.green, (byte)report.color.blue, 255));
                    break;
            }
        }
    }
}
