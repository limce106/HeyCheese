using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterReaction : MonoBehaviour
{
    private MiniGame2_1Manager miniGame2_1Manager;

    public MiniGame2_1Manager.CharacterName characterName;

    // 음식 하나씩 채워질 때마다
    // 앉아있는 치즈와 부기 표정이 기본 -> 미소→ 행복 → 기대 이미지 바뀜
    public enum ReactionState
    {
        Basics,
        Smile,
        Happy,
        Excitement
    }

    public ReactionState reactionState;

    // 표정 상태 이미지 리스트
    [Header("표정 이미지 리스트 | 기본->미소->행복->기대")]
    [SerializeField] private List<Sprite> ReactionImages;

    private Image characterImage;


    void Start()
    {
        miniGame2_1Manager = FindObjectOfType<MiniGame2_1Manager>();
        characterImage = GetComponent<Image>();
        InitReactionImage();
    }

    // MiniGame2_1Manager에서 메소드 호출하면 리액션 상태와 이미지 바뀜
    public void ChangeReactionImage()
    {
        reactionState = (ReactionState)miniGame2_1Manager.GetFoodInPlateNum();

        characterImage.sprite = ReactionImages[(int)reactionState];

        Debug.Log($"{characterName} 상태 : {reactionState}");
    }

    private void InitReactionImage()
    {
        reactionState = ReactionState.Basics;

        characterImage.sprite = ReactionImages[(int)reactionState];
    }
}
