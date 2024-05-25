using Photon.Pun;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float directionDampTime = 0.25f;

        private void Update()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Base Layer.Run") && Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }
    }
}