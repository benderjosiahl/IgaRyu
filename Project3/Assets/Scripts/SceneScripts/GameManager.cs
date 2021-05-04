using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public levels currentLevel;

    public GameObject RespawnLocation;
    public GameObject KillVolume;

    public enum levels
    {
        CLIMBING_LEVEL,
        TELEPORT_LEVEL,
        PATROL_LEVEL,
        HUB_LEVEL
    }

    private void Awake()
    {
        instance = this;
    }

    public static void TransitionToLevel(levels destinationLevel)
    {
        print("you're going to the " + destinationLevel + " level");
        // transition to level based on level arg
        SceneManager.LoadScene(destinationLevel.ToString());
    }

    public static void WinGame()
    {
        print("you won the level");
        SceneManager.LoadScene(levels.HUB_LEVEL.ToString());

        // transition back to the hub world
    }
    
    public static void LoseGame()
    {

        print("you lost the level");

        SceneManager.LoadScene(instance.currentLevel.ToString());
    }

    public void OnTriggerEnter (Collider other)
    {
        
    }
}
