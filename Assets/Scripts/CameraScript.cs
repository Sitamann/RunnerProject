using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float Fspeed = 5f; // speed of the horizontal movement

    void Update()
    {
        transform.Translate(Vector3.forward * Fspeed * Time.deltaTime);
        if (Fspeed <= 10)
        {
            Fspeed *= 1.000025f;
        }
    }
}
