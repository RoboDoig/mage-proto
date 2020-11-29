using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public List<SpellEffect> spellEffects;
    public SpellEffect currentEffect;
    private int effectCounter = 0;

    public void Initiate(CharacterControl caster, Vector3 initiateTarget, Transform wandCastTransform) {
        spellEffects[effectCounter].OnInitiate(caster, initiateTarget, Vector3.zero, wandCastTransform);
    }

    public void Release(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        spellEffects[effectCounter].OnRelease(caster, initiateTarget, castTarget, wandCastTransform);
    }

    public bool SpellUpdate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        return spellEffects[effectCounter].EffectUpdate(caster, initiateTarget, castTarget);
    }

    public void NextElement() {
        effectCounter++;
        currentEffect = spellEffects[effectCounter];
    }

    public void ResetSpell() {
        effectCounter = 0;
        currentEffect = spellEffects[effectCounter];
    }

    public bool CanAdvance() {
        if ((effectCounter + 1) < spellEffects.Count) {
            return true;
        } else {
            return false;
        }
    }
}
