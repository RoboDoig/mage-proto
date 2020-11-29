using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControl : MonoBehaviour
{
    public float rotationSpeed;

    // Components cache
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private SpellCasting spellCasting;

    // Speed/Movement calculation
    private Vector3 lastPosition;
    private Vector3 currentMoveTarget;
    private Vector3 currentLookTarget;
    private float velocityMag;
    private Vector3 direction;
    private Vector3 velocityVector;
    private float speedY;
    private float speedX;

    // State variables
    private bool inSpellCast = false;
    private bool canRelease = false;

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spellCasting = GetComponent<SpellCasting>();

        lastPosition = transform.position;
        currentMoveTarget = transform.position;
    }

    void Update() {
        CalculateSpeeds();
        UpdateAnimator();
    }

    public void LookAtTarget(Vector3 position) {
        direction = (position - transform.position).normalized;
        direction.y = transform.position.y - 0.5f;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        currentLookTarget = position;
    }

    public void GoToTarget(Vector3 position) {
        navMeshAgent.SetDestination(position);
        currentMoveTarget = position;
    }

    public void InitiateSpell() {
        if (!inSpellCast) {
            animator.SetLayerWeight(1, 1f);
            animator.SetTrigger("spellInitiate");
            inSpellCast = true;
            canRelease = true;

            spellCasting.InitiateSpell(currentLookTarget);
        }
    }

    public void ReleaseSpell() {
        if (canRelease) {
            animator.SetTrigger("spellRelease");
            canRelease = false;
        }
    }

    public void SpellFire() {
        spellCasting.ReleaseSpell(currentLookTarget);
    }

    public void EndSpellRelease() {
        animator.SetLayerWeight(1, 0f);
        inSpellCast = false;
    }

    void CalculateSpeeds() {
        velocityVector = (transform.position - lastPosition) / Time.deltaTime;
        velocityMag = velocityVector.magnitude;
        lastPosition = transform.position;

        speedY = Vector3.Dot(velocityVector, (currentLookTarget - transform.position).normalized);
        speedX = Vector3.Dot(velocityVector, Quaternion.Euler(0, 90, 0) * (currentLookTarget - transform.position).normalized);
    }

    void UpdateAnimator() {
        animator.SetFloat("speedY", speedY);
        animator.SetFloat("speedX", speedX);
    }
}
