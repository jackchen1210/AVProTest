using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);
    }
}
