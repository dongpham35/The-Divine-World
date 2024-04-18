using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;

    public static SceneController Instance { get=>  instance; }

    private void Awake()
    {
        if(instance != null) Destroy(instance);
        SceneController.instance = this;
    }
/*
    public static SceneController getInstance()
    {
        if (instance == null)
        {
            instance = new SceneController();
        }
        return instance;
    }*/

    public void MoveSignIn()
    {
        SceneManager.LoadScene("SignIn");
    }

    public void MoveSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }

    public void MoveMenuGame()
    {
        SceneManager.LoadScene("MenuGame");
    }
}
