using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

     void OnTriggerEnter(Collider other)
    {
        // 바닥에 닿았는지 확인
        if (other.CompareTag("Player"))
        {
            EnemyMovement.isEnter = true;
            //Debug.Log("true");
        }
            
        else
        {
           EnemyMovement.isEnter = false;
            //Debug.Log("false");
        }
           
    }
    
}
