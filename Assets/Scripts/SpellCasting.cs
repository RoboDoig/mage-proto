using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    private SpellEffect currentEffect;
    private SpellEffect nextEffect;
    public Transform wandCastTransform;
    public delegate void UpdateAction();
    public UpdateAction updateAction;
    private CharacterControl caster;
    private Vector3 initiateTarget;
    private Vector3 castTarget;
    private float elementTimer;
    private bool spellStarted = false;

    void Start() {
        updateAction = DefaultUpdate;

        caster = GetComponent<CharacterControl>();
    }

    public void SelectSpellEffect(SpellEffect spellEffect) {
        if (!spellStarted) {
            currentEffect = spellEffect;
            nextEffect = spellEffect;
        } else {
            nextEffect = spellEffect;
        }
    }

    public void InitiateSpell(Vector3 _initiateTarget) {
        initiateTarget = _initiateTarget;
        spellStarted = true;

        currentEffect.OnInitiate(caster, initiateTarget, Vector3.zero, wandCastTransform);
    }

    public void ReleaseSpell(Vector3 _castTarget) {
        castTarget = _castTarget;

        currentEffect.OnRelease(caster, initiateTarget, castTarget, wandCastTransform);

        updateAction = SpellUpdate;
    }

    public void SpellUpdate() {
        bool effectFinished = currentEffect.EffectUpdate(caster, initiateTarget, castTarget);
        if (effectFinished) {
            updateAction = DefaultUpdate;
            
            currentEffect = nextEffect;

            spellStarted = false;
        }
    }

    void Update() {
        updateAction();
    }

    void DefaultUpdate() {

    }
}
