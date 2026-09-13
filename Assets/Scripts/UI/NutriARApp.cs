using System.Collections;
using NutriAR.Models;
using NutriAR.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NutriAR.UI
{
    public sealed class NutriARApp : MonoBehaviour
    {
        private static readonly Color Background = Hex("F4F8F3");
        private static readonly Color Surface = Hex("FFFFFF");
        private static readonly Color Ink = Hex("18342C");
        private static readonly Color Muted = Hex("63766F");
        private static readonly Color Primary = Hex("0D8A5B");
        private static readonly Color PrimaryDark = Hex("086442");
        private static readonly Color CameraFallbackTop = Hex("173D34");
        private static readonly Color CameraFallbackBottom = Hex("071B17");
        private static readonly Color CarbColor = Hex("F2A93B");
        private static readonly Color ProteinColor = Hex("4C83E6");
        private static readonly Color FatColor = Hex("B06BDD");

        private Font font;
        private Text resultName;
        private Text resultPortion;
        private Text calories;
        private Text dailyReference;
        private Text carbValue;
        private Text proteinValue;
        private Text fatValue;
        private Image carbBar;
        private Image proteinBar;
        private Image fatBar;
        private Image alertPanel;
        private Text alertTitle;
        private Text alertMessage;
        private Text guidance;
        private Button scanButton;
        private Text scanButtonLabel;
        private RectTransform scanLine;
        private GameObject scanningLabel;
        private int nextProfileIndex;
        private bool scanning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindAnyObjectByType<NutriARApp>() != null)
            {
                return;
            }

            var app = new GameObject("NutriAR Application");
            DontDestroyOnLoad(app);
            app.AddComponent<NutriARApp>();
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            BuildInterface();
            ShowResult(NutritionCatalog.GetAt(0));
        }

        private void BuildInterface()
        {
            CreateEventSystem();

            var canvasObject = new GameObject("NutriAR Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = CreatePanel("Background", canvasObject.transform, Background, 0f);
            Stretch(background.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var safeArea = CreateUIObject("Safe Area", background.transform);
            Stretch(safeArea, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            safeArea.gameObject.AddComponent<SafeAreaFitter>();

            BuildHeader(safeArea);
            BuildCamera(safeArea);
            BuildSampleButtons(safeArea);
            BuildScanButton(safeArea);
            BuildResultCard(safeArea);
            BuildDisclaimer(safeArea);
        }

        private void BuildHeader(RectTransform parent)
        {
            var leaf = CreatePanel("Brand Mark", parent, Primary, 22f);
            SetRect(leaf.rectTransform, 0.045f, 0.938f, 0.125f, 0.986f);
            CreateText("Brand", leaf.transform, "N", 42, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);

            var title = CreateText("Title", parent, "NutriAR", 52, FontStyle.Bold, Ink, TextAnchor.MiddleLeft);
            SetRect(title.rectTransform, 0.15f, 0.944f, 0.62f, 0.985f);

            var subtitle = CreateText("Subtitle", parent, "Entenda o que está no seu prato", 25, FontStyle.Normal, Muted, TextAnchor.MiddleLeft);
            SetRect(subtitle.rectTransform, 0.15f, 0.91f, 0.78f, 0.946f);

            var badge = CreatePanel("Prototype Badge", parent, new Color(0.05f, 0.54f, 0.36f, 0.11f), 26f);
            SetRect(badge.rectTransform, 0.70f, 0.94f, 0.955f, 0.982f);
            var badgeText = CreateText("Badge Text", badge.transform, "DEMONSTRAÇÃO", 19, FontStyle.Bold, PrimaryDark, TextAnchor.MiddleCenter);
            Stretch(badgeText.rectTransform, Vector2.zero, Vector2.one, new Vector2(10f, 0f), new Vector2(-10f, 0f));
        }

        private void BuildCamera(RectTransform parent)
        {
            var cameraCard = CreatePanel("Camera Card", parent, CameraFallbackBottom, 36f);
            SetRect(cameraCard.rectTransform, 0.045f, 0.505f, 0.955f, 0.895f);
            AddShadow(cameraCard.gameObject, new Color(0f, 0.12f, 0.08f, 0.18f), new Vector2(0f, -8f));

            var previewObject = new GameObject("Camera Preview", typeof(RectTransform), typeof(RawImage));
            previewObject.transform.SetParent(cameraCard.transform, false);
            var previewRect = previewObject.GetComponent<RectTransform>();
            Stretch(previewRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var preview = previewObject.GetComponent<RawImage>();
            preview.texture = CreateGradientTexture(CameraFallbackTop, CameraFallbackBottom);
            preview.color = Color.white;

            var shade = CreatePanel("Readability Shade", cameraCard.transform, new Color(0f, 0f, 0f, 0.16f), 0f);
            Stretch(shade.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var mode = CreatePanel("Mode", cameraCard.transform, new Color(0.03f, 0.12f, 0.1f, 0.74f), 22f);
            SetRect(mode.rectTransform, 0.055f, 0.865f, 0.49f, 0.955f);
            var modeText = CreateText("Mode Text", mode.transform, "ALIMENTO OU RÓTULO", 21, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            Stretch(modeText.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 0f), new Vector2(-12f, 0f));

            var statusText = CreateText("Camera Status", cameraCard.transform, "Iniciando câmera...", 22, FontStyle.Normal, new Color(1f, 1f, 1f, 0.88f), TextAnchor.MiddleCenter);
            SetRect(statusText.rectTransform, 0.05f, 0.035f, 0.95f, 0.12f);

            var frame = CreateUIObject("Scan Frame", cameraCard.transform);
            SetRect(frame, 0.19f, 0.24f, 0.81f, 0.78f);
            AddBorder(frame, new Color(0.64f, 1f, 0.83f, 0.95f), 6f, 22f);

            var lineImage = CreatePanel("Scan Line", frame, new Color(0.38f, 1f, 0.72f, 0.85f), 3f);
            scanLine = lineImage.rectTransform;
            SetRect(scanLine, 0.08f, 0.78f, 0.92f, 0.80f);
            scanLine.gameObject.SetActive(false);

            scanningLabel = CreatePanel("Scanning Label", frame, new Color(0.02f, 0.16f, 0.12f, 0.86f), 18f).gameObject;
            var scanningRect = scanningLabel.GetComponent<RectTransform>();
            SetRect(scanningRect, 0.25f, 0.42f, 0.75f, 0.58f);
            var scanningText = CreateText("Scanning Text", scanningLabel.transform, "ANALISANDO...", 26, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            Stretch(scanningText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            scanningLabel.SetActive(false);

            var cameraController = gameObject.AddComponent<CameraFeedController>();
            cameraController.Initialize(preview, statusText);
        }

        private void BuildSampleButtons(RectTransform parent)
        {
            var hint = CreateText("Examples Hint", parent, "TESTE RÁPIDO", 19, FontStyle.Bold, Muted, TextAnchor.MiddleLeft);
            SetRect(hint.rectTransform, 0.05f, 0.473f, 0.28f, 0.502f);

            var labels = new[] { "Banana", "Refrigerante", "Barra", "Pão de queijo" };
            var ids = new[] { "banana", "refrigerante", "barra", "pao-queijo" };
            for (var i = 0; i < labels.Length; i++)
            {
                var capturedId = ids[i];
                var minX = 0.05f + i * 0.23f;
                var button = CreateButton("Sample " + labels[i], parent, labels[i], 21, Surface, Ink, () => Scan(capturedId));
                SetRect(button.GetComponent<RectTransform>(), minX, 0.437f, minX + 0.215f, 0.477f);
                AddShadow(button.gameObject, new Color(0f, 0.1f, 0.07f, 0.09f), new Vector2(0f, -3f));
            }
        }

        private void BuildScanButton(RectTransform parent)
        {
            scanButton = CreateButton("Scan Button", parent, "ESCANEAR AGORA", 30, Primary, Color.white, ScanNext);
            SetRect(scanButton.GetComponent<RectTransform>(), 0.14f, 0.374f, 0.86f, 0.425f);
            AddShadow(scanButton.gameObject, new Color(0.02f, 0.35f, 0.22f, 0.28f), new Vector2(0f, -7f));
            scanButtonLabel = scanButton.GetComponentInChildren<Text>();
        }

        private void BuildResultCard(RectTransform parent)
        {
            var card = CreatePanel("Nutrition Result", parent, Surface, 34f);
            SetRect(card.rectTransform, 0.045f, 0.075f, 0.955f, 0.352f);
            AddShadow(card.gameObject, new Color(0f, 0.12f, 0.08f, 0.13f), new Vector2(0f, -6f));

            resultName = CreateText("Food Name", card.transform, "", 39, FontStyle.Bold, Ink, TextAnchor.MiddleLeft);
            SetRect(resultName.rectTransform, 0.045f, 0.82f, 0.68f, 0.96f);
            resultPortion = CreateText("Portion", card.transform, "", 22, FontStyle.Normal, Muted, TextAnchor.MiddleRight);
            SetRect(resultPortion.rectTransform, 0.57f, 0.82f, 0.955f, 0.96f);

            calories = CreateText("Calories", card.transform, "", 64, FontStyle.Bold, PrimaryDark, TextAnchor.MiddleLeft);
            SetRect(calories.rectTransform, 0.045f, 0.53f, 0.40f, 0.81f);
            dailyReference = CreateText("Daily Reference", card.transform, "", 20, FontStyle.Normal, Muted, TextAnchor.UpperLeft);
            SetRect(dailyReference.rectTransform, 0.05f, 0.43f, 0.38f, 0.57f);

            CreateMacroRow(card.transform, "CARBOIDRATOS", CarbColor, 0.69f, out carbValue, out carbBar);
            CreateMacroRow(card.transform, "PROTEÍNAS", ProteinColor, 0.57f, out proteinValue, out proteinBar);
            CreateMacroRow(card.transform, "GORDURAS", FatColor, 0.45f, out fatValue, out fatBar);

            alertPanel = CreatePanel("Health Alert", card.transform, new Color(0.05f, 0.54f, 0.36f, 0.10f), 22f);
            SetRect(alertPanel.rectTransform, 0.035f, 0.055f, 0.965f, 0.395f);
            alertTitle = CreateText("Alert Title", alertPanel.transform, "", 23, FontStyle.Bold, Ink, TextAnchor.MiddleLeft);
            SetRect(alertTitle.rectTransform, 0.035f, 0.69f, 0.965f, 0.94f);
            alertMessage = CreateText("Alert Message", alertPanel.transform, "", 19, FontStyle.Normal, Ink, TextAnchor.UpperLeft);
            SetRect(alertMessage.rectTransform, 0.035f, 0.36f, 0.965f, 0.72f);
            guidance = CreateText("Guidance", alertPanel.transform, "", 18, FontStyle.Italic, Muted, TextAnchor.UpperLeft);
            SetRect(guidance.rectTransform, 0.035f, 0.05f, 0.965f, 0.39f);
        }

        private void BuildDisclaimer(RectTransform parent)
        {
            var disclaimer = CreateText(
                "Disclaimer",
                parent,
                "Estimativas educativas — não substituem rótulo, diagnóstico ou orientação profissional.",
                18,
                FontStyle.Normal,
                Muted,
                TextAnchor.MiddleCenter);
            SetRect(disclaimer.rectTransform, 0.055f, 0.012f, 0.945f, 0.064f);
        }

        private void ScanNext()
        {
            var profile = NutritionCatalog.GetAt(nextProfileIndex);
            nextProfileIndex = (nextProfileIndex + 1) % NutritionCatalog.All.Count;
            StartCoroutine(ScanRoutine(profile));
        }

        private void Scan(string id)
        {
            StartCoroutine(ScanRoutine(NutritionCatalog.GetById(id)));
        }

        private IEnumerator ScanRoutine(NutritionProfile profile)
        {
            if (scanning)
            {
                yield break;
            }

            scanning = true;
            scanButton.interactable = false;
            scanButtonLabel.text = "ANALISANDO...";
            scanningLabel.SetActive(true);
            scanLine.gameObject.SetActive(true);

            const float duration = 1.25f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var phase = Mathf.PingPong(elapsed * 1.45f, 1f);
                var y = Mathf.Lerp(0.78f, 0.20f, phase);
                scanLine.anchorMin = new Vector2(0.08f, y);
                scanLine.anchorMax = new Vector2(0.92f, y + 0.02f);
                yield return null;
            }

            ShowResult(profile);
            scanLine.gameObject.SetActive(false);
            scanningLabel.SetActive(false);
            scanButtonLabel.text = "ESCANEAR NOVAMENTE";
            scanButton.interactable = true;
            scanning = false;
        }

        private void ShowResult(NutritionProfile profile)
        {
            resultName.text = profile.Name;
            resultPortion.text = profile.Portion;
            calories.text = profile.Calories + " kcal";
            dailyReference.text = "≈ " + Mathf.RoundToInt(profile.Calories / 2000f * 100f) + "% de uma referência de 2.000 kcal";
            carbValue.text = profile.Carbohydrates.ToString("0.#") + " g";
            proteinValue.text = profile.Proteins.ToString("0.#") + " g";
            fatValue.text = profile.Fats.ToString("0.#") + " g";
            carbBar.fillAmount = Mathf.Clamp01(profile.Carbohydrates / 60f);
            proteinBar.fillAmount = Mathf.Clamp01(profile.Proteins / 25f);
            fatBar.fillAmount = Mathf.Clamp01(profile.Fats / 25f);
            alertTitle.text = profile.AlertTitle;
            alertMessage.text = profile.AlertMessage;
            guidance.text = "Sugestão: " + profile.Guidance;

            switch (profile.AlertLevel)
            {
                case HealthAlertLevel.High:
                    alertPanel.color = new Color(0.91f, 0.27f, 0.20f, 0.13f);
                    alertTitle.color = Hex("A7352E");
                    break;
                case HealthAlertLevel.Attention:
                    alertPanel.color = new Color(0.95f, 0.60f, 0.10f, 0.15f);
                    alertTitle.color = Hex("88570B");
                    break;
                default:
                    alertPanel.color = new Color(0.05f, 0.54f, 0.36f, 0.10f);
                    alertTitle.color = PrimaryDark;
                    break;
            }
        }

        private void CreateMacroRow(Transform parent, string label, Color color, float top, out Text value, out Image fill)
        {
            var rowLabel = CreateText(label, parent, label, 18, FontStyle.Bold, Muted, TextAnchor.MiddleLeft);
            SetRect(rowLabel.rectTransform, 0.42f, top - 0.055f, 0.69f, top);
            value = CreateText(label + " Value", parent, "0 g", 20, FontStyle.Bold, Ink, TextAnchor.MiddleRight);
            SetRect(value.rectTransform, 0.78f, top - 0.055f, 0.95f, top);

            var track = CreatePanel(label + " Track", parent, new Color(0.09f, 0.20f, 0.17f, 0.09f), 10f);
            SetRect(track.rectTransform, 0.42f, top - 0.09f, 0.95f, top - 0.064f);
            var fillPanel = CreatePanel(label + " Fill", track.transform, color, 10f);
            Stretch(fillPanel.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            fill = fillPanel;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
        }

        private void CreateEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("Event System", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(transform, false);
        }

        private Button CreateButton(string name, Transform parent, string label, int fontSize, Color background, Color foreground, UnityEngine.Events.UnityAction action)
        {
            var image = CreatePanel(name, parent, background, 26f);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.96f, 1f, 0.98f, 1f);
            colors.pressedColor = new Color(0.82f, 0.92f, 0.87f, 1f);
            colors.disabledColor = new Color(0.72f, 0.76f, 0.74f, 0.65f);
            button.colors = colors;
            button.onClick.AddListener(action);

            var text = CreateText("Label", image.transform, label, fontSize, FontStyle.Bold, foreground, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 4f), new Vector2(-12f, -4f));
            return button;
        }

        private Text CreateText(string name, Transform parent, string value, int size, FontStyle style, Color color, TextAnchor alignment)
        {
            var rect = CreateUIObject(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private Image CreatePanel(string name, Transform parent, Color color, float radius)
        {
            var rect = CreateUIObject(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            if (radius > 0f)
            {
                image.sprite = CreateRoundedSprite(radius);
                image.type = Image.Type.Sliced;
            }
            return image;
        }

        private Sprite CreateRoundedSprite(float radius)
        {
            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "Rounded UI " + radius;
            texture.wrapMode = TextureWrapMode.Clamp;
            var r = Mathf.Clamp(radius, 2f, size * 0.5f);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = Mathf.Max(r - x, x - (size - 1 - r), 0f);
                    var dy = Mathf.Max(r - y, y - (size - 1 - r), 0f);
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var alpha = Mathf.Clamp01(r + 0.5f - distance);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(r, r, r, r));
        }

        private Texture2D CreateGradientTexture(Color top, Color bottom)
        {
            const int height = 128;
            var texture = new Texture2D(2, height, TextureFormat.RGB24, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            for (var y = 0; y < height; y++)
            {
                var color = Color.Lerp(bottom, top, y / (height - 1f));
                texture.SetPixel(0, y, color);
                texture.SetPixel(1, y, color);
            }
            texture.Apply();
            return texture;
        }

        private void AddBorder(RectTransform parent, Color color, float thickness, float radius)
        {
            var top = CreatePanel("Top Border", parent, color, radius);
            SetRect(top.rectTransform, 0f, 0.98f, 1f, 1f, new Vector2(thickness, 0f), new Vector2(-thickness, 0f));
            var bottom = CreatePanel("Bottom Border", parent, color, radius);
            SetRect(bottom.rectTransform, 0f, 0f, 1f, 0.02f, new Vector2(thickness, 0f), new Vector2(-thickness, 0f));
            var left = CreatePanel("Left Border", parent, color, radius);
            SetRect(left.rectTransform, 0f, 0f, 0.02f, 1f, Vector2.zero, Vector2.zero);
            var right = CreatePanel("Right Border", parent, color, radius);
            SetRect(right.rectTransform, 0.98f, 0f, 1f, 1f, Vector2.zero, Vector2.zero);
        }

        private static void AddShadow(GameObject target, Color color, Vector2 distance)
        {
            var shadow = target.AddComponent<Shadow>();
            shadow.effectColor = color;
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private static RectTransform CreateUIObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void SetRect(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            SetRect(rect, minX, minY, maxX, maxY, Vector2.zero, Vector2.zero);
        }

        private static void SetRect(RectTransform rect, float minX, float minY, float maxX, float maxY, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out var color);
            return color;
        }
    }

    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect lastSafeArea;
        private Vector2Int lastScreenSize;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Apply();
        }

        private void Update()
        {
            if (lastSafeArea != Screen.safeArea || lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            {
                Apply();
            }
        }

        private void Apply()
        {
            var safeArea = Screen.safeArea;
            var min = safeArea.position;
            var max = safeArea.position + safeArea.size;
            min.x /= Screen.width;
            min.y /= Screen.height;
            max.x /= Screen.width;
            max.y /= Screen.height;
            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            lastSafeArea = safeArea;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        }
    }
}

