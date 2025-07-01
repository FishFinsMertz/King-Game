using UnityEditor;
using UnityEngine;

public class DestroyPrefab : MonoBehaviour
{    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
