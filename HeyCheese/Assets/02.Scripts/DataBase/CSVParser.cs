using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVParser
{
    // CSV 파일
    private TextAsset eduMenuCSV = Resources.Load<TextAsset>("Datas/EduMenu");
    //private TextAsset eduStoryCSV = Resources.Load<TextAsset>("Datas/EduStory");
    //private TextAsset mainStoryCSV = Resources.Load<TextAsset>("Datas/MainStory");

    // Data Structure
    private Dictionary<string, List<EduMenu>> eduMenus = new Dictionary<string, List<EduMenu>>();

    // EduMenu
    // emotionID 하위에 situationID들이 리스트로 저장되는 형태의 딕셔너리
    // 키: emotionID, 값: [{emotionID, situationID, iconPath}, ...]
    public Dictionary<string, List<EduMenu>> ParseEduMenus()
    {
        string[] lines = eduMenuCSV.text.Split('\n');

        for(int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
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
}
