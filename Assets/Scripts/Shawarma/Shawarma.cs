using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shawarma : MonoBehaviour
{
    internal ShawrmaType shawarmaType;
    public Transform target;
    private Vector3 InitialPos;
    private NavMeshAgent agent;
    bool OnStartMove;
    private void Start()
    {
        InitialPos = transform.position;
    }
    private void OnStart()
    {
        // target = GameObject.Find("TargetPoint").transform;
        agent = GetComponent<NavMeshAgent>();
        //  Debug.Log("rrrrr   " + agent);
        if (target != null)
        {
            agent.SetDestination(target.position);
            OnStartMove = true;
        }
    }
    public void SetTarget(Transform _Target)
    {
        target = _Target;
        //Select Randomly on percentage chance of golden shawrma probability
        shawarmaType = ShawrmaType.Regular;
        Invoke("OnStart", 0.1f);
    }

    void Update()
    {
        // Optional: Continuously update destination if the target moves
        if (OnStartMove)
        {
            // Debug.Log(agent.remainingDistance);
            if (agent.remainingDistance < 1.1f)
            {
                OnStartMove = false;
                transform.position = InitialPos;
                gameObject.SetActive(false);

            }
            // agent.SetDestination(target.position);
        }
    }
}
public enum ShawrmaType
{
    Regular,
    Rear,
    Golden
}
