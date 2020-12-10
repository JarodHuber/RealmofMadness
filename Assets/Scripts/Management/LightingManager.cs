using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public Vector3 fogColorIni, fogColorFin;
    Vector3 fogColorForScene;
    Color fogColor = new Color();
    public float levelFin = 100f;
    public float fogDecayMax = 0.08f, fogDecay, decayDegree;

    public void SetFog(int lvlNum)
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        float lerpAmount = (float)lvlNum / (float)PuzzleGeneration.MaxDifficulty;
        fogColorForScene = Vector3.Lerp(fogColorIni, fogColorFin, lerpAmount);
        fogColor.r = fogColorForScene.x / 255f;
        fogColor.g = fogColorForScene.y / 255f;
        fogColor.b = fogColorForScene.z / 255f;
        fogColor.a = 1;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDecayMax;
    }

    private void Update()
    {
        if (GameObject.Find("Player").transform.position.y <= -10)
        {
            decayDegree = (puzzleManager.player.transform.position.y / -100f);
            fogDecay = Mathf.Lerp(fogDecayMax, 0, decayDegree);
            RenderSettings.fogDensity = fogDecay;
        }
    }
}
