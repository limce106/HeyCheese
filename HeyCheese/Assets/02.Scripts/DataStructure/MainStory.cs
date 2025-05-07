[System.Serializable]
public class MainStory
{
    //public string EpisodeID { get; set; }
    //public string ChapterTitle { get; set; }
    //public int ID { get; set; }
    //public int NextID { get; set; }
    //public string EventType { get; set; }
    //public string ScreenEffect { get; set; }
    //public string SpeakerID { get; set; }
    //public string ScriptID { get; set; }
    //public int Score { get; set; }
    //public string Choice1 { get; set; }
    //public string Choice2 { get; set; }
    //public string Choice3 { get; set; }
    //public string MiniGame { get; set; }
    //public string SpeakerImageID { get; set; }
    //public string ImageID { get; set; }
    //public string VideoID { get; set; }
    //public string ARFilterID { get; set; }
    //public string FrameID { get; set; }
    //public string BGM { get; set; }
    //public string SFX { get; set; }

    public string EpisodeID { get; private set; }
    public string ChapterTitle { get; private set; }
    public int ID { get; private set; }
    public int NextID { get; private set; }
    public string EventType { get; private set; }
    public string ScreenEffect { get; private set; }
    public string SpeakerID { get; private set; }
    public string ScriptID { get; private set; }
    public int Score { get; private set; }
    public string Choice1 { get; private set; }
    public string Choice2 { get; private set; }
    public string Choice3 { get; private set; }
    public string MiniGame { get; private set; }
    public string SpeakerImageID { get; private set; }
    public string ImageID { get; private set; }
    public string VideoID { get; private set; }
    public string ARFilterID { get; private set; }
    public string FrameID { get; private set; }
    public string BGM { get; private set; }
    public string SFX { get; private set; }

    public MainStory(string episodeID, string chapterTitle, int id, int nextID, string eventType, string screenEffect, string speakerID, string scriptID,
        int score, string choice1, string choice2, string choice3, string minigame, string speakerImageID, string imageID, string videoID, string arFilterID, string frameID, string bgm, string sfx)
    {
        EpisodeID = episodeID;
        ChapterTitle = chapterTitle;
        ID = id;
        NextID = nextID;
        EventType = eventType;
        ScreenEffect = screenEffect;
        SpeakerID = speakerID;
        ScriptID = scriptID;
        Score = score;
        Choice1 = choice1;
        Choice2 = choice2;
        Choice3 = choice3;
        MiniGame = minigame;
        SpeakerImageID = speakerImageID;
        ImageID = imageID;
        VideoID = videoID;
        ARFilterID = arFilterID;
        FrameID = frameID;
        BGM = bgm;
        SFX = sfx;
    }
}
