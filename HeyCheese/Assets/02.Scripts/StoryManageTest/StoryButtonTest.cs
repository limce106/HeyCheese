using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 학습 스토리 버튼 클릭 시 처리
public class StoryButtonTest : MonoBehaviour
{
    public StoryDataSOTest storyData;
    private StoryManagerTest storyManagerTest;

    private void Start()
    {
        storyManagerTest = StoryManagerTest.Instance;
    }

    public void OnClick()
    {

        Debug.Log($"레벨: {storyData.level}, 감정: {storyData.emotion}, 상황: {storyData.situation}");
        // StoryManager에 전달하거나 직접 처리 가능
        storyManagerTest.OnStartStoryButtonClicked(storyData.emotion, storyData.situation, storyData.level);
    }
}
