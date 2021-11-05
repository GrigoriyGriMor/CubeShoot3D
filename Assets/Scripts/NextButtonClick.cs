using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NextButtonClick : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevel   = sceneIndex + 1 != SceneManager.sceneCountInBuildSettings ? sceneIndex + 1 : 0;

        // Если продули - грузим ту же сцену
        if (!GameController.link.win) {
            nextLevel = sceneIndex;
        }

        SceneManager.LoadScene(nextLevel);
    }
}
