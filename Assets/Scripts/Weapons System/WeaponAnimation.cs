using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip rackBack, rackForward;
    [SerializeField] private AudioClip magOut, magIn;



    public const string IDLE = "Weapon_Idle";
    public const string ATTACK = "Weapon_Attack";
    public const string Reload = "Weapon_Reload";
    
    private Weapon weapon;
    private Animator animator;
    private AudioSource source;

    private bool hasAudioSource;
    private bool hasAnimator;

    private void Start()
    {
        weapon = GetComponent<Weapon>();
        hasAudioSource = TryGetComponent(out source);
        hasAnimator = TryGetComponent(out animator);
    }

    public void PlayAnimation(string animationName)
    {
        if(!hasAnimator) return;
        
        animator.Play(animationName);
    }

    public void AnimatorReset()
    {
        animator.StopPlayback();
    }

    public void OnRackBack()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(rackBack);
    }

    public void OnRackForward()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(rackForward);
    }

    public void OnMagOut()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(magOut);
    }

    public void OnMagIn()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(magIn);
    }

}
