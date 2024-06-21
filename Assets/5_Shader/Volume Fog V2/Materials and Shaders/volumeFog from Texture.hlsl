float Unity_RandomRange_float(float2 Seed)
{
    float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
    return  randomno - 0.5;
} 

SamplerState sampler_linear_repeat;

void volumeFog_float(
    Texture3D NoiseTexture,
    Texture2D Noise2DTexture,
    float Noise2DScale,
    float Samples,
    float MeshDistance,
    float StartingHeight,
    float OverallHeight,
    float randomness,
    float3 Size,
    float Threshold,
    float Multiplier,
    float MaxDistance,
    float3 Position,
    float3 View,
    out float Fog
    ) { 
    Fog = 0;

    if ((Position.y < StartingHeight - OverallHeight && View.y >= 0) || (Position.y > StartingHeight && View.y <= 0)) {
        return;
    }

    float OverallDistance;
    float yDistanceToStart = StartingHeight - Position.y;
    float yDistanceToEnd = Position.y - (StartingHeight - OverallHeight);
    float distanceToStart;
    float distanceToEnd;
    float maxDistance;

    bool between = Position.y < StartingHeight && Position.y > StartingHeight - OverallHeight;
    if (View.y > 0) {
        // above fog
        distanceToStart =  length((yDistanceToStart / View.y) * View);
        distanceToEnd =  length((yDistanceToEnd / View.y) * View);
        

    } else {
        // below fog
        distanceToStart =  length(((yDistanceToEnd) / View.y) * View);
        distanceToEnd =  length(((yDistanceToStart) / View.y) * View);


    }

 
    distanceToStart =  between? 0 : distanceToStart;
    OverallDistance = abs(distanceToStart - distanceToEnd) * -1;


    Samples = distanceToStart < MeshDistance ? min(Samples, 50) : 0; // 50 will be the maximum samples, you can change this value if needed

    float Distance = max(OverallDistance,( distanceToStart - MeshDistance )) / Samples;

    float random = Unity_RandomRange_float(View.xz);
    float3 randVec = random * randomness * View * (Distance / 10);


    float3 p;
    float yDistance;
    float noise;
    float topBottomFade;
    float3 vectorToAdd;

    float3 vectorToStart = (View * distanceToStart * -1);

    for (int i = 0; i < Samples ; i++) {
        if (Fog >= 4.5) {
            break;
        }
        
        vectorToAdd = vectorToStart + View * Distance * i;
        if (length(vectorToAdd) > MaxDistance + random) {
            Fog += 0.02;
            continue;
        }

        p = Position + vectorToAdd;
        yDistance =  StartingHeight - p.y;

        p += randVec;
        p *= Size;
        
        float n3D = SAMPLE_TEXTURE3D(NoiseTexture, sampler_linear_repeat, p).x;
        float n = SAMPLE_TEXTURE2D(Noise2DTexture, sampler_linear_repeat , p.xz * Noise2DScale).x  - .5;
        noise = n + n3D ; 
        topBottomFade = saturate(yDistance* 1.25)  * saturate((OverallHeight - yDistance)* 1.25);
        Fog += saturate((noise - Threshold) * Multiplier * topBottomFade) ;
    }

    Fog = 1 - saturate (exp(-Fog));
}