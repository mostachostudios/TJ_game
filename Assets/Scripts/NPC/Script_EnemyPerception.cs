using UnityEngine;
using System.Collections;

//https://www.youtube.com/watch?v=mBGUY7EUxXQ&list=PLX2vGYjWbI0QGyfO8PKY1pC8xcRb0X-nP&index=21&t=0s
//https://gitlab.science.ru.nl/ru/NDL-OVR/-/tree/0191c3cfc079d79230989c5a3a01c487c3441d54/Assets%2FScripts%2FEnemy
//https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
//https://answers.unity.com/questions/1088188/change-color-gradually.html

// Requires both isTrigger Collider and Rigidbody either in player or in enemy 
//[ExecuteAlways]
public class Script_EnemyPerception : MonoBehaviour
{
	[SerializeField] bool m_GameOverDetection = false;
	[Tooltip("Time lapse between player detection and game over. Ignored if GameOverDectection is set to false")]
	[SerializeField] float m_TimeGameOver = 6f;

	[Header("Sight")]
	[SerializeField] float m_ViewDistance = 2f;
	[SerializeField] [Range(0.01f, 179f)] float m_ViewAngle = 90f;
	[Header("Hearing")]
	public bool m_RenderHearingArea = true;
	[Tooltip("Minimun speed player movement that can be heard")]
	[SerializeField] float m_HearMinSpeed = 0.31f;
	[Tooltip("Inner hearing radius - Player is detected immediately if moving over Hear Min Speed")]
	[SerializeField] float m_HearDistanceClose = 1f;
	[Tooltip("Outer hearing radius - Player is detected if moving over Hear Min Speed after spending Duration Hear Far seconds in the circle")]
	[SerializeField] float m_HearDistanceFar = 2f;
	[SerializeField] float m_DurationHearFar = 10f;

	[SerializeField] Gradient m_GradientHearFar; // If set to private, this field will stop working! (Unity's bug?)

	[Tooltip("Is the player detected? For outside script checking purpose only (Required in Triggers in State Machine)")]
	public bool m_PlayerDetected = false;
	[Tooltip("Is the player inside the audible area? For outside script checking purpose only")]
	public bool m_PlayerInAudibleArea = false;

	[Tooltip("Is the player detected by sight? For outside script checking purpose only")]
	public bool m_PlayerDetectedInSight = false;
	[Tooltip("Is the player detected by audible area? For outside script checking purpose only")]
	public bool m_PlayerDetectedInAudibleArea = false;

	private float m_TimeHearFar = 0f;
	private Material m_MaterialHearFar;

	private SphereCollider m_SphereCollider;
	private Projector[] m_Projectors;
	private Script_ConeOfSightRenderer m_Script_ConeOfSightRenderer;
	private GameObject m_Player;
	private Script_PlayerController m_Script_PlayerController;
	private Script_GameController m_Script_GameController;

	private bool m_RenderArea = true;
	private void Awake()
	{
		m_HearDistanceFar = Mathf.Max(m_HearDistanceFar, m_HearDistanceClose);
		m_SphereCollider = GetComponent<SphereCollider>();
		m_SphereCollider.radius = Mathf.Max(m_ViewDistance, m_HearDistanceFar);

		m_Projectors = GetComponentsInChildren<Projector>();
		GradientColorKey GCK1 = new GradientColorKey(m_Projectors[0].material.color, 0f);
		GradientColorKey GCK2 = new GradientColorKey(m_Projectors[1].material.color, 1f);
		m_GradientHearFar.colorKeys = new GradientColorKey[] { GCK1, GCK2 };

		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_Script_PlayerController = m_Player.GetComponent<Script_PlayerController>();

		m_Script_GameController = FindObjectOfType<Script_GameController>();

		m_Script_ConeOfSightRenderer = GetComponentInChildren<Script_ConeOfSightRenderer>();
		m_Script_ConeOfSightRenderer.m_ScaledViewDistance = m_ViewDistance * transform.localScale.x;
		m_Script_ConeOfSightRenderer.m_ViewDistance = m_ViewDistance;
		m_Script_ConeOfSightRenderer.m_ViewAngle = m_ViewAngle;

		m_MaterialHearFar = new Material(m_Projectors[0].material); // This generates a copy of the material
		m_Projectors[0].material = m_MaterialHearFar;

		foreach(var projector in m_Projectors)
		{
			projector.enabled = m_RenderHearingArea && m_RenderArea;
		}

	}

	//TODO to be removed for better performance once parameter adjustment, or just make sure this executes in Build environment, not in release.
	private void Update()
	{
		//#if UNITY_EDITOR
		//		if (!Application.isPlaying)
		//		{
		//	m_SphereCollider = GetComponent<SphereCollider>();
		//	m_Script_ConeOfSightRenderer = GetComponentInChildren<Script_ConeOfSightRenderer>();
		m_HearDistanceFar = Mathf.Max(m_HearDistanceFar, m_HearDistanceClose);

		m_Script_ConeOfSightRenderer.m_ScaledViewDistance = m_ViewDistance * transform.localScale.x;
		m_Script_ConeOfSightRenderer.m_ViewDistance = m_ViewDistance;
		m_Script_ConeOfSightRenderer.m_ViewAngle = m_ViewAngle;
		m_Script_ConeOfSightRenderer.transform.localScale = new Vector3(m_ViewDistance * 2, m_Script_ConeOfSightRenderer.transform.localScale.y, m_ViewDistance * 2);
		m_Projectors[0].orthographicSize = m_HearDistanceFar * transform.localScale.x;
		m_Projectors[1].orthographicSize = m_HearDistanceClose * transform.localScale.x;

		foreach (var projector in m_Projectors)
		{
			projector.enabled = m_RenderHearingArea && m_RenderArea;
		}
		//		}
		//#endif

		m_SphereCollider.radius = Mathf.Max(m_ViewDistance, m_HearDistanceFar);
	}

	private void OnTriggerStay(Collider other)
	{
		if (m_Script_ConeOfSightRenderer.enabled || this.enabled)
		{
			if (other.gameObject == m_Player)
			{
				m_PlayerDetected = false;
				m_PlayerDetectedInSight = false;
				m_PlayerDetectedInAudibleArea = false;

				//Check Sighting
				Vector3 direction1 = other.transform.position - transform.position;
				Vector3 direction2 = other.transform.position - (transform.position + transform.up * 0.75f);

				//Debug.Log(Vector3.Angle(direction1, transform.forward) + " - " + Vector3.Angle(direction2, transform.forward));
				float angle = Vector3.Angle(direction1, transform.forward);

				if (angle < m_ViewAngle * 0.5f)
				{
					RaycastHit hit;
					// Careful when using transform.up since it might lead to some incorret results depeding on model size and scaling
					Debug.DrawRay(transform.position + transform.up, direction1.normalized * m_ViewDistance * transform.localScale.x, Color.blue);
					Debug.DrawRay(transform.position + transform.up, direction2.normalized * m_ViewDistance * transform.localScale.x, Color.blue);

					//int layer = ~(1 << LayerMask.NameToLayer("Enemy")); // Avoid hitting other enemies colliders
					int layer = LayerMask.GetMask("Player", "Obstacle"); // Better idea to only hit player and obstacles
					if (Physics.Raycast(transform.position + transform.up, direction1.normalized, out hit, m_ViewDistance * transform.localScale.x, layer))
					{
						if (hit.collider.gameObject == m_Player)
						{
							m_PlayerDetected = true;
							m_PlayerDetectedInSight = true;
							//Debug.Log("Player Sight Detected head");
						}
						//else
						//{
						//Debug.Log("Cone Hit: " + hit.collider.gameObject.name);
						//}
					}
					if (!m_PlayerDetected && Physics.Raycast(transform.position + transform.up, direction2.normalized, out hit, m_ViewDistance * transform.localScale.x, layer))
					{
						if (hit.collider.gameObject == m_Player)
						{
							m_PlayerDetected = true;
							m_PlayerDetectedInSight = true;
							//Debug.Log("Player Sight Detected feet");
						}
						//else
						//{
						//Debug.Log("Cone Hit: " + hit.collider.gameObject.name);
						//}
					}
				}

				if (Vector3.Distance(transform.position, m_Player.transform.position) <= (m_HearDistanceClose * transform.localScale.x))
				{
					GradientColorHearFar();
					m_PlayerInAudibleArea = true;
					m_PlayerDetected = true;
					m_PlayerDetectedInAudibleArea = true;
					//Debug.Log("Player Detected in Inner Hear Area");
				}
				else if (Vector3.Distance(transform.position, m_Player.transform.position) <= (m_HearDistanceFar * transform.localScale.x))
				{
					if (m_Script_PlayerController.CurrentSpeed() >= m_HearMinSpeed)
					{
						//Debug.Log("Player Entered Outer Hear Area");						
						if (m_TimeHearFar >= m_DurationHearFar)
						{
							m_PlayerDetected = true;
							m_PlayerDetectedInAudibleArea = true;
							//Debug.Log("Player Detected in Outer Hear Area");
						}
						m_PlayerInAudibleArea = true;
						GradientColorHearFar();
					}
				}
				else
				{
					m_TimeHearFar = 0;
					m_PlayerInAudibleArea = false;
					GradientColorHearFar();
				}

				if (m_PlayerDetected)
				{
					// set here chase state or end game
					if (m_GameOverDetection)
					{
						m_Script_PlayerController.SetTerrified();
						StartCoroutine(WaitAndSetGameOver());
					}
				}
				else
				{
					// continue patrol state
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (m_Script_ConeOfSightRenderer.enabled || this.enabled)
		{
			if (other.gameObject == m_Player)
			{
				m_TimeHearFar = 0;
				GradientColorHearFar();
				m_PlayerInAudibleArea = false;
				m_PlayerDetected = false;
				m_PlayerDetectedInSight = false;
				m_PlayerDetectedInAudibleArea = false;
				m_RenderArea = true;
				m_Script_ConeOfSightRenderer.SetRenderingCone(true);
				// set here patrol state
			}
		}
	}

	private void GradientColorHearFar()
	{
		float value = Mathf.Lerp(0f, 1f, m_TimeHearFar / m_DurationHearFar);
		m_TimeHearFar += Time.deltaTime;
		Color color = m_GradientHearFar.Evaluate(value);
		m_MaterialHearFar.color = color;
	}

	public void SetRenderingHearingArea(bool active)
	{
		m_RenderHearingArea = active;
	}

	IEnumerator WaitAndSetGameOver()
	{
		yield return new WaitForSeconds(m_TimeGameOver);
		string text = "GAME OVER\n\nSorry. You have been caught up.";
		m_Script_GameController.EndLevel(Script_GameController.EndOption.Lose, text);
		yield return null;
	}

	private void OnEnable()
	{
		//Activate render cone and hearing
	}

	private void OnDisable()
	{
		//Deactivate render cone and hearingw
	}

}
