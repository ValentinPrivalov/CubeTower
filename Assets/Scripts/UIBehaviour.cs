using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBehaviour : MonoBehaviour {
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenInstagram() {
        Application.OpenURL("https://www.instagram.com/valik.privalov/");
    }
}
