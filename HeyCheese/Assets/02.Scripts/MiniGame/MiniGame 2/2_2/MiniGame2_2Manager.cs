using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGame2_2Manager : MonoBehaviour
{
    public enum ToDoList
    {
        PickUpTissue,
        PickUpFoodWaste,
        SortDishes
    }
    // 할 일 상태 딕셔너리
    private Dictionary<int, bool> toDoListDictionary = new Dictionary<int, bool>();

    [SerializeField] private List<int> currentToDoThingCountsList;  // 현재 할일 별 치운 개수 리스트
    [SerializeField] private List<int> toDoThingFinishCriteriaList; // 할일 별 치운 개수 완료 기준 리스트

    [Header("패널들")]
    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private GameObject ClearPanel;



    private void Awake()
    {
        // toDoListDictionary 초기화 딱 한번만
        InitToDoListDictionary();
    }

    void Start()
    {
        GuidePanel.SetActive(true);
        ClearPanel.SetActive(false);
    }

    public void StartCleanTheTable()
    {
        GuidePanel.SetActive(false);
    }

    // toDoListDictionary 초기화
    private void InitToDoListDictionary()
    {
        toDoListDictionary.Add((int)ToDoList.PickUpTissue, false);
        toDoListDictionary.Add((int)ToDoList.PickUpFoodWaste, false);
        toDoListDictionary.Add((int)ToDoList.SortDishes, false);

        currentToDoThingCountsList = new List<int> { 0, 0, 0 };
        toDoThingFinishCriteriaList = new List<int> { 5, 2, 6 };
    }

    public void IncrementCurrentToDoThingCount(ToDoList toDoList)
    {
        currentToDoThingCountsList[(int)toDoList] += 1;
        CheckFinishToDoThing(toDoList);

        // 코루틴 위치가 여기가 맞나..? 메소드의 역할이 애매해진 기분
        // 어떻게 더 코드를 괜찮게 유지 보수 할 수 있을지 생각해보기..
        StartCoroutine(CheckToDoListState());   
    }

    public void CheckFinishToDoThing(ToDoList toDoList)
    {

        switch (toDoList)
        {
            case ToDoList.PickUpTissue:
                toDoListDictionary[(int)toDoList] =
                currentToDoThingCountsList[(int)toDoList] == toDoThingFinishCriteriaList[(int)toDoList] ?
                    true : false;
                break;

            case ToDoList.PickUpFoodWaste:
                toDoListDictionary[(int)toDoList] =
                currentToDoThingCountsList[(int)toDoList] == toDoThingFinishCriteriaList[(int)toDoList] ?
                    true : false;
                break;

            case ToDoList.SortDishes:
                toDoListDictionary[(int)toDoList] =
                currentToDoThingCountsList[(int)toDoList] == toDoThingFinishCriteriaList[(int)toDoList] ?
                    true : false;
                break;
        }

        if (toDoListDictionary[(int)toDoList])
            Debug.Log($"{toDoList} : 할일 완료!");
    }


    private IEnumerator CheckToDoListState()
    {
        // toDoListDictionary 의 모든 value가 true이면
        bool allTrue = toDoListDictionary.Values.All(value => value);

        if (allTrue)
        {
            yield return new WaitForSeconds(1.5f);

            // 식탁 다 치우기 성공!
            // 클리어 패널 활성화

            ClearPanel.SetActive(true);
        }
        else
            yield return null;
    }

}
