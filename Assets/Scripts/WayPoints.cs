using UnityEngine;
using System.Collections;

public class WayPoints : MonoBehaviour
{

    // put the points from unity interface
    public Transform[] wayPointList;

    public int currentWayPoint = 0;
    Transform targetWayPoint;
    public float rotationSpeed = 0.05f;

    public float speed = 4f;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // check if we have somewere to walk
        if (currentWayPoint < this.wayPointList.Length)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();

        }else
        {
            currentWayPoint = 0;
            targetWayPoint = null;
        }
        

    }

    void walk()
    {
        // rotate towards the target
        if(targetWayPoint.position != transform.position)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(targetWayPoint.position - transform.position),
                Time.deltaTime * rotationSpeed
            );
            // move towards the target
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);
        }
        

        if (transform.position == targetWayPoint.position)
        { 
            targetWayPoint = wayPointList[currentWayPoint];
            currentWayPoint++;
        }
       
    }
}
