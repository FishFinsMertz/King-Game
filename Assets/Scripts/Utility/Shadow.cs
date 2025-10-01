using UnityEngine;

public class BlobShadow2D : MonoBehaviour
{
    [Header("References")]
    public Transform character;          // The player/character to follow
    public SpriteRenderer shadowSprite;  // SpriteRenderer of the shadow

    [Header("Settings")]
    public float maxDistance = 5f;       // Max distance to fade out
    public float minScale = 0.3f;        // Scale when very high
    public float maxScale = 1f;          // Scale when on the ground
    public float maxOpacity = 0.8f;      // Maximum alpha (0–1)
    public LayerMask groundMask;         // Ground layers

    private Vector3 _originalScale;

    void Start()
    {
        if (shadowSprite == null)
            shadowSprite = GetComponent<SpriteRenderer>();

        _originalScale = shadowSprite.transform.localScale;
    }

    void Update()
    {
        // Raycast straight down from the character
        RaycastHit2D hit = Physics2D.Raycast(character.position, Vector2.down, maxDistance, groundMask);

        if (hit.collider != null)
        {
            // Move shadow to ground point
            shadowSprite.transform.position = hit.point + Vector2.up * 0.05f; // small offset

            // Distance from character to ground
            float distance = Vector2.Distance(character.position, hit.point);

            // Fade alpha based on height, clamped by maxOpacity
            float alphaFactor = Mathf.Clamp01(1 - (distance / maxDistance));
            float alpha = alphaFactor * maxOpacity;

            Color c = shadowSprite.color;
            c.a = alpha;
            shadowSprite.color = c;

            // Scale shadow based on height
            float scaleFactor = Mathf.Lerp(minScale, maxScale, alphaFactor);
            shadowSprite.transform.localScale = _originalScale * scaleFactor;

            shadowSprite.enabled = true;
        }
        else
        {
            // No ground below → hide shadow
            shadowSprite.enabled = false;
        }
    }
}
