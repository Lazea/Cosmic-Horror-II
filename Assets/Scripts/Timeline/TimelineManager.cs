using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector gameStartDirector;
    public PlayableDirector playerDeathDirector;

    // Start is called before the first frame update
    void Start()
    {
        gameStartDirector.Play();
        playerDeathDirector.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayDirector(PlayableDirector director)
    {
        director.Play();
    }
}
