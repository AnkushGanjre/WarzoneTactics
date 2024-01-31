using System.Collections.Generic;
using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class OnServerConnection : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] NetworkPrefabRef _playerPrefab;
        Dictionary<PlayerRef, NetworkObject> SpawnedPlayer = new Dictionary<PlayerRef, NetworkObject>();
        PlayerDataManager _playerDataManager;

        private void Start()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            //Debug.Log("Connected to server");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            //Debug.Log("Runner Shut Down due to: " + shutdownReason);
            UIController.Instance.GamePlayQuitButton.onClick.Invoke();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            //Debug.Log($"Player {player} Joined");

            if (runner.IsServer)
            {
                NetworkObject playerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject);
                SpawnedPlayer.Add(player, playerObject);
            }

            if (player == runner.LocalPlayer)
            {
                _playerDataManager.LocalPlayerRef = player;
                //Debug.Log("LocalPlayer: " + player);
            }
            else
            {
                _playerDataManager.RemotePlayerRef = player;
                //Debug.Log("RemotePlayer: " + player);
            }

            if (runner.SessionInfo.PlayerCount > 1)
            {
                runner.SessionInfo.IsOpen = false;
                runner.SessionInfo.IsVisible = false;
                //Debug.Log("Room Closed");

                UIController.Instance.OnBothPlayersReady();
                if (runner.IsServer)
                {
                    FindObjectOfType<PlayerDataNetworked>().OnBothPlayerSpawned();
                }
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            //Debug.Log($"Player {player} Left");

            if (player == _playerDataManager.RemotePlayerRef)
            {
                //Debug.Log("Opponent Left");
                UIController.Instance.DisplayErrorMsg("Opponent Left");
            }

            if (SpawnedPlayer.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                SpawnedPlayer.Remove(player);
            }
            runner.Shutdown();
        }

        #region Unused Callbacks

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        #endregion
    }
}

