using UnityEngine;

// A frozen ghost clone — does not move, does not replay
// Spawned at a stamped position when a loop ends
// Activates pressure plates, weight zones, triggers
// Persists for the rest of the level — never resets between loops
// Looks identical to the main ghost (same material, same flicker)

public class FrozenClone : MonoBehaviour
{
    [Header("Visual")]
    public float ghostAlpha = 0.35f;   // Slightly more transparent than main ghost
                                        // So player can tell them apart

    // ── Private ───────────────────────────────────────────────────
    private Renderer  cloneRenderer;
    private Material  cloneMaterial;

    void Start()
    {
        // ── Tag ───────────────────────────────────────────────────
        // Must be tagged Ghost so pressure plates and weight zones
        // detect this clone exactly like a moving ghost
        gameObject.tag = "Ghost";

        // ── Collider ──────────────────────────────────────────────
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log("FrozenClone: Collider set to trigger.");
        }

        // ── Rigidbody ─────────────────────────────────────────────
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity  = false;
        }

        // ── Material ──────────────────────────────────────────────
        cloneRenderer = GetComponent<Renderer>();
        if (cloneRenderer != null)
        {
            cloneMaterial = cloneRenderer.material;
            ApplyGhostMaterial();
        }

        Debug.Log($"FrozenClone: Placed at {transform.position}. Will hold this position forever.");
    }

    void ApplyGhostMaterial()
    {
        if (cloneMaterial == null) return;

        // Same sepia ghost look as GhostReplay
        // Slightly more transparent so it reads as "placed" not "walking"
        cloneMaterial.SetFloat("_Mode", 3);
        cloneMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        cloneMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        cloneMaterial.SetInt("_ZWrite", 0);
        cloneMaterial.DisableKeyword("_ALPHATEST_ON");
        cloneMaterial.EnableKeyword("_ALPHABLEND_ON");
        cloneMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        cloneMaterial.renderQueue = 3000;

        // Slightly cooler sepia than the moving ghost
        // Moving ghost: (0.7, 0.5, 0.2, 0.4) warm amber
        // Frozen clone: (0.5, 0.6, 0.7, 0.35) cool blue-grey
        // Visual difference tells player: this one is placed, not walking
        Color frozenColor = new Color(0.5f, 0.6f, 0.7f, ghostAlpha);
        cloneMaterial.color = frozenColor;
    }

    // FrozenClone has no Update — it does nothing
    // Its entire job is to exist at a position and have a Ghost tag
    // Pressure plates and weight zones do the rest
}