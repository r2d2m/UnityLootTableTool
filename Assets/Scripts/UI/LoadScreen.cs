using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [Tooltip("Image used to fill the loading progress bar.")]
    [SerializeField] private Image m_fillImage = null;

    private void Awake()
    {
        // We dont destroy on load in case we want to use this between scenes.
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    // Set the progress bar to the desired fill amount.
    public void SetFillPercent(float fillPercent)
    {
        if (fillPercent > 1.0f)
            fillPercent = 1.0f;
        if (fillPercent < 0.0f)
            fillPercent = 0.0f;

        m_fillImage.fillAmount = fillPercent;
    }
}
