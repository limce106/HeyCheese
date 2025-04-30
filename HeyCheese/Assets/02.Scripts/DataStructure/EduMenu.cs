public class EduMenu
{ 
    public string  EmotionID { get; private set; }
    public string SituationID { get; private set; }
    public string IconPath { get; private set; }

    public EduMenu(string emotionID, string situationID, string iconPath)
    {
        EmotionID = emotionID;
        SituationID = situationID;
        IconPath = iconPath;
    }
}
