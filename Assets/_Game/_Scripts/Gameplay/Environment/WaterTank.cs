using UnityEngine;
using UnityEngine.Events;

public class WaterTank : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private int maxWater;

    [Header("Visual")]
    [SerializeField] private TankVisual tankVisual;

    public int MaxWater => maxWater;
    public int CurrentWater { get; private set; }

    public UnityEvent<int, int> OnWaterChanged;
    public UnityEvent OnWaterEmpty;

    /// <summary>
    /// Mengisi tangki berdasarkan total bobot MST.
    /// </summary>
    public void Initialize(int amount)
    {
        maxWater = amount;
        CurrentWater = amount;

        UpdateVisual();

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        Debug.Log($"Tangki diisi {CurrentWater} Liter");
    }

    /// <summary>
    /// Mengecek apakah air masih cukup digunakan.
    /// </summary>
    public bool CanUseWater(int amount)
    {
        return CurrentWater >= amount;
    }

    /// <summary>
    /// Mengurangi air berdasarkan bobot edge.
    /// </summary>
    public void UseWater(int amount)
    {
        CurrentWater -= amount;

        if (CurrentWater < 0)
            CurrentWater = 0;

        UpdateVisual();

        Debug.Log($"Menggunakan {amount} Liter");
        Debug.Log($"Sisa Air : {CurrentWater}/{MaxWater}");

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);

        if (CurrentWater == 0)
        {
            Debug.Log("Air Habis");
            OnWaterEmpty?.Invoke();
        }
    }

    /// <summary>
    /// Mengisi ulang tangki.
    /// </summary>
    public void Refill()
    {
        CurrentWater = MaxWater;

        UpdateVisual();

        OnWaterChanged?.Invoke(CurrentWater, MaxWater);
    }

    /// <summary>
    /// Mengecek apakah air habis.
    /// </summary>
    public bool IsEmpty()
    {
        return CurrentWater <= 0;
    }

    /// <summary>
    /// Mengupdate visual isi air pada tangki.
    /// </summary>
    private void UpdateVisual()
    {
        if (tankVisual == null)
            return;

        float fill = MaxWater == 0
            ? 0f
            : (float)CurrentWater / MaxWater;

        tankVisual.SetFill(fill);
    }
}