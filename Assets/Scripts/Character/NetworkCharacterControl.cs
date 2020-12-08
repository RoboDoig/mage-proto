using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterControl : MonoBehaviour
{
    private Animator animator;
    private SpellCasting spellCasting;
    private NetworkMessenger networkMessenger;
    private SpellEffectManager spellEffectManager;
    private Vector3 currentLookTarget;
    private Vector2 animSpeed;

    void Awake() {
        animator = GetComponent<Animator>();
        spellCasting = GetComponent<SpellCasting>();
        spellEffectManager = Camera.main.GetComponent<SpellEffectManager>();
        spellCasting.SelectSpellEffect(spellEffectManager.spellEffects[0]);
    }

    public void SetRotation(Quaternion _rotation) {
        transform.rotation = _rotation;
    }

    public void SetLookTarget(Vector3 _target) {
        currentLookTarget = _target;
    }

    public void SetPosition(Vector3 _position) {
        transform.position = _position;
    }

    public void SetAnimatorSpeeds(Vector2 _animSpeed) {
        animator.SetFloat("speedX", _animSpeed.x);
        animator.SetFloat("speedY", _animSpeed.y);
    }

    public void InitiateSpell(string spellName) {
        spellCasting.SelectSpellEffect(spellEffectManager.spellEffectsDict[spellName]);

        animator.SetLayerWeight(1, 1f);
        animator.SetTrigger("spellInitiate");

        spellCasting.InitiateSpell(currentLookTarget);
    }

    public void ReleaseSpell() {
        animator.SetTrigger("spellRelease");
    }

    public void SpellFire() {
        spellCasting.ReleaseSpell(currentLookTarget);
    }

    public void EndSpellRelease() {
        animator.SetLayerWeight(1, 0f);
    }
}
