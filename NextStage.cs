using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextStage : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name == "Main_Menu")
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name == "Room0")
            {
                SceneManager.LoadScene("Room1");
            }
            else if(SceneManager.GetActiveScene().name == "Room2")
            {
                SceneManager.LoadScene("Room1");
            }
            else if (SceneManager.GetActiveScene().name == "Room1")
            {
                SceneManager.LoadScene("Room3");
            }
            else if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                SceneManager.LoadScene("Room0");
            }
            


        }
    }

}

