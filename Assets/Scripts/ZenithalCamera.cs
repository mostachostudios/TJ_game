using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZenithalCamera : MonoBehaviour
{
    public float flatDistance = 5;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + new Vector3(0.0f, flatDistance, -flatDistance);
        transform.LookAt(player.transform.position);
    }
}
