//----------------------------------------------
//                 Koreographer                 
//    Copyright © 2014-2022 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Cube Scaler")]
	public class CubeScaler : MonoBehaviour
	{
		[EventID]
		public string eventID;
		public float minScale = 0.5f;
		public float maxScale = 1.5f;
		public float scaleAdjustment = 1.0f;
        private Vector3 baseScale;

        void Start()
		{
            baseScale = transform.localScale;
            // Register for Koreography Events.  This sets up the callback.
            Koreographer.Instance.RegisterForEventsWithTime(eventID, AdjustScale);
		}
		
		void OnDestroy()
		{
			// Sometimes the Koreographer Instance gets cleaned up before hand.
			//  No need to worry in that case.
			if (Koreographer.Instance != null)
			{
				Koreographer.Instance.UnregisterForAllEvents(this);
			}
		}
		
		void AdjustScale(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
		{
            if (evt.HasCurvePayload())
            {
                // Get the value of the curve at the current audio position. This will be a
                // value between [0, 1] and will be used, below, to adjust the scale.
                float curveValue = evt.GetValueOfCurveAtTime(sampleTime);

                // Calculate the new scale of the cube based on the current scale and the
                // adjustment value.
                float currentScale = transform.localScale.x;
                float newScale = currentScale + (scaleAdjustment * curveValue);

                // Make sure the new scale is within the allowed range.
                newScale = Mathf.Clamp(newScale, baseScale.x * 0.5f, baseScale.x * 1.5f);

                // Set the new scale of the cube.
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
	}
}
