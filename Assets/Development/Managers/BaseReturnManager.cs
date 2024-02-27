using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseReturnManager : Singleton<BaseReturnManager>
{
    public void ReturnToBase()
    {
        StartCoroutine(ReturnBaseCoroutine());
    }

    IEnumerator ReturnBaseCoroutine()
    {
        SceneManagement.Instance.LoadLevel("Map Scene");
        yield return null;
        //Ara iþlemler
        yield return null;
        SceneManagement.Instance.UnloadLevel("Combat Scene");
    }
}