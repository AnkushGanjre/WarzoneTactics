using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class UIController : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static UIController Instance;

        [Header("Script Reference")]
        PlayerDataManager _playerDataManager;
        CameraFollow _cameraFollow;

        [Header("Strategy Panel")]
        [SerializeField] GameObject _gameRoundPrefab;
        [SerializeField] Sprite _burnedBunkerSprite;

        [Header("GameObjects")]
        [SerializeField] GameObject _menuCanvas;
        public GameObject WorldCanvasPlayer;
        public GameObject WorldCanvasOpponent;
        GameObject WorldCanvasTarget;
        GameObject StrategyPanel;
        GameObject _transitionPanel;
        GameObject _errorPanel;

        [Header("Transforms")]
        Transform _playerLivesTransf;
        Transform _opponentLivesTransf;
        Transform _strategyTableTransf;

        [Header("Text Elements")]
        public TextMeshProUGUI PlayerNameText;
        public TextMeshProUGUI PlayerTrophyText;
        public TextMeshProUGUI OpponentNameText;
        public TextMeshProUGUI OpponentTrophyText;
        public TextMeshProUGUI GameRoundNumText;
        public TextMeshProUGUI GameCountDownText;
        public TextMeshProUGUI GameInfoText;
        TextMeshProUGUI _headerStrategyPanelText;
        TextMeshProUGUI _errorMessageText;

        [Header("Button Elements")]
        public Button GamePlayQuitButton;
        public Button StrategyButton;
        public Button StrategyBackButton;
        Button _playerStrategyButton;
        Button _opponentStrategyButton;
        Button _errorPanelBackButton;

        Animator _gameInfoTextAnimator;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _menuCanvas = GameObject.Find("Menu_Canvas");
            WorldCanvasPlayer = GameObject.Find("World_Player_Canvas");
            WorldCanvasOpponent = GameObject.Find("World_Opponent_Canvas");
            WorldCanvasTarget = GameObject.Find("World_Target_Canvas");
            StrategyPanel = GameObject.Find("Strategy_Panel");
            _transitionPanel = GameObject.Find("Transition_Panel");
            _errorPanel = GameObject.Find("Error_Panel");

            _playerLivesTransf = GameObject.Find("Player_Lives").GetComponent<Transform>();
            _opponentLivesTransf = GameObject.Find("Opponent_Lives").GetComponent<Transform>();
            _strategyTableTransf = GameObject.Find("StrategyTable_Content").GetComponent<Transform>();

            PlayerNameText = GameObject.Find("PlayerName_Text").GetComponent<TextMeshProUGUI>();
            PlayerTrophyText = GameObject.Find("PlayerTrophy_Text").GetComponent<TextMeshProUGUI>();
            OpponentNameText = GameObject.Find("OpponentName_Text").GetComponent<TextMeshProUGUI>();
            OpponentTrophyText = GameObject.Find("OpponentTrophy_Text").GetComponent<TextMeshProUGUI>();
            GameRoundNumText = GameObject.Find("GameRound_Text").GetComponent<TextMeshProUGUI>();
            GameCountDownText = GameObject.Find("CountDownTimer_Text").GetComponent<TextMeshProUGUI>();
            GameInfoText = GameObject.Find("GameInfo_Text").GetComponent<TextMeshProUGUI>();
            _headerStrategyPanelText = GameObject.Find("HeaderStrategyPanel_Text").GetComponent<TextMeshProUGUI>();
            _errorMessageText = GameObject.Find("ErrorMessage_Text").GetComponent<TextMeshProUGUI>();

            GamePlayQuitButton = GameObject.Find("GamePlayQuit_Button").GetComponent<Button>();
            StrategyButton = GameObject.Find("Strategy_Button").GetComponent<Button>();
            StrategyBackButton = GameObject.Find("StrategyClose_Button").GetComponent<Button>();
            _playerStrategyButton = GameObject.Find("PlayerStrategy_Button").GetComponent<Button>();
            _opponentStrategyButton = GameObject.Find("OpponentStrategy_Button").GetComponent<Button>();
            _errorPanelBackButton = GameObject.Find("ErrorPanelClose_Button").GetComponent<Button>();

            _gameInfoTextAnimator = GameInfoText.GetComponent<Animator>();
        }

        void Start()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            _cameraFollow = Camera.main.GetComponent<CameraFollow>();
            GamePlayQuitButton.onClick.AddListener(() => { OnGamePlayBackBtn(); });
            StrategyButton.onClick.AddListener(() => { StrategyPanel.SetActive(true); GenerateOpponentStrategy(); });
            StrategyBackButton.onClick.AddListener(() => { StrategyPanel.SetActive(false); });
            _playerStrategyButton.onClick.AddListener(() => { GeneratePlayerStrategy(); });
            _opponentStrategyButton.onClick.AddListener(() => { GenerateOpponentStrategy(); });
            _errorPanelBackButton.onClick.AddListener(() => { _errorPanel.SetActive(false); });

            StrategyPanel.SetActive(false);
            _errorPanel.SetActive(false);
            WorldCanvasPlayer.SetActive(false);
            WorldCanvasOpponent.SetActive(false);
            WorldCanvasTarget.SetActive(false);
            OnTransition();
        }

        #region Start & Exit

        public void OnBothPlayersReady()
        {
            _menuCanvas.SetActive(false);
            GameRoundNumText.gameObject.SetActive(false);
            GameCountDownText.gameObject.SetActive(false);
            WorldCanvasPlayer.gameObject.SetActive(false);
            WorldCanvasOpponent.gameObject.SetActive(false);
            _playerLivesTransf.gameObject.SetActive(false);
            _opponentLivesTransf.gameObject.SetActive(false);
        }

        public void OnGamePlayBackBtn()
        {
            _menuCanvas.SetActive(true);
            FindObjectOfType<PublicSession>().OnPublicSessionBackBtnClick();
            FindObjectOfType<HostSession>().OnHostSessionBackBtnClick();
            FindObjectOfType<JoinSession>().OnJoinSessionBackBtnClick();

            WorldCanvasPlayer.SetActive(false);
            WorldCanvasOpponent.SetActive(false);
            OnTransition();
            FusionManager.Instance.Runner.Shutdown();
        }

        #endregion

        #region Troop & Attack Selection

        public void OnTroopSelection()
        {
            GameRoundNumText.gameObject.SetActive(true);
            GameInfoText.gameObject.SetActive(false);
            _playerLivesTransf.gameObject.SetActive(true);
            _opponentLivesTransf.gameObject.SetActive(true);

            _playerDataManager.PlayerTroopSelection = 0;
            _playerDataManager.PlayerAttackSelection = 0;
            _playerDataManager.OpponentTroopSelection = 0;
            _playerDataManager.OpponentAttackSelection = 0;

            StartCoroutine(TroopSelectionDelay());
        }

        private IEnumerator TroopSelectionDelay()
        {
            yield return new WaitForSeconds(1f);

            GameInfoText.gameObject.SetActive(true);
            _gameInfoTextAnimator.SetTrigger("TextGoUpperTrgr");
            GameCountDownText.gameObject.SetActive(true);
            WorldCanvasPlayer.SetActive(true);
            _cameraFollow.OnCamTroopPlacement();
        }

        public void OnAttackSelection()
        {
            GameInfoText.gameObject.SetActive(false);
            WorldCanvasPlayer.SetActive(false);
            GameCountDownText.gameObject.SetActive(false);

            StartCoroutine(AttackSelectionDelay());
        }

        private IEnumerator AttackSelectionDelay()
        {
            yield return new WaitForSeconds(1f);

            WorldCanvasOpponent.SetActive(true);
            GameInfoText.gameObject.SetActive(true);
            _gameInfoTextAnimator.SetTrigger("TextGoUpperTrgr");
            GameCountDownText.gameObject.SetActive(true);
            _cameraFollow.OnCamAttackSelection();
        }

        #endregion

        #region Gameplay

        public void OnAttackBunkerSelected(int bunkerNum)
        {
            OnCameraDefaultPosition();
            WorldCanvasOpponent.SetActive(false);
            WorldCanvasTarget.SetActive(true);
            Transform targetTransform = WorldCanvasTarget.transform;
            foreach (Transform t in targetTransform)
            {
                t.gameObject.SetActive(false);
            }

            switch (bunkerNum)
            {
                case 6:
                    targetTransform.GetChild(4).gameObject.SetActive(true);
                    break;
                case 7:
                    targetTransform.GetChild(3).gameObject.SetActive(true);
                    break;
                case 8:
                    targetTransform.GetChild(2).gameObject.SetActive(true);
                    break;
                case 9:
                    targetTransform.GetChild(1).gameObject.SetActive(true);
                    break;
                case 10:
                    targetTransform.GetChild(0).gameObject.SetActive(true);
                    break;
            }

            GameInfoText.gameObject.SetActive(false);
            FindObjectOfType<PlayerDataManager>().LocalPlayerObj.GetComponent<PlayerDataNetworked>().OnOpponentBunkerSelection(bunkerNum);
        }

        public void OnCameraDefaultPosition()
        {
            _cameraFollow.OnCamDefaultPosition();
        }

        public void OnCannonFire(string roundResult)
        {
            GameCountDownText.gameObject.SetActive(false);
            _cameraFollow.OnCamDefaultPosition();
            GameInfoText.text = "Fire";
            _gameInfoTextAnimator.SetTrigger("TextGoUpperTrgr");
            StartCoroutine(DisplayResult(roundResult));
        }

        private IEnumerator DisplayResult(string roundResult)
        {
            yield return new WaitForSeconds(1.5f);
            GameInfoText.gameObject.SetActive(false);
            yield return new WaitForSeconds(3.5f);
            GameInfoText.gameObject.SetActive(true);
            GameInfoText.text = roundResult;
            _gameInfoTextAnimator.SetTrigger("TextGoUpperTrgr");
        }

        public void NewRoundStart()
        {
            GameRoundNumText.gameObject.SetActive(false);
            OnCameraDefaultPosition();
            WorldCanvasTarget.gameObject.SetActive(false);
        }

        public void GameWonText(string msg)
        {
            StartCoroutine(DisplayWonStatus(msg));
        }
        private IEnumerator DisplayWonStatus(string roundResult)
        {
            yield return new WaitForSeconds(6f);
            GameInfoText.gameObject.SetActive(true);
            GameInfoText.text = roundResult;
            _gameInfoTextAnimator.SetTrigger("TextGoNormalTrgr");
        }

        #endregion

        #region Player Lives

        public void OnPlayerLifeLost()
        {
            Transform lastActiveChild = FindLastActiveChildOf(_playerLivesTransf);
            if (lastActiveChild != null)
            {
                StartCoroutine(RemoveLife(lastActiveChild.gameObject));
            }
        }

        public void OnOpponentLifeLost()
        {
            Transform lastActiveChild = FindLastActiveChildOf(_opponentLivesTransf);
            if (lastActiveChild != null)
            {
                StartCoroutine(RemoveLife(lastActiveChild.gameObject));
            }
        }

        private IEnumerator RemoveLife(GameObject go)
        {
            yield return new WaitForSeconds(2f);
            Animator animator = go.GetComponent<Animator>();
            animator.SetTrigger("LiveLostTrgr");
            yield return new WaitForSeconds(1.5f);
            // Deactivate the last child
            go.SetActive(false);
        }

        private Transform FindLastActiveChildOf(Transform parent)
        {
            Transform lastActiveChild = null;

            // Loop through each child of the parent transform
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);

                // Check if the child is active
                if (child.gameObject.activeSelf)
                {
                    lastActiveChild = child;
                    break; // Found the last active child, exit the loop
                }
            }

            return lastActiveChild;
        }

        #endregion 

        #region Strategy Panel

        public void GenerateOpponentStrategy()
        {
            // Set Header Text & Button's Alpha
            _headerStrategyPanelText.text = "Opponent's  Strategy";
            _playerStrategyButton.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
            _playerStrategyButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 100);
            _opponentStrategyButton.GetComponent<Image>().color = new Color32(0, 0, 0, 225);
            _opponentStrategyButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);

            // Displaying Opponent's Attack Pattern
            foreach (Transform t in _strategyTableTransf)
            {
                Destroy(t.gameObject);
            }

            for (int i = 0; i < _playerDataManager.OpponentTroopList.Count; i++)
            {
                int a = i;
                int bunkerNum = _playerDataManager.OpponentTroopList[a];
                GameObject prefab = Instantiate(_gameRoundPrefab, _strategyTableTransf);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = $"Round {a + 1}";
                if (a > 6) prefab.transform.GetChild(3).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                if (a > 10) prefab.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                if (a > 10) prefab.transform.GetChild(5).GetComponent<Image>().color = new Color32(255, 255, 255, 0);

                switch (bunkerNum)
                {
                    case 1:
                        bunkerNum = 5;
                        break;
                    case 2:
                        bunkerNum = 4;
                        break;
                    case 3:
                        bunkerNum = 3;
                        break;
                    case 4:
                        bunkerNum = 2;
                        break;
                    case 5:
                        bunkerNum = 1;
                        break;
                }

                if (bunkerNum > 0)
                {
                    prefab.transform.GetChild(bunkerNum).GetComponent<Image>().color = new Color32(255, 0, 0, 255);
                }
            }
        }
        public void GeneratePlayerStrategy()
        {
            // Set Header Text & Button's Alpha
            _headerStrategyPanelText.text = "Player's  Strategy";
            _opponentStrategyButton.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
            _opponentStrategyButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 100);
            _playerStrategyButton.GetComponent<Image>().color = new Color32(0, 0, 0, 225);
            _playerStrategyButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);

            // Displaying Opponent's Attack Pattern
            foreach (Transform t in _strategyTableTransf)
            {
                Destroy(t.gameObject);
            }

            for (int i = 0; i < _playerDataManager.PlayerTroopList.Count; i++)
            {
                int a = i;
                int bunkerNum = _playerDataManager.PlayerTroopList[a];
                GameObject prefab = Instantiate(_gameRoundPrefab, _strategyTableTransf);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = $"Round {a + 1}";
                if (a > 6) prefab.transform.GetChild(3).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                if (a > 10) prefab.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                if (a > 10) prefab.transform.GetChild(5).GetComponent<Image>().color = new Color32(255, 255, 255, 0);

                if (bunkerNum > 0)
                {
                    prefab.transform.GetChild(bunkerNum).GetComponent<Image>().color = new Color32(0, 255, 0, 255);
                }
            }
        }

        #endregion

        #region Error Panel

        public void DisplayErrorMsg(string msg)
        {
            _errorPanel.SetActive(true);
            _errorMessageText.text = msg;
        }

        #endregion

        #region Transition

        public void OnTransition()
        {
            _transitionPanel.SetActive(true);
            StartCoroutine(StartTransition());
        }

        private IEnumerator StartTransition()
        {
            yield return new WaitForSeconds(1);
            _transitionPanel.SetActive(false);
        }

        #endregion
    }
}

