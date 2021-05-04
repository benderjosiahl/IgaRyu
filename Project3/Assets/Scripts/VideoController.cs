using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private float vidTimer;

    private void Awake()
    {
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        vidTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        vidTimer += Time.deltaTime;
        print(vidTimer);
        if(vidTimer >= videoPlayer.length)
        {
            GameManager.WinGame();
        }
    }
}
