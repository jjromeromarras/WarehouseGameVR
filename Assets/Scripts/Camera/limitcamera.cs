using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limitcamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 6, player.transform.position.z);
    }
}
