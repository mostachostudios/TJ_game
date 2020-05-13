using UnityEngine;
using UnityEngine.AI;

public class MoveToPointAction : Action
{
    //Default values: WALK = 1f, RUN = 2f.
    public enum Speed
    {
        WALK,
        RUN,
    }
    
    public NavMeshAgent agent;
    public Transform targetPoint;
    public Speed speed;
    //public float actualSpeed; //TODO Consider if adding a custom speed, and therefore ignoring walk and run if != 0

    //If there is an NPC script, use the associated values, otherwise, use default values (valid also for Enemy players since they inherit from NPCController)
    private Script_NPCController NPCController;

    protected override bool StartDerived()
    {        
        if (!agent || !targetPoint || (Vector3.Distance(targetPoint.position, agent.transform.position) <= agent.stoppingDistance))
        {
            return true;
        }

        NPCController = agent.GetComponent<Script_NPCController>(); 

        if (NPCController)
        {
            switch (speed)
            {
                case Speed.WALK:
                    NPCController.WalkToPosition(targetPoint.position);
                    break;
                case Speed.RUN:
                    NPCController.RunToPosition(targetPoint.position);
                    break;
            }
        }
        else
        {
            switch (speed)
            {
                case Speed.WALK:
                    agent.speed = 1f;
                    break;
                case Speed.RUN:
                    agent.speed = 2f;
                    break;
            }
            agent.SetDestination(targetPoint.position);
        }

        return false;       
    }

    protected override bool UpdateDerived()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            if (NPCController)
            {
                NPCController.SetIdle();
            }

            return true;
        }
        return false;
    }

    protected override Action CloneDerived()
    {
        MoveToPointAction clone = new MoveToPointAction();
        clone.agent = null;
        clone.targetPoint = this.targetPoint;
        clone.speed = this.speed;

        return clone;
    }
}
