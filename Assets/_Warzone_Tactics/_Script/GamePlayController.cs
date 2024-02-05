using System.Collections;
using UnityEngine;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class GamePlayController : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static GamePlayController Instance;

        CannonFireScript _cannonFireScript;

        [Header("GameObjects")]
        GameObject _playerTroops;
        GameObject _opponentTroops;
        GameObject _playerFlag;
        GameObject _opponentFlag;

        [Header("Transform")]
        Transform _bunkerTransform;
        Transform _playerCannon;
        Transform _opponentCannon;

        [Header("Material")]
        [SerializeField] Material _bunkerNormal;
        [SerializeField] Material _bunkerTransparent;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _playerTroops = GameObject.Find("Player_Troop");
            _opponentTroops = GameObject.Find("Enemy_Troop");
            _playerFlag = GameObject.Find("Flag_Player");
            _opponentFlag = GameObject.Find("Flag_Opponent");

            _bunkerTransform = GameObject.Find("All_Bunkers").GetComponent<Transform>();
            _playerCannon = GameObject.Find("Player_Cannon").GetComponent<Transform>();
            _opponentCannon = GameObject.Find("Opponent_Cannon").GetComponent<Transform>();
        }

        void Start()
        {
            _cannonFireScript = FindAnyObjectByType<CannonFireScript>();

            _playerTroops.SetActive(false);
            _opponentTroops.SetActive(false);
            _playerFlag.SetActive(false);
            _opponentFlag.SetActive(false);
        }

        #region Troop Placement

        public void OnTroopPlaced(int bunkerNum)
        {
            UIController.Instance.OnCameraDefaultPosition();
            Vector3 pos = _playerTroops.transform.position;

            switch (bunkerNum)
            {
                case 1:
                    pos.x = 2f;
                    break;
                case 2:
                    pos.x = 1f;
                    break;
                case 3:
                    pos.x = 0f;
                    break;
                case 4:
                    pos.x = -1f;
                    break;
                case 5:
                    pos.x = -2f;
                    break;
            }

            _playerTroops.transform.position = pos;
            _playerTroops.SetActive(true);
            UIController.Instance.WorldCanvasPlayer.SetActive(false);
            UIController.Instance.GameInfoText.gameObject.SetActive(false);
            FindObjectOfType<PlayerDataManager>().LocalPlayerObj.GetComponent<PlayerNetworked>().OnPlayerBunkerSelection(bunkerNum);
        }

        #endregion

        #region Fire cannon

        public void OnCannonFire(int pas, int oas)
        {
            switch (pas)
            {
                case 6:
                    _cannonFireScript.PlayerAttackChoice = EnemySidebunker.Bunker_10;
                    break;
                case 7:
                    _cannonFireScript.PlayerAttackChoice = EnemySidebunker.Bunker_9;
                    break;
                case 8:
                    _cannonFireScript.PlayerAttackChoice = EnemySidebunker.Bunker_8;
                    break;
                case 9:
                    _cannonFireScript.PlayerAttackChoice = EnemySidebunker.Bunker_7;
                    break;
                case 10:
                    _cannonFireScript.PlayerAttackChoice = EnemySidebunker.Bunker_6;
                    break;
            }

            switch (oas)
            {
                case 6:
                    _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.Bunker_1;
                    break;
                case 7:
                    _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.Bunker_2;
                    break;
                case 8:
                    _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.Bunker_3;
                    break;
                case 9:
                    _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.Bunker_4;
                    break;
                case 10:
                    _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.Bunker_5;
                    break;
            }
            StartCoroutine(StartCannonFire());
        }

        private IEnumerator StartCannonFire()
        {
            yield return new WaitForSeconds(1f);
            _cannonFireScript.AimCannonAndLaunch();
        }

        public void OnOpponentTroopRevel(int pas, int ots)
        {
            Vector3 pos = _opponentTroops.transform.position;
            switch (ots)
            {
                case 1:
                    pos.x = -2f;
                    break;
                case 2:
                    pos.x = -1f;
                    break;
                case 3:
                    pos.x = 0f;
                    break;
                case 4:
                    pos.x = 1f;
                    break;
                case 5:
                    pos.x = 2f;
                    break;
            }
            _opponentTroops.transform.position = pos;
            StartCoroutine(Reveltroops());

            if (ots != (pas - 5))
            {
                MeshRenderer mr = _bunkerTransform.GetChild(ots + 4).GetComponent<MeshRenderer>();
                StartCoroutine(ChangeBunkerMat(mr));
            }
        }

        private IEnumerator Reveltroops()
        {
            yield return new WaitForSeconds(4f);
            _opponentTroops.SetActive(true);
            yield return new WaitForSeconds(3f);
            UIController.Instance.StrategyButton.onClick.Invoke();
            UIController.Instance.GameCountDownText.gameObject.SetActive(true);
        }

        private IEnumerator ChangeBunkerMat(MeshRenderer meshRenderer)
        {
            yield return new WaitForSeconds(3f);
            meshRenderer.material = _bunkerTransparent;
        }

        #endregion

        #region Flags

        public void FlagReposition(int playerTroopSelection, int opponenetTroopSelection, int roundNum)
        {
            Vector3 flagPlayerPos = _playerFlag.transform.position;
            switch (playerTroopSelection)
            {
                case 1:
                    flagPlayerPos.x = 2f;
                    if (roundNum == 11)
                        flagPlayerPos.x = 10f;
                    break;
                case 2:
                    flagPlayerPos.x = 1f;
                    break;
                case 3:
                    flagPlayerPos.x = 0f;
                    if (roundNum == 7)
                        flagPlayerPos.x = 10f;
                    break;
                case 4:
                    flagPlayerPos.x = -1f;
                    break;
                case 5:
                    flagPlayerPos.x = -2f;
                    if (roundNum == 11)
                        flagPlayerPos.x = 10f;
                    break;
                default:
                    flagPlayerPos.x = 10f;
                    break;
            }
            _playerFlag.transform.position = flagPlayerPos;


            Vector3 flagOpponentPos = _opponentFlag.transform.position;
            switch (opponenetTroopSelection)
            {
                // Selection will be in reverse order
                case 1:
                    flagOpponentPos.x = -2;
                    if (roundNum == 11)
                        flagOpponentPos.x = 10f;
                    break;
                case 2:
                    flagOpponentPos.x = -1;
                    break;
                case 3:
                    flagOpponentPos.x = 0;
                    if (roundNum == 7)
                        flagOpponentPos.x = 10f;
                    break;
                case 4:
                    flagOpponentPos.x = 1;
                    break;
                case 5:
                    flagOpponentPos.x = 2;
                    if (roundNum == 11)
                        flagOpponentPos.x = 10f;
                    break;
                default:
                    flagOpponentPos.x = 10;
                    break;
            }
            _opponentFlag.transform.position = flagOpponentPos;
        }

        public void DisplayBunkerFlags()
        {
            _playerFlag.SetActive(true);
            _opponentFlag.SetActive(true);
        }

        public void HideBunkerFlags()
        {
            _playerFlag.SetActive(false);
            _opponentFlag.SetActive(false);
        }

        #endregion

        #region New Game round

        public void StartRoundOne()
        {
            foreach (Transform t in _bunkerTransform)
            {
                t.gameObject.SetActive(true);
            }
            StartNewRound();
        }

        public void StartNewRound()
        {
            foreach (Transform t in _bunkerTransform)
            {
                MeshRenderer mr = t.GetComponent<MeshRenderer>();
                mr.material = _bunkerNormal;
            }

            _cannonFireScript.EnemyAttackChoice = PlayerSidebunker.None;
            _cannonFireScript.PlayerAttackChoice = EnemySidebunker.None;

            _playerCannon.rotation = Quaternion.Euler(-90f, 0f, 0f);
            _opponentCannon.rotation = Quaternion.Euler(-90f, 0f, -180f);

            _playerTroops.SetActive(false);
            _opponentTroops.SetActive(false);
        }

        public void HideCentreBunkers()
        {
            _bunkerTransform.GetChild(2).gameObject.SetActive(false);
            _bunkerTransform.GetChild(7).gameObject.SetActive(false);

            UIController.Instance.WorldCanvasPlayer.transform.GetChild(2).gameObject.SetActive(false);
            UIController.Instance.WorldCanvasOpponent.transform.GetChild(2).gameObject.SetActive(false);
        }

        public void HideCornerBunkers()
        {
            _bunkerTransform.GetChild(0).gameObject.SetActive(false);
            _bunkerTransform.GetChild(4).gameObject.SetActive(false);
            _bunkerTransform.GetChild(5).gameObject.SetActive(false);
            _bunkerTransform.GetChild(9).gameObject.SetActive(false);

            UIController.Instance.WorldCanvasPlayer.transform.GetChild(0).gameObject.SetActive(false);
            UIController.Instance.WorldCanvasPlayer.transform.GetChild(4).gameObject.SetActive(false);
            UIController.Instance.WorldCanvasOpponent.transform.GetChild(0).gameObject.SetActive(false);
            UIController.Instance.WorldCanvasOpponent.transform.GetChild(4).gameObject.SetActive(false);
        }

        #endregion
    }
}

