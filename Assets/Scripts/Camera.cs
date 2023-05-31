using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float Fspeed = 5f; // speed of the horizontal movement

    void Update()
    {
        transform.Translate(Vector3.forward * Fspeed * Time.deltaTime);
        if (Fspeed <= 20)
        {
            Fspeed *= 1.00001f;
        }
    }
}
