using UnityEngine;
using UnityEngine.AI;

public class StopMovingAction : Action
{    
    public NavMeshAgent agent;
    public float timeSeconds = .0f;
    private float currentTimeSeconds;

    private Script_NPCController NPCController;

    protected override bool StartDerived()
    {
        currentTimeSeconds = .0f;

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

        return false;       
    }

    protected override bool UpdateDerived()
    {
        currentTimeSeconds = Mathf.Min(currentTimeSeconds + Time.deltaTime, timeSeconds);

        return currentTimeSeconds == timeSeconds;
    }

    protected override Action CloneDerived()
    {
        StopMovingAction clone = new StopMovingAction();
        clone.agent = null;
        clone.timeSeconds = this.timeSeconds;

        return clone;
    }
}
