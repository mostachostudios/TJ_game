using UnityEngine;
using UnityEngine.AI;

public class Script_NPCController : MonoBehaviour
{
    public float walkSpeed = 1f;
    public float runSpeed = 1.8f;

    protected Animator m_Animator;
    protected NavMeshAgent m_Agent;

    GameObject m_Player;

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();

        m_Agent.speed = walkSpeed;

        m_Player = GameObject.FindWithTag("Player");
    }

    public void SetIdle()
    {
        m_Agent.SetDestination(transform.position);
        m_Agent.speed = walkSpeed;
        Utils.SetAnimatorParameterByName(m_Animator, "isIdle");
    }

    public void WalkToPosition(Vector3 position)
    {
        m_Agent.SetDestination(position);
        m_Agent.speed = walkSpeed;
        Utils.SetAnimatorParameterByName(m_Animator, "isWalking");
    }

    public void RunToPosition(Vector3 position)
    {
        m_Agent.SetDestination(position);
        m_Agent.speed = runSpeed;
        Utils.SetAnimatorParameterByName(m_Animator, "isRunning");
    }

    public void SetChase()
    {
        m_Agent.SetDestination(m_Player.transform.position);
        m_Agent.speed = runSpeed;
        Utils.SetAnimatorParameterByName(m_Animator, "isRunning");
    }
}
