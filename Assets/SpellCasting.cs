using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    public List<SpellEffect> spell;
    private int currentEffect = 0;
    public Transform wandCastTransform;
    public delegate void UpdateAction();
    public UpdateAction updateAction;
    private CharacterControl caster;
    private Vector3 initiateTarget;
    private Vector3 castTarget;
    private float elementTimer;

    void Start() {
        updateAction = DefaultUpdate;

        caster = GetComponent<CharacterControl>();
    }

    public void InitiateSpell(Vector3 _initiateTarget) {
        initiateTarget = _initiateTarget;

        spell[currentEffect].OnInitiate(caster, initiateTarget, Vector3.zero, wandCastTransform);
    }

    public void ReleaseSpell(Vector3 _castTarget) {
        castTarget = _castTarget;

        spell[currentEffect].OnRelease(caster, initiateTarget, castTarget, wandCastTransform);

        updateAction = SpellUpdate;
    }

    public void SpellUpdate() {
        bool effectFinished = spell[currentEffect].EffectUpdate(caster, initiateTarget, castTarget);
        if (effectFinished) {
            updateAction = DefaultUpdate;
            NextEffect();
        }
    }

    void Update() {
        updateAction();
    }

    void DefaultUpdate() {

    }

    void NextEffect() {
        currentEffect++;
        if (currentEffect > spell.Count-1) {
            currentEffect = 0;
        }
    }
}
