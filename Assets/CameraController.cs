using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetMonster;

    public Vector3 startPosition;

    public float smoothSpeed = 10f;

    public Vector3 cameraOffset;
    public Vector3 startCameraOffset;

    Vector3 desiredPosition;

    public void Start()
    {
        
    }

    public void Awake()
    {
        startPosition = transform.position;
        startCameraOffset = cameraOffset;
    }

    public IEnumerator FocusOnTarget(Transform _target, Vector3 _offset)
    {
        //StopAllCoroutines();
        //if (_target == TargetMonster)
            //yield return null;

        TargetMonster = _target;
        cameraOffset = _offset;

        desiredPosition = TargetMonster.position + cameraOffset;

        while (transform.position != desiredPosition)
        {
            //Camera.main.orthographicSize = Mathf.MoveTowards(13f, 12.5f, smoothSpeed);
            Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            yield return null;
        }
    }

    public IEnumerator ResetPosition()
    {
        //StopAllCoroutines();

        desiredPosition = startPosition + startCameraOffset;

        while (transform.position != startPosition)
        {
            //Camera.main.orthographicSize = Mathf.MoveTowards(12.5f, 13f, smoothSpeed);
            Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            yield return null;
        }
    }
}
