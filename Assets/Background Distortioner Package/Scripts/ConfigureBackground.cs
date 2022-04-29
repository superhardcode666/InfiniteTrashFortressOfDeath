using UnityEngine;
using UnityEngine.Rendering;

/// Creates Textures and sets values utilized by the shader to produce multiple effects.
public class ConfigureBackground : MonoBehaviour
{
    private static readonly int RampTex = Shader.PropertyToID("_RampTex");
    private static readonly int ColorShift = Shader.PropertyToID("_ColorShift");
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    private static readonly int Speed = Shader.PropertyToID("_Speed");
    private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
    private static readonly int Frequency = Shader.PropertyToID("_Frequency");
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int LineWidth = Shader.PropertyToID("_LineWidth");
    private static readonly int InPatternRand = Shader.PropertyToID("in_PatternRand");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int Max = Shader.PropertyToID("_Max");
    private static readonly int Min = Shader.PropertyToID("_Min");
    private static readonly int DistortionType = Shader.PropertyToID("_DistortionType");
    private static readonly int BlendModeSrc = Shader.PropertyToID("_BlendModeSrc");
    private static readonly int MyDstMode = Shader.PropertyToID("_BlendModeDst");


    /// Represents the amplitude for the Texture manipulation.
    [Tooltip("Use this to adjust amplitude of the vertex position")]
    public float amplitude;

    /// Represents the value applied for a Bloom effect.
    [Tooltip("Use this to adjust hsv manipulation")]
    public float bloomEffect;

    /// Represents the texture color map obtained from the colors of the source texture.
    private Texture2D colorMap;

    /// Represents the boolean variable to indicate if color palette must shift or not, assigned in the inspector.
    [Tooltip("Use this to set color palette shifting")]
    public bool colorShift = true;

    /// Represents the color Gradient to be assigned in the inspector.
    [Tooltip("Use this to configure the color palette to shift")]
    public Gradient colorShiftGradient;

    /// Represents the frequency for the Texture manipulation.
    [Tooltip("Use this to adjust the frequency that the sinusoidal wave moves")]
    public float frequency;

    /// Represents the width of the division for the offset manipulations.
    [Tooltip("Use this to change the spacing in the offset effects")]
    public float lineWidth;

    // Represents the minimum and maximum red values after applying greyscale
    private float min, max = 256;

    /// Represents the 0-1 range of Opacity, assigned in the inspector.
    [Range(0.0f, 1f)] [Tooltip("Use this to adjust the opacity of this layer")]
    public float opacity;

    /// Represents an auxiliary Texture variable to detect when the Texture changes during runtime.
    private Texture2D originalTexture;

    // Renderer reference
    private Renderer rend;

    /// Represents the resulting texture after manipulation.
    private Texture2D resultTexture;

    /// Represents the scale for the Texture manipulation.
    [Tooltip("Use this to adjust scale or power of the distortion")]
    public float scale;


    /// Represents the speed of displacement in the X and Y axis respectively.
    [Tooltip("Use this to adjust speed of scrolling")]
    public Vector2 ScrollSpeed;


    /// Represents the original Texture to manipulate, assigned in the inspector.
    [Header("Texture and Color Gradient")] [Tooltip("Use this to set the texture to manipulate")]
    public Texture2D sourceTexture;


    /// Represents the speed at which the palette shifts, bigger values than 100 are unnoticable.
    [Tooltip("Use this to adjust speed of distortion")] [Range(0.0f, 100f)]
    public float speed;

    /// Represents the size of tiling.
    [Tooltip("Use this to adjust size of tiling")]
    public Vector2 TilesSize = new Vector2(1f, 1f);

    /// Reprsents the blend modes options available in a drop-down manner to use in the inspector.
    [Tooltip("Use this to adjust the blend mode of this layer")]
    [StringInList("Traditional", "Premultiplied", "Additive", "Soft-Additive", "Multiplicative", "2x-Multiplicative")]
    public string typeOfBlend = "Traditional";


    /// Reprsents the effects options available in a drop-down manner to use in the inspector.
    [Header("Configuration")]
    [Tooltip("Use this to adjust type of distortion")]
    [Space(25)]
    [StringInList("No Effect", "Horizontal Offset Wave", "Horizontal Wave", "Horizontal and Vertical Wave",
        "Horizontal and Vertical Offset Wave", "Vertical Offset Wave", "Vertical Wave")]
    public string typeOfDistortion;


    //Initiate
    private void Awake()
    {
        rend = GetComponent<Renderer>();

        CreateImageEffect();
        if (colorShift)
            rend.material.SetFloat(ColorShift, 1);
        else
            rend.material.SetFloat(ColorShift, 0);
        rend.material.SetFloat(Alpha, opacity);
        rend.material.SetFloat(Speed, speed);
        rend.material.SetFloat(Amplitude, amplitude);
        rend.material.SetFloat(Frequency, frequency);
        rend.material.SetFloat(Scale, scale);
        rend.material.SetFloat(LineWidth, lineWidth);
        rend.material.SetFloat(InPatternRand, bloomEffect);
        switch (typeOfBlend)
        {
            case "Traditional":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.SrcAlpha);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.OneMinusSrcAlpha);
                break;

            case "Premultiplied":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.One);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.OneMinusSrcAlpha);
                break;

            case "Additive":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.One);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.One);
                break;

            case "Soft-Additive":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.OneMinusDstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.One);
                break;

            case "Multiplicative":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.DstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.Zero);
                break;

            case "2x-Multiplicative":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.DstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.SrcColor);
                break;

            default:
                print("Blend mode not correct");
                break;
        }
    }

    //Calls color map function, calls grayscale convertion function, finds min max values, and finally applies the textures to the shader.
    private void CreateImageEffect()
    {
        resultTexture = new Texture2D(sourceTexture.width, sourceTexture.height);
        colorMap = new Texture2D(200, 1);
        CreateColorMap();
        ConvertToGrayscale();

        var position = transform.position;
        var x = Mathf.RoundToInt(position.x);
        var y = Mathf.RoundToInt(position.y);
        var width = Mathf.FloorToInt(sourceTexture.width);
        var height = Mathf.FloorToInt(sourceTexture.height);

        for (var i = y; i < height; i++)
        for (var j = x; j < width; j++)
        {
            var aux = resultTexture.GetPixel(j, i).r * 255.0f;
            if (aux > min)
                min = aux;
            if (aux < max)
                max = aux;
        }

        rend.material.SetTexture(MainTex, resultTexture);
        rend.material.SetFloat(Max, max);
        rend.material.SetFloat(Min, min);
    }


    //Update values if there are changes in the inspector.
    private void Update()
    {
        if (originalTexture != sourceTexture)
        {
            CreateImageEffect();
            originalTexture = sourceTexture;
        }

        rend.material.SetFloat(ColorShift, colorShift ? 1 : 0);
        rend.material.SetFloat(Alpha, opacity);
        rend.material.SetFloat(Speed, speed);
        rend.material.SetFloat(Amplitude, amplitude);
        rend.material.SetFloat(Frequency, frequency);
        rend.material.SetFloat(Scale, scale);
        rend.material.SetFloat(LineWidth, lineWidth);
        rend.material.SetFloat(InPatternRand, bloomEffect);

        switch (typeOfDistortion)
        {
            case "No Effect":
                rend.material.SetFloat(DistortionType, 0);
                break;
            case "Horizontal Offset Wave":
                rend.material.SetFloat(DistortionType, 1);
                break;
            case "Horizontal Wave":
                rend.material.SetFloat(DistortionType, 2);
                break;
            case "Horizontal and Vertical Wave":
                rend.material.SetFloat(DistortionType, 3);
                break;
            case "Horizontal and Vertical Offset Wave":
                rend.material.SetFloat(DistortionType, 4);
                break;
            case "Vertical Offset Wave":
                rend.material.SetFloat(DistortionType, 5);
                break;
            case "Vertical Wave":
                rend.material.SetFloat(DistortionType, 6);
                break;

            default:
                rend.material.SetFloat(DistortionType, 0);
                break;
        }

        switch (typeOfBlend)
        {
            case "Traditional":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.SrcAlpha);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.OneMinusSrcAlpha);
                break;

            case "Premultiplied":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.One);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.OneMinusSrcAlpha);
                break;

            case "Additive":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.One);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.One);
                break;

            case "Soft-Additive":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.OneMinusDstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.One);
                break;

            case "Multiplicative":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.DstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.Zero);
                break;

            case "2x-Multiplicative":
                rend.material.SetFloat(BlendModeSrc, (int) BlendMode.DstColor);
                rend.material.SetFloat(MyDstMode, (int) BlendMode.SrcColor);
                break;

            default:
                print("Blend mode not correct");
                break;
        }

        var OffsetX = Time.time * ScrollSpeed.x % 1.0f;
        var OffsetY = Time.time * ScrollSpeed.y % 1.0f;
        var material = rend.material;
        material.mainTextureOffset = new Vector2(OffsetX, OffsetY);
        material.mainTextureScale = new Vector2(TilesSize.x, TilesSize.y);
    }

    //Create color map and assign it to the shader
    private void CreateColorMap()
    {
        for (var i = 0; i <= 100; i++)
        {
            var pos = i / 100f;
            colorMap.SetPixel(i, 1, colorShiftGradient.Evaluate(pos));
        }

        for (var i = 100; i >= 0; i--)
        {
            var pos = i / 100f;
            colorMap.SetPixel(200 - i, 1, colorShiftGradient.Evaluate(pos));
        }

        colorMap.Apply();
        rend.material.SetTexture(RampTex, colorMap);
    }

    //Convert the source texture to grayscale and save is as the resultTexture
    private void ConvertToGrayscale()
    {
        for (var x = 0; x < resultTexture.width; x++)
        for (var y = 0; y < resultTexture.height; y++)
        {
            var pixel = sourceTexture.GetPixel(x, y);

            var l = pixel.r * 0.3f + pixel.g * 0.59f + pixel.b * 0.11f;
            var c = new Color(l, l, l, 1);
            resultTexture.SetPixel(x, y, c);
        }

        resultTexture.Apply();
    }
}