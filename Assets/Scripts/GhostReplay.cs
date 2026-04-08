using UnityEngine;
using System.Collections.Generic;

public class GhostReplay : MonoBehaviour
{
    // ── Inspector Variables ─────────────────────────────────────
    [Header("Replay Settings")]
    public float ghostAlpha = 0.4f;

    [Header("Offset - Tweak if ghost floats or clips into floor")]
    public Vector3 replayOffset = new Vector3(0, 0, 0);

    [Header("References")]
    public Renderer ghostRenderer;

    // ── State ────────────────────────────────────────────────────
    [HideInInspector] public bool isReplaying = false;
    [HideInInspector] public bool isFinished = false;

    // ── Private Data ─────────────────────────────────────────────
    private List<FrameData> frames = new List<FrameData>();
    private int currentFrame = 0;
    private Material ghostMaterial;

    void Start()
    {
        // Disable collider so ghost never pushes player
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log("GhostReplay: Collider set to trigger.");
        }

        // Setup transparent material
        if (ghostRenderer != null)
        {
            ghostMaterial = ghostRenderer.material;
            SetGhostTransparency(ghostAlpha);
        }
        else
        {
            Debug.LogWarning("GhostReplay: No Renderer assigned!");
        }

        // ✅ KEY FIX: Hide the ghost at start so it doesn't
        // appear at the wrong position before replay begins
        

        Debug.Log("GhostReplay: Ghost ready and waiting for frames.");
    }

    void FixedUpdate()
    {
        if (!isReplaying || frames.Count == 0) return;

        if (currentFrame < frames.Count)
        {
            ApplyFrame(frames[currentFrame]);
            currentFrame++;
        }
        else
        {
            isReplaying = false;
            isFinished = true;
            Debug.Log($"GhostReplay: Replay finished at frame {currentFrame}.");
        }
    }

    void ApplyFrame(FrameData frame)
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Vector3 ghostPosition = frame.position;
        if (col != null)
        {
            // Add back the capsule offset so ghost stands correctly
            ghostPosition = frame.position + new Vector3(0, col.height / 2f - col.radius, 0);
        }

        transform.position = ghostPosition;
        transform.rotation = frame.rotation;
    }

    void SetGhostTransparency(float alpha)
    {
        if (ghostMaterial == null) return;

        ghostMaterial.SetFloat("_Mode", 3);
        ghostMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ghostMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ghostMaterial.SetInt("_ZWrite", 0);
        ghostMaterial.DisableKeyword("_ALPHATEST_ON");
        ghostMaterial.EnableKeyword("_ALPHABLEND_ON");
        ghostMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        ghostMaterial.renderQueue = 3000;

        Color sepiaColor = new Color(0.7f, 0.5f, 0.2f, alpha);
        ghostMaterial.color = sepiaColor;
    }

    public void SetFrames(List<FrameData> recordedFrames)
    {
        frames = recordedFrames;
        Debug.Log($"GhostReplay: Received {frames.Count} frames.");
    }

    public void StartReplay()
    {
        if (frames.Count == 0)
        {
            Debug.LogWarning("GhostReplay: No frames loaded!");
            return;
        }

        currentFrame = 0;
        isReplaying = true;
        isFinished = false;

        // ✅ KEY FIX: Show the ghost only when replay actually starts
        

        // Snap to frame 0 immediately (with offset)
        transform.position = frames[0].position + replayOffset;
        transform.rotation = frames[0].rotation;

        Debug.Log($"GhostReplay: Replay started. Frame[0] Y = {frames[0].position.y}");
    }

    public void ResetReplay()
    {
        currentFrame = 0;
        isReplaying = false;
        isFinished = false;
        
    }
}