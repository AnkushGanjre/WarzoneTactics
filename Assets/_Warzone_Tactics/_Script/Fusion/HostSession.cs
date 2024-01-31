using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class HostSession : MonoBehaviour
    {
        private Button _hostSessionBtn;

        private GameObject _hostPanel;
        private TextMeshProUGUI _hostSessionIDText;
        private Button _hostSessionBackButton;


        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // List of available letters
        private int _sessionNameLength = 6;


        private void Awake()
        {
            _hostSessionBtn = GameObject.Find("Host_Button").GetComponent<Button>();

            _hostPanel = GameObject.Find("Host_Panel");
            _hostSessionIDText = GameObject.Find("HostPanel_SessionID_Text").GetComponent<TextMeshProUGUI>();
            _hostSessionBackButton = GameObject.Find("HostSessionBack_Button").GetComponent<Button>();
        }

        private void Start()
        {
            _hostSessionBtn.onClick.AddListener(() => { OnHostSessionBtnClick(); });
            _hostSessionBackButton.onClick.AddListener(() => { OnHostSessionBackBtnClick(); });

            _hostPanel.SetActive(false);
        }

        private void OnHostSessionBtnClick()
        {
            _hostPanel.SetActive(true);
            GenerateUniqueSessionName(out string roomID);
            _hostSessionIDText.text = "Room ID- " + roomID;
            Debug.Log(roomID);

            FusionManager.Instance.GameRoomHost(roomID);
        }

        public void OnHostSessionBackBtnClick()
        {
            _hostPanel.SetActive(false);
            FusionManager.Instance.Runner.Shutdown();
        }

        private void GenerateUniqueSessionName(out string result)
        {
            string sessionName = "";
            for (int i = 0; i < _sessionNameLength; i++)
            {
                int randomIndex = Random.Range(0, _letters.Length);
                sessionName += _letters[randomIndex];
            }

            // Check if the 

            result = sessionName;
        }
    }
}

