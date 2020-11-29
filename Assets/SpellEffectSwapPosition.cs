using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectSwapPosition : SpellEffect
{
    public override bool OnInitiate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentInitiateIndicator = Instantiate(initiateIndicator, wandCastTransform.position, Quaternion.identity);
        currentInitiateIndicator.transform.SetParent(wandCastTransform);
        return true;
    }

    public override bool OnRelease(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        Destroy(currentInitiateIndicator);
        OnEffectStart(caster, initiateTarget, castTarget, wandCastTransform);
        return true;
    }
    
    public override bool OnEffectStart(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget, Transform wandCastTransform) {
        currentProjectile = Instantiate(projectile, wandCastTransform.position, Quaternion.identity);
        return true;
    }

    public override bool EffectUpdate(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        currentProjectile.transform.position = Vector3.MoveTowards(currentProjectile.transform.position, castTarget, 0.1f);
        if ((currentProjectile.transform.position - castTarget).magnitude < 0.1f) {
            OnEffectEnd(caster, initiateTarget, castTarget);
            return true;
        } else {
            return false;
        }
    }

    public override bool OnEffectEnd(CharacterControl caster, Vector3 initiateTarget, Vector3 castTarget) {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);

        WorldObject castObj = null;
        WorldObject initObj = null;

        // Swap objects
        Collider[] hitColliders = Physics.OverlapSphere(castTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            WorldObject worldObj = hitCollider.GetComponent<WorldObject>();
            if (worldObj) {
                castObj = worldObj;
            }
        }

        hitColliders = Physics.OverlapSphere(initiateTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            WorldObject worldObj = hitCollider.GetComponent<WorldObject>();
            if (worldObj) {
                initObj = worldObj;
            }
        }

        if (castObj != null && initObj != null) {
            Vector3 castObjPosition = castObj.transform.position;
            Vector3 initObjPosition = initObj.transform.position;

            initObj.transform.position = castObjPosition;
            castObj.transform.position = initObjPosition;
        }

        Destroy(currentImpactIndicator, 1f);
        return true;
    }
}
