using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kaae;


public enum Location
{
	START,
	STAGE,
	SEAFRONT,
	HIGH_CLEARING,
	HIGH_CLIFF,
	SMALL_CLIFF,
	TREES,
}

[ExecuteInEditMode]
public class AppCommander : Singleton<AppCommander> {

	public List<DistanceSource> locations = new List<DistanceSource>();

	// Use this for initialization
	void OnEnable () {
		GetLocations();
		Application.targetFrameRate = 60;
	}

	[DebugButton]
	void GetLocations()
	{
		locations.Clear();
		var p = FindObjectsOfType<DistanceSource>(); 
		foreach (var loc in p) {
			locations.Add(loc);
		}
	}

	public void SetDistance(Location loc, float distance )
	{
		foreach (var l in locations) {
			if (l.location == loc)
				l.transform.position = new Vector3(distance,0,0);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
