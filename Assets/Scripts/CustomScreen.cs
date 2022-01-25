using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomScreen : MonoBehaviour
{
    [Header("Selection")]
    public TMP_InputField roomSelected;


    public void SelectOption1()
    {
        roomSelected.text = "100";
        UIManager.instance.UserDataScreen();
    }
    public void SelectOption2()
    {
        roomSelected.text = "200";
        UIManager.instance.UserDataScreen();
    }
    public void SelectOption3()
    {
        roomSelected.text = "300";
        UIManager.instance.UserDataScreen();
    }
}
