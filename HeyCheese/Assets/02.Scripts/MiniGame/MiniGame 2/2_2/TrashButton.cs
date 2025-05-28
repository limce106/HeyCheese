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
            // miniGame2_2Manager의 toDoList 딕셔너리 상태 해당하는 일 횟수 +1 해줌
            miniGame2_2Manager.IncrementCurrentToDoThingCount(toDoThing);
            // 해야 할 일 TMP 업데이트하여 진행도 반영
            miniGame2_2Manager.UpdateToDoListTMPS(toDoThing);

            this.gameObject.SetActive(false);
        }

    }
}
