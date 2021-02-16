using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private NavMeshPath _navMeshPath;

    [SerializeField]
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        if (_target == null)
        {
            _target = GameObject.Find("TargetPos");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.enabled)
        {
            MoveToTarget(_target.transform.position);
        }
    }

    private void MoveToTarget(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
        
    }
}
