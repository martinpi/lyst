using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GPSController : MonoBehaviour {

	public Text DebugLog = null;
	public Transform Player = null;
	public float[] Distances;
	public float[] DistancesRaw;

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
		DistancesRaw = new float[6];
		Distances = new float[6];
	}

	float meterDistance(Vector2 origin, Vector2 point) {
		var R = 6378.137f; // Radius of earth in KM
		var dLat = (point.x - origin.x) * Mathf.PI / 180f;
		var dLon = (point.y - origin.y) * Mathf.PI / 180f;
		var a = Mathf.Sin(dLat/2) * Mathf.Sin(dLat/2f) +
			Mathf.Cos(origin.x * Mathf.PI / 180f) * Mathf.Cos(point.x * Mathf.PI / 180f) *
			Mathf.Sin(dLon/2) * Mathf.Sin(dLon/2);
		var c = 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f-a));
		var d = R * c;
		return d * 1000f; // meters
	}

	Vector3 pos3D(float latitude, float longitude) {
		var centre = new Vector3(0, 0, 0);
		var radius = 10f;

		Vector3 coordPosition = new Vector3();
		coordPosition.x = radius*Mathf.Cos(latitude*Mathf.Deg2Rad)*Mathf.Cos(longitude*Mathf.Deg2Rad);
		coordPosition.y = radius*Mathf.Cos(latitude*Mathf.Deg2Rad)*Mathf.Sin(longitude*Mathf.Deg2Rad);
		coordPosition.z = radius*Mathf.Sin(latitude*Mathf.Deg2Rad);
		coordPosition += centre;
		return coordPosition;
	}

	float latToZ (double lat){
		return (float)( (lat - 60.79379) * -137.2 / (60.79208 - 60.79379));
	}

	float lonToX (double lon){
		return (float)( (lon - 11.03634) * -190.5 / (11.03382 - 11.03634));
	}

	void Update() {
		if (!_active) {
			StartCoroutine(CheckActive());
			return;
		}

		if (Input.location.status == LocationServiceStatus.Failed) {
			DebugLog.text = "Unable to determine device location";

		} else {
			
			Vector2 pos = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
			Vector2 p0 = new Vector2(60.79379f, 11.03634f); // start
			Vector2 p1 = new Vector2(60.7929f, 11.03643f);  // stage
			Vector2 p2 = new Vector2(60.79295f, 11.03518f); // high clearing
			Vector2 p3 = new Vector2(60.79208f, 11.03382f); // High cliff by lake
			Vector2 p4 = new Vector2(60.79152f, 11.03595f); // Small cliff by lake
			Vector2 p5 = new Vector2(60.7925f, 11.03627f);  // Trees

			DistancesRaw[0] = meterDistance(pos, p0);
			DistancesRaw[1] = meterDistance(pos, p1);
			DistancesRaw[2] = meterDistance(pos, p2);
			DistancesRaw[3] = meterDistance(pos, p3);
			DistancesRaw[4] = meterDistance(pos, p4);
			DistancesRaw[5] = meterDistance(pos, p5);

			for (int i=0; i<DistancesRaw.Length; ++i) {
				Distances[i] = Distances[i] * 0.9f + DistancesRaw[i] * 0.1f;
			}

			float latM = 111412.0f;
			float lonM = 108553.3f;

			float px = (Input.location.lastData.latitude - 60.79379f) * latM;
			float py = (Input.location.lastData.longitude - 11.03634f) * lonM;

			float x = lonToX (Input.location.lastData.longitude);
			float z = latToZ (Input.location.lastData.latitude);

			string distances = "";
			for (int i=0; i<DistancesRaw.Length; ++i) {
				distances += "S"+i+" "+Distances[i];
			}

			DebugLog.text = 
				"LOCd " + Input.location.lastData.latitude + " / " + Input.location.lastData.longitude + 
				"\nLOCm " + x + " / " + z + "\n" + distances;
		
			Player.transform.position = new Vector3 (x, z, 0f);
		}

		Player.rotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);

	}
}
