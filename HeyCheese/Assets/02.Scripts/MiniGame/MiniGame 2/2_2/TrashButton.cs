using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashButton : MonoBehaviour
{
    private MiniGame2_2Manager miniGame2_2Manager;

    public MiniGame2_2Manager.ToDoList toDoThing;

    void Start()
    {
        miniGame2_2Manager = FindObjectOfType<MiniGame2_2Manager>();
    }

    public void onClick()
    {
        if (miniGame2_2Manager != null)
        {
            // miniGame2_2Manager¿« toDoList µÒº≈≥ ∏Æ ªÛ≈¬ «ÿ¥Á«œ¥¬ ¿œ »Ωºˆ +1 «ÿ¡‹
            miniGame2_2Manager.IncrementCurrentToDoThingCount(toDoThing);

            this.gameObject.SetActive(false);
        }

    }
}
