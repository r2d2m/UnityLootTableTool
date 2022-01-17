using UnityEngine;
using UnityEngine.UI;

public class HUDBarManager : MonoBehaviour
{
    [SerializeField] private Image m_healthBar = null;
    [SerializeField] private Image m_staminaBar = null;
    [SerializeField] private Image m_magicBar = null;

    private static float s_kAnimationRateMultiplier = 0.1f;

    private void Start()
    {
        m_healthBar.fillAmount = 0.0f;
        m_staminaBar.fillAmount = 0.0f;
        m_magicBar.fillAmount = 0.0f;
    }

    private void Update()
    {
        m_healthBar.fillAmount += (s_kAnimationRateMultiplier * Time.deltaTime);
        m_staminaBar.fillAmount += (s_kAnimationRateMultiplier * Time.deltaTime);
        m_magicBar.fillAmount += (s_kAnimationRateMultiplier * Time.deltaTime);
    }
}
