using UnityEngine;
using UnityEngine.Events;

public class WaterTank : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private int maxWater;

    public int MaxWater => maxWater;
    public int CurrentWater { get; private set; }

    public UnityEvent<int, int> OnWaterChanged;
    public UnityEvent OnWaterEmpty;

    public void Initialize(int amount)
    {
        maxWater = amount;
        CurrentWater = amount;

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        Debug.Log($"Tangki diisi {CurrentWater} Liter");
    }

    /// <summary>
    /// Mengecek apakah air masih cukup.
    /// </summary>
    public bool CanUseWater(int amount)
    {
        return CurrentWater >= amount;
    }

    /// <summary>
    /// Mengurangi air.
    /// </summary>
    public void UseWater(int amount)
    {
        CurrentWater -= amount;

        if (CurrentWater < 0)
            CurrentWater = 0;

        Debug.Log($"Menggunakan {amount} Liter");
        Debug.Log($"Sisa Air : {CurrentWater}/{MaxWater}");

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        if (CurrentWater == 0)
            OnWaterEmpty?.Invoke();
    }

    public bool IsEmpty()
    {
        return CurrentWater <= 0;
    }

    public void Refill()
    {
        CurrentWater = MaxWater;

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);
    }
}