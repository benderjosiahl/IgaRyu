using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour {

	public void MainMenu()
	{
		SceneManager.LoadScene("menu");
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	public void Play()
	{
		SceneManager.LoadScene("GameScene");
	}

    public void Credits()
    {
        SceneManager.LoadScene("credits");
    }

	public void SourceCode()
	{
		Application.OpenURL("https://github.com/Yurrushia/HallwayWithGuard");
	}
}
