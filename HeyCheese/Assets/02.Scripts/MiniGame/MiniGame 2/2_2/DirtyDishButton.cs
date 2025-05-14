using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirtyDishButton : MonoBehaviour
{
    private MiniGame2_2Manager miniGame2_2Manager;

    public MiniGame2_2Manager.ToDoList toDoThing;

    private RectTransform rectTransform;
    private Button button;

    void Start()
    {
        miniGame2_2Manager = FindObjectOfType<MiniGame2_2Manager>();
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    public void onClick()
    {
        if (miniGame2_2Manager != null)
        {
            // miniGame2_2Manager의 toDoList 딕셔너리 상태 해당하는 일의 것을 
            // 접시의 rectTransform 포지션을 
            // Vector2.zero 으로 맞춰서 가운데에 정리되게 함.
            rectTransform.localPosition = Vector3.zero;

            // 정리된 접시의 버튼은 비활성화 처리
            button.interactable = false;

            // miniGame2_2Manager에 있는 정리된 접시 개수 카운트 올리기
            miniGame2_2Manager.IncrementCurrentToDoThingCount(toDoThing);
        }

    }

}
