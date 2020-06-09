/*
using UnityEngine;

// TO BE REMOVED. NO LONGER USED AFTER USING STATE MACHINE.
public class Script_EnemyController : Script_NPCController
{
    GameObject m_Player;
    Script_EnemyPerception m_Script_EnemyPerception;

    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindWithTag("Player");
        m_Script_EnemyPerception = GetComponent<Script_EnemyPerception>();
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
    

    public void SetChase()
    {
        m_Agent.SetDestination(m_Player.transform.position);
        m_Agent.speed = runSpeed;
        Utils.SetAnimatorParameterByName(m_Animator, "isRunning");
    }

}
*/