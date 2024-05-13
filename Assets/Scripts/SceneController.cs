using Assets.Scripts.Models;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviourPunCallbacks
{
    private static SceneController instance;

    public static SceneController Instance { get=> instance; }

    private void Awake()
    {
        if(instance != null) Destroy(instance);
        SceneController.instance = this;
    }


    public void MoveToSignIn()
    {
        SceneManager.LoadScene("SignIn");
    }

    public void MoveToSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }

    public void MoveToMenuGame()
    {
        SceneManager.LoadScene("MenuGame");
    }


    public void MoveToDemo()
    {
        SceneManager.LoadScene("Demo");
    }

    public void MoveToLoading()
    {
        SceneManager.LoadScene("Loading");
    }



}
