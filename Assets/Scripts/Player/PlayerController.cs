using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerAnimation))]
public class PlayerController : MonoBehaviour
{
    private static readonly string s_kMissingComponentsText = "PlayerController is missing components.";

    [Tooltip("The players follow camera. The camera tagged as MainCamera will be used by default, if not set.")]
    [SerializeField] private Camera m_followCamera = null;

    // The layer the player can move on.
    // NOTE: There is a bug where incorrect tooltip info will display.
    [SerializeField] private LayerMask m_playerWalkableLayer = 0;

    // The motor controlled by the controller.
    private PlayerMotor m_playerMotor = null;

    // The cached game object and transform
    private GameObject m_thisObject = null;

    // The current target interactable object of the player.
    private Interactable m_target = null;

    // The length a ray will test against for movement.
    // More performant than an infinite ray.
    private static readonly float s_kRayMaxLength = 50.0f;

    void Awake()
    {
        if (m_followCamera == null)
            m_followCamera = Camera.main;

        if (m_playerMotor == null)
            m_playerMotor = GetComponent<PlayerMotor>();

        m_thisObject = this.gameObject;
    }

    void Update()
    {
        if (m_playerMotor == null && m_followCamera == null)
        {
            Debug.LogWarning(s_kMissingComponentsText);
            return;
        }

        // This could probably live somewhere else...
        if (Input.GetKeyUp(KeyCode.Escape))
            EventManager.TriggerEvent(EventID.s_kPauseGame);

        // If we have targeted an interactable, then try to interact with it.
        if (m_target != null)
            m_target.OnInteract(m_thisObject);

        // Ignore clicks over UI.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Toggle Inventory.
        if (Input.GetKeyUp(KeyCode.I))
            EventManager.TriggerEvent(EventID.s_kToggleInventoryUIEvent);

        if (Input.GetMouseButtonDown(0)) // Left Mouse Button
            TryMoveToMousePoint();
        else if (Input.GetMouseButtonDown(1)) // Right Mouse Button
            TryInteract();
    }

    // Cast a ray from screen space to world space and move the player if valid.
    private void TryMoveToMousePoint()
    {
        Ray screenPointRay = m_followCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(screenPointRay, out RaycastHit rayHit, s_kRayMaxLength, m_playerWalkableLayer))
        {
            ClearTarget();
            m_playerMotor.MoveToPoint(rayHit.point);
        }
    }

    // Attempt to interact with whatever was clicked on.
    private void TryInteract()
    {
        Ray screenPointRay = m_followCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(screenPointRay, out RaycastHit rayHit, s_kRayMaxLength))
        {
            Interactable interactable = rayHit.collider.GetComponent<Interactable>();
            if (interactable != null)
                SetTarget(interactable);
        }
    }

    // Sets our target to the interactable that was clicked on.
    private void SetTarget(Interactable target)
    {
        if (target != m_target)
        {
            ClearTarget();
            m_target = target;
        }
        
        m_playerMotor.MoveToTarget(m_target);
        target.ResetInteractionState();
    }

    // Removes the current target, if any.
    private void ClearTarget()
    {
        if (m_target != null)
        {
            m_target.ResetInteractionState();
            m_target = null;
        }

        m_playerMotor.CancelAction();
    }
}
