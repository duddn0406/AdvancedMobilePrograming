using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnClick : MonoBehaviour
{
    // 이동할 씬 이름
    public string nextSceneName;

    // 마우스 클릭 이벤트 감지
    private void OnMouseDown()
    {
        // 다음 씬 로드
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
