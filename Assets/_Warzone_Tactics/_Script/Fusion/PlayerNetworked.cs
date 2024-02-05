using Fusion;

namespace DonzaiGamecorp.WarzoneTactics
{
    public enum GameState
    {
        None,
        Starting,
        PlayerBunkerSelection,
        OpponentBunkerSelection,
        StrategyTimeOut,
        Won
    }
    public class PlayerNetworked : NetworkBehaviour
    {
        [Networked] public string NickName { get; set; }
        [Networked] public int AvatarNum { get; set; }
        [Networked] public int TrophyCount { get; set; }
        [Networked] public GameState GameServerState { get; set; } = GameState.None;
        [Networked] public int CountDown {  get; set; }
        [Networked] public int HostLives { get; set; } = STARTING_LIVES;
        [Networked] public int ClientLives { get; set; } = STARTING_LIVES;
        [Networked] public int GameRoundNum { get; set; } = 0;

        ChangeDetector _changeDetector;
        NetworkRunner _runnerInstance;
        TickTimer _countDown_TickTimer = TickTimer.None;
        const int STARTING_LIVES = 5;
        
        PlayerDataManager _playerDataManager;

        public override void Spawned()
        {
            _runnerInstance = Object.Runner;
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            
            if (Object.HasInputAuthority)
            {
                _playerDataManager.LocalPlayerObj = Object;
                RPC_SetPlayerDetails(_playerDataManager.NickName, _playerDataManager.PlayerAvatarNum, _playerDataManager.PlayerTrophyCount);

                UIController.Instance.PlayerNameText.text = _playerDataManager.NickName;
                UIController.Instance.PlayerAvatarImg.sprite = UIController.Instance.AllAvatars[_playerDataManager.PlayerAvatarNum - 1];
                UIController.Instance.PlayerTrophyText.text = _playerDataManager.PlayerTrophyCount.ToString();
            }
            else
            {
                _playerDataManager.RemotePlayerObj = Object;
                if (Object.HasStateAuthority) return;
                _playerDataManager.OpponentName = Object.GetComponent<PlayerNetworked>().NickName;
                _playerDataManager.OpponentAvatarNum = Object.GetComponent<PlayerNetworked>().AvatarNum;
                _playerDataManager.OpponentTrophyCount = Object.GetComponent<PlayerNetworked>().TrophyCount;

                UIController.Instance.OpponentNameText.text = _playerDataManager.OpponentName;
                UIController.Instance.OpponentAvatarImg.sprite = UIController.Instance.AllAvatars[_playerDataManager.OpponentAvatarNum - 1];
                UIController.Instance.OpponentTrophyText.text = _playerDataManager.OpponentTrophyCount.ToString();
            }

            UIController.Instance.StrategyBackButton.onClick.Invoke();
            UIController.Instance.StartRoundOne();
            GamePlayController.Instance.StartRoundOne();

            _playerDataManager.PlayerTroopSelection = 0;
            _playerDataManager.OpponentTroopSelection = 0;
            _playerDataManager.PlayerAttackSelection = 0;
            _playerDataManager.OpponentAttackSelection = 0;
        }

        public override void FixedUpdateNetwork()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(CountDown):
                        OnCountDownChange();
                        break;
                    case nameof(HostLives):
                        if (Object.HasStateAuthority)
                        {
                            UIController.Instance.OnPlayerLifeLost();
                        }
                        else
                        {
                            UIController.Instance.OnOpponentLifeLost();
                        }
                        break;
                    case nameof(ClientLives):
                        if (Object.HasStateAuthority)
                        {
                            UIController.Instance.OnOpponentLifeLost();
                        }
                        else
                        {
                            UIController.Instance.OnPlayerLifeLost();
                        }
                        break;
                }
            }

            if (_countDown_TickTimer.IsRunning)
            {
                CountDown = (int)_countDown_TickTimer.RemainingTime(_runnerInstance);
            }
            if (_countDown_TickTimer.Expired(_runnerInstance))
            {
                _countDown_TickTimer = TickTimer.None;
                OnCountDownExpired();
            }
        }

        public void OnBothPlayerSpawned()
        {
            GameServerState = GameState.Starting;
            if (_countDown_TickTimer.IsRunning)
            {
                _countDown_TickTimer = TickTimer.None;
            }

            _countDown_TickTimer = TickTimer.CreateFromSeconds(_runnerInstance, 10);
            RPC_BroadcastGameStart();
        }

        #region Game Mechanics

        private void OnCountDownChange()
        {
            if (GameServerState == GameState.Starting)
            {
                UIController.Instance.GameInfoText.text = "Game Starts In\n<size=92>" + CountDown;
            }
            else
            {
                UIController.Instance.GameCountDownText.text = CountDown.ToString();
            }
        }

        private void OnCountDownExpired()
        {
            if (_runnerInstance.IsServer)
            {
                switch (GameServerState)
                {
                    case GameState.None:
                        break;

                    case GameState.Starting:
                        RPC_PlayerBunkerSelection();
                        break;

                    case GameState.PlayerBunkerSelection:
                        RPC_OpponentBunkerSelection();
                        break;

                    case GameState.OpponentBunkerSelection:
                        RPC_RoundResults(_playerDataManager.PlayerTroopSelection, _playerDataManager.PlayerAttackSelection, _playerDataManager.OpponentTroopSelection, _playerDataManager.OpponentAttackSelection);
                        break;

                    case GameState.StrategyTimeOut:
                        RPC_NextRoundStart();
                        RPC_PlayerBunkerSelection();
                        break;

                    case GameState.Won:

                        break;
                }
            }
        }

        #endregion

        #region Bunker Selection

        public void OnPlayerBunkerSelection(int bunkerNum)
        {
            if (Object.HasInputAuthority)
            {
                if (Object.HasStateAuthority)
                {
                    RPC_TroopBunkerSelectedByHost(bunkerNum);
                }
                else
                {
                    RPC_TroopBunkerSelectedByClient(bunkerNum);
                }
            }
        }

        public void OnOpponentBunkerSelection(int bunkerNum)
        {
            if (Object.HasInputAuthority)
            {
                if (Object.HasStateAuthority)
                {
                    RPC_AttackBunkerSelectedByHost(bunkerNum);
                }
                else
                {
                    RPC_AttackBunkerSelectedByClient(bunkerNum);
                }
            }
            GamePlayController.Instance.HideBunkerFlags();
        }

        #region Checking After Selection Made
        private void CheckTroopSelection()
        {
            if (_playerDataManager.PlayerTroopSelection != 0 && _playerDataManager.OpponentTroopSelection != 0)
            {
                if (_countDown_TickTimer.IsRunning)
                {
                    _countDown_TickTimer = TickTimer.None;
                    RPC_OpponentBunkerSelection();
                }
                else
                {
                    _playerDataManager.RemotePlayerObj.GetComponent<PlayerNetworked>().RPC_OpponentBunkerSelection();
                }
                //Debug.Log("Client: " + _playerDataManager.OpponentTroopSelection + ", Host: " + _playerDataManager.PlayerTroopSelection);
            }
            else if (_playerDataManager.PlayerTroopSelection == 0)
            {
                //Debug.Log("Waiting for Host");
            }
            else if (_playerDataManager.OpponentTroopSelection == 0)
            {
                //Debug.Log("Waiting for Client");
            }
        }

        private void CheckAttackSelection()
        {
            if (_playerDataManager.PlayerAttackSelection != 0 && _playerDataManager.OpponentAttackSelection != 0)
            {
                if (_countDown_TickTimer.IsRunning)
                {
                    _countDown_TickTimer = TickTimer.None;
                    RPC_RoundResults(_playerDataManager.PlayerTroopSelection, _playerDataManager.PlayerAttackSelection, _playerDataManager.OpponentTroopSelection, _playerDataManager.OpponentAttackSelection);
                }
                else
                {
                    _playerDataManager.RemotePlayerObj.GetComponent<PlayerNetworked>().RPC_RoundResults(_playerDataManager.PlayerTroopSelection, _playerDataManager.PlayerAttackSelection, _playerDataManager.OpponentTroopSelection, _playerDataManager.OpponentAttackSelection);
                }
                //Debug.Log("Client: " + _playerDataManager.OpponentAttackSelection + ", Host: " + _playerDataManager.PlayerAttackSelection);

            }
            else if (_playerDataManager.PlayerAttackSelection == 0)
            {
                //Debug.Log("Waiting for Host");
            }
            else if (_playerDataManager.OpponentAttackSelection == 0)
            {
                //Debug.Log("Waiting for Client");
            }
        }

        #endregion

        #endregion

        #region RPC

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetPlayerDetails(string nickName, int avatarNum, int trophyCount, RpcInfo info = default)
        {
            if (string.IsNullOrEmpty(nickName)) return;
            NickName = nickName;
            UIController.Instance.OpponentNameText.text = nickName;
            _playerDataManager.OpponentName = nickName;

            if (avatarNum == 0) return;
            AvatarNum = avatarNum;
            UIController.Instance.OpponentAvatarImg.sprite = UIController.Instance.AllAvatars[avatarNum - 1];
            _playerDataManager.OpponentAvatarNum = avatarNum;

            if (trophyCount == 0) return;
            TrophyCount = trophyCount;
            UIController.Instance.OpponentTrophyText.text = trophyCount.ToString();
            _playerDataManager.OpponentTrophyCount = trophyCount;
        }

        #region GameStates RPC

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_BroadcastGameStart(RpcInfo info = default)
        {
            GameRoundNum = 1;
            HostLives = STARTING_LIVES;
            ClientLives = STARTING_LIVES;
            GameServerState = GameState.Starting;
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayerBunkerSelection(RpcInfo info = default)
        {
            if (_countDown_TickTimer.IsRunning)
            {
                _countDown_TickTimer = TickTimer.None;
            }
            if (GameRoundNum == 8)
            {
                GamePlayController.Instance.HideCentreBunkers();
            }
            if (GameRoundNum == 12)
            {
                GamePlayController.Instance.HideCornerBunkers();
            }
            if (GameRoundNum != 1) GamePlayController.Instance.DisplayBunkerFlags();

            UIController.Instance.GameRoundNumText.gameObject.SetActive(true);
            UIController.Instance.GameRoundNumText.text = $"Round {GameRoundNum}";
            if (GameRoundNum == 21) UIController.Instance.GameRoundNumText.text = "Last round";
            UIController.Instance.GameInfoText.text = "Place Your Troops";
            _countDown_TickTimer = TickTimer.CreateFromSeconds(_runnerInstance, 21);
            GameServerState = GameState.PlayerBunkerSelection;
            UIController.Instance.OnTroopSelection();
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OpponentBunkerSelection(RpcInfo info = default)
        {
            UIController.Instance.OnAttackSelection();

            if (_countDown_TickTimer.IsRunning)
            {
                _countDown_TickTimer = TickTimer.None;
            }

            UIController.Instance.GameInfoText.text = "Select Enemy Bunker";
            _countDown_TickTimer = TickTimer.CreateFromSeconds(_runnerInstance, 21);
            GameServerState = GameState.OpponentBunkerSelection;
            UIController.Instance.OnAttackSelection();
        }


        string roundResult;
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_RoundResults(int pts, int pas, int ots, int oas, RpcInfo info = default)
        {
            // pts: player troop selection, pas: player attack selection, likewise

            if (_countDown_TickTimer.IsRunning)
            {
                _countDown_TickTimer = TickTimer.None;
            }

            UIController.Instance.GameInfoText.text = "Select Enemy Bunker";
            _countDown_TickTimer = TickTimer.CreateFromSeconds(_runnerInstance, 17);
            GameServerState = GameState.StrategyTimeOut;


            roundResult = "Missed";
            if (Object.HasStateAuthority)
            {
                // If Any player doesn't place his troop then Player troop selection is automatically Opponent's Attack Target
                if (pts == 0) pts = (oas - 5);
                if (ots == 0) ots = (pas - 5);

                if (pts == (oas - 5))
                {
                    HostLives--;
                }
                if (ots == (pas - 5))
                {
                    ClientLives--;
                    roundResult = "Hit";
                }
            }
            else
            {
                if (pts == (oas - 5))
                {
                    roundResult = "Hit";
                }
            }

            UIController.Instance.GameInfoText.gameObject.SetActive(true);
            if (Object.HasStateAuthority)
            {
                GamePlayController.Instance.OnCannonFire(pas, oas);
                GamePlayController.Instance.OnOpponentTroopRevel(pas, ots);
                GamePlayController.Instance.FlagReposition(pts, ots, GameRoundNum);

                _playerDataManager.PlayerTroopList.Add(pts);
                _playerDataManager.OpponentTroopList.Add(ots);
            }
            else
            {
                GamePlayController.Instance.OnCannonFire(oas, pas);
                GamePlayController.Instance.OnOpponentTroopRevel(oas, pts);
                GamePlayController.Instance.FlagReposition(ots, pts, GameRoundNum);

                _playerDataManager.PlayerTroopList.Add(ots);
                _playerDataManager.OpponentTroopList.Add(pts);
            }
            UIController.Instance.OnCannonFire(roundResult);

            if (HostLives <= 0 || ClientLives <= 0)
            {
                if (_countDown_TickTimer.IsRunning)
                {
                    _countDown_TickTimer = TickTimer.None;
                }
                GameServerState = GameState.Won;

                if (Object.HasStateAuthority)
                {
                    if (HostLives <= 0)
                        UIController.Instance.GameWonText("You Lost");
                    if (ClientLives <= 0)
                        UIController.Instance.GameWonText("You Won");
                }
                else
                {
                    if (ClientLives <= 0)
                        UIController.Instance.GameWonText("You Lost");
                    if (HostLives <= 0)
                        UIController.Instance.GameWonText("You Won");
                }
            }

            if (GameRoundNum == 21)
            {
                if (_countDown_TickTimer.IsRunning)
                {
                    _countDown_TickTimer = TickTimer.None;
                }
                GameServerState = GameState.Won;
                
                if (HostLives == ClientLives)
                {
                    UIController.Instance.GameWonText("Draw");
                }
                else
                {
                    if (Object.HasStateAuthority)
                    {
                        if (HostLives > ClientLives)
                        {
                            UIController.Instance.GameWonText("You Won");
                        }
                        else 
                        {
                            UIController.Instance.GameWonText("You Lost");
                        }
                    }
                    else
                    {
                        if (HostLives < ClientLives)
                        {
                            UIController.Instance.GameWonText("You Won");
                        }
                        else
                        {
                            UIController.Instance.GameWonText("You Lost");
                        }
                    }
                }
            }
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_NextRoundStart(RpcInfo info = default)
        {
            if (Object.HasStateAuthority) GameRoundNum++;
            UIController.Instance.StrategyBackButton.onClick.Invoke();
            UIController.Instance.NewRoundStart();
            GamePlayController.Instance.StartNewRound();

            _playerDataManager.PlayerTroopSelection = 0;
            _playerDataManager.OpponentTroopSelection = 0;
            _playerDataManager.PlayerAttackSelection = 0;
            _playerDataManager.OpponentAttackSelection = 0;
        }

        #endregion

        #region Selection Choice RPC

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_TroopBunkerSelectedByHost(int bunkerNum, RpcInfo info = default)
        {
            if (_runnerInstance.IsServer)
            {
                _playerDataManager.PlayerTroopSelection = bunkerNum;
                CheckTroopSelection();
                //Debug.Log("RPC by HOST");
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_TroopBunkerSelectedByClient(int bunkerNum, RpcInfo info = default)
        {
            if (_runnerInstance.IsServer)
            {
                _playerDataManager.OpponentTroopSelection = bunkerNum;
                CheckTroopSelection();
                //Debug.Log("RPC by CLIENT");
            }
        }


        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_AttackBunkerSelectedByHost(int bunkerNum, RpcInfo info = default)
        {
            if (_runnerInstance.IsServer)
            {
                _playerDataManager.PlayerAttackSelection = bunkerNum;
                CheckAttackSelection();
                //Debug.Log("RPC by HOST");
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_AttackBunkerSelectedByClient(int bunkerNum, RpcInfo info = default)
        {
            if (_runnerInstance.IsServer)
            {
                _playerDataManager.OpponentAttackSelection = bunkerNum;
                CheckAttackSelection();
                //Debug.Log("RPC by CLIENT");
            }
        }

        #endregion

        #region Rematch RPC


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_RematchByHost(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "<size=30>Rematch Requested";
            if (!_runnerInstance.IsServer)
            {
                UIController.Instance.GameInfoText.text = $"<size=30>{_playerDataManager.OpponentName} Wants Rematch";
            }
            if (_runnerInstance.IsServer && _playerDataManager.didClientReqRematch == true)
            {
                RPC_StartRematch();
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_RematchByClient(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "<size=30>Rematch Requested";
            if (_runnerInstance.IsServer)
            {
                UIController.Instance.GameInfoText.text = $"<size=30>{_playerDataManager.OpponentName} Wants Rematch";
                _playerDataManager.didClientReqRematch = true;
            }
            if (_runnerInstance.IsServer && _playerDataManager.didHostReqRematch == true)
            {
                RPC_StartRematch();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_StartRematch(RpcInfo info = default)
        {
            UIController.Instance.GameRematchButton.gameObject.SetActive(false);
            _playerDataManager.didHostReqRematch = false;
            _playerDataManager.didClientReqRematch = false;

            UIController.Instance.StrategyBackButton.onClick.Invoke();
            UIController.Instance.OnBothPlayersReady();
            UIController.Instance.StartRoundOne();
            GamePlayController.Instance.StartRoundOne();

            _playerDataManager.PlayerTroopSelection = 0;
            _playerDataManager.OpponentTroopSelection = 0;
            _playerDataManager.PlayerAttackSelection = 0;
            _playerDataManager.OpponentAttackSelection = 0;

            if (Object.HasStateAuthority)
            {
                OnBothPlayerSpawned();
            }
        }

        #endregion

        #endregion


    }
}

