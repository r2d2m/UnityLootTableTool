using UnityEngine;

abstract public class Menu : MonoBehaviour
{
    // Abstract functions called by the menu manager.
    abstract public void OnEnterMenu();
    abstract public void OnExitMenu();
}
