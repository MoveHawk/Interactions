using UnityEngine;

public class SmokeStart : MonoBehaviour, IInteractable
{
    [Header("Effects")]
    public ParticleSystem smokeEffect;

    [Header("Sound")]
    public AudioSource audioSource;

    private bool isSmokeOn = false;

    void Start()
    {
        if (smokeEffect == null)
            Debug.LogError("Smoke ParticleSystem not assigned!");

        if (audioSource == null)
            Debug.LogError("AudioSource not assigned!");
        else
            audioSource.loop = true; // Ensure looping is enabled
    }

    public void PlayerInteracted()
    {
        if (smokeEffect == null || audioSource == null) return;

        if (isSmokeOn)
        {
            smokeEffect.Stop();
            audioSource.Stop();
            isSmokeOn = false;
        }
        else
        {
            smokeEffect.Play();
            audioSource.Play();
            isSmokeOn = true;
        }
    }
}
