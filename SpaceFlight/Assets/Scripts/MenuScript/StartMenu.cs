using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("MenuLVL");
    }
    public void SetingButton()
    {
        SceneManager.LoadScene("MenuSeting");
    }
    public void ExitButton()
    {

    }
}
