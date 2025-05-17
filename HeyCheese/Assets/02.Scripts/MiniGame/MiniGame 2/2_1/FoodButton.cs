using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodButton : MonoBehaviour
{
    private MiniGame2_1Manager miniGame2_1Manager;

    public MiniGame2_1Manager.FoodName foodName;

    void Start()
    {
        miniGame2_1Manager = FindObjectOfType<MiniGame2_1Manager>();
    }

    public void onClick()
    {
        if (miniGame2_1Manager != null)
            miniGame2_1Manager.SetFoodStateDictionary(foodName, true);
    }

}
