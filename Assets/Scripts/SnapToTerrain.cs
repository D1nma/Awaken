using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapToTerrain : MonoBehaviour
{
    public float offsetY = 1;
    public bool snap = true;
    public LayerMask groundMask;


    // Update is called once per frame
    void Update()
    {
        if (snap)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + Vector3.up * 100 , Vector3.down , out hit , 1000 , groundMask))
            {
                transform.position = hit.point + Vector3.up * offsetY;
            }
        }
    }
}
