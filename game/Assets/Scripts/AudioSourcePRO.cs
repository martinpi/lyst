using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Atomtwist.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioSourcePRO : MonoBehaviour {
		

		private AudioSource audio;
		private AudioLowPassFilter filter;

		public virtual void OnEnable()
		{
			audio = GetComponent<AudioSource>();
			filter = gameObject.AddComponent<AudioLowPassFilter>();
		}

		#region Properties

		public AudioClip clip {
			get {return audio.clip;}
			set {audio.clip = value;}
		}
		public float dopplerLevel {
			get {return audio.dopplerLevel;}
			set {audio.dopplerLevel = value;}
		}
		public bool enabled {
			get {return audio.enabled;}
			set {audio.enabled = value;}
		}

		public AnimationCurve GetCustomCurve(AudioSourceCurveType type)
		{
			return audio.GetCustomCurve(type);
		}
			
		public bool ignoreListenerPause
		{
			get {return audio.ignoreListenerPause;}
			set {audio.ignoreListenerPause = value;}
		}

		public bool ignoreListenerVolume
		{
			get {return audio.ignoreListenerVolume;}
			set {audio.ignoreListenerVolume = value;}
		}

		public bool isPlaying
		{
			get {return audio.isPlaying;}
		}

		public bool loop
		{
			get {return audio.loop;}
			set {audio.loop = value;}
		}

		public float maxDistance
		{
			get {return audio.maxDistance;}
			set {audio.maxDistance = value;}
		}

		public float minDistance
		{
			get {return audio.minDistance;}
			set {audio.minDistance = value;}
		}

		public bool mute
		{
			get {return audio.mute;}
			set {audio.mute = value;}
		}

		public AudioMixerGroup outputAudioMixerGroup
		{
			get {return audio.outputAudioMixerGroup;}
			set {audio.outputAudioMixerGroup = value;}
		}

		public float panStereo
		{
			get {return audio.panStereo;}
			set {audio.panStereo = value;}
		}

		public float pitch
		{
			get {return audio.pitch;}
			set {audio.pitch = value;}
		}

		public bool playOnAwake
		{
			get {return audio.playOnAwake;}
			set {audio.playOnAwake = value;}
		}

		public int priority
		{
			get {return audio.priority;}
			set {audio.priority = value;}
		}

		public AudioRolloffMode rollOffMode
		{
			get {return audio.rolloffMode;}
			set {audio.rolloffMode = value;}
		}
        /// <summary>
        /// 0 = 2D; 1 = 3D
        /// </summary>
		public float spatialBlend
		{
			get {return audio.spatialBlend;}
			set {audio.spatialBlend = value;}
		}

		public bool spatialize
		{
			get {return audio.spatialize;}
			set {audio.spatialize = value;}
		}

		public float spread
		{
			get {return audio.spread;}
			set {audio.spread = value;}
		}

		public float time
		{
			get {return audio.time;}
			set {audio.time = value;}
		}

		public int timeSamples
		{
			get {return audio.timeSamples;}
			set {audio.timeSamples = value;}
		}
			
		public AudioVelocityUpdateMode velocityUpdateMode
		{
			get {return audio.velocityUpdateMode;}
			set {audio.velocityUpdateMode = value;}
		}

		public float volume
		{
			get {return audio.volume;}
			set {audio.volume = value;}
		}

		//filter
		public float lopassFrequency
		{
			get {return filter.cutoffFrequency;}
			set {filter.cutoffFrequency = value;}
		}

		public float lopassResonance
		{
			get {return filter.lowpassResonanceQ;}
			set {filter.lowpassResonanceQ = value;}
		}

		#endregion


		#region Methods

		public AudioSourcePRO Pause()
		{
			audio.Pause();
			return this;
		}

		public AudioSourcePRO Play()
		{
			audio.Play();
			return this;
		}

		/// <summary>
		/// Play with fade in.
		/// </summary>
		/// <param name="fadeIn">Fade time</param>
		public AudioSourcePRO Play(float fadeIn)
		{
			audio.Play();
			StartFadeIn(fadeIn);
			return this;
		}

		public AudioSourcePRO PlayDelayed(float delay)
		{
			audio.PlayDelayed(delay);
			return this;
		}

		public AudioSourcePRO PlayDelayed(float delay, float fadeIn)
		{
			audio.PlayDelayed(delay);
			StartFadeIn(fadeIn);
			return this;
		}

		public AudioSourcePRO PlayOneShot(AudioClip clip)
		{
			audio.PlayOneShot(clip);
			return this;
		}

		public AudioSourcePRO PlayOneshot(AudioClip clip, float volumeScale)
		{
			audio.PlayOneShot(clip,volumeScale);
			return this;
		}

		public AudioSourcePRO PlayScheduled(double time)
		{
			audio.PlayScheduled(AudioSettings.dspTime + time);
			return this;
		}

		/// <summary>
		/// Play scheduled with fade in.
		/// </summary>
		/// <param name="time">Schedule time</param>
		/// <param name="fadeIn">fade in time</param>
		public AudioSourcePRO PlayScheduled(double time, float fadeIn)
		{
			audio.PlayScheduled(AudioSettings.dspTime + time);
			StartFadeIn(fadeIn, (float)time);
			return this;
		}

		public void SetCustomCurve(AudioSourceCurveType type, AnimationCurve curve)
		{
			audio.SetCustomCurve(type, curve);
		}

		public void SetScheduledEndTime(double time)
		{
			audio.SetScheduledEndTime(time);
		}

		public void SetScheduledStartTime(double time)
		{
			audio.SetScheduledStartTime(time);
		}

		public void Stop()
		{
			audio.Stop();
		}

		public void Stop(float fadeOut)
		{
			audio.SetScheduledEndTime(AudioSettings.dspTime + fadeOut);
			StartFadeOut(fadeOut);
		}

		public void UnPause()
		{
			audio.UnPause();
		}

		//just a few missing

		#endregion



		#region Fading

		bool fadingIn;
		void StartFadeIn(float fadeIn, float delayTime = 0)
		{
			timeInSamples = AudioSettings.outputSampleRate * 2 * fadeIn;
			delayTimeInSamples = AudioSettings.outputSampleRate * 2 * delayTime;
			delayCounter = 0;
			gain = 0;
			counter =0;
			fadingOut = false;
			fadingIn = true;
		}

		bool fadingOut;
		void StartFadeOut(float fadeOut)
		{
			timeInSamples = AudioSettings.outputSampleRate * 2 * fadeOut;
			//gain = 1;
			counter =0;
			fadingIn = false;
			fadingOut = true;
		}

		float timeInSamples;
		float delayTimeInSamples;
		double delayCounter;
		double gain;
		int counter;
		void OnAudioFilterRead(float[] data, int channels)
		{
			for (var i = 0; i < data.Length; ++i)
			{

				if (fadingIn)
				{
					delayCounter += 1/delayTimeInSamples;
					if(delayCounter > 1)
					{
						gain += 1/timeInSamples;	
					}
					counter++;
					if (gain > 1)
					{
						gain = 1;
						fadingIn = false;
					}
				}

				if (fadingOut)
				{
					gain -= 1/timeInSamples;
					counter++;
					if (gain < 0)
					{
						gain = 0;
						fadingOut = false;
					}
				}

				data[i] = Mathf.Clamp( data[i] * (float)gain, -1,1) ;			
			}
		}
		#endregion

			
	}
	
}

