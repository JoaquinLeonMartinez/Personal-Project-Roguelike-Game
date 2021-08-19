using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjects : MonoBehaviour
{
    RaycastHit oldHit;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //XRay();
    }

    private void XRay()
    {

        float characterDistance = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, characterDistance))
        {
            Debug.Log($"El raycast choca con {hit.collider.gameObject.name}");

            if (oldHit.transform)
            {

                // Add transparence
                Color colorA = oldHit.transform.gameObject.GetComponent<Renderer>().material.color;
                colorA.a = 1f;
                oldHit.transform.gameObject.GetComponent<Renderer>().material.SetColor("_Color", colorA);
            }

            // Add transparence
            Color colorB = hit.transform.gameObject.GetComponent<Renderer>().material.color;
            colorB.a = 0.5f;
            hit.transform.gameObject.GetComponent<Renderer>().material.SetColor("_Color", colorB);

            // Save hit
            oldHit = hit;
        }
    }
}
