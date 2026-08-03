using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Sawi : MonoBehaviour
{
    [Header("Animation Delay")]
    [SerializeField] private float animationDelay = 0.5f;

    private Animator _sawiAnim;
    private readonly int GrowHash = Animator.StringToHash("Grow");
    private WaitForSeconds _growWait;

    private void Start()
    {
        _sawiAnim = GetComponent<Animator>();
        _growWait = new WaitForSeconds(animationDelay);
    }

    public void PlayGrowAnimation()
    {
        StartCoroutine(PlayGrowCorotine());
    }

    IEnumerator PlayGrowCorotine()
    {
        yield return _growWait;
        _sawiAnim.SetTrigger(GrowHash);
    }
}
