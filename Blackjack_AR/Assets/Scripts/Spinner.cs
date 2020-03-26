using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(transform.position,transform.up,Time.deltaTime * 90f);
    }
}
