using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BillboardGroup : MonoBehaviour
{
    [Tooltip("Yaln�zca Y ekseninde mi d�necek (true) yoksa tamamen kameraya m� bakacak (false)")]
    public bool yAxisOnly = true;

    private Camera cam;
    private Transform[] childSprites;

    void Awake()
    {
        cam = Camera.main;

        // T�m SpriteRenderer�l� child objeleri topla
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        childSprites = new Transform[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            childSprites[i] = renderers[i].transform;
        }
    }

    void LateUpdate()
    {
        if (!cam) return;

        foreach (var t in childSprites)
        {
            if (!t) continue;

            if (yAxisOnly)
            {
                Vector3 dir = cam.transform.position - t.position;
                dir.y = 0f; // sadece yatay eksende
                if (dir.sqrMagnitude < 0.0001f) continue;
                t.rotation = Quaternion.LookRotation(dir);
            }
            else
            {
                t.forward = cam.transform.forward; // tam billboard
            }
        }
    }
}

