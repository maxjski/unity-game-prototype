using UnityEngine;

public class ClickIndicator : MonoBehaviour
{
    public float duration = 0.5f;
    public bool scaleDown = true; // Also shrink while fading
    
    private float timer;
    private Material material;
    private Color startColor;
    private Vector3 startScale;

    void Start()
    {
        timer = duration;
        startScale = transform.localScale;
        
        // Get the material (we'll fade its alpha)
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Create instance so we don't modify the shared material
            material = rend.material;
            
            // Try to get the base color (works with URP Lit shader)
            if (material.HasProperty("_BaseColor"))
            {
                startColor = material.GetColor("_BaseColor");
            }
            else
            {
                startColor = material.color;
            }
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        float t = timer / duration; // 1 -> 0 over time

        // Fade out alpha
        if (material != null)
        {
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, startColor.a * t);
            
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", newColor);
            }
            else
            {
                material.color = newColor;
            }
        }

        // Scale down for extra visual effect
        if (scaleDown)
        {
            transform.localScale = startScale * (0.5f + 0.5f * t); // Shrinks to 50%
        }
    }
}
