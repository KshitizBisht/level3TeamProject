using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject userDataUI;
    public GameObject CreateProfileUI;
    public GameObject profilesUI;
    public GameObject AboutUI;
    public GameObject RoomUI;
    public GameObject viewCodeUI;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI

    public void ClearScreen() //Turn off all screens
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUI.SetActive(false);
        CreateProfileUI.SetActive(false);
        profilesUI.SetActive(false);
        AboutUI.SetActive(false);
        RoomUI.SetActive(false);
        viewCodeUI.SetActive(false);

    }

    public void OpenURL()
    {
        Application.OpenURL("https://viarama.co.uk/"); // Does not make any changes in the UI
    }

    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void RegisterScreen() // Register button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void UserDataScreen() //Logged in
    {
        ClearScreen();
        userDataUI.SetActive(true);
    }

    public void RoomSelectionUI()
    {
        ClearScreen();
        RoomUI.SetActive(true);
    }

    public void CreateProfile() {
        ClearScreen();
        CreateProfileUI.SetActive(true);
    }

    public void Profiles()
    {
        ClearScreen();
        profilesUI.SetActive(true);
    }

    public void About()
    {
        ClearScreen();
        AboutUI.SetActive(true);
    }

    public void viewCode()
    {
        ClearScreen();
        viewCodeUI.SetActive(true);
    }



}
