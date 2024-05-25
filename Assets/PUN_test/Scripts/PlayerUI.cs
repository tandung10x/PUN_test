using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        [Tooltip("Pixel offset from the player target")]
        [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
        [SerializeField] private Text playerNameText;
        [SerializeField] private Slider playerHealthSlider;
        private PlayerManager target;
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup canvasGroup;
        Vector3 targetPosition;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        private void Update()
        {
            playerHealthSlider.value = target.health / 100;
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer != null)
            {
                canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }

        public void SetTarget(PlayerManager target)
        {
            this.target = target;
            playerNameText.text = target.photonView.Owner.NickName;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = this.target.GetComponent<CharacterController>();
            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }
    }
}