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
			SendDeviceData ("v1.012");

			Debug.Log ("Data sent");
		}
	}


	public void SendDeviceData (string DFVersion)
	{

		GoogleAnalyticsV4.instance.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (3, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Graphics Card")
			.SetEventAction (SystemInfo.graphicsDeviceVendor)
			.SetEventLabel (SystemInfo.graphicsDeviceName)
			.SetEventValue (1)
			.Validate ()
		);

		GoogleAnalyticsV4.instance.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (3, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Processor")
			.SetEventAction ("Core number")
			.SetEventLabel (SystemInfo.processorCount.ToString ())
			.SetEventValue (1)
			.Validate ()
		);

		GoogleAnalyticsV4.instance.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (3, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Processor")
			.SetEventAction ("Core frequency")
			.SetEventLabel (SystemInfo.processorFrequency.ToString ())
			.SetEventValue (1)
			.Validate ()
		);

		GoogleAnalyticsV4.instance.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (3, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("OS Version")
			.SetEventAction (SystemInfo.operatingSystem)
			.SetEventLabel (SystemInfo.operatingSystem)
			.SetEventValue (1)
			.Validate ()
		);


		SendDwarfFortressVesion (DFVersion);
	}

	void SendDwarfFortressVesion (string version)
	{
		GoogleAnalyticsV4.instance.LogEvent (new EventHitBuilder ()
			.SetCustomDimension (3, SystemInfo.operatingSystemFamily.ToString ())
			.SetEventCategory ("Program Version")
			.SetEventAction ("Dwarf Fortress Version")
			.SetEventLabel (version)
			.SetEventValue (1)
			.Validate ()
		);
	}
}
