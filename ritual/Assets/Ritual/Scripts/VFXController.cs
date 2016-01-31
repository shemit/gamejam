using UnityEngine;
using System.Collections;

public class VFXController : MonoBehaviour {
    public bool m_DestroyOnCompletion = true;
    public void Start()
    {
        if (m_DestroyOnCompletion)
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            float longestDuration = 0.0f;
            for (int i = 0, n = particleSystems.Length; i < n; ++i)
            {
                if (particleSystems[i].duration > longestDuration)
                {
                    longestDuration = particleSystems[i].duration;
                }
            }
            Destroy(gameObject, longestDuration);
        }
    }
}
