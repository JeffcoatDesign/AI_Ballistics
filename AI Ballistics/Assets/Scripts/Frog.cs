using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Frog : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    [SerializeField] private float m_force = 5f;
    [SerializeField] private bool useMin = true;
    private Rigidbody m_rb;
    private Vector3 spawn;
    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();

        spawn = transform.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3? force = CalculateFiringSolution(transform.position, m_target.position, m_force);
            if (force.HasValue) m_rb.AddForce(m_force * force.Value.normalized, ForceMode.VelocityChange);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            Respawn();
        }
    }
    public void Respawn()
    {
        transform.position = spawn;
        m_rb.velocity = Vector3.zero;
        m_rb.angularVelocity = Vector3.zero;
    }
    public Vector3? CalculateFiringSolution (Vector3 start, Vector3 end, float muzzleV)
    {
        Vector3 delta = end - start;

        var a = Physics.gravity.sqrMagnitude;
        var b = -4 * (Vector3.Dot(Physics.gravity, delta) + muzzleV * muzzleV);
        var c = 4 * delta.sqrMagnitude;

        var b2minus4ac = b * b - 4 * a * c;
        if (b2minus4ac < 0) return null;

        var time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b2minus4ac)) / (2 * a));
        var time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b2minus4ac)) / (2 * a));

        float timeToTarget;

        if (time0 < 0)
        {
            if (time1 < 0)
                return null;
            else
                timeToTarget = time1;
        }
        else if (time1 < 0)
            timeToTarget = time0;
        else
        {
            if (useMin)
                timeToTarget = Mathf.Min(time0, time1);
            else
                timeToTarget = Mathf.Max(time0, time1);
        }
        return ((delta * 2) - (Physics.gravity * (timeToTarget * timeToTarget)) / (2 * muzzleV * timeToTarget));
    }
}
