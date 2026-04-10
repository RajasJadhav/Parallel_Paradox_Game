using UnityEngine;
using System.Collections.Generic;

public class GhostReplay : MonoBehaviour
{
    [Header("Replay Settings")]
    public float ghostAlpha = 0.4f;

    [Header("References")]
    public Renderer ghostRenderer;

    [HideInInspector] public bool isReplaying = false;
    [HideInInspector] public bool isFinished = false;

    private List<FrameData> frames = new List<FrameData>();
    private int currentFrame = 0;
    private Material ghostMaterial;
    private Animator animator;                              // ← ADD
    private Rigidbody rb;                                   // ← ADD

    // Same hashes as PlayerController
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");

    void Awake()   // ← was Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("GhostReplay: No Animator found!");

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null) col.isTrigger = true;

        if (ghostRenderer != null)
        {
            ghostMaterial = ghostRenderer.material;
            SetGhostTransparency(ghostAlpha);
        }
        else
        {
            Debug.LogWarning("GhostReplay: No Renderer assigned!");
        }

        gameObject.SetActive(false); // Now runs immediately during Instantiate, BEFORE SetFrames
        Debug.Log("GhostReplay: Ready.");
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

            // ← Force animator to idle state when replay ends
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, 0f);
                animator.SetBool(IsGroundedHash, true);  // ← grounded = true stops jump anim
            }

            Debug.Log($"GhostReplay: Replay finished at frame {currentFrame}.");
        }
    }

    void ApplyFrame(FrameData frame)
    {
        transform.position = frame.position;
        transform.rotation = frame.rotation;

        if (animator != null)
        {
            animator.SetFloat(SpeedHash, frame.speed);

            // Only set bool if parameter exists
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                Debug.Log($"Ghost Animator param: '{p.name}' hash={p.nameHash}");
            }
        }
    }

    // ── rest of your existing methods unchanged ───────────────

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

        // Reset animator to neutral before replay begins
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, 0f);
            animator.SetBool(IsGroundedHash, true);  // ← ADD
        }

        gameObject.SetActive(true);
        transform.position = frames[0].position;
        transform.rotation = frames[0].rotation;

        Debug.Log($"GhostReplay: Replay started. Frame[0] pos = {frames[0].position}");
    }

    public void ResetReplay()
    {
        currentFrame = 0;
        isReplaying = false;
        isFinished = false;
        gameObject.SetActive(false);                        // ← Hide ghost on reset
        Debug.Log("GhostReplay: Reset");
    }
}