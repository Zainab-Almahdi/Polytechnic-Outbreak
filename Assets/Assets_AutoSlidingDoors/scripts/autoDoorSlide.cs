using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class autoDoorSlide : MonoBehaviour
{
    List<GameObject>    objectsInTriggerZone;

    public GameObject[] doors;
    public Vector3[]    doorsClosedPosition;
    public Vector3[]    doorsOpenPosition;

    public float    doorSpeed = 3;
    public float    doorOpenDuration = 3;

    public AudioClip    soundOpenDoor;
    public AudioClip    soundCloseDoor;
    AudioSource         doorSound;

    float    doorCloseTime;
    bool     openDoors;
    bool     playedCloseSound;

	void Start ()
    {
        playedCloseSound = true;

        doorSound = GetComponent<AudioSource>();
        objectsInTriggerZone = new List<GameObject>();

        for ( int i = 0; i < doors.Length; i++ )
            doorsClosedPosition[i] = doors[i].transform.localPosition;
	}
	
	void Update ()
    {
        if (openDoors)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                // The closer the door gets to its final open position, increase the Lerp Speed ( this makes the door movement look more realistic )
                Vector3 vecDistanceLeft = doors[i].transform.localPosition - doorsOpenPosition[i];
                float doorSpeedModifier = 1 / vecDistanceLeft.magnitude;

                doors[i].transform.localPosition = Vector3.Lerp(doors[i].transform.localPosition, doorsOpenPosition[i], Time.deltaTime * doorSpeed * doorSpeedModifier);
            }
        }
        else
        {
            if (doorCloseTime <= Time.time)
            {
                if (playedCloseSound == false)
                {
                    playedCloseSound = true;
                    doorSound.clip = soundCloseDoor;
                    doorSound.Play();
                }

                for (int i = 0; i < doors.Length; i++)
                {
                    // The closer the door gets to its final closed position, increase the Lerp Speed ( this makes the door movement look more realistic )
                    Vector3 vecDistanceLeft = doors[i].transform.localPosition - doorsClosedPosition[i];
                    float doorSpeedModifier = 1 / vecDistanceLeft.magnitude;

                    doors[i].transform.localPosition = Vector3.Lerp(doors[i].transform.localPosition, doorsClosedPosition[i], Time.deltaTime * doorSpeed * doorSpeedModifier);
                }
            }
        }
	}


    private void OnTriggerEnter(Collider other)
    {
        if (objectsInTriggerZone.Contains(other.gameObject) == false)
        {
            // this is the first object that entered the zone,  open the door!
            if (objectsInTriggerZone.Count == 0)
            {
                openDoors = true;
                doorSound.clip = soundOpenDoor;
                doorSound.Play();
            }

            objectsInTriggerZone.Add(other.gameObject);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if ( objectsInTriggerZone.Contains( other.gameObject ) == true )
            objectsInTriggerZone.Remove( other.gameObject );

        // if there's no more objects in the trigger zones, then close the door
        if (objectsInTriggerZone.Count <= 0)
        {
            doorCloseTime = Time.time + doorOpenDuration;
            openDoors = false;
            playedCloseSound = false;
        }
    }

}
