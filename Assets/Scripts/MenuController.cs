using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene((int)Scenes.SpaceIntro);
    }
}
