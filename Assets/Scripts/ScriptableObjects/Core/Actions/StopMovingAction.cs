using UnityEngine;
using UnityEngine.AI;

public class StopMovingAction : Action
{    
    public NavMeshAgent agent;

    private Script_NPCController NPCController;

    protected override bool StartDerived()
    {        
        if (!agent)
        {
            return true;
        }

        NPCController = agent.GetComponent<Script_NPCController>(); 

        if (NPCController)
        {
            NPCController.SetIdle();
        }
        else
        {            
            agent.SetDestination(agent.transform.position);
        }
        return true;

        //return false;       
    }

    protected override bool UpdateDerived()
    {
        //if(agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    return true;
        //}
        //return false;

        return true;

    }

    protected override Action CloneDerived()
    {
        StopMovingAction clone = new StopMovingAction();
        clone.agent = null;

        return clone;
    }
}
