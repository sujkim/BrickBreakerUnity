using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource bgMusic;
    // Start is called before the first frame update
    void Awake()
    {
        AudioListener.volume = 0.1f;
    }
    void Start()
    {
        // AudioListener.volume = 0.1f;


        // bgMusic.volume = 0.1f;
        // bgMusic.loop = true;
        // bgMusic.Play();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
