using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class PlayerInfoEdit : MonoBehaviour
    {
        [Header("Script Reference")]
        PlayerDataManager _playerDataManager;

        [Header("Home Page Display Elements")]
        Button _playerInfoEditBtn;
        Image _playerDisplayAvatar;
        TextMeshProUGUI _playerNameDisplayText;
        TextMeshProUGUI _playerTrophyDisplayText;
        TextMeshProUGUI _playerRankDisplayText;
        TextMeshProUGUI _playerCoinDisplayText;

        [Header("Palyer Info Edit Panel Elements")]
        GameObject _playerInfoEditPanel;
        TMP_InputField _playerNameInputField;
        Transform _playerAvatarList;
        Button _playerInfoEditSubmitBtn;
        Button _playerInfoEditBackBtn;

        int _selectedAvatarNum;

        private void Awake()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();

            _playerInfoEditBtn = GameObject.Find("PlayerInfoEdit_Button").GetComponent<Button>();
            _playerDisplayAvatar = GameObject.Find("Player_Display_Avatar").GetComponent<Image>();
            _playerNameDisplayText = GameObject.Find("PlayerName_DisplayText").GetComponent<TextMeshProUGUI>();
            _playerTrophyDisplayText = GameObject.Find("PlayerTrophy_DisplayText").GetComponent<TextMeshProUGUI>();
            _playerRankDisplayText = GameObject.Find("PlayerRank_DisplayText").GetComponent<TextMeshProUGUI>();
            _playerCoinDisplayText = GameObject.Find("PlayerCoin_DisplayText").GetComponent<TextMeshProUGUI>();

            _playerInfoEditPanel = GameObject.Find("PlayerInfoEdit_Panel");
            _playerNameInputField = GameObject.Find("NewPlayerName_InputField").GetComponent<TMP_InputField>();
            _playerAvatarList = GameObject.Find("Player_Avatars_List").GetComponent<Transform>();
            _playerInfoEditSubmitBtn = GameObject.Find("PlayerInfoSubmit_Button").GetComponent<Button>();
            _playerInfoEditBackBtn = GameObject.Find("PlayerInfoBack_Button").GetComponent<Button>();
        }

        private void Start()
        {
            //PlayerPrefs.DeleteAll();
            if (PlayerPrefs.HasKey("PlayerNickname"))
            {
                _playerDataManager.NickName = PlayerPrefs.GetString("PlayerNickname");
            }
            else if (string.IsNullOrWhiteSpace(_playerDataManager.NickName))
            {
                var rngPlayerNumber = Random.Range(0, 999);
                _playerDataManager.NickName = $"Player_{rngPlayerNumber.ToString("000")}";
            }

            if (PlayerPrefs.HasKey("PlayerAvatarNum"))
            {
                _playerDataManager.PlayerAvatarNum = PlayerPrefs.GetInt("PlayerAvatarNum");
            }
            else
            {
                int num = 1;
                _playerDataManager.PlayerAvatarNum = num;
            }

            _playerNameDisplayText.text = _playerDataManager.NickName;
            _playerDisplayAvatar.sprite = _playerAvatarList.GetChild(_playerDataManager.PlayerAvatarNum).GetComponent<Image>().sprite;

            _playerInfoEditBtn.onClick.AddListener(() => { OnPlayerEditBtnClick(); });
            _playerInfoEditSubmitBtn.onClick.AddListener(() => { OnSubmitBtnClicked(); });
            _playerInfoEditBackBtn.onClick.AddListener(() => { OnBackBtnClicked(); });

            for (int i = 1; i < _playerAvatarList.childCount; i++)
            {
                int a = i;
                Button btn = _playerAvatarList.GetChild(i).GetComponent<Button>();
                btn.onClick.AddListener(() => { OnAvatarClicked(a); });
            }

            _playerInfoEditPanel.SetActive(false);
        }

        private void UpdatePlayerTrophy()
        {
            _playerTrophyDisplayText.text = _playerDataManager.PlayerTrophyCount.ToString();
        }

        private void UpdatePlayerRank()
        {
            _playerTrophyDisplayText.text = _playerDataManager.PlayerRankNum.ToString();
        }

        private void UpdatePlayerCoins()
        {
            _playerTrophyDisplayText.text = _playerDataManager.PlayerCoinCount.ToString();
        }

        private void OnPlayerEditBtnClick()
        {
            _playerInfoEditPanel.SetActive(true);
            OnAvatarClicked(_playerDataManager.PlayerAvatarNum);
        }

        private void OnAvatarClicked(int avatarNum)
        {
            StartCoroutine(AvatarClick(avatarNum));
        }

        private IEnumerator AvatarClick(int avatarNum)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Vector3 position = _playerAvatarList.GetChild(avatarNum).position;
            _playerAvatarList.GetChild(0).position = position;
            _selectedAvatarNum = avatarNum;
        }

        private void OnSubmitBtnClicked()
        {
            if (!string.IsNullOrWhiteSpace(_playerNameInputField.text))
            {
                PlayerPrefs.SetString("PlayerNickname", _playerNameInputField.text);
                _playerNameDisplayText.text = _playerNameInputField.text;
                _playerDataManager.NickName = _playerNameInputField.text;
            }

            _playerDisplayAvatar.sprite = _playerAvatarList.GetChild(_selectedAvatarNum).GetComponent<Image>().sprite;
            _playerDataManager.PlayerAvatarNum = _selectedAvatarNum;

            PlayerPrefs.SetInt("PlayerAvatarNum", _selectedAvatarNum);
            PlayerPrefs.Save(); // Save the PlayerPrefs to persist the data
            _playerNameInputField.text = "";
            _playerInfoEditPanel.SetActive(false);
        }

        private void OnBackBtnClicked()
        {
            _playerNameInputField.text = "";
            _playerInfoEditPanel.SetActive(false);
        }
    }
}

