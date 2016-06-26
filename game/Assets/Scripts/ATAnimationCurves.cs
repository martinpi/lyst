using UnityEngine;
using System.Collections;



namespace Atomtwist {

	public static class ATAnimationCurves {
		
		// extension method
		public static AnimationCurve ATEaseIn (this AnimationCurve curve ,
		                                       float startTime, float startValue, float endTime, float endValue)
		{
			curve.AddKey(new Keyframe(startTime,startValue,0,0));
			curve.AddKey(new Keyframe(endTime,endValue,2,2));
			return curve;
		}
		
		//initialization method
		public static AnimationCurve ATEaseIn (float startTime, float startValue, float endTime, float endValue)
		{
			var curve = new AnimationCurve();
			curve.AddKey(new Keyframe(startTime,startValue,0,0));
			curve.AddKey(new Keyframe(endTime,endValue,2,2));
			return curve;
		}
		
		
		public static AnimationCurve ATEaseOut (this AnimationCurve curve ,
		                                        float startTime, float startValue, float endTime, float endValue)
		{
			curve.AddKey(new Keyframe(startTime,startValue,2,2));
			curve.AddKey(new Keyframe(endTime,endValue,0,0));
			return curve;
		}
		
		public static AnimationCurve ATEaseOut (float startTime, float startValue, float endTime, float endValue)
		{
			var curve = new AnimationCurve();
			curve.AddKey(new Keyframe(startTime,startValue,2,2));
			curve.AddKey(new Keyframe(endTime,endValue,0,0));
			return curve;
		}
		
		
	}
	
}