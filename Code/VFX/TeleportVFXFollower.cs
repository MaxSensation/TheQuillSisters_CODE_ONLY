// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using UnityEngine;

public class TeleportVFXFollower : MonoBehaviour
{
    [SerializeField]
    private Transform objectToFollow = default;
    [SerializeField] [Range(0f, 1f)]
    private float speed = default;
    [SerializeField]
    private float height = default;

    private TrailRenderer _trailRenderer;

    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        StartCoroutine(DelayedStart());
    }

    private void Update()
    {
        if (objectToFollow == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, objectToFollow.transform.position + Vector3.up * height, speed);
        }
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        _trailRenderer.emitting = true;
    }
}