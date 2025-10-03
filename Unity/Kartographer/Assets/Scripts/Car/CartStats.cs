using UnityEngine;

public class CartStats : MonoBehaviour
{
    public BatteryBar batteryBar;
    public GameObject batteryCanvas;
    public CarMovement carMovement;  // Reference to your BatteryBar UI
    public CartStatsSO cartStatsSO;  // Reference to your CartStatsSO ScriptableObject
    public int health;
    [HideInInspector]
    public float distance = 0f;

    void Start()
    {
        batteryCanvas.SetActive(false);
        health = cartStatsSO.health;
        // Initialize the battery bar at full health
        UpdateBatteryUI();
    }

    // Call this method to apply damage to the cart
    public void TakeDamage()
    {
        float distance = carMovement.DistanceTravelled();
        int damage = Mathf.FloorToInt(distance); // Example
        health = Mathf.Max(cartStatsSO.health - damage * 1/(cartStatsSO.batteryLife), 0);
        UpdateBatteryUI();
        // Debug.Log("Cart took " + damage + " damage from distance: " + distance);
        // Debug.Log("Cart health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateBatteryUI()
    {
        batteryBar.UpdateBatteryBar(health);
    }

    private void Die()
    {
        carMovement.isCharged = false;
        Debug.Log("Cart's battery is dead!");
        // Add effects, disable movement, etc.
    }
}
