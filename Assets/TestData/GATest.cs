using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GATest : MonoBehaviour {

	float timer = 1;
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (itr ());
	}

	//for testing
	IEnumerator itr ()
	{
		while (true) {
			yield return new WaitForSeconds (timer);
			GoogleAnalyticsV4.instance.SendDeviceData ("v1.012", "1.0");
			Debug.Log ("Data sent");
		}
	}

}

public static class GoogleAnalyticExtensions{
	/// <summary>
	/// Will only send data if not in Editor mode.
	/// </summary>
	/// <param name="GA">G.</param>
	/// <param name="DFVersion">Dwarf Fortress version.</param>
	/// <param name="pluginVersion">Plugin version.</param>
	public static void SendDeviceData (this GoogleAnalyticsV4 GA, string DFVersion , string pluginVersion)
	{
		#if !UNITY_EDITOR

		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Graphics Card")
			.SetEventAction (SystemInfo.graphicsDeviceVendor)
			.SetEventLabel (SystemInfo.graphicsDeviceName)
			.SetEventValue (1)
			.Validate ()
		);

		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Processor")
			.SetEventAction ("Core number")
			.SetEventLabel (SystemInfo.processorCount.ToString ())
			.SetEventValue (1)
			.Validate ()
		);

		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Processor")
			.SetEventAction ("Core frequency")
			.SetEventLabel (SystemInfo.processorFrequency.ToString ())
			.SetEventValue (1)
			.Validate ()
		);

		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("OS Version")
			.SetEventAction (SystemInfo.operatingSystem)
			.SetEventLabel (SystemInfo.operatingSystem)
			.SetEventValue (1)
			.Validate ()
		);


		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Program Version")
			.SetEventAction ("Dwarf Fortress Version")
			.SetEventLabel (DFVersion)
			.SetEventValue (1)
			.Validate ()
		);

		GA.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (1, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Program Version")
			.SetEventAction ("Plugin Version")
			.SetEventLabel (pluginVersion)
			.SetEventValue (1)
			.Validate ()
		);
		#endif

	}


}