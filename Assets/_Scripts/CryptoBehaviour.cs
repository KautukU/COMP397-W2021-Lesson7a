using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CryptoState
{
    IDLE,
    RUN,
    JUMP,
    KICK
}

public class CryptoBehaviour : MonoBehaviour
{
    [Header("Line of Sight")]
    public bool HasLOS;

    public GameObject player;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Attack")]
    public float attackDistance;
    public PlayerBehaviour playerBehaviour;
    public float damageDelay = 1.0f;
    private bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerBehaviour = FindObjectOfType<PlayerBehaviour>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (HasLOS)
        {
            agent.SetDestination(player.transform.position);
        }


        if (HasLOS && distanceToPlayer < attackDistance)
        {
            // could be an attack
            animator.SetInteger("AnimState", (int)CryptoState.KICK);
            transform.LookAt(transform.position - player.transform.forward);

            if(!isAttacking)

            StartCoroutine(DoKickDamage());

            if (agent.isOnOffMeshLink)
            {
                animator.SetInteger("AnimState", (int)CryptoState.JUMP);
            }
        }
        else if (HasLOS)
        {
            animator.SetInteger("AnimState", (int)CryptoState.RUN);
        }
        else
        {
            animator.SetInteger("AnimState", (int)CryptoState.IDLE);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasLOS = true;
            player = other.transform.gameObject;
        }
    }

    private IEnumerator DoKickDamage()
    {
        yield return new WaitForSeconds(damageDelay);
        playerBehaviour.TakeDamage(20);
        playerBehaviour.controller.Move(Vector3.forward * attackDistance);
        StopCoroutine(DoKickDamage());
    }

}
