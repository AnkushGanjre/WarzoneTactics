using System;
using UnityEngine;
using Fusion;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class FusionManager : MonoBehaviour
    {
        [Header("Network Runner")]
        [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
        public NetworkRunner Runner = null;

        [Header("Singleton")]
        private static FusionManager _instance;

        public static FusionManager Instance
        {
            get => _instance;
            private set
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException();
                }
                _instance = value;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                _instance = null;
            }
            else
            {
                throw new InvalidOperationException();
            }

        }

        public async void GameRoomHost(string sessionName)
        {
            Runner = FindObjectOfType<NetworkRunner>();
            if (Runner == null)
            {
                Runner = Instantiate(_networkRunnerPrefab);
            }

            // Let the Fusion Runner know that we will be providing user input
            Runner.ProvideInput = true;

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = sessionName,
                PlayerCount = 2,
            };

            await Runner.StartGame(startGameArgs);
        }

        public async void GameRoomClient(string sessionName)
        {
            Runner = FindObjectOfType<NetworkRunner>();
            if (Runner == null)
            {
                Runner = Instantiate(_networkRunnerPrefab);
            }

            // Let the Fusion Runner know that we will be providing user input
            Runner.ProvideInput = true;

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                PlayerCount = 2,
            };

            await Runner.StartGame(startGameArgs);
            if (Runner.SessionInfo.PlayerCount == 1)
            {
                Debug.Log("Invalid Room ID");
                await Runner.Shutdown();
            }
        }

        public async void GameRoomAutoMatch()
        {
            Runner = FindObjectOfType<NetworkRunner>();
            if (Runner == null)
            {
                Runner = Instantiate(_networkRunnerPrefab);
            }

            // Let the Fusion Runner know that we will be providing user input
            Runner.ProvideInput = true;

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                PlayerCount = 2,
            };

            await Runner.StartGame(startGameArgs);
        }
    }
}

