using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public string elementName;
    public Sprite icon;
    public float damage;
    public GameObject projectile;
    public GameObject initiateIndicator;
    public GameObject impactIndicator;
    private GameObject currentInitiateIndicator;
    private GameObject currentProjectile;
    private GameObject currentImpactIndicator;

    public bool OnInitiate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentInitiateIndicator = Instantiate(initiateIndicator, wandCastTransform.position, Quaternion.identity);
        currentInitiateIndicator.transform.SetParent(wandCastTransform);
        return true;
    }

    public bool OnRelease(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        Destroy(currentInitiateIndicator);
        OnEffectStart(caster, initiateTarget, castTarget, wandCastTransform);
        return true;
    }
    
    public bool OnEffectStart(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentProjectile = Instantiate(projectile, wandCastTransform.position, Quaternion.identity);
        return true;
    }

    public bool EffectUpdate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        currentProjectile.transform.position = Vector3.MoveTowards(currentProjectile.transform.position, castTarget, 0.1f);
        if ((currentProjectile.transform.position - castTarget).magnitude < 0.1f) {
            OnEffectEnd(caster, initiateTarget, castTarget);
            return true;
        } else {
            return false;
        }
    }

    public bool OnEffectEnd(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);
        Destroy(currentImpactIndicator, 5f);
        return true;
    }
}
