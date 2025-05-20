using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MiniGame2_1Manager : MonoBehaviour
{
    public enum FoodName
    {
        Tteokbokki,
        Pizza,
        Chicken
    }

    public enum CharacterName
    {
        Cheese,
        Bugi
    }

    // 음식 담은 상태 딕셔너리
    private Dictionary<int, bool> foodState = new Dictionary<int, bool>();
    // 담겨진 음식 이미지 리스트
    // 떡볶이, 피짜, 치킨 순으로 이미지 리스트 넣기
    [SerializeField] private List<GameObject> foodInPlateImages;

    [Header("패널들")]
    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private GameObject ClearPanel;

    [Header("치즈와 부기")]
    [SerializeField] private List<CharacterReaction> characterReactionsList;

    private int foodInPlateNum = 0;

    private void Awake()
    {
        // 음식 담은 상태 딕셔너리 초기화 한번만
        InitFoodStateDictionary();

        GuidePanel.SetActive(true);
        ClearPanel.SetActive(false);
    }

    public void StartPutFoodInPlate()
    {
        GuidePanel.SetActive(false);
    }

    // 클릭된 음식 버튼으로 접시에 음식이 채워짐
    private void PutFooldInPlate(FoodName foodName)
    {
        foodInPlateImages[(int)foodName].SetActive(true);

        // 캐릭터 리액션 변경 메소드 호출
        foreach(CharacterReaction characterReaction in characterReactionsList)
        {
            characterReaction.ChangeReactionImage();
        }

        StartCoroutine(CheckfoodInPlateState());
    }

    // 현재 담겨진 음식 개수 현황
    public int GetFoodInPlateNum()
    {
        foodInPlateNum = 0;

        foreach(bool isTrue in foodState.Values)
        {
            if (isTrue)
                ++foodInPlateNum;
        }

        Debug.Log($"현재 담긴 음식 개수 : {foodInPlateNum}");
        return foodInPlateNum;
    }


    // 클릭된 음식의 상태를 true로 바꿔줌
    public void SetFoodStateDictionary(FoodName foodName, bool isTrue)
    {
        foodState[(int)foodName] = isTrue;

        PutFooldInPlate(foodName);

        Debug.Log($"{foodName} is Clicked!");
    }

    // FoodStateDictionary 초기화
    private void InitFoodStateDictionary()
    {
        foodState.Add((int)FoodName.Tteokbokki, false);
        foodState.Add((int)FoodName.Pizza, false);
        foodState.Add((int)FoodName.Chicken, false);

        foodInPlateNum = 0;
    }

    private IEnumerator CheckfoodInPlateState()
    {
        // foodInPlateNum가 3개면
        if (foodInPlateNum == 3)
        {
            yield return new WaitForSeconds(1.5f);

            // 모든 음식 다 나눠주기 성공!
            // 클리어 패널 활성화

            ClearPanel.SetActive(true);
        }
        else
            yield return null;
    }
}
