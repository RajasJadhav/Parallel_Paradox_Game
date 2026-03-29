using UnityEngine;
using System.Collections.Generic;

// Attach this to the Ghost Prefab
// When given a list of FrameData, it replays them exactly

public class GhostReplay : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Replay Settings")]
    public float ghostAlpha = 0.4f;       // How transparent the ghost is (0 = invisible, 1 = solid)

    [Header("References")]
    public Renderer ghostRenderer;        // Drag the ghost's mesh renderer here in Inspector

    // ── State ────────────────────────────────────────────────────
    [HideInInspector] public bool isReplaying = false;
    [HideInInspector] public bool isFinished = false;

    // ── Private Data ─────────────────────────────────────────────
    private List<FrameData> frames = new List<FrameData>();
    private int currentFrame = 0;
    private Material ghostMaterial;

    void Start()
    {
        // Disable the collider on spawn so the ghost
        // never physically pushes the player
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.isTrigger = true; // Make it a trigger instead of solid
            Debug.Log("GhostReplay: Collider set to trigger — no physics push.");
        }

        // Get the ghost's material and make it semi-transparent
        if (ghostRenderer != null)
        {
            // Create an instance of the material so we don't affect other objects
            ghostMaterial = ghostRenderer.material;
            SetGhostTransparency(ghostAlpha);
        }
        else
        {
            Debug.LogWarning("GhostReplay: No Renderer assigned. Ghost won't look transparent.");
        }

        Debug.Log("GhostReplay: Ghost ready and waiting for frames.");
    }

    void FixedUpdate()
    {
        // Replay runs in FixedUpdate to match the recording rate exactly
        if (!isReplaying || frames.Count == 0) return;

        if (currentFrame < frames.Count)
        {
            // Apply this frame's position and rotation to the ghost
            ApplyFrame(frames[currentFrame]);
            currentFrame++;
        }
        else
        {
            // No more frames — ghost stays in its final position
            isReplaying = false;
            isFinished = true;

            Debug.Log($"GhostReplay: Replay finished. Ghost stopped at frame {currentFrame}.");
        }
    }

    // ── Private Methods ──────────────────────────────────────────

    void ApplyFrame(FrameData frame)
    {
        // Move and rotate the ghost to match recorded player data
        transform.position = frame.position;
        transform.rotation = frame.rotation;
    }

    void SetGhostTransparency(float alpha)
    {
        if (ghostMaterial == null) return;

        // Set the material to transparent rendering mode
        ghostMaterial.SetFloat("_Mode", 3);  // 3 = Transparent in Standard shader
        ghostMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ghostMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ghostMaterial.SetInt("_ZWrite", 0);
        ghostMaterial.DisableKeyword("_ALPHATEST_ON");
        ghostMaterial.EnableKeyword("_ALPHABLEND_ON");
        ghostMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        ghostMaterial.renderQueue = 3000;

        // Apply the sepia tint and alpha
        Color sepiaColor = new Color(0.7f, 0.5f, 0.2f, alpha); // Warm brown + transparent
        ghostMaterial.color = sepiaColor;
    }

    // ── Public Methods ───────────────────────────────────────────

    // Called by LevelManager when spawning the ghost
    public void SetFrames(List<FrameData> recordedFrames)
    {
        frames = recordedFrames;

        Debug.Log($"GhostReplay: Received {frames.Count} frames. Ready to replay.");
    }

    // Called by LevelManager at the start of every new loop
    public void StartReplay()
    {
        if (frames.Count == 0)
        {
            Debug.LogWarning("GhostReplay: Tried to replay but no frames loaded!");
            return;
        }

        currentFrame = 0;
        isReplaying = true;
        isFinished = false;

        // Move ghost immediately to frame 0 position (no pop-in delay)
        transform.position = frames[0].position;
        transform.rotation = frames[0].rotation;

        Debug.Log("GhostReplay: Replay started.");
    }

    // Called by LevelManager on loop reset
    public void ResetReplay()
    {
        currentFrame = 0;
        isReplaying = false;
        isFinished = false;
    }
}