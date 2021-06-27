using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuOverlay, numbersCanvas;
    public GameObject EventManager;
    EventManager EventManagerScript;
    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 0f;
        numbersCanvas.SetActive(false);
        mainMenuOverlay.SetActive(true);
        if (EventManager != null)
            EventManagerScript = EventManager.GetComponent<EventManager>();
    }
    public void StartSim()
    {
        Time.timeScale = 1f;
        numbersCanvas.SetActive(true);
        mainMenuOverlay.SetActive(false);
        if (EventManagerScript != null)
            EventManagerScript.BeginBuild();
    }
}
