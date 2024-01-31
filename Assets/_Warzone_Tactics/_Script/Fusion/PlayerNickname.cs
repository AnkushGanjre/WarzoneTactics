using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class PlayerNickname : MonoBehaviour
    {
        private PlayerDataManager _playerDataManager;
        private GameObject _settingsPanel;
        private TMP_InputField _playerNameInputField;
        private TextMeshProUGUI _playerNameDisplayText;

        private Button _settingsButton;
        private Button _settingsSubmitButton;
        private Button _settingsBackButton;

        private void Awake()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            _settingsPanel = GameObject.Find("Settings_Panel");
            _playerNameInputField = GameObject.Find("NewPlayerName_InputField").GetComponent<TMP_InputField>();
            _playerNameDisplayText = GameObject.Find("PlayerName_DisplayText").GetComponent<TextMeshProUGUI>();


            _settingsButton = GameObject.Find("Settings_Button").GetComponent<Button>();
            _settingsSubmitButton = GameObject.Find("SettingsSubmit_Button").GetComponent<Button>();
            _settingsBackButton = GameObject.Find("SettingsBack_Button").GetComponent<Button>();
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("PlayerNickname"))
            {
                _playerDataManager.NickName = PlayerPrefs.GetString("PlayerNickname");
            }
            else if (string.IsNullOrWhiteSpace(_playerDataManager.NickName))
            {
                var rngPlayerNumber = Random.Range(0, 999);
                _playerDataManager.NickName = $"Player_{rngPlayerNumber.ToString("000")}";
            }

            _playerNameDisplayText.text = _playerDataManager.NickName;

            _settingsButton.onClick.AddListener(() => { OnSettingButton(); });
            _settingsSubmitButton.onClick.AddListener(() => { OnPlayerNameChange(); });
            _settingsBackButton.onClick.AddListener(() => { OnSettingsBackBtn(); });
            _settingsPanel.SetActive(false);
        }

        private void OnPlayerNameChange()
        {
            if (_playerNameInputField.text != "")
            {
                PlayerPrefs.SetString("PlayerNickname", _playerNameInputField.text);
                PlayerPrefs.Save(); // Save the PlayerPrefs to persist the data
            
                _playerNameDisplayText.text = _playerNameInputField.text;
            }

            _playerDataManager.NickName = _playerNameInputField.text;
            _settingsPanel.SetActive(false);
        }

        private void OnSettingButton()
        {
            _settingsPanel.SetActive(true);
        }

        private void OnSettingsBackBtn()
        {
            _settingsPanel.SetActive(false);
        }
    }
}

