using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip rackBack, rackForward;
    [SerializeField] private AudioClip magOut, magIn;


    public bool BoltRackForward {get => _boltRackForward;}
    private bool _boltRackForward = true;

    public readonly int IDLE = Animator.StringToHash("Weapon_Idle");
    public readonly int ATTACK = Animator.StringToHash("Weapon_Attack");
    public readonly int RELOAD = Animator.StringToHash("Weapon_Reload");
    
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

    public void PlayAttack()
    {
        if (!hasAnimator) return;
        animator.Play(ATTACK);
    }

    public void PlayReload()
    {
        if (!hasAnimator) return;
        animator.Play(RELOAD);
    }

    public void PlayIdle()
    {
        if (!hasAnimator) return;
        animator.Play(IDLE);
    }

    public void AnimatorReset()
    {
        if(!hasAnimator) return;
        
        animator.Rebind();
        animator.Update(0f);
    }

    public void OnRackBack()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(rackBack);

        _boltRackForward = false;
    }

    public void OnRackForward()
    {
        if(!hasAudioSource)  return;

        source.PlayOneShot(rackForward);

        _boltRackForward = true;
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
