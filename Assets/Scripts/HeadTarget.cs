using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadTarget : MonoBehaviour
{
    public float lookRadius = 10f;
    GameObject Object;
    public Rig rig = null;
    MultiAimConstraint MA;
    float distance;
    private bool instancier;
    GameObject oldOne;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AfterInstance());
    }

    // Update is called once per frame
    void Update()
    {
        Object = GameObject.FindGameObjectWithTag("Player");
        if (instancier)
        {
            if (distance <= lookRadius)
            {
                
            }
            if (Object != null)
            {

            }
            else
            {

            }

            if (Object != null)
            {
                distance = Vector3.Distance(Object.transform.position, transform.position);
            }
        }
    }
    IEnumerator AfterInstance()
    {
        yield return new WaitForSeconds(1);
        instancier = true;
        rig = this.gameObject.GetComponentInChildren<Rig>();
        MA = rig.GetComponentInChildren<MultiAimConstraint>();
        oldOne = MA.data.sourceObjects.GetTransform(0).gameObject;
        if (!rig)
        {
            Debug.Log("Il est ou le rig? Tag le !");
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
