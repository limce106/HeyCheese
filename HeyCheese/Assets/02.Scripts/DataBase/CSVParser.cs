using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVParser
{
    // CSV 파일
    private TextAsset eduMenuCSV = Resources.Load<TextAsset>("Datas/EduMenu");
    private TextAsset storyMenuCSV = Resources.Load<TextAsset>("Datas/StoryMenu");
    private TextAsset mainStoryCSV = Resources.Load<TextAsset>("Datas/MainStory");
    
    // Data Structure
    private Dictionary<string, List<EduMenu>> eduMenus = new Dictionary<string, List<EduMenu>>();
    private Dictionary<string, StoryMenu> storyMenus = new Dictionary<string, StoryMenu>();

    // EduMenu
    // emotionID 하위에 situationID들이 리스트로 저장되는 형태의 딕셔너리
    // 키: emotionID, 값: [{emotionID, situationID, iconPath}, ...]
    public Dictionary<string, List<EduMenu>> ParseEduMenus()
    {
        string[] lines = eduMenuCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');
            //if (fields.Length < 3) continue; // 열은 항상 3개, 데이터가 모두 차있어야 함

            string emotionID = fields[0].Trim();
            string situationID = fields[1].Trim();
            string iconPath = fields[2].Trim();

            EduMenu eduMenu = new EduMenu(emotionID, situationID, iconPath);

            if (!eduMenus.ContainsKey(emotionID))
            {
                eduMenus[emotionID] = new List<EduMenu>();
            }

            eduMenus[emotionID].Add(eduMenu); // 리스트에 추가
        }

        return eduMenus;
    }

    // StoryMenu
    // episodeID 하위에 chapterTitle들이 리스트로 저장되는 형태의 딕셔너리
    // 키: episodeID, 값: StoryMenu 객체 -> {"episdoeID": StoryMenu객체, ...}
    // 에피소드 클릭 시 에피소드 번호에 따라 메인스토리 진행될 수 있도록 해야 됨
    public Dictionary<string, StoryMenu> ParseStoryMenus()
    {
        string[] lines = storyMenuCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string episodeID = fields[0].Trim();
            string chapterTitle = fields[1].Trim();
            string imagePath = fields[2].Trim();

            StoryMenu storyMenu = new StoryMenu(episodeID, chapterTitle, imagePath);

            if (!storyMenus.ContainsKey(episodeID))
            {
                storyMenus[episodeID] = storyMenu;
            }
        }

        return storyMenus;
    }

    // MainStory
    //public Dictionary<string, List<EduMenu>> ParseMainStories()
    //{
    //    string[] lines = mainStoryCSV.text.Split('\n');

    //    for (int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
    //    {
    //        if (string.IsNullOrWhiteSpace(lines[i])) continue;

    //        string[] fields = lines[i].Split(',');

    //        string episodeID = fields[0].Trim();
    //        string chapterTitle = fields[1].Trim();
    //        string imagePath = fields[2].Trim();

    //        StoryMenu storyMenu = new StoryMenu(episodeID, chapterTitle, imagePath);

    //        if (!storyMenus.ContainsKey(episodeID))
    //        {
    //            storyMenus[episodeID] = new List<StoryMenu>();
    //        }

    //        storyMenus[episodeID].Add(storyMenu); // 리스트에 추가
    //    }

    //    return eduMenus;
    //}
}
