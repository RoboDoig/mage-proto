using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;

public class CharacterControl : MonoBehaviour
{
    public bool controllabe;
    public float rotationSpeed;
    public UnityClient client;

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
    private Vector2 animSpeed = Vector2.zero;

    // State variables
    private bool inSpellCast = false;
    private bool canRelease = false;

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spellCasting = GetComponent<SpellCasting>();

        lastPosition = transform.position;
        currentMoveTarget = transform.position;

        if (controllabe)
            Camera.main.GetComponent<PlayerInterface>().SelectCharacter(this);
    }

    void Update() {
        // If character is controllable, we need to update the network with movement
        if (controllabe)
            CalculateSpeeds();
            UpdateAnimator();
            UpdateNetwork();
    }

    void UpdateNetwork() {
        Message moveMessage = Message.Create(Tags.MovePlayerTag,
            new NetworkPlayerManager.MovementMessage(transform.position, transform.rotation, currentLookTarget));

        client.SendMessage(moveMessage, SendMode.Unreliable);
    }

    public void LookAtTarget(Vector3 position) {
        Vector3 offsetPosition = new Vector3(position.x, position.y + 0.5f, position.z);
        direction = (offsetPosition - transform.position).normalized;

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

        animSpeed.y = Vector3.Dot(velocityVector, (currentLookTarget - transform.position).normalized);
        animSpeed.x = Vector3.Dot(velocityVector, Quaternion.Euler(0, 90, 0) * (currentLookTarget - transform.position).normalized);
    }

    void UpdateAnimator() {
        animator.SetFloat("speedY", animSpeed.y);
        animator.SetFloat("speedX", animSpeed.x);
    }
}
