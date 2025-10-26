using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    [Header("Skybox Materials")]
    [SerializeField] private Material[] skyboxMaterials;
    [SerializeField] private float transitionDuration = 3f;

    [Header("Transition Timing")]
    [SerializeField] private int roundsPerTransition = 3;

    private int currentSkyboxIndex = 0;
    private Material blendedSkybox;
    private bool isTransitioning = false;
    private float transitionProgress = 0f;
    private Material previousSkybox;
    private Material targetSkybox;

    private void Start()
    {
        if (skyboxMaterials == null || skyboxMaterials.Length == 0)
        {
            Debug.LogWarning("SkyboxTransition: No skybox materials assigned!");
            return;
        }

        blendedSkybox = new Material(Shader.Find("Skybox/Procedural"));
        RenderSettings.skybox = skyboxMaterials[0];
        currentSkyboxIndex = 0;
    }

    public void OnRoundStart(int round)
    {
        if (skyboxMaterials == null || skyboxMaterials.Length <= 1) return;

        if (round % roundsPerTransition == 0 && round > 0)
        {
            int nextIndex = (currentSkyboxIndex + 1) % skyboxMaterials.Length;
            StartSkyboxTransition(nextIndex);
        }
    }

    private void StartSkyboxTransition(int targetIndex)
    {
        if (isTransitioning || targetIndex >= skyboxMaterials.Length) return;

        previousSkybox = skyboxMaterials[currentSkyboxIndex];
        targetSkybox = skyboxMaterials[targetIndex];
        currentSkyboxIndex = targetIndex;

        isTransitioning = true;
        transitionProgress = 0f;
    }

    private void Update()
    {
        if (!isTransitioning) return;

        transitionProgress += Time.deltaTime / transitionDuration;

        if (transitionProgress >= 1f)
        {
            RenderSettings.skybox = targetSkybox;
            isTransitioning = false;
            transitionProgress = 1f;
        }
        else
        {
            BlendSkyboxes(previousSkybox, targetSkybox, transitionProgress);
        }
    }

    private void BlendSkyboxes(Material from, Material to, float t)
    {
        if (from.shader.name == "Skybox/Cubemap" && to.shader.name == "Skybox/Cubemap")
        {
            blendedSkybox.shader = Shader.Find("Skybox/Cubemap");
            
            if (from.HasProperty("_Tex"))
            {
                Cubemap fromCubemap = from.GetTexture("_Tex") as Cubemap;
                Cubemap toCubemap = to.GetTexture("_Tex") as Cubemap;
                
                if (t < 0.5f)
                {
                    blendedSkybox.SetTexture("_Tex", fromCubemap);
                    blendedSkybox.SetFloat("_Exposure", Mathf.Lerp(1f, 0.5f, t * 2f));
                }
                else
                {
                    blendedSkybox.SetTexture("_Tex", toCubemap);
                    blendedSkybox.SetFloat("_Exposure", Mathf.Lerp(0.5f, 1f, (t - 0.5f) * 2f));
                }
            }
        }
        else if (from.shader.name == "Skybox/6 Sided" && to.shader.name == "Skybox/6 Sided")
        {
            RenderSettings.skybox = t < 0.5f ? from : to;
        }
        else
        {
            RenderSettings.skybox = t < 0.5f ? from : to;
        }

        DynamicGI.UpdateEnvironment();
    }
}
