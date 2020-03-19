using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Sprite _cameraImage;
    [SerializeField]
    Sprite _mapImage;
    private GameObject[] arButtons;

    private void Awake()
    {
        SceneManager.LoadScene("Location");
        arButtons = GameObject.FindGameObjectsWithTag("ExtraARButtons");
        changeARButtonsState(true);
    }

    //Switch between Camera and map scene
    public void switchScenes()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        GameObject buttonImage = GameObject.Find("UI Button Image");
        print(sceneName);
        if(sceneName == "SelectPath")
        {
            SceneManager.LoadScene("Location");
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1].sprite = _cameraImage;
        }
        else
        {
            SceneManager.LoadScene("SelectPath");
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1].sprite = _mapImage;
        }
    }

    //Show or hide the extra AR UI buttons
    public void changeARButtonsState(bool turnButtonsOff)
    {
        bool state = turnButtonsOff ? false : !arButtons[0].activeSelf;

        foreach(GameObject button in arButtons)
        {
            button.SetActive(state);
        }
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        changeARButtonsState(true);
    }
}
