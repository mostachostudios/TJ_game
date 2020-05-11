using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_EnemyController : MonoBehaviour
{
    public float walkSpeed = 1f;
    public float runSpeed = 1.8f;

    Animator m_Animator;
    NavMeshAgent m_Agent;

    Script_EnemyPerception m_Script_EnemyPerception;

    GameObject m_Player;

    private List<string> m_AnimationStates;

    void Awake()
    {
        m_Player = GameObject.FindWithTag("Player");
        m_Animator = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();

        m_Script_EnemyPerception = GetComponent<Script_EnemyPerception>();

        m_Agent.speed = walkSpeed;

        m_AnimationStates = new List<string>();
        m_AnimationStates.Add("isIdle");
        m_AnimationStates.Add("isWalking");
        m_AnimationStates.Add("isRunning");
    }

    void Update()
    {
        if (m_Script_EnemyPerception.IsPlayerDetected() && Vector3.Distance(this.transform.position, m_Player.transform.position) > m_Agent.stoppingDistance)
        {
            SetChase();

        }
        else
        {
            //TODO Set back to patrol
            SetIdle();
        }
    }

    private void SetAnimatorState(string state)
    {
        foreach (string entry in m_AnimationStates)
        {
            m_Animator.SetBool(entry, false);
        }
        m_Animator.SetBool(state, true);
    }

    public void SetChase()
    {
        m_Agent.SetDestination(m_Player.transform.position);
        m_Agent.speed = runSpeed;
        SetAnimatorState("isRunning");
    }

    public void SetIdle()
    {
        m_Agent.SetDestination(transform.position);
        m_Agent.speed = walkSpeed;
        SetAnimatorState("isIdle");
    }

    public void SetWalk(Vector3 position)
    {
        m_Agent.SetDestination(position);
        m_Agent.speed = walkSpeed;
        SetAnimatorState("isWalking");
    }
}
