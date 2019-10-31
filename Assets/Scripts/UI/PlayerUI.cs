using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerController target;
    
    [SerializeField] private Text playerNameText;
    [SerializeField] private Slider playerHealthSlider;

    [SerializeField] private Vector3 screenOffset = new Vector3(0f,2);
    
    private Transform targetTransform;
    private Vector2 targetPosition;
    
    void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        
        playerHealthSlider.maxValue = PlayerController.MaxHealth;
        playerHealthSlider.value = PlayerController.MaxHealth;
    }

    public void SetTarget(PlayerController playerTarget)
    {
        if (playerTarget == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        target = playerTarget;
        
        playerNameText.text = target.photonView.Owner.NickName;
        
        targetTransform = target.transform;
    }

    private void LateUpdate()
    {
        targetPosition = targetTransform.position;
        transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
    }

    public void UpdateHealth()
    {
        playerHealthSlider.value = target.Health;
    }
}
