using UnityEngine;
using System.Collections;

//https://www.youtube.com/watch?v=mBGUY7EUxXQ&list=PLX2vGYjWbI0QGyfO8PKY1pC8xcRb0X-nP&index=21&t=0s
//https://gitlab.science.ru.nl/ru/NDL-OVR/-/tree/0191c3cfc079d79230989c5a3a01c487c3441d54/Assets%2FScripts%2FEnemy
//https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
//https://answers.unity.com/questions/1088188/change-color-gradually.html

// Requires both isTrigger Collider and Rigidbody either in player or in enemy 
//[ExecuteAlways]
public class Script_WetFloor : MonoBehaviour
{
	[SerializeField] float m_SpeedFallDown = 1.01f;
	[SerializeField] float m_SpeedFall = 0.601f;
	[SerializeField] float m_TimeNextFall = 0.7f;

	private bool m_WillFall = true;
	private GameObject m_Player;
	private Script_PlayerController m_Script_PlayerController;

	private void Awake()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_Script_PlayerController = m_Player.GetComponent<Script_PlayerController>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (m_WillFall) // Wait TimeNextFall seconds for next fall to happen
		{
			if (other.gameObject == m_Player)
			{
				if (m_Script_PlayerController.CurrentSpeed() > m_SpeedFallDown)
				{
					m_WillFall = false;
					float nextFall = m_Script_PlayerController.SetFallingDown();
					StartCoroutine(WaitForNextFall(nextFall + m_TimeNextFall));
				}
				else if (m_Script_PlayerController.CurrentSpeed() > m_SpeedFall)
				{
					m_WillFall = false;
					float nextFall = m_Script_PlayerController.SetFalling();
					StartCoroutine(WaitForNextFall(nextFall + m_TimeNextFall));
				}
			}
		}
	}

	IEnumerator WaitForNextFall(float time)
	{
		yield return new WaitForSeconds(time);
		m_WillFall = true;
		yield return null;
	}
}
