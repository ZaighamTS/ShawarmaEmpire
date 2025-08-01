using UnityEngine;

public class ShawarmaProductionSystem : MonoBehaviour
{
    public static ShawarmaProductionSystem Instance;

    [Header("Tapping Settings")]
    public float tapMultiplier = 1.0f;
    public float maxMultiplier = 1.5f;
    public float multiplierDecayRate = 0.5f;
    public float multiplierIncreasePerTap = 0.05f;
    public float timeBetweenTapsThreshold = 0.3f; // seconds for faster tapping

    private float lastTapTime;

    [Header("Shawarma Settings")]
    public double shawarmaValue = 10.0;
    public double upgradedShawarmaValue = 30.0;
    public bool isUpgraded = false;

    [Header("Auto Chef Settings")]
    public bool autoChefUnlocked = false;
    public float autoCookInterval = 2f;
    private float autoCookTimer = 0f;

    [Header("Storage Settings")]
    public int storageCapacity = 0;
    public int currentStorage = 0;

    [Header("Level Progression")]
    public int currentLevel = 1;
    public int shawarmaProducedTotal = 0;
    public int targetShawarmaForNextLevel = 5000;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Handle auto cooking
        if (autoChefUnlocked && currentStorage < storageCapacity)
        {
            autoCookTimer += Time.deltaTime;
            if (autoCookTimer >= autoCookInterval)
            {
                autoCookTimer = 0f;
                ProduceShawarma();
            }
        }

        // Decay multiplier
        if (Time.time - lastTapTime > timeBetweenTapsThreshold && tapMultiplier > 1.0f)
        {
            tapMultiplier = Mathf.Max(1.0f, tapMultiplier - multiplierDecayRate * Time.deltaTime);
        }

        // 👇 Add this for live UI feedback
        //UIManager.Instance?.UpdateUI();
    }

    public void TapToCook()
    {
      
        float timeSinceLastTap = Time.time - lastTapTime;
        lastTapTime = Time.time;

        if (timeSinceLastTap < timeBetweenTapsThreshold)
        {
            tapMultiplier = Mathf.Min(maxMultiplier, tapMultiplier + multiplierIncreasePerTap);
        }
        else
        {
            tapMultiplier = 1.0f; // Reset multiplier on slow tap
        }

        ProduceShawarma();
    }

    void ProduceShawarma()
    {
        if (currentStorage >= storageCapacity) return;

        currentStorage++;
        shawarmaProducedTotal++;

        double value = isUpgraded ? upgradedShawarmaValue : shawarmaValue;
        double cashEarned = value * tapMultiplier;

        // Level Up Logic
        if (shawarmaProducedTotal >= targetShawarmaForNextLevel)
        {
            AdvanceLevel();
        }
    }

    void AdvanceLevel()
    {
        currentLevel++;
        shawarmaProducedTotal = 0;
        targetShawarmaForNextLevel += 5000; // You can adjust how this scales
        isUpgraded = true;

        Debug.Log("Level Up! Shawarma visuals upgraded.");
    }

    public void UpgradeStorage(int additionalCapacity)
    {
        storageCapacity += additionalCapacity;
    }

    public void UnlockAutoChef()
    {
        autoChefUnlocked = true;
    }

    public void ResetStorage()
    {
        currentStorage = 0;
    }

    public int GetCurrentStorage()
    {
        return currentStorage;
    }

    public int GetStorageCapacity()
    {
        return storageCapacity;
    }

    public float GetMultiplier()
    {
        return tapMultiplier;
    }
}
