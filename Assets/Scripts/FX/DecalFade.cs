using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFade : MonoBehaviour
{
    public float lifeTime;

    float life;
    DecalProjector decalProjector;

    private void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        life = lifeTime;
    }


    private void Update()
    {
        Material material = decalProjector.material;
        float scalingFactor = 1 / lifeTime;
        float opacity = life * scalingFactor;

        decalProjector.fadeFactor = opacity;

        life -= Time.deltaTime;
    }
}
