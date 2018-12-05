using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private delegate void DelegateOnSceneLoad();

    private DelegateOnSceneLoad OnLoadFinished;

    public void Init()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [ContextMenu("ToGame")]
    public void ToGameScene()
    {
        StartCoroutine(LoadScene("Game"));
    }

    [ContextMenu("ToLauncher")]
    public void ToLauncher()
    {
        SceneManager.LoadScene("Launcher");
    }

    [ContextMenu("ToLobby")]
    public void ToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    [ContextMenu("ToReward")]
    public void ToReward()
    {
        SceneManager.LoadScene("Reward");
    }

    [ContextMenu("ToCredits")]
    public void ToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    private IEnumerator LoadScene(string scene)
    {
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(scene);
        if (asyncLoading == null) yield break;

        asyncLoading.allowSceneActivation = false;
        UIPanelLoading loadingScreen = MainUI.Instance.LoadingPanel;
        float progress = 0;
        loadingScreen.SetLoadingBarProcess(progress);
        loadingScreen.FadeIn();
        MusicManager.Instance.FadeOut(1f);
        yield return new WaitForSeconds(1.0f);

        while (progress <= 0.9f)
        {
            progress += 0.01f;
            loadingScreen.SetLoadingBarProcess(progress);
            yield return new WaitForSeconds(0.01f);
        }

        while (true)
        {
            if (asyncLoading.isDone || asyncLoading.progress > 0.89999f) break;
            yield return 0;
        }

        MusicManager.Instance.PlayMusic(eMusicSelection.BattleField, 2);
        loadingScreen.SetLoadingBarProcess(1);
        yield return new WaitForSeconds(2.0f);

        asyncLoading.allowSceneActivation = true;
        loadingScreen.FadeOut();

        yield break;
    }
}