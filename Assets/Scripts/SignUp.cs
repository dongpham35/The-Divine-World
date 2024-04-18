using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SignUp : MonoBehaviour
{
    public TMP_InputField username_InputField;
    public TMP_InputField email_InputField;
    public TMP_InputField password_InputField;
    public TMP_InputField repassword_InputField;

    private string URL;
    private string username;
    private string email;
    private string password;
    private string repassword;
    private string data;
    private bool isSignUp = false;


    public void SignUpAcount()
    {
        username = username_InputField.text;
        email = email_InputField.text;
        password = password_InputField.text;
        repassword = repassword_InputField.text;

        if(username.Count() > 16)
        {
            Debug.Log("Tên tài khoản không được quá 16 ký tự");
            return;
        }
        if(password.Count() > 16)
        {
            Debug.Log("Mật không được quá 16 ký tự");
            return;
        }    
        if(!password.Equals(repassword))
        {
            Debug.Log("Mật khẩu không đúng");
            return;
        }

        URL = "http://192.168.1.7/TheDiVWorld/api/Account";
        StartCoroutine(getAccount());
        if (isSignUp)
        {
            Debug.Log("bat dau dang ky");
            StartCoroutine(saveToAccount());
        }    
    }

    IEnumerator getAccount()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(URL + "?username=" + username))
        {
            yield return request.SendWebRequest();
            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                if (!json.Equals("null", System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Tài khoản đã tồn tại");
                }
                else
                {
                    isSignUp = true;
                    data = string.Format($"?username={username}&email={email}&password={password}");
                }
            }
            request.Dispose();
        }
    }

    IEnumerator saveToAccount()
    {
        Debug.Log("vao ham save");
        using (UnityWebRequest request = UnityWebRequest.Put(URL ,data))
        {
            Debug.Log("Vao request");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Đăng ký thành công");
                SceneController.getInstance().MoveSignIn();
            }
            request.Dispose();
        }
    }


}
