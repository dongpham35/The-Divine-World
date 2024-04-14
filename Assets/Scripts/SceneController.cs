using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Start()
    {
        if (instance == null)
            Destroy(instance);
        else
        {
            instance = this;
        }
    }

    public void MoveSignIn()
    {
        SceneManager.LoadScene(0);
    }

    public void MoveSignUp()
    {
        SceneManager.LoadScene(1);
    }


}
