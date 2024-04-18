using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public static SceneController getInstance()
    {
        if (instance == null)
        {
            instance = new SceneController();
        }
        return instance;
    }

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
