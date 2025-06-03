using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlashFX : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    // The SpriteRenderer that should flash.
    private SpriteRenderer spriteRenderer;

    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine); // Prevent multiple flashes at the same time
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        yield return new WaitForSeconds(delay); //A slight delay

        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);

        spriteRenderer.material = originalMaterial;

        flashRoutine = null;
    }
}
