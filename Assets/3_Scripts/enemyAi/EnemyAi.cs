using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Idle,
    Wandering,
    Alert,
    chase,
    Attack
   
}

public class EnemyAi : MonoBehaviour
{
    EnemyState currentState = EnemyState.Idle;
    Vector3 destination;
    public GameObject Player;


    void Start()
    {
        destination = GetRandomDestination();
    }


    void Update()
    {
        Player = GameObject.FindWithTag("Player");
        UpdateFSM();

    }


    void UpdateFSM()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Debug.Log("Enemy is idle");

                 if (Random.value < 0.5f)
                {
                    currentState = EnemyState.Wandering;
                }

                else if (Player != null && Player.activeInHierarchy && IsPlayerInsideColliderBool())
                {
                    currentState = EnemyState.Alert;
                }

                break;

            case EnemyState.Wandering:
                Debug.Log("Enemy is wandering");
                if (Player != null && Player.activeInHierarchy && IsPlayerInsideColliderBool())
                {
                    currentState = EnemyState.Alert;
                }
                else if (ReachedDestination())
                {

                    destination = GetRandomDestination();

                    if (Random.value < 0.5f)
                    {
                        currentState = EnemyState.Idle;
                    }
                }
                else
                {
                    Vector3 desiredVelocity = (destination - transform.position).normalized * 1.5f;
                    Vector3 steeringForce = desiredVelocity - GetComponent<Rigidbody>().velocity;
                    GetComponent<Rigidbody>().AddForce(steeringForce);
                  
                }
                break;


            case EnemyState.chase:
                Debug.Log("enemy is chasing");
              

                  //when enemy reach player
                    if (Vector3.Distance(Player.transform.position, transform.position) <= 2.0f)
                    {
                      
                        Debug.Log("enemy is now attaking");
                        currentState = EnemyState.Attack;
                       
                    }
                    else
                    {
                        // go towards player
                        Vector3 desiredVelocity = (Player.transform.position - transform.position).normalized * 20.0f;
                        Vector3 steeringForce = desiredVelocity - GetComponent<Rigidbody>().velocity;
                        GetComponent<Rigidbody>().AddForce(steeringForce);
                    }

                break;

            case EnemyState.Alert:
                Debug.Log("Enemy is Alreted ");
                // play some alert animation 
                currentState=EnemyState.chase;
              
                break;



            case EnemyState.Attack:

              

                if (Vector3.Distance(Player.transform.position, transform.position) > 2.0f)
                {

                    Debug.Log("Get back here bij");
                    currentState = EnemyState.chase;

                }
                else
                {
                   Debug.Log("enemy whacking u");
                  
                }

                break;
        }
    }


   

    bool ReachedDestination()
    {
        float distance = Vector3.Distance(transform.position, destination);
        if (distance < 0.1f)
        {
            return true;
        }
        return false;
    }

    bool IsPlayerInsideColliderBool()
    {
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            return false;
        }
        return collider.bounds.Contains(Player.transform.position);

    }

    Vector3 GetRandomDestination()
    {
        float x = Random.Range(-56f, 56f);
        float z = Random.Range(-56f, 56f);
        return new Vector3(x, transform.position.y, z);
    }
}
