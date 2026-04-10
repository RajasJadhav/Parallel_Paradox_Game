using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GhostReplay : MonoBehaviour
{
    [Header("Replay Settings")]
    public float ghostAlpha = 0.4f;

    [Header("References")]
    public Renderer  ghostRenderer;

    // ── NEW — FrozenClone spawning ────────────────────────────────
    [Header("Stamp Clone Settings")]
    public GameObject frozenClonePrefab;  // Drag Ghost Prefab here
                                           // FrozenClone.cs will be on it
    // ─────────────────────────────────────────────────────────────

[HideInInspector] public bool isReplaying = false;
    [HideInInspector] public bool isFinished  = false;
    [HideInInspector] public bool isFalling = false;

[Header("Fall Settings")]
    public float fallGravityMultiplier = 1f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    private List<FrameData> frames       = new List<FrameData>();
    private int             currentFrame = 0;
    private Material        ghostMaterial;
    private Rigidbody rb;

    private bool isGrounded;
    private int groundContactCount = 0;

    // ── NEW — tracks all frozen clones spawned from this ghost ────
    // Static list so LevelManager can clear ALL clones on level reset
    public static List<GameObject> AllFrozenClones = new List<GameObject>();
    // ─────────────────────────────────────────────────────────────

    void Start()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
            col.isTrigger = false; // Physics collisions during fall

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        if (ghostRenderer != null)
        {
            ghostMaterial = ghostRenderer.material;
            SetGhostTransparency(ghostAlpha);
        }

        Debug.Log("GhostReplay: Ready and waiting for frames.");
    }

    void FixedUpdate()
    {
        if (isReplaying)
        {
            if (frames.Count == 0) return;

            if (currentFrame < frames.Count)
            {
                ApplyFrame(frames[currentFrame]);
                currentFrame++;
            }
            else
            {
                isReplaying = false;
                isFinished  = true;
                StartCoroutine(StartFalling());
                Debug.Log($"GhostReplay: Replay finished at frame {currentFrame}.");
            }
        }
        else if (isFalling && rb != null)
        {
            // Apply extra gravity during fall
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    void ApplyFrame(FrameData frame)
    {
        transform.position = frame.position;
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
        isReplaying  = true;
        isFinished   = false;

        transform.position = frames[0].position;
        transform.rotation = frames[0].rotation;

        // ── NEW — spawn frozen clones at all stamped positions ────
        SpawnFrozenClones();
        // ─────────────────────────────────────────────────────────

        Debug.Log("GhostReplay: Replay started.");
    }

    // ── NEW METHOD ────────────────────────────────────────────────
    void SpawnFrozenClones()
    {
        if (frozenClonePrefab == null)
        {
            Debug.LogWarning("GhostReplay: No FrozenClonePrefab assigned! " +
                             "Drag the Ghost Prefab into this field.");
            return;
        }

        int stampCount = 0;

        foreach (FrameData frame in frames)
        {
            if (!frame.isStampFrame) continue;

            // Spawn a frozen clone at this exact stamped position
            GameObject clone = Instantiate(
                frozenClonePrefab,
                frame.position,
                frame.rotation
            );

            // Add FrozenClone.cs if not already on the prefab
            // (safe to call even if it's already there)
            if (clone.GetComponent<FrozenClone>() == null)
                clone.AddComponent<FrozenClone>();

            // Remove GhostReplay from the clone — it must NOT replay
            GhostReplay replayComp = clone.GetComponent<GhostReplay>();
            if (replayComp != null)
                Destroy(replayComp);

            // Register in the static list so LevelManager can clear all
            AllFrozenClones.Add(clone);

            stampCount++;
            Debug.Log($"GhostReplay: Frozen clone {stampCount} spawned at {frame.position}.");
        }

        if (stampCount > 0)
            Debug.Log($"GhostReplay: {stampCount} frozen clone(s) placed this loop.");
        else
            Debug.Log("GhostReplay: No stamps found in this recording. " +
                      "Press C during recording to place clones.");
    }
    // ─────────────────────────────────────────────────────────────

    IEnumerator StartFalling()
    {
        isFalling = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        yield return null; // Let physics tick once
        Debug.Log("GhostReplay: Started falling from end position.");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isFalling && IsGroundLayer(collision))
        {
            groundContactCount++;
            isGrounded = true;
            FreezeOnGround();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsGroundLayer(collision))
        {
            groundContactCount--;
            if (groundContactCount <= 0)
            {
                groundContactCount = 0;
                isGrounded = false;
            }
        }
    }

    bool IsGroundLayer(Collision collision)
    {
        return (groundLayer.value & (1 << collision.gameObject.layer)) != 0;
    }

    void FreezeOnGround()
    {
        isFalling = false;
        isGrounded = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        Debug.Log("GhostReplay: Froze on ground contact.");
    }

    public void ResetReplay()
    {
        StopAllCoroutines();
        isReplaying  = false;
        isFinished   = false;
        isFalling = false;
        groundContactCount = 0;
        isGrounded = false;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    // ── NEW — called by LevelManager on full level reset ──────────
    public static void ClearAllFrozenClones()
    {
        foreach (GameObject clone in AllFrozenClones)
        {
            if (clone != null)
                Destroy(clone);
        }

        AllFrozenClones.Clear();
        Debug.Log("GhostReplay: All frozen clones cleared.");
    }
    // ─────────────────────────────────────────────────────────────
}