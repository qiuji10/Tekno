using UnityEngine;

[ExecuteAlways]
public class SpotlightEditor : MonoBehaviour
{
    public float innerSpotAngleRatio = 0.005f;
    public float rangeRatio = 0.5f;

    private Light spotlight;
    private Transform cone;

    private void OnEnable()
    {
        spotlight = GetComponent<Light>();
        cone = transform.GetChild(0);
    }

    private void Update()
    {
        // Update cone scale when inner spot angle and range changes
        cone.localScale = new Vector3(spotlight.spotAngle * spotlight.range * innerSpotAngleRatio, spotlight.range * rangeRatio, spotlight.spotAngle * spotlight.range * innerSpotAngleRatio);

        // Update cone height when range changes
        cone.localPosition = new Vector3(cone.localPosition.x, cone.localPosition.z, cone.localPosition.z);
    }
}
