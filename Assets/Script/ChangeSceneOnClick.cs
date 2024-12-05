using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnClick : MonoBehaviour
{
    // �̵��� �� �̸�
    public string nextSceneName;

    // ���콺 Ŭ�� �̺�Ʈ ����
    private void OnMouseDown()
    {
        // ���� �� �ε�
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
