using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // message shown to player when looking at interactable
    public string promptMessage;


    public void BaseInteract()
    {
        Interact();
    }
    // Start is called before the first frame update
    protected virtual void Interact()
    {
        // to be overwritten
    }
}
