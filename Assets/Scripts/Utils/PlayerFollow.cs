using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform PlayerTransform;

    public Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;
    void Start()
    {
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindWithTag("Player").transform;
        }
        _cameraOffset = transform.position - PlayerTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerTransform != null)
        {
            Vector3 newPos = PlayerTransform.position + _cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
        }

    }

    public void SetOffset()
    {
        _cameraOffset = transform.position - PlayerTransform.position;
    }
}
