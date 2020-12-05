using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public string effectName;
    public Sprite icon;
    public float damage;
    public float areaOfEffect;
    public GameObject projectile;
    public GameObject initiateIndicator;
    public GameObject impactIndicator;
    protected GameObject currentInitiateIndicator;
    protected GameObject currentProjectile;
    protected GameObject currentImpactIndicator;

    public virtual bool OnInitiate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentInitiateIndicator = Instantiate(initiateIndicator, wandCastTransform.position, Quaternion.identity);
        currentInitiateIndicator.transform.SetParent(wandCastTransform);
        return true;
    }

    public virtual bool OnRelease(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        Destroy(currentInitiateIndicator);
        OnEffectStart(caster, initiateTarget, castTarget, wandCastTransform);
        return true;
    }
    
    public virtual bool OnEffectStart(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentProjectile = Instantiate(projectile, wandCastTransform.position, Quaternion.identity);
        return true;
    }

    public virtual bool EffectUpdate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        currentProjectile.transform.position = Vector3.MoveTowards(currentProjectile.transform.position, castTarget, 20f * Time.deltaTime);
        if ((currentProjectile.transform.position - castTarget).magnitude < 0.1f) {
            OnEffectEnd(caster, initiateTarget, castTarget);
            return true;
        } else {
            return false;
        }
    }

    public virtual bool OnEffectEnd(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);
        // Deal damage
        Collider[] hitColliders = Physics.OverlapSphere(castTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            CharacterStats stats = hitCollider.GetComponent<CharacterStats>();
            if (stats) {
                stats.ApplyEffect("health", damage);
            }
        }

        Destroy(currentImpactIndicator, 5f);

        return true;
    }
}
