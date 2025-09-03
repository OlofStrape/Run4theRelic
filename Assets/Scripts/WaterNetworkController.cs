using UnityEngine;

public class WaterNetworkController : MonoBehaviour
{
    public WaterWheel valveA, valveB;
    public float tankFill; // 0..1
    public float baseFlowPerSec = 0.25f;

    [Header("UI")]
    public FillGauge gauge;

    void Update()
    {
        float a = valveA != null ? valveA.Value : 0f;
        float b = valveB != null ? valveB.Value : 0f;

        float routeEff = RouteEfficiency(a, b); // 0..1
        tankFill += baseFlowPerSec * routeEff * Time.deltaTime;
        tankFill = Mathf.Clamp01(tankFill);

        if (gauge != null)
        {
            gauge.Set01(tankFill);
        }

        if (tankFill >= 1f)
        {
            GetComponent<Puzzles.PuzzleControllerBase>()?.SendMessage("Complete");
        }
    }

    float RouteEfficiency(float a, float b)
    {
        // enkel S-kurva där sweetspot ~0.65/0.35 – tweak fritt
        float effA = 1f - Mathf.Abs(a - 0.65f) * 2f;
        float effB = 1f - Mathf.Abs(b - 0.35f) * 2f;
        return Mathf.Clamp01((effA + effB) * 0.5f);
    }
}

