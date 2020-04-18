using UnityEngine;
using System.Collections;

//https://www.youtube.com/watch?v=mBGUY7EUxXQ&list=PLX2vGYjWbI0QGyfO8PKY1pC8xcRb0X-nP&index=21&t=0s
//https://gitlab.science.ru.nl/ru/NDL-OVR/-/tree/0191c3cfc079d79230989c5a3a01c487c3441d54/Assets%2FScripts%2FEnemy
//https://docs.unity3d.com/ScriptReference/Physics.Raycast.html

// Requires both isTrigger Collider and Rigidbody either in player or in enemy 
//[ExecuteAlways]
public class Script_EnemyPerception : MonoBehaviour
{
	[SerializeField] float m_ViewDistance = 15f;
	[SerializeField] [Range(0.01f, 179f)] float m_ViewAngle = 90f;
	[SerializeField] float m_HearDistance = 10f;
	[SerializeField] float m_HearMinSpeed = 0.31f;
	[SerializeField] bool m_PlayerDetected;                    

	private SphereCollider m_SphereCollider;
	private Projector m_Projector;
	private Script_ConeOfSightRenderer m_Script_ConeOfSightRenderer;
	private GameObject m_Player;
	private Script_PlayerController m_Script_PlayerController;

	void Awake()
	{
		m_SphereCollider = GetComponent<SphereCollider>();
		m_SphereCollider.radius = Mathf.Max(m_ViewDistance, m_HearDistance);

		m_Projector = GetComponentInChildren<Projector>();

		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_Script_PlayerController = m_Player.GetComponent<Script_PlayerController>();
		
		m_Script_ConeOfSightRenderer = GetComponentInChildren<Script_ConeOfSightRenderer>();
		m_Script_ConeOfSightRenderer.m_ScaledViewDistance = m_ViewDistance * transform.localScale.x;
		m_Script_ConeOfSightRenderer.m_ViewDistance = m_ViewDistance;
		m_Script_ConeOfSightRenderer.m_ViewAngle = m_ViewAngle;
	}

	private void Update()
	{
		//#if UNITY_EDITOR
		//		if (!Application.isPlaying)
		//		{
		//	m_SphereCollider = GetComponent<SphereCollider>();
		//	m_Script_ConeOfSightRenderer = GetComponentInChildren<Script_ConeOfSightRenderer>();
			m_Script_ConeOfSightRenderer.m_ScaledViewDistance = m_ViewDistance * transform.localScale.x;
			m_Script_ConeOfSightRenderer.m_ViewDistance = m_ViewDistance;
			m_Script_ConeOfSightRenderer.m_ViewAngle = m_ViewAngle;
			m_Script_ConeOfSightRenderer.transform.localScale = new Vector3(m_ViewDistance * 2, m_Script_ConeOfSightRenderer.transform.localScale.y, m_ViewDistance * 2);
			m_Projector.orthographicSize = m_HearDistance * transform.localScale.x;

		//		}
		//#endif

		m_SphereCollider.radius = Mathf.Max(m_ViewDistance,m_HearDistance);
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == m_Player)
		{
			m_PlayerDetected = false;

			//Check Sighting
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			if (angle < m_ViewAngle * 0.5f)
			{
				RaycastHit hit;
				// Careful when using transform.up since it might lead to some incorret results depeding on model size and scaling
				Debug.DrawRay(transform.position + transform.up, direction.normalized * m_ViewDistance * transform.localScale.x, Color.blue);
				//int layer = ~(1 << LayerMask.NameToLayer("Enemy")); // Avoid hitting other enemies colliders
				int layer = LayerMask.GetMask("Player", "Obstacle"); // Better idea to only hit player and obstacles
				if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, m_ViewDistance * transform.localScale.x, layer))
				{
					if (hit.collider.gameObject == m_Player)
					{
						m_PlayerDetected = true;
						Debug.Log("Player Detected");
					}
					else
					{
						Debug.Log("Hit: "+ hit.collider.gameObject.name);
					}
				}
				else
				{
					Debug.Log("No hit");
				}
			}

			//Check hearing (Only if not already detected)
			if (!m_PlayerDetected && (Vector3.Distance(transform.position, m_Player.transform.position) <= (m_HearDistance * transform.localScale.x))) 
			{
				if(m_Script_PlayerController.CurrentSpeed() >= m_HearMinSpeed)
				{
					m_PlayerDetected = true;
					Debug.Log("Player Detected");
				}
			}

			//TODO
			//if (m_PlayerDetected) // set here chase state
			// else // set here patrol state
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == m_Player)
		{
			m_PlayerDetected = false;
			// TODO set here patrol state
		}
	}
}