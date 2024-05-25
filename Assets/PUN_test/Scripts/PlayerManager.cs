using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        public float health = 100;
        [SerializeField] private GameObject beams;
        [SerializeField] private CameraWork cameraWork;
        [SerializeField] private GameObject playerUiPrefab;
        bool isFiring;

        private void Awake()
        {
            beams.SetActive(false);

            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (photonView.IsMine)
                cameraWork.OnStartFollowing();
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return;
            if (other.name.Contains("Beam"))
            {
                health -= 1f;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine) return;
            if (other.name.Contains("Beam"))
            {
                health -= 1f * Time.deltaTime;
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInputs();
                if (beams != null && isFiring != beams.activeInHierarchy)
                {
                    beams.SetActive(isFiring);
                }
                if (health <= 0)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

#if !UNITY_5_4_OR_NEWER
        private void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
            GameObject _uiGo = Instantiate(playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        private void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1") && !isFiring)
            {
                isFiring = true;
            }
            if (Input.GetButtonUp("Fire1") && isFiring)
            {
                isFiring = false;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(health);
                stream.SendNext(isFiring);
            }
            else
            {
                health = (float)stream.ReceiveNext();
                isFiring = (bool)stream.ReceiveNext();
            }
        }

#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif
    }
}