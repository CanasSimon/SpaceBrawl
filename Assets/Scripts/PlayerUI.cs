using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player target;
    
    [SerializeField] private Text playerNameText;
    [SerializeField] private Slider playerHealthSlider;

    [SerializeField] private Vector3 screenOffset = new Vector3(0f,2);
    
    private Transform targetTransform;
    private SpriteRenderer targetRenderer;
    private CanvasGroup canvasGroup;
    private Vector2 targetPosition;
    
    void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetTarget(Player playerTarget)
    {
        if (playerTarget == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        target = playerTarget;
        playerNameText.text = target.photonView.Owner.NickName;
        
        targetTransform = target.transform;
        targetRenderer = target.GetComponentInChildren<SpriteRenderer>();
        Debug.Log(targetRenderer);
    }
    
    /*void Update()
    {
        playerHealthSlider.value = target.Health;
    }*/

    private void LateUpdate()
    {
        canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        
        targetPosition = targetTransform.position;
        transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
    }
}
