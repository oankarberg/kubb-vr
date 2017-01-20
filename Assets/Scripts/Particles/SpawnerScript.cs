using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class SpawnerScript : MonoBehaviour {
	public float spawnTime = 3f;		// The amount of time between each spawn.
	public float spawnDelay = 0f;		// The amount of time before spawning starts.
	
	public float spawnCrowTime = 8f;
	public float spawnCrowDelay = 6f;

	float[] particle_states;
	float[] center;

	public GameObject[] selectorArr;

    public GameObject parentForFlock;

    //Rotate direction for bird wings, -1 or 1 
    private int[] rotateDirection;
    private float wingRotationSpeed = 70.0f;
    private GameObject [] object_prefabs;		// Array of prefabs.
	private int number;
	private Transform[] IC;
	public const int number_of_part = 20;
    private Vector3 lastPosition = new Vector3(0.0f, 0.0f, 0.0f);
	[DllImport("ParticlePlugin")]
	private static extern void flockTogether(float[] particle_states,float[] center,float dt, int number_of_part);

    // Use this for initialization
    void Start() {
       
        rotateDirection = new int[number_of_part];
        if (parentForFlock == null)
        {
            Debug.LogError("Must Assign parent to flock!");
        }
        particle_states = new float[number_of_part * 9];
        center = new float[3];
        center[0] = 1.0f;
        center[1] = 1.0f;
        center[2] = 0.0f;
        ////position

        ////velocity
        for (int i = 0; i < number_of_part * 9; i = i + 9) {
            particle_states[i] = 0.0f;
            particle_states[i + 1] = 0.0f;
            particle_states[i + 2] = 0.0f;
            particle_states[i + 3] = UnityEngine.Random.Range(0.0f, 4.0f);
            particle_states[i + 4] = UnityEngine.Random.Range(0.0f, 4.0f);
            particle_states[i + 5] = UnityEngine.Random.Range(0.0f, 4.0f);
            particle_states[i + 6] = 0.0f;
            particle_states[i + 7] = 0.0f;
            particle_states[i + 8] = 0.0f;
            rotateDirection[i / 9] = 1;
        }

        object_prefabs = new GameObject[1];
        object_prefabs[0] = Resources.Load<GameObject>("Prefabs/seagull_prefab");
        

        number = 0;
		Spawn ();
		//InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}
	
	void Spawn ()
	{
		if(number>=number_of_part)return;
		selectorArr = new GameObject[number_of_part];
		//transform.position = new Vector3(5.0f, 5.0f, 5.0f);
        int half_particles = 0;
        if ((number_of_part % 2) == 0)
        {
            half_particles = number_of_part * 9 / 2;
        }
        else
        {
            half_particles =(number_of_part + 1)* 9 / 2;
        }
		    
		
		for(int i=0; i<half_particles; i = i+9){
			float r = 2.0f;
			float x_perturb = UnityEngine.Random.Range (-r, r);
			float y_perturb = UnityEngine.Random.Range (-r, r);
			float z_perturb = UnityEngine.Random.Range (-r, r);
			Vector3 pos = new Vector3(transform.position.x+x_perturb, transform.position.y+y_perturb, transform.position.z+z_perturb);
			GameObject go = Instantiate(object_prefabs[0], pos, transform.rotation) as GameObject;
			go.transform.localScale = Vector3.one / 1;//* new Vector3(0.4f, 0.4f, 0.4f);
            go.transform.parent = parentForFlock.transform;
            
            ///parentForFlock.transform.position = new Vector3(-50.0f, 10.0f, 0.0f);
            particle_states [i] = go.transform.position[0];
			particle_states [i + 1] = go.transform.position [1];
			particle_states [i +2] = go.transform.position[2];
			selectorArr[i/9] = go;
			number++;
		}
		//transform.position = new Vector3(-5.0f, -5.0f, -5.0f);
		for(int i=half_particles; i<number_of_part*9; i = i+9){
			float r = 4.0f;
			float x_perturb = UnityEngine.Random.Range (-r, r);
			float y_perturb = UnityEngine.Random.Range (-r, r);
			float z_perturb = UnityEngine.Random.Range (-r, r);
			Vector3 pos = new Vector3(transform.position.x+x_perturb, transform.position.y+y_perturb, transform.position.z+z_perturb);
			GameObject go = Instantiate(object_prefabs[0], pos, transform.rotation) as GameObject;
			go.transform.localScale = Vector3.one / 1;//* new Vector3(0.4f, 0.4f, 0.4f);
            go.transform.parent = parentForFlock.transform;
            
            //parentForFlock.transform.position = new Vector3(-50.0f, 10.0f, 0.0f);
            particle_states [i] = go.transform.position[0];
			particle_states [i + 1] = go.transform.position [1];
			particle_states [i + 2] = go.transform.position[2];
			selectorArr[i/9] = go;
			number++;
		}
	}
	
	// Update is called once per frame
	void Update () {

		float dt = Time.deltaTime;
        //		Debug.Log("dt " + dt +  "   " + center[2] );
        
		flockTogether (particle_states,center,dt, number_of_part);
        Vector3 velocity = parentForFlock.transform.position - lastPosition;

        for (int i = 0; i < number_of_part*9; i = i + 9) {
//			Debug.Log (selectorArr[i / 9 ].transform.position + " i "  + particle_states[i]);

			selectorArr[i / 9].transform.localPosition = new Vector3 (particle_states[i],particle_states[i+1],particle_states[i+2]) * 0.5f;
           
            //selectorArr[i / 9].transform.Rotate(this.transform.up, 90);

            Vector3 dir = new Vector3(particle_states[i+3] , particle_states[i + 4] ,
                particle_states[i + 5]);

            if (velocity != Vector3.zero)
            {
                float rotationSpeed = 1.0f;
                selectorArr[i / 9].transform.rotation = Quaternion.Slerp(
                    selectorArr[i / 9].transform.rotation,
                    Quaternion.LookRotation(velocity),
                    dt  * rotationSpeed
                );
                 
               
            }

            
            Vector3 birdSpeed = new Vector3(particle_states[i+3], particle_states[i+4], particle_states[i+5]);
            //Rotate wingposition if speed upwards is above 0.5
            if (birdSpeed.y > 0.5f)
            {
                float absAcc = Math.Abs(birdSpeed.y);
                //RIGHT WING
                selectorArr[i / 9].transform.GetChild(0).GetChild(6).Rotate(Vector3.right * dt * rotateDirection[i / 9] * wingRotationSpeed * absAcc);
                //LEFT WING
                selectorArr[i / 9].transform.GetChild(0).GetChild(7).Rotate(-(Vector3.right * dt * rotateDirection[i / 9] * wingRotationSpeed * absAcc));
                
                float rotationXleft = selectorArr[i / 9].transform.GetChild(0).GetChild(7).eulerAngles.x;
                float rotationXright = selectorArr[i / 9].transform.GetChild(0).GetChild(6).eulerAngles.x;

                if (rotationXleft < 320.0f && rotationXleft > 30.0f && rotationXright < 330.0f && rotationXright > 40.0f)
                {
                    rotateDirection[i / 9] = -rotateDirection[i / 9];
                }

            }
            
            else {
                //Else rotate to original rotation to keep wings in place
                Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                selectorArr[i / 9].transform.GetChild(0).GetChild(6).localRotation = rotation;
                //TODO: Fix interpolation to initial wing position to get smooth transition
                /*= Quaternion.Slerp(
                    selectorArr[i / 9].transform.GetChild(0).GetChild(6).localRotation,
                    rotation,
                    dt * 4.0f
                );
                */

                selectorArr[i / 9].transform.GetChild(0).GetChild(7).localRotation = rotation; 
                /* Quaternion.Slerp(
                    selectorArr[i / 9].transform.GetChild(0).GetChild(7).localRotation,
                    rotation,
                    dt * 4.0f
                );
                */
               


            }
            
           

        }
        lastPosition = parentForFlock.transform.position;


    }
}