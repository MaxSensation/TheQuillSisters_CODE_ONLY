// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using UnityEngine;

public class TrailChecker : MonoBehaviour
{
    [SerializeField] 
    private LayerMask layerMask = default;

    private SphereCollider _sphereCollider;
    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.localPosition;
        _sphereCollider = GetComponent<SphereCollider>();
    }

    public bool CheckPath(IEnumerable<Vector3> points)
    {
        transform.localPosition = _startPos;
        foreach (var point in points)
        {
            transform.position = point;
            if (Physics.CheckSphere(transform.position, _sphereCollider.radius, layerMask))
            {
                return true;
            }
        }

        return false;
    }
}