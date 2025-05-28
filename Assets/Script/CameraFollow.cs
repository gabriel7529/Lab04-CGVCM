using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowWithFade : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    public Transform Target;                  // Target to follow
    public Vector3 Offset;                    // Offset from target
    public float SmoothTime = 0.7f;           // Smooth follow time
    private Vector3 velocity = Vector3.zero;

    [Header("Fade Settings")]
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private float FadedAlpha = 0.33f;
    [SerializeField] private FadeMode FadingMode;
    [SerializeField] private float ChecksPerSecond = 10;
    [SerializeField] private int FadeFPS = 30;
    [SerializeField] private float FadeSpeed = 1;

    private List<FadingObject> ObjectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> RunningCoroutines = new Dictionary<FadingObject, Coroutine>();
    private RaycastHit[] Hits = new RaycastHit[10];

    private void Start()
    {
        Offset = transform.position - Target.position;
        StartCoroutine(CheckForObjects());
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = Target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        transform.LookAt(Target);
    }

    private IEnumerator CheckForObjects()
    {
        WaitForSeconds wait = new WaitForSeconds(1f / ChecksPerSecond);

        while (true)
        {
            int hits = Physics.RaycastNonAlloc(transform.position, (Target.position - transform.position).normalized, Hits, Vector3.Distance(transform.position, Target.position), LayerMask);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(Hits[i]);
                    if (fadingObject != null && !ObjectsBlockingView.Contains(fadingObject))
                    {
                        if (RunningCoroutines.ContainsKey(fadingObject))
                        {
                            if (RunningCoroutines[fadingObject] != null)
                                StopCoroutine(RunningCoroutines[fadingObject]);

                            RunningCoroutines.Remove(fadingObject);
                        }

                        RunningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        ObjectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();
            ClearHits();
            yield return wait;
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        for (int i = ObjectsBlockingView.Count - 1; i >= 0; i--)
        {
            bool objectIsBeingHit = false;
            for (int j = 0; j < Hits.Length; j++)
            {
                FadingObject fadingObject = GetFadingObjectFromHit(Hits[j]);
                if (fadingObject != null && fadingObject == ObjectsBlockingView[i])
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit)
            {
                var obj = ObjectsBlockingView[i];

                if (RunningCoroutines.ContainsKey(obj))
                {
                    if (RunningCoroutines[obj] != null)
                        StopCoroutine(RunningCoroutines[obj]);

                    RunningCoroutines.Remove(obj);
                }

                RunningCoroutines.Add(obj, StartCoroutine(FadeObjectIn(obj)));
                ObjectsBlockingView.RemoveAt(i);
            }
        }
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        float waitTime = 1f / FadeFPS;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        foreach (var mat in fadingObject.Materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);

            if (FadingMode == FadeMode.Fade)
                mat.EnableKeyword("_ALPHABLEND_ON");
            else
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        while (fadingObject.Materials[0].color.a > FadedAlpha)
        {
            for (int i = 0; i < fadingObject.Materials.Count; i++)
            {
                if (fadingObject.Materials[i].HasProperty("_Color"))
                {
                    var c = fadingObject.Materials[i].color;
                    fadingObject.Materials[i].color = new Color(
                        c.r, c.g, c.b,
                        Mathf.Lerp(fadingObject.InitialAlpha, FadedAlpha, waitTime * ticks * FadeSpeed)
                    );
                }
            }

            ticks++;
            yield return wait;
        }
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float waitTime = 1f / FadeFPS;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        while (fadingObject.Materials[0].color.a < fadingObject.InitialAlpha)
        {
            for (int i = 0; i < fadingObject.Materials.Count; i++)
            {
                if (fadingObject.Materials[i].HasProperty("_Color"))
                {
                    var c = fadingObject.Materials[i].color;
                    fadingObject.Materials[i].color = new Color(
                        c.r, c.g, c.b,
                        Mathf.Lerp(FadedAlpha, fadingObject.InitialAlpha, waitTime * ticks * FadeSpeed)
                    );
                }
            }

            ticks++;
            yield return wait;
        }

        foreach (var mat in fadingObject.Materials)
        {
            if (FadingMode == FadeMode.Fade)
                mat.DisableKeyword("_ALPHABLEND_ON");
            else
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }

    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null;
    }

    private void ClearHits()
    {
        for (int i = 0; i < Hits.Length; i++)
        {
            Hits[i] = new RaycastHit();
        }
    }

    public enum FadeMode
    {
        Transparent,
        Fade
    }
}
