using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public GameObject Player;

    private Vector3 player_position;
    private CheckPoint cp = new CheckPoint();
    private float walkSpeed = 1.0f;
    public static bool isEnter = false;
    private bool isEnter_player = false;


    private void Start()
    {



    }

    private void Update()
    {
        player_position = new Vector3(Player.transform.position.x, 10.006f, Player.transform.position.z);

        if (isEnter)//방에 입장하면 일어난 후 쳐다보기 시작
        {
           
            
        
            if (!isEnter_player) // 플레이어와 충돌하기 전까지 걷기
            {
                transform.LookAt(player_position);
                gameObject.GetComponent<Animator>().SetBool("Walk", true);
                gameObject.transform.Translate(Vector3.forward.x * walkSpeed * Time.deltaTime, 0, Vector3.forward.z * walkSpeed * Time.deltaTime);

                  
            }
            else
            {
                 gameObject.transform.Translate(0, 0, 0);//멈춤
                 gameObject.GetComponent<Animator>().SetBool("Breath", true);
                 isEnter = false;
                 gameObject.GetComponent<Animator>().SetBool("Walk", false);
          



            }
                

           
        }




    }

   
    void OnCollisionEnter(Collision other)
    {
        // 바닥에 닿았는지 확인
        if (other.collider.CompareTag("Player"))
        {
            isEnter_player = true;
            //Debug.Log("true");
        }

        else
        {
            isEnter_player = false;
            //Debug.Log("false");
        }

    }
}
