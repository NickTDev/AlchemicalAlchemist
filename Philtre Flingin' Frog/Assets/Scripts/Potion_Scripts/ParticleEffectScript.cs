using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleEffectScript : MonoBehaviour
{
    bool expires = false;
    float elapsedDuration = 0;
    VisualEffect particleEffect;

    void Start()
    {
        particleEffect = GetComponent<VisualEffect>();
    }

    public void Initialize(Color particleColor, bool willExpire)
    {
        expires = willExpire;
        //sets up the color
        Gradient particleGradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[1];
        colorKey[0].color = particleColor;
        colorKey[0].time = 0.0f;

        alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;

        particleGradient.SetKeys(colorKey, alphaKey);
            
        //actually sets the gradient
        GetComponent<VisualEffect>().SetGradient("Color Range", particleGradient);
    }

    void Update()
    {
        if (GameManager.GetPaused())
        {
            particleEffect.pause = true;
        }
        else 
        {
            elapsedDuration += Time.deltaTime; //makes sure that the effect doesn't destroy BEFORE particle spawn
            particleEffect.pause = false;
        }
        if (expires && elapsedDuration > 0.5f && particleEffect.aliveParticleCount < 1) //does not happen while paused or if this potion doesn't expire automatically
        {
            Destroy(gameObject);
        }
    }
}
