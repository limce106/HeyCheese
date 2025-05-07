using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadEmotionGalleryScene()
    {
        SceneManager.LoadScene("EmotionGallery");
    }
}