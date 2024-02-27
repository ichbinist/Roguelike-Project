using UnityEngine;
using System.Collections;

namespace EpicToonFX
{

	public class ETFXPitchRandomizer : MonoBehaviour
	{
        public float randomPercent = 10;
        private ParticleSystem _particleSystem;
        private AudioSource audioSource;
        private int lastParticleCount;
        private float basePitch;
        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
            lastParticleCount = _particleSystem.particleCount;
            basePitch = audioSource.pitch;
        }

        private void Update()
        {
            int currentParticleCount = _particleSystem.particleCount;

            if (currentParticleCount > lastParticleCount)
            {
                // Particle(s) emitted
                OnParticleEmitted();
            }

            lastParticleCount = currentParticleCount;
        }

        private void OnParticleEmitted()
        {
            // Your custom logic when a particle is emitted
            audioSource.pitch = basePitch + Random.Range(-randomPercent / 100, randomPercent / 100);
        }

	}
}