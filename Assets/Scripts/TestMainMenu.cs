using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("TestScene"); // ���� ������ �̵�
    }

    public void OpenOptions()
    {
        Debug.Log("�ɼ� �޴��� ���� �������� �ʾҽ��ϴ�.");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ����Ƽ �����Ϳ��� ���� ����
        #else
            Application.Quit(); // ����� ���ӿ��� ����
        #endif
    }

}
