using UnityEngine;

namespace DonzaiGamecorp.WarzoneTactics
{
    public class CameraFollow : MonoBehaviour
    {
        Vector3 targetPosiion;
        float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;

        [Header("Camera Positions")]
        [SerializeField] Vector3 _defaultPosition = new Vector3(0f, 5f, 10.5f);
        [SerializeField] Vector3 _troopPlacementPosition = new Vector3(0f, 5f, 8f);
        [SerializeField] Vector3 _attackSelectionPosition = new Vector3(0f, 5f, 3f);

        private void Start()
        {
            targetPosiion = _defaultPosition;
        }
        void Update()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosiion, ref velocity, smoothTime);
        }

        public void OnCamDefaultPosition()
        {
            targetPosiion = _defaultPosition;
        }

        public void OnCamTroopPlacement()
        {
            targetPosiion = _troopPlacementPosition;
        }

        public void OnCamAttackSelection()
        {
            targetPosiion = _attackSelectionPosition;
        }
    }
}

