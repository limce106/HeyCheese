using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameNextStep : MonoBehaviour
{
    public void BackToMainStory()
    {
        MainStoryGameManager.MainStoryGM.NextStep();
    }
     
}
