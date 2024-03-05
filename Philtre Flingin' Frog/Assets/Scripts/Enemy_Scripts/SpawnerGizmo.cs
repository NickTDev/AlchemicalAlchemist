using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        float size = gameObject.GetComponent<SpawnEnemy>().spawnRadius;
        // Draw a yellow sphere at the transform's position
        Color color = new Color(1f, 0.92f, 0.016f, 0.5f);
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}
