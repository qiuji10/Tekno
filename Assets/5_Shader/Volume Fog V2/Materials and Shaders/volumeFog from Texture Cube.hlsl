float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
    float t1 = (boundsMin.x - rayOrigin.x)*invRaydir.x;
    float t2 = (boundsMax.x - rayOrigin.x)*invRaydir.x;
    float t3 = (boundsMin.y - rayOrigin.y)*invRaydir.y;
    float t4 = (boundsMax.y - rayOrigin.y)*invRaydir.y;
    float t5 = (boundsMin.z - rayOrigin.z)*invRaydir.z;
    float t6 = (boundsMax.z - rayOrigin.z)*invRaydir.z;

    float tmin = max(max(min(t1, t2), min(t3, t4)), min(t5, t6));
    float tmax = min(min(max(t1, t2), max(t3, t4)), max(t5, t6));

    return float2(tmin, tmax);
}


float Unity_RandomRange_float(float2 Seed)
{
    float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
    return  randomno - 0.5;
} 

SamplerState sampler_linear_repeat;

float SamplePoint (
  Texture3D NoiseTexture,
  Texture2D Noise2DTexture,
  float Noise2DScale,
  float Noise2DPower,
  float3 distanceToMinBounds,
  float3 distanceToMaxBounds,
  float3 randVec,
  float3 PositionOffset,
  float3 Size,
  float Threshold,
  float Multiplier,
  float BoundsFade,
  float3 p
) {
  
    float n3D = NoiseTexture.Sample(sampler_linear_repeat, p).x;
  float n = (Noise2DTexture.Sample(sampler_linear_repeat, p.xz * Noise2DScale).x - 0.5) * Noise2DPower;
  float noise = n + n3D ; 

  float bordersFade = min(
    min(distanceToMinBounds.x, distanceToMaxBounds.x),
    min(min(distanceToMinBounds.y, distanceToMaxBounds.y),
    min(distanceToMinBounds.z, distanceToMaxBounds.z))
    );

    float noiseAdjusted = (noise - Threshold) * Multiplier;
    float fade = saturate((bordersFade * .025 * BoundsFade));
  return saturate(noiseAdjusted * fade* fade);
}

void volumeFog_float(
    Texture3D NoiseTexture,
    Texture2D Noise2DTexture,
    float Noise2DScale,
    float Noise2DPower,
    float Samples,
    float MeshDistance,
    float BoundsFade,
    float Randomness,
    float3 Size,
    float Threshold,
    float Multiplier,
    float3 PositionOffset,
    float3 Position,
    float3 View,
    out float Fog
    ) { 
    float3 objectPosition = SHADERGRAPH_OBJECT_POSITION;
    float3 objectScale = float3(length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x)),
                             length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y)),
                             length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z)));


    float3 cameraPosition = _WorldSpaceCameraPos;

    float3 boundsMin = objectPosition - objectScale / 2;
    float3 boundsMax = objectPosition + objectScale / 2;

    float2 rayToContainerInfo = rayBoxDst(boundsMin, boundsMax, cameraPosition, 1/View);
    float distanceToStart = rayToContainerInfo.y;
    float OverallDistance = -abs(rayToContainerInfo.y - rayToContainerInfo.x);

    Samples = min(Samples, 40) ; // 40 will be the maximum samples, you can change this value if needed

    float Distance = max(OverallDistance, -MeshDistance) / Samples;

    float random = Unity_RandomRange_float(View.xz);
    float3 randVec = random * Randomness * View * (Distance / 10);


    float3 p;
    float3 vectorToAdd;
    float3 distanceToMinBounds;
    float3 distanceToMaxBounds;

    Fog = 1;

    for (int i = 0; i < Samples ; i++) {
        vectorToAdd = View * Distance * i;

        p = Position + vectorToAdd;

        float3 distanceToMinBounds = abs(p - boundsMin);
        float3 distanceToMaxBounds = abs(p - boundsMax);
          
        float3 offset = (randVec + PositionOffset) * 0.01;
        p -= offset;
        p *= Size;
        p += offset;

       float density = SamplePoint (
            NoiseTexture,
            Noise2DTexture,
            Noise2DScale,
            Noise2DPower,
            distanceToMinBounds,
            distanceToMaxBounds,
            randVec,
            PositionOffset,
            Size,
            Threshold,
            Multiplier,
            BoundsFade,
            p
        );

        if (density > 0) {
          Fog *= exp(-density);
      
          // Exit early if T is close to zero as further samples won't affect the result much
          if (Fog < 0.01) {
              break;
          }
         }
    }
}