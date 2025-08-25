using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    [SerializeField] private GameObject GameMenu;
    [SerializeField] private GameObject MenuOpçoes;

    public void Load_Game()
    {
        SceneManager.LoadScene("Game");
    }

    public void Options()
    {
        GameMenu.SetActive(false);
        MenuOpçoes.SetActive(true);
    }

    public void Back()
    {
        MenuOpçoes.SetActive(false);
        GameMenu.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Sair do jogo");
        Application.Quit();
    }

}
