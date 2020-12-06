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

    protected SpellCasting caster;
    protected Vector3 initiateTarget;
    protected Vector3 castTarget;
    protected Transform wandCastTransform;

    public virtual bool OnInitiate(SpellCasting _caster, Vector3 _initiateTarget, Vector3 _castTarget, Transform _wandCastTransform) {
        caster = _caster;
        initiateTarget = _initiateTarget;
        castTarget = _castTarget;
        wandCastTransform = _wandCastTransform;

        currentInitiateIndicator = Instantiate(initiateIndicator, wandCastTransform.position, Quaternion.identity);
        currentInitiateIndicator.transform.SetParent(wandCastTransform);

        return true;
    }

    public virtual bool OnRelease(SpellCasting _caster, Vector3 _initiateTarget, Vector3 _castTarget, Transform _wandCastTransform) {
        castTarget = _castTarget;
        Destroy(currentInitiateIndicator);
        OnEffectStart();
        return true;
    }

    public virtual bool OnEffectStart() {
        currentProjectile = Instantiate(projectile, wandCastTransform.position, Quaternion.identity);
        return true;
    }

    protected virtual void Update() {
        if (currentProjectile != null) {
            currentProjectile.transform.position = Vector3.MoveTowards(currentProjectile.transform.position, castTarget, 20f * Time.deltaTime);
            if ((currentProjectile.transform.position - castTarget).magnitude < 0.1f) {
                OnEffectEnd();
            }
        }
    }

    public virtual bool OnEffectEnd() {
        Destroy(currentProjectile);
        currentImpactIndicator = Instantiate(impactIndicator, castTarget, Quaternion.identity);

        // Deal damage - TODO, lots of get component stuff here, can it be simplified? Mainly to do with getting network IDs
        Collider[] hitColliders = Physics.OverlapSphere(castTarget, areaOfEffect);
        foreach (Collider hitCollider in hitColliders) {
            CharacterStats stats = hitCollider.GetComponent<CharacterStats>();
            NetworkMessenger networkMessenger = caster.GetComponent<NetworkMessenger>();
            if (stats && networkMessenger) {
                networkMessenger.RequestStatsEffectMessage(stats.transform.GetComponent<NetworkData>().networkID, "health", -damage);
            }
        }

        Destroy(currentImpactIndicator, 5f);
        Destroy(this.gameObject);

        return true;
    }
}
