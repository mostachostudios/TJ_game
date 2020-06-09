using UnityEngine;
using UnityEngine.AI;

public class ChaseAction : Action
{
    public NavMeshAgent agent;

    private Script_NPCController NPCController;

    protected override bool StartDerived()
    {
        NPCController = agent.GetComponent<Script_NPCController>(); 

        if(NPCController == null)
        {
            throw new UnityException("There is no Script_EnemyController component attached to the selected object");
        }

        NPCController.SetChase();

        return false;       
    }

    protected override bool UpdateDerived()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {            
            NPCController.SetIdle();           
            return true;
        }
        else
        {
            NPCController.SetChase();
        }
        return false;
    }

    protected override Action CloneDerived()
    {
        ChaseAction clone = new ChaseAction();
        clone.agent = null;

        return clone;
    }
}
