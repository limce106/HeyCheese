using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiddenCharacterButton : MonoBehaviour
{
    public enum CharacterIconName
    {
        Cheese,
        Bugi
    }
    public CharacterIconName characterName;

    public GameObject CorrectCircleImage;
    private Button characterButton;

    private MiniGame1Manager miniGame1Manager;

    void Start()
    {
        miniGame1Manager = FindObjectOfType<MiniGame1Manager>();
        characterButton = GetComponent<Button>();

        initHiddenState();
    }

    public void onClick()
    {
        CorrectCircleImage.SetActive(true);
        characterButton.interactable = false;

        miniGame1Manager.IncrementFindOutScore();
        miniGame1Manager.SetCharacterIcon((int)characterName);
    }

    public void initHiddenState()
    {
        if (characterButton == null)
            return;
        CorrectCircleImage.SetActive(false);
        characterButton.interactable = true;
    }

}
