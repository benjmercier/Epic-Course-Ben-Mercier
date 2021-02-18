﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent _navMeshAgent;

    private Vector3 _target;

    private void OnEnable()
    {
        _target = SpawnManager.Instance.AssignTargetPos();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.enabled)
        {
            MoveToTarget(_target); // doesn't have to be called each frame, set in start
        }
    }

    private void MoveToTarget(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }
}
