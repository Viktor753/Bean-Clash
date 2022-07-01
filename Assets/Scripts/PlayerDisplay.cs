using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image icon;

    private void Awake()
    {
        //Default to nothing
        nameText.text = string.Empty;
    }
}
