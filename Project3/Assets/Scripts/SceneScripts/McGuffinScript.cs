using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McGuffinScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.other.CompareTag("Player")){
            GameManager.WinGame();
        }
    }
}
