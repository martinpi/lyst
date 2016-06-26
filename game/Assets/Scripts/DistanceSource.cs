using UnityEngine;
using System.Collections;
using Atomtwist.Audio;
using Kaae;
using Atomtwist;
using System.Linq;
using UnityEngine.Audio;

[ExecuteInEditMode]
public class DistanceSource : MonoBehaviour {


	public Location location;

	public AnimationCurve volumeCurve;

	public float startPulseTime;
	public float endPulseTime;

	public float radius = 5;
	public float timeBeforeMessage = 15;
	public float timeAfterMessage = 90;
	public AudioClip messageClip;
	public AudioSourcePRO message;
	public AudioSourcePRO jingle;
	public AudioClip[] clip;

	public AudioMixerGroup mixerGroup;
	public bool isPulsing;

	public bool solved;



	AudioSource s;
	bool inRadius;
	AudioListener listener;

	// Use this for initialization
	void Start () {
		if(!Application.isPlaying) return;
		s = GetComponent<AudioSource>();
		listener = FindObjectOfType<AudioListener>();
		s.outputAudioMixerGroup = mixerGroup;
		jingle.outputAudioMixerGroup = mixerGroup;
		if (location == Location.START || location == Location.STAGE)
			StartThisSource();
	}

	IEnumerator Pulse()
	{
		Debug.Log("Pulse");
		s.PlayOneShot(clip[Random.Range(0,clip.Length-1)]);
		yield return new WaitForSeconds(GetcurrentPulseTime());
		StartCoroutine(Pulse());
	}


	float GetcurrentPulseTime()
	{
		//distance between listener and spot 
		// mapped from minDistance to maxdistance of the curves
		// mapßped to min
		var currentPulseTime = DistanceToListener().ATScaleRange(s.maxDistance,s.minDistance,startPulseTime,endPulseTime);
		return currentPulseTime;
			
	}

	float GetcurrentVolume()
	{
		//distance between listener and spot 
		// mapped from minDistance to maxdistance of the curves
		// mapßped to min
		var currentVolume = volumeCurve.Evaluate(DistanceToListener().ATScaleRange(s.maxDistance,s.minDistance,0,1));
		return currentVolume;

	}

	[DebugButton]
	public void StartThisSource()
	{
		StartCoroutine(Pulse());
		isPulsing = true;
	}

	[DebugButton]
	public void StopThisSource()
	{
		StopAllCoroutines();
		isPulsing = false;
	}
		

	float DistanceToListener()
	{
		var distance = Vector3.Distance(transform.position,listener.transform.position);
		return distance;
	}


	bool messageStarted;
	void CheckDistanceToListener()
	{
		if (Vector3.Distance(transform.position,listener.transform.position) < radius)
		{
			inRadius = true;
			StopThisSource();

			if(!jingle.isPlaying)
			{
				if (!message.isPlaying && ! solved)
				{
					message.PlayDelayed(timeBeforeMessage, 0.5f);
					messageStarted = true;
				}
				jingle.Play(0.1f);
				StopAllOtherLocations();
				//FOR NOW
				solved = true;
			}
			if (messageStarted)
			{
				if(!message.isPlaying)
				{
					Invoke("WaitAfterSolved",timeAfterMessage);
				
				}
			}

		}
		if (Vector3.Distance(transform.position,listener.transform.position) > radius)
		{
			if(!inRadius) return;
		/*	if (jingle.isPlaying)
			{
				jingle.Stop(1.5f);
				StartAllOtherLocations();
				inRadius = false;
			}*/
		}
	}
		
	void WaitAfterSolved()
	{
		if (jingle.isPlaying)
		{
			jingle.Stop(4.5f);
			StartAllOtherLocations();
		}
	}


	void StopAllOtherLocations()
	{
		var locs = AppCommander.Instance.locations;
		//locs = locs.Where(l => !l == this).ToList();
		foreach (var item in locs) {
			item.StopThisSource();
		}
	}

	void StartAllOtherLocations()
	{
		var locs = AppCommander.Instance.locations;
		//locs = locs.Where(l => !l == this).ToList();
		foreach (var item in locs) {
			if (item.isPulsing || item.solved)
				continue;
			if (item != this)
			{
				if(location == Location.START && item.location == Location.STAGE)
					item.StartThisSource();
				if(location == Location.STAGE && item.location == Location.SEAFRONT)
					item.StartThisSource();
				if(location == Location.SEAFRONT && item.location == Location.HIGH_CLIFF)
					item.StartThisSource();
					
			}
		}
	}

	//if !solved wat x before plazing recording
	//wait x after plazing recording
	//mark soilved, enable all others


	void UpdateVolume()
	{
		s.volume = GetcurrentVolume();
	}

	void Update()
	{
		if(!Application.isPlaying) return;
		CheckDistanceToListener();
		UpdateVolume();
	}

}
