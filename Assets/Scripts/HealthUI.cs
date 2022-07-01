using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class HealthUI : MonoBehaviour
{

    public PhotonView pv;
    public Health healthComponent;
    public TextMeshProUGUI healthText;
    public Image healthSlider;

    private void OnEnable()
    {
        if(pv.IsMine == false)
        {
            return;
        }

        healthComponent.OnHealthChanged += OnHealthUpdated;
    }

    private void OnDisable()
    {
        if (pv.IsMine == false)
        {
            return;
        }

        healthComponent.OnHealthChanged -= OnHealthUpdated;
    }

    public void OnHealthUpdated(int health)
    {
        healthText.text = health.ToString();
        healthSlider.fillAmount = ((float)health) / 100;
    }
}
