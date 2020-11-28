using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    public List<SpellElement> spellElements;
    private Spell currentSpell;
    public delegate void UpdateAction();
    public UpdateAction updateAction;
    private CharacterControl caster;
    private Vector3 target;
    private float elementTimer;

    void Start() {
        updateAction = DefaultUpdate;

        currentSpell = new Spell(new List<SpellElement>(spellElements));
    }

    public void StartSpell(CharacterControl _caster, Vector3 _target) {
        updateAction = SpellUpdate;
        caster = _caster;
        target = _target;

        currentSpell.currentElement.OnElementStart(caster, target);
        elementTimer = 0f;
    }

    void SpellUpdate() {
        bool spellElementFinished = currentSpell.currentElement.ElementRun(caster, target, elementTimer);
        elementTimer += Time.deltaTime;

        if (spellElementFinished) {
            currentSpell.currentElement.OnElementEnd(caster, target);
            if (currentSpell.CanAdvance()) {
                currentSpell.NextElement();
                currentSpell.currentElement.OnElementStart(caster, target);
                elementTimer = 0f;
            } else {
                SpellEnd();
            }
        }
    }

    void SpellEnd() {
        updateAction = DefaultUpdate;
        currentSpell.ResetSpell();
    }

    void Update() {
        updateAction();
    }

    void DefaultUpdate() {

    }
}
