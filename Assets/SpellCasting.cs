using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    public Spell spell;
    public Transform wandCastTransform;
    private Spell currentSpell;
    public delegate void UpdateAction();
    public UpdateAction updateAction;
    private CharacterControl caster;
    private Vector3 initiateTarget;
    private Vector3 castTarget;
    private float elementTimer;

    void Start() {
        updateAction = DefaultUpdate;

        currentSpell = Instantiate(spell);
        caster = GetComponent<CharacterControl>();
    }

    public void InitiateSpell(Vector3 _initiateTarget) {
        initiateTarget = _initiateTarget;

        currentSpell.Initiate(caster, initiateTarget, wandCastTransform);
    }

    public void ReleaseSpell(Vector3 _castTarget) {
        castTarget = _castTarget;

        currentSpell.Release(caster, initiateTarget, castTarget, wandCastTransform);

        updateAction = SpellUpdate;
    }

    public void SpellUpdate() {
        bool effectFinished = currentSpell.SpellUpdate(caster, initiateTarget, castTarget);
        if (effectFinished) {
            updateAction = DefaultUpdate;
            currentSpell.NextElement();
        }
    }

    void Update() {
        updateAction();
    }

    void DefaultUpdate() {

    }
}
