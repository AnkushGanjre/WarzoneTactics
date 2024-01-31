using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class JoinSession : MonoBehaviour
    {
        private Button _joinSessionBtn;

        private GameObject _joinPanel;
        private TMP_InputField _joinSessionInputField;
        private Button _joinSessionSubmitButton;
        private Button _joinSessionBackButton;


        private void Awake()
        {
            _joinSessionBtn = GameObject.Find("Join_Button").GetComponent<Button>();

            _joinPanel = GameObject.Find("Join_Panel");
            _joinSessionInputField = GameObject.Find("JoinSession_InputField").GetComponent<TMP_InputField>();
            _joinSessionSubmitButton = GameObject.Find("JoinSessionSubmit_Button").GetComponent<Button>();
            _joinSessionBackButton = GameObject.Find("JoinSessionBack_Button").GetComponent<Button>();
        }

        private void Start()
        {
            _joinSessionBtn.onClick.AddListener(() => { OnJoinSessionBtnClick(); });
            _joinSessionSubmitButton.onClick.AddListener(() => { OnJoinSessionSubmitBtnClick(); });
            _joinSessionBackButton.onClick.AddListener(() => { OnJoinSessionBackBtnClick(); });

            _joinPanel.SetActive(false);
        }

        private void OnJoinSessionBtnClick()
        {
            _joinPanel.SetActive(true);
        }

        private void OnJoinSessionSubmitBtnClick()
        {
            string roomID = _joinSessionInputField.text.ToUpper();
            Debug.Log(roomID);
            
            FusionManager.Instance.GameRoomClient(roomID);
        }

        public void OnJoinSessionBackBtnClick()
        {
            _joinSessionInputField.text = "";
            _joinPanel.SetActive(false);
            FusionManager.Instance.Runner.Shutdown();
        }
    }
}

