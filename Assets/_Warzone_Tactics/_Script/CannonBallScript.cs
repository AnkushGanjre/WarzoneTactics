using System.Collections;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    [SerializeField] ParticleSystem _explosionPrefab; // Drag your explosion prefab here in the Unity Editor
    [SerializeField] Material _burnedBunkerMat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BunkerTag"))
        {
            // Do something when a GameObject with the specified tag enters the trigger
            float xPos = other.transform.position.x;
            float zPos = other.transform.position.z;
            OnExplosion(xPos, zPos);
            MeshRenderer meshRenderer = other.GetComponent<MeshRenderer>();
            StartCoroutine(MaterialChange(meshRenderer));
        }
    }

    private void OnExplosion(float xPos, float zPos)
    {
        // Instantiate the explosion at the Bunker's coordinate
        if (zPos > 0)
        {
            zPos = 3f;
        }
        else
        {
            zPos = -2.8f;
        }

        Vector3 bunkerPos = new Vector3 (xPos, 0.4f, zPos);
        ParticleSystem explosionInstance = Instantiate(_explosionPrefab, bunkerPos, Quaternion.identity);

        // Get the duration of the Particle System
        float explosionDuration = explosionInstance.main.duration;

        // Destroy the instantiated explosion after the duration
        Destroy(explosionInstance.gameObject, explosionDuration);
    }

    private IEnumerator MaterialChange(MeshRenderer mr)
    {
        yield return new WaitForSeconds(0.5f);
        mr.material = _burnedBunkerMat;
        Destroy(gameObject);
    }
}
