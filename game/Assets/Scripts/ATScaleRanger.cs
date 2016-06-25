using UnityEngine;
using System.Collections;

namespace Atomtwist
{

	public static class ATScaleRanger {
		
		/// <summary>
		/// Scales the range between two values. Even takes reverse input & output range.
		/// </summary>
		/// <returns>The range.</returns>
		/// <param name="value">Value.</param>
		/// <param name="oldMin">Old minimum.</param>
		/// <param name="oldMax">Old max.</param>
		/// <param name="newMin">New minimum.</param>
		/// <param name="newMax">New max.</param>
		public static float ATScaleRange( this float value, float oldMin, float oldMax, float newMin, float newMax ) 
		{
			
			//check reversed input range
			var reverseInput = false;
			var oMin = Mathf.Min( oldMin, oldMax );
			var oMax = Mathf.Max( oldMin, oldMax );
			if ( oMin != oldMin )
			{
				reverseInput = true;
			}
			
			//check reversed output range
			var reverseOutput = false;  
			var nMin = Mathf.Min( newMin, newMax );
			var nMax = Mathf.Max( newMin, newMax );
			if ( nMin != newMin )
			{
				reverseOutput = true;
			}
			
			var portion = ( value - oMin ) * ( nMax - nMin ) / ( oMax - oMin );
			if ( reverseInput )
			{
				portion = ( oMax - value ) * ( nMax - nMin ) / ( oMax - oMin );
			}
			
			var result = portion + nMin;
			if ( reverseOutput )
			{
				result = nMax - portion;
			}
			
			return result;
		}
	}
}