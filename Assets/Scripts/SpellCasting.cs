using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    public SpellEffect currentEffect {get; private set;}
    // private SpellEffect nextEffect;
    private SpellEffect spellEffectInstance;
    public Transform wandCastTransform;
    public delegate void UpdateAction();
    public UpdateAction updateAction;
    private Vector3 initiateTarget;
    private Vector3 castTarget;
    private float elementTimer;

    void Start() {
        updateAction = DefaultUpdate;
    }

    public void SelectSpellEffect(SpellEffect spellEffect) {
        currentEffect = spellEffect;
    }

    public void InitiateSpell(Vector3 _initiateTarget) {
        initiateTarget = _initiateTarget;

        spellEffectInstance = Instantiate(currentEffect, transform.position, Quaternion.identity);

        spellEffectInstance.OnInitiate(this, initiateTarget, Vector3.zero, wandCastTransform);
    }

    public void ReleaseSpell(Vector3 _castTarget) {
        castTarget = _castTarget;

        spellEffectInstance.OnRelease(this, initiateTarget, castTarget, wandCastTransform);
    }

    void Update() {
        updateAction();
    }

    void DefaultUpdate() {

    }
}
