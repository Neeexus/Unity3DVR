using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    public void openDoor(bool doorOpen)
    {
        Debug.Log(doorOpen);
        if (doorOpen)
            gameObject.GetComponent<Animator>().enabled = true;
    }
}
