using System.Collections;
using UnityEngine;

namespace DonzaiGamecorp.WarzoneTactics
{
    public enum PlayerSidebunker
    {
        None,
        Bunker_1,
        Bunker_2,
        Bunker_3,
        Bunker_4,
        Bunker_5
    }
    public enum EnemySidebunker
    {
        None,
        Bunker_6,
        Bunker_7,
        Bunker_8,
        Bunker_9,
        Bunker_10
    }

    public class CannonFireScript : MonoBehaviour
    {
        [SerializeField] GameObject _ballPrefab;         // The prefab of the ball
        [SerializeField] Transform _playerCannonTransform;     // The transform of the player cannon
        [SerializeField] Transform _enemyCannonTransform;     // The transform of the enemy cannon
        [SerializeField] Transform _playerCannonBallOriginTransform;     // The transform representing the instantiation position of player cannon ball
        [SerializeField] Transform _enemyCannonBallOriginTransform;     // The transform representing the instantiation position of enemy cannon ball

        float _launchForce = 8.5f;       // The force with which to launch the ball
        float _launchDelay = 1f;

        private Vector3 _playerTargetPosition;        // The target position to launch the ball towards
        private Vector3 _enemyTargetPosition;        // The target position to launch the ball towards
        private Vector3 _playerCannonInitialRot;
        private Vector3 _enemyCannonInitialRot;

        public PlayerSidebunker EnemyAttackChoice;
        public EnemySidebunker PlayerAttackChoice;


        private void Start()
        {
            _playerCannonInitialRot = _playerCannonTransform.rotation.eulerAngles;
            _enemyCannonInitialRot = _enemyCannonTransform.rotation.eulerAngles;
        }

        public void AimCannonAndLaunch()
        {
            // Use Target Position to rotate the cannon
            GetTargetAndCannonPos();

            // Use Invoke to launch the ball after a delay
            Invoke(nameof(OnBallLaunch), _launchDelay);
        }

        private void OnBallLaunch()
        {
            // Instantiate the ball prefab at the specified origin position
            GameObject playerCannonBall = Instantiate(_ballPrefab, _playerCannonBallOriginTransform.position, Quaternion.identity);
            GameObject enemyCannonBall = Instantiate(_ballPrefab, _enemyCannonBallOriginTransform.position, Quaternion.identity);

            // Calculate the direction from the origin position to the target position
            Vector3 playerLaunchDirection = (_playerTargetPosition - _playerCannonBallOriginTransform.position).normalized;
            Vector3 enemyLaunchDirection = (_enemyTargetPosition - _enemyCannonBallOriginTransform.position).normalized;

            // Calculate the launch velocity with a 45-degree angle
            Vector3 playerLaunchVelocity = Quaternion.AngleAxis(45, Vector3.right) * playerLaunchDirection * _launchForce;
            Vector3 enemyLaunchVelocity = Quaternion.AngleAxis(45, Vector3.left) * enemyLaunchDirection * _launchForce;

            // Launch the ball with the calculated velocity if target is selected.
            if (PlayerAttackChoice == EnemySidebunker.None)
            {
                // Player Didn't selected a target
                Destroy(playerCannonBall);
            }
            else
            {
                playerCannonBall.GetComponent<Rigidbody>().velocity = playerLaunchVelocity;
            }

            if (EnemyAttackChoice == PlayerSidebunker.None)
            {
                // Enemy Didn't selected a target
                Destroy(enemyCannonBall);
            }
            else
            {
                enemyCannonBall.GetComponent<Rigidbody>().velocity = enemyLaunchVelocity;
            }
        }

        private void GetTargetAndCannonPos()
        {
            Quaternion playerCannonRotation;
            Quaternion enemyCannonRotation;

            switch (EnemyAttackChoice)
            {
                case PlayerSidebunker.Bunker_1:
                    _enemyTargetPosition = new Vector3(1.5f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y + 15f, _enemyCannonInitialRot.z);
                    break;

                case PlayerSidebunker.Bunker_2:
                    _enemyTargetPosition = new Vector3(0.75f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y + 7f, _enemyCannonInitialRot.z);
                    break;

                case PlayerSidebunker.Bunker_3:
                    _enemyTargetPosition = new Vector3(0f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y, _enemyCannonInitialRot.z);
                    break;

                case PlayerSidebunker.Bunker_4:
                    _enemyTargetPosition = new Vector3(-0.75f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y - 7f, _enemyCannonInitialRot.z);
                    break;

                case PlayerSidebunker.Bunker_5:
                    _enemyTargetPosition = new Vector3(-1.5f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y - 15f, _enemyCannonInitialRot.z);
                    break;

                default:
                    _enemyTargetPosition = new Vector3(0f, 0.5f, 3f);
                    enemyCannonRotation = Quaternion.Euler(_enemyCannonInitialRot.x, _enemyCannonInitialRot.y, _enemyCannonInitialRot.z);
                    break;
            }

            switch (PlayerAttackChoice)
            {
                case EnemySidebunker.Bunker_6:
                    _playerTargetPosition = new Vector3(1.5f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, _playerCannonInitialRot.y - 15f, _playerCannonInitialRot.z);
                    break;

                case EnemySidebunker.Bunker_7:
                    _playerTargetPosition = new Vector3(0.75f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, _playerCannonInitialRot.y - 7f, _playerCannonInitialRot.z);
                    break;

                case EnemySidebunker.Bunker_8:
                    _playerTargetPosition = new Vector3(0f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, _playerCannonInitialRot.y, _playerCannonInitialRot.z);
                    break;

                case EnemySidebunker.Bunker_9:
                    _playerTargetPosition = new Vector3(-0.75f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, _playerCannonInitialRot.y + 7f, _playerCannonInitialRot.z);
                    break;

                case EnemySidebunker.Bunker_10:
                    _playerTargetPosition = new Vector3(-1.5f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, _playerCannonInitialRot.y + 15f, _playerCannonInitialRot.z);
                    break;

                default:
                    _playerTargetPosition = new Vector3(0f, 0.5f, -3f);
                    playerCannonRotation = Quaternion.Euler(_playerCannonInitialRot.x, 0f, _playerCannonInitialRot.z);
                    break;
            }

            // Smoothly interpolate between the current rotation and the target rotation over 1 second
            StartCoroutine(RotateOverTime(_playerCannonTransform, playerCannonRotation, _launchDelay / 2));
            StartCoroutine(RotateOverTime(_enemyCannonTransform, enemyCannonRotation, _launchDelay / 2));
        }

        private IEnumerator RotateOverTime(Transform transformToRotate, Quaternion targetRotation, float duration)
        {
            float elapsed = 0f;
            Quaternion startRotation = transformToRotate.rotation;

            while (elapsed < duration)
            {
                transformToRotate.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure the final rotation is exactly the target rotation
            transformToRotate.rotation = targetRotation;
        }

        public void OnBunkerSelection(int num)
        {
            switch (num)
            {
                case 1:
                    PlayerAttackChoice = EnemySidebunker.Bunker_6;
                    break;
                case 2:
                    PlayerAttackChoice = EnemySidebunker.Bunker_7;
                    break;
                case 3:
                    PlayerAttackChoice = EnemySidebunker.Bunker_8;
                    break;
                case 4:
                    PlayerAttackChoice = EnemySidebunker.Bunker_9;
                    break;
                case 5:
                    PlayerAttackChoice = EnemySidebunker.Bunker_10;
                    break;

                case 6:
                    EnemyAttackChoice = PlayerSidebunker.Bunker_1;
                    break;
                case 7:
                    EnemyAttackChoice = PlayerSidebunker.Bunker_2;
                    break;
                case 8:
                    EnemyAttackChoice = PlayerSidebunker.Bunker_3;
                    break;
                case 9:
                    EnemyAttackChoice = PlayerSidebunker.Bunker_4;
                    break;
                case 10:
                    EnemyAttackChoice = PlayerSidebunker.Bunker_5;
                    break;
            }

            if (UIController.Instance.WorldCanvasPlayer.activeInHierarchy)
            {
                UIController.Instance.WorldCanvasPlayer.SetActive(false);
            }
            if (UIController.Instance.WorldCanvasOpponent.activeInHierarchy)
            {
                UIController.Instance.WorldCanvasOpponent.SetActive(false);
            }
        }

        public void OnFire()
        {
            AimCannonAndLaunch();
        }
    }
}

