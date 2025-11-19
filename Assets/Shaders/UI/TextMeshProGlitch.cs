using UnityEngine;
using TMPro;

public class TMPMeshProGlitch : MonoBehaviour
{
    private TMP_Text textComponent;

    [Header("Glitch Settings")]
    [Tooltip("글리치가 얼마나 자주 발생하는지 (0~1)")]
    public float glitchFrequency = 0.1f;

    [Tooltip("가로로 찢어지는 강도")]
    public float tearIntensity = 5.0f;

    [Tooltip("가로 줄무늬의 촘촘함")]
    public float scanlineScale = 20.0f;

    [Tooltip("글리치 속도")]
    public float speed = 10.0f;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        // 글리치 타이밍 결정 (랜덤하게 발생했다가 멈췄다 함)
        // PerlinNoise를 사용해 불규칙하지만 자연스러운 흐름 생성
        float glitchTrigger = Mathf.PerlinNoise(Time.time * speed * 0.2f, 0);
        bool isGlitching = glitchTrigger < glitchFrequency;

        if (!isGlitching) return; // 글리치 타이밍이 아니면 원본 유지

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var materialIndex = charInfo.materialReferenceIndex;
            var sourceVertices = textInfo.meshInfo[materialIndex].vertices;
            int vertexIndex = charInfo.vertexIndex;

            // 글자 하나를 구성하는 4개의 버텍스(점)를 각각 처리
            for (int j = 0; j < 4; j++)
            {
                Vector3 vert = sourceVertices[vertexIndex + j];

                // 핵심 로직: 버텍스의 Y(높이) 값에 따라 노이즈를 생성
                // 높이가 비슷하면(같은 가로줄이면) 같은 방향으로 찢어짐
                float scanlineNoise = Mathf.PerlinNoise(vert.y * scanlineScale * 0.01f, Time.time * speed);

                // 노이즈가 특정 임계값을 넘을 때만 가로(X)로 확 밀어버림
                if (scanlineNoise > 0.6f)
                {
                    // -1 또는 1 방향으로 랜덤하게 찢음
                    float direction = (scanlineNoise > 0.8f) ? 1 : -1;
                    vert.x += direction * tearIntensity * Random.Range(0.5f, 1.5f);
                }

                sourceVertices[vertexIndex + j] = vert;
            }
        }

        // 변경된 메쉬 적용
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}