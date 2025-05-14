public class StoryMenu
{ 
    public string EpisodeID { get; private set; }
    public string ChapterTitle { get; private set; }
    public string ImagePath { get; private set; }

    public StoryMenu(string episodeID, string chapterTitle, string imagePath)
    {
        EpisodeID = episodeID;
        ChapterTitle = chapterTitle;
        ImagePath = imagePath;
    }
}
