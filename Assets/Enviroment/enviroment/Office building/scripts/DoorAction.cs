using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAction : MonoBehaviour {


   

   

    private PlayerInputHandler inputHandler;

    void Start()
    {
        inputHandler = GetComponentInParent<PlayerInputHandler>();
    }

    void Update()
    {
        if (inputHandler != null && inputHandler.InteractPressed)
        {
            RaycastHit hit;
            float interactDistance = 3f;

            if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance))
            {
                if (hit.transform.CompareTag("door"))
                {
                    Door door = hit.transform.GetComponent<Door>();
                    if (door != null)
                    {
                        door.ActionDoor();
                    }
                }

                if (hit.collider.gameObject.name.StartsWith("Button floor"))
                {
                    pass_on_parent pop = hit.transform.GetComponent<pass_on_parent>();
                    if (pop != null && pop.MyParent != null)
                    {
                        evelator_controll ec = pop.MyParent.GetComponent<evelator_controll>();
                        if (ec != null)
                        {
                            ec.AddTaskEve(hit.collider.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}
