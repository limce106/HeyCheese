using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVParser
{
    // CSV 파일
    // Resources 폴더에서 빌드 시 포함된 .csv 파일을 불러옴
    // but 런타임에 외부에서 다운로드한 파일은 Resources.Load로 접근 불가
    private TextAsset storyMenuCSV = Resources.Load<TextAsset>("Datas/StoryMenu");
    private TextAsset mainStoryCSV = Resources.Load<TextAsset>("Datas/MainStory");
    // Android용
    private string storyMenuCSVPath = Path.Combine(Application.persistentDataPath, "StoryMenu.csv");
    private string mainStoryCSVPath = Path.Combine(Application.persistentDataPath, "MainStory.csv");

    // Data Structure
    private Dictionary<string, StoryMenu> storyMenus = new Dictionary<string, StoryMenu>();
    //public List<MainStory> storyDataList = new List<MainStory>();

    // CSV 파일 로드
    private string[] LoadCSV(string filename, string fallbackResourcePath)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filename);

        // 플랫폼과 상관없이 저장된 파일 있다면 먼저 사용
        if (File.Exists(fullPath))
        {
            Debug.Log($"[CSVParser] Using downloaded file: {fullPath}");
            return File.ReadAllLines(fullPath);
        }

        // 저장된 파일이 없다면 Resources에서 fallback
        TextAsset resourceFile = Resources.Load<TextAsset>(fallbackResourcePath);
        if (resourceFile != null)
        {
            Debug.LogWarning($"[CSVParser] Using fallback resource: {fallbackResourcePath}");
            return resourceFile.text.Split('\n');
        }

        Debug.LogError($"CSV 파일을 찾을 수 없습니다: {filename}");
        return new string[0];
    }

    // StoryMenu
    // episodeID 하위에 chapterTitle들이 리스트로 저장되는 형태의 딕셔너리
    // 키: episodeID, 값: StoryMenu 객체 -> {"episdoeID": StoryMenu객체, ...}
    // 에피소드 클릭 시 에피소드 번호에 따라 메인스토리 진행될 수 있도록 해야 됨
    public Dictionary<string, StoryMenu> ParseStoryMenus()
    {
        Dictionary<string, StoryMenu> parsedMenusDict = new Dictionary<string, StoryMenu>();

        string[] lines = LoadCSV("StoryMenu.csv", "Datas/StoryMenu");

        if(lines.Length == 0)
        {
            Debug.LogError("StoryMenu CSV 에 아무것도 없습니다.");
            return parsedMenusDict;
        }

        for (int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string episodeID = fields[0].Trim();
            string chapterTitle = fields[1].Trim();
            string imagePath = fields[2].Trim();

            StoryMenu storyMenu = new StoryMenu(episodeID, chapterTitle, imagePath);
            parsedMenusDict[storyMenu.EpisodeID] = storyMenu;

            //if (!storyMenus.ContainsKey(episodeID))
            //{
            //    parsedMenusDict[episodeID] = storyMenu;
            //}

            //storyMenus[episodeID].Add(storyMenu); // 리스트에 추가
        }

        return parsedMenusDict;
    }

    // MainStory 파싱
    // Episode별로 스토리 로드
    // get: 에피소드 메뉴 누른 후 저장된 에피소드ID를 전달하여 해당 에피소드ID에 대한 부분만 파싱하여 저장
    // (x)return: 리스트 형태로 MainStory 객체 저장 [MainStory("Episode1", "Chapter 1", 0, 1, "dialogue", ..., "bgm1", "sfx1"),..]
    // return: 딕셔너리 형태로 {0:MainStory("Episode1", "Chapter1", 0, 1...), 1:} => {ID:MainStory객체}
    public Dictionary<int, MainStory> ParseMainStories(string episodeID)
    {
        var episodeDict = new Dictionary<int, MainStory>();

        string[] lines = LoadCSV("MainStory.csv", "Datas/MainStory");

        if (lines.Length == 0)
        {
            Debug.LogError("MainStory CSV 에 아무것도 없습니다.");
            return episodeDict;
        }

        for (int i = 1; i < lines.Length; i++) // 0은 헤더라 1부터 시작
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            //string[] fields = lines[i].Split(',');
            List<string> fields = ParseCSVLine(lines[i]);
            if (fields[0].Trim() != episodeID) continue; // 해당 episodeID에 대한 내용만 딕셔너리에 포함시키기 위한 작업


            string epiID = fields[0].Trim();
            string chapterTitle = fields[1].Trim();
            int id = int.TryParse(fields[2].Trim(), out int parsedId) ? parsedId : -1; // 공백 제거 후 변환 성공 시 true 반환 및 parsedID에 결과 저장, 실패 시 -1 대입
            int nextID = int.TryParse(fields[3].Trim(), out int parsedNextId) ? parsedNextId : -1;
            string eventType = fields[4].Trim();
            string screenEffect = fields[5].Trim();
            string speakerID = fields[6].Trim();
            string scriptID = fields[7].Trim();
            int score = int.TryParse(fields[8].Trim(), out int parsedScore) ? parsedScore : -1;
            string choice1 = fields[9].Trim();
            string choice2 = fields[10].Trim();
            string choice3 = fields[11].Trim();
            string minigame = fields[12].Trim();
            string speakerImageID = fields[13].Trim();
            string imageID = fields[14].Trim();
            string videoID = fields[15].Trim();
            string arFilterID = fields[16].Trim();
            string frameID = fields[17].Trim();
            string bgm = fields[18].Trim();
            string sfx = fields[19].Trim();

            MainStory data = new MainStory(episodeID, chapterTitle, id, nextID, eventType, screenEffect, speakerID, scriptID, score, choice1, choice2, choice3, minigame, speakerImageID, imageID, videoID, arFilterID, frameID, bgm, sfx);
            //storyDataList.Add(data);
            episodeDict[data.ID] = data;

            // int 타입인 ID, NextID 파싱 실패 시 디버그 표시
            if (data.ID == -1 || data.NextID == -1)
            {
                Debug.LogWarning($"[MainStory] ID or NextID 파싱 실패: Line {i}, Episode: {fields[0]}");
            }

            Debug.Log($"[CSVParser] EpisodeID: {episodeID}, Parsed Line: {lines[i]}");
            Debug.Log($"[CSVParser] Adding ID: {id} for Episode: {epiID}");
        }
        //return storyDataList;
        return episodeDict;
    }

    // CSV에서 따옴표(") 사이에 있는 컴마(,)는 파싱하지 않도록 처리하는 파써
    // Get: CSV의 한 라인을 받음
    // Return: 컴마(,)별로 파싱한 라인 리스트
    public List<string> ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for(int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if(c=='\"') // 큰따옴표면
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes) // 큰따옴표 속 컴마가 아닐 경우
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }            
        }
        result.Add(current);
        return result;
    }
}
