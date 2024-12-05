using UnityEngine;


public class PlayerTouchHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
        {
            networkManager.OnPlayerTouched(this.gameObject);
        }
    }
}