using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPSController : MonoBehaviour {

	public Text DebugLog = null;
	public Transform Player = null;

	private bool _active = false;

	IEnumerator CheckActive() {
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser)
			yield break;

		// Start service before querying location
		Input.location.Start(5f,5f);
		Input.compass.enabled = true;

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			DebugLog.text = "Timed out";
			yield break;
		}

		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			DebugLog.text = "Unable to determine device location";
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			DebugLog.text = "Location: " + Input.location.lastData.latitude + " / " + Input.location.lastData.longitude + 
				"\n" + Input.location.lastData.altitude + 
				"\n" + Input.location.lastData.horizontalAccuracy + 
				"\n" + Input.location.lastData.timestamp;
		}
		_active = true;

	}

	void Start() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		StartCoroutine(CheckActive());
	}

	double meterDistance(Vector2 origin, Vector2 point) {
		var R = 6378.137; // Radius of earth in KM
		var dLat = (point.x - origin.x) * Mathf.PI / 180f;
		var dLon = (point.y - origin.y) * Mathf.PI / 180f;
		var a = Mathf.Sin(dLat/2) * Mathf.Sin(dLat/2f) +
			Mathf.Cos(origin.x * Mathf.PI / 180f) * Mathf.Cos(point.x * Mathf.PI / 180f) *
			Mathf.Sin(dLon/2) * Mathf.Sin(dLon/2);
		var c = 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f-a));
		var d = R * c;
		return d * 1000f; // meters
	}

	void Update() {
		if (!_active) {
			StartCoroutine(CheckActive());
			return;
		}

		if (Input.location.status == LocationServiceStatus.Failed) {
			DebugLog.text = "Unable to determine device location";

		} else {
			// Access granted and location value could be retrieved
			DebugLog.text = "LOC " + Input.location.lastData.latitude + " / " + Input.location.lastData.longitude + 
				"\nALT " + Input.location.lastData.altitude + 
				"\nACC " + Input.location.lastData.horizontalAccuracy + 
				"\nTIME " + Input.location.lastData.timestamp;
		}

		Player.rotation = Quaternion.Euler(0, 0, -Input.compass.trueHeading);

	}
}
