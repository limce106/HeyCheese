public static class SceneHistoryManager
{
    public static string PreviousSceneName { get; private set; }

    public static void SetPreviousScene(string sceneName)
    {
        PreviousSceneName = sceneName;
    }
}

