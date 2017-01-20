using UnityEngine;
using System.Collections;

public class ControllSpeed : MonoBehaviour {

    private Animator anim;
    private float controlSpeed;
    private float randomOffset;
    public AudioSource horseScream;
    public AudioSource longHorseScream;
    // Use this for initialization
    void Start () {
        // Get the Animator component from your gameObject
        anim = transform.GetComponent<Animator>();
        controlSpeed = transform.GetComponent<WayPoints>().speed;
        anim.SetFloat("Speed", controlSpeed);
        randomOffset = Random.Range(0.0f, 40.0f);
    }
	

	void Update () {
        
        //Run
        if ((int)(Time.time + randomOffset) % 20 == 0) // Idle
        {

            controlSpeed = 0.0f;
            transform.GetComponent<WayPoints>().speed = controlSpeed;
            anim.SetFloat("Speed", controlSpeed);
            if((int)(Time.time + randomOffset) % 60 == 0){ horseScream.Play(); }
            
        }
        else    if ((int)(Time.time + randomOffset) % 73 == 0) //Walk
        {

            controlSpeed = 2.0f;
            transform.GetComponent<WayPoints>().speed = controlSpeed;
            anim.SetFloat("Speed", controlSpeed);
            horseScream.Play();
        }
        else if ((int)(Time.time + randomOffset) % 101 == 0)
        {
            controlSpeed = 5.0f;
            transform.GetComponent<WayPoints>().speed = controlSpeed;
            anim.SetFloat("Speed", controlSpeed);
            longHorseScream.Play();
        }
    }
}
