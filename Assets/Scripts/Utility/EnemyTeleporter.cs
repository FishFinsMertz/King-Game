using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTeleporter : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float fadeDuration = 0.1f;

    [SerializeField] private float teleportBufferDistance;

    private Transform ownerTransform;
    private GameObject player;
    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        ownerTransform = transform.parent; // Assumes this is a child of the entity
        player = GameObject.FindGameObjectWithTag("Player");

        if (sprite == null)
        {
            Debug.LogWarning("SpriteRenderer not found on King or its children.");
        }
    }

    public IEnumerator TryTeleport(float distance, float probability = 1f)
    {
        float distanceFromPlayer = Vector2.Distance (this.transform.position, player.transform.position);
        if (Random.value <= Mathf.Clamp01(probability) && distanceFromPlayer >= teleportBufferDistance)
        {
            yield return TeleportWithFade(distance);
        }
    }

    private IEnumerator TeleportWithFade(float distance)
    {
        if (player == null || ownerTransform == null || sprite == null) yield break;

        // Fade out
        yield return FadeTo(0f, fadeDuration);

        // Calculate teleport position
        Vector2 playerPos = player.transform.position;
        Vector2 dirToKing = (Vector2)ownerTransform.position - playerPos;
        dirToKing.Normalize();
        Vector2 teleportDir = -dirToKing;
        Vector2 targetPos = playerPos + teleportDir * distance;

        // Adjust Y position
        float heightOffset = 0f;
        if (ownerTransform.TryGetComponent(out Collider2D col))
            heightOffset = col.bounds.extents.y;
        targetPos.y += heightOffset;

        // Check for obstacle
        bool blocked = Physics2D.OverlapCircle(targetPos, checkRadius, obstacleMask);
        if (!blocked)
        {
            ownerTransform.position = targetPos;
        }

        // Fade back in
        yield return FadeTo(1f, fadeDuration);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = sprite.color.a;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        // Final set just in case
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);
    }
}
