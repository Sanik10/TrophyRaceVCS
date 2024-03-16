using UnityEditor;
using UnityEngine;

public class MultipleCurveEditor : EditorWindow
{
    public RealisticEngineSound res;
    public Color[] colors = { Color.white, Color.green, Color.yellow, Color.red, Color.blue, Color.black };
    public int width = 700;
    public int height = 350;
    public float soundTransitionLine = 60f;
    public float soundVolumeLine = 60f;
    public float angle = 64.4f;
    public float margin = 20f;

    [MenuItem("Window/Engine Sound Tuner")]
    public static void ShowWindow()
    {
        GetWindow<MultipleCurveEditor>("Multiple Curve Editor");
    }

    private void OnGUI()
    {
        // GUILayout.Space(20f);
        // GUILayout.Label("Combined Curve", EditorStyles.boldLabel);

        // Расстояние между полем для назначения объекта и графиком
        GUILayout.Space(height);
        // Поле для назначения объекта RealisticEngineSound
        res = EditorGUILayout.ObjectField("Realistic Engine Sound", res, typeof(RealisticEngineSound), true) as RealisticEngineSound;
        // Поля для ввода чисел
        soundVolumeLine = EditorGUILayout.FloatField("Линия звука", soundVolumeLine);
        soundTransitionLine = EditorGUILayout.FloatField("Линия переходов", soundTransitionLine);
        angle = EditorGUILayout.FloatField("Наклон", angle);
        

        // Проверяем, был ли назначен объект RealisticEngineSound
        if (res == null)
        {
            EditorGUILayout.LabelField("Assign a RealisticEngineSound script in the inspector.");
            return;
        }

        // Определяем минимальное и максимальное значения для оси X и Y
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;

        // Определяем минимальное и максимальное значения для каждой из кривых
        for (int i = 0; i < 5; i++)
        {
            AnimationCurve curve = GetCurveByIndex(i);

            foreach (Keyframe keyframe in curve.keys)
            {
                minX = Mathf.Min(minX, keyframe.time); // Для оси X используем time
                maxX = Mathf.Max(maxX, keyframe.time); // Для оси X используем time
                // maxX = 1;
                minY = Mathf.Min(minY, keyframe.value); // Для оси Y используем value
                maxY = Mathf.Max(maxY, keyframe.value); // Для оси Y используем value
            }
        }

        // Рисуем оси координат
        DrawAxes(minX, maxX, minY, maxY);

        // Рисуем кривые
        for (int i = 0; i < 5; i++)
        {
            AnimationCurve curve = GetCurveByIndex(i);
            DrawCurve(curve, colors[i], minX, maxX, maxY, minY);
        }
    }

    private AnimationCurve GetCurveByIndex(int index)
    {
        switch (index)
        {
            case 0: return res.idleVolCurve;
            case 1: return res.lowVolCurve;
            case 2: return res.medVolCurve;
            case 3: return res.highVolCurve;
            case 4: return res.maxRPMVolCurve;
            default: return null;
        }
    }

    private void DrawAxes(float minX, float maxX, float minY, float maxY)
    {
        // Рисуем ось X
        Handles.DrawAAPolyLine(new Color[] { Color.black, Color.black }, new Vector3[] { new Vector3(margin, height - margin), new Vector3(width - margin, height - margin) });

        // Рисуем ось Y
        Handles.DrawAAPolyLine(new Color[] { Color.black, Color.black }, new Vector3[] { new Vector3(margin, height - margin), new Vector3(margin, margin) });

        // Наносим метки на оси X
        // Наносим метки на оси X
        DrawEngineRPMLabels(height - margin/2, (int)res.maxRPMLimit);
        // Handles.Label(new Vector3(width - margin, height - margin - 5f, 0f), maxX.ToString());
        // Handles.Label(new Vector3(margin - 15f, height - margin - 5f, 0f), minX.ToString());

        // Наносим метки на оси Y
        // Handles.Label(new Vector3(margin - 15f, height - margin/2, 0f), minY.ToString());
        Handles.Label(new Vector3(margin - 15f, margin/2, 0f), maxY.ToString());

        // Дополнительные линии
        DrawAdditionalLines(minX, maxX, minY, maxY);
    }

    private void DrawEngineRPMLabels(float yPos, int maxRPMLimit)
    {
        // Определяем количество меток на основе максимального числа оборотов двигателя
        int labelCount = Mathf.CeilToInt(maxRPMLimit / 1000f) + 1;

        // Рисуем метки
        for (int i = 0; i < labelCount; i++)
        {
            // Определяем значение оборотов для текущей метки
            int rpmValue = i*1000;

            // Если это последняя метка, используем максимальное значение оборотов
            if (i == labelCount - 1)
                rpmValue = maxRPMLimit;

            // Вычисляем позицию X для текущей метки
            float labelXPos = (float)rpmValue / maxRPMLimit * (width - 2 * margin) + margin;

            // Наносим метку
            Handles.Label(new Vector3(labelXPos, yPos, 0f), rpmValue.ToString());
        }
    }

    private void DrawAdditionalLines(float minX, float maxX, float minY, float maxY)
    {
        // Расстояния от 0 координаты, где будут находиться дополнительные линии по вертикали
        float[] distancesX = { 0.195f, 0.46f, 0.82f }; // 0.22

        // Расстояния от 0 координаты, где будут находиться дополнительные линии по горизонтали
        float[] distancesY = { 0.525f, 0.60f, 0.675f};

        // Расстояния от 0 координаты, где будут находиться дополнительные задаваемые линии по горизонтали
        float[] distancesYcustom = { soundTransitionLine / 100, soundVolumeLine / 100 };

        // Установим цвет линий
        Handles.color = Color.gray;

        // Рисуем дополнительные вертикальные линии
        foreach (float distance in distancesX)
        {
            // Вычисляем позицию по оси X
            float xPos = Mathf.Lerp(margin, width - margin, Mathf.InverseLerp(minX, maxX, distance));

            // Рисуем линию
            Handles.DrawAAPolyLine(new Vector3[] { new Vector3(xPos, margin), new Vector3(xPos, height - margin) });
        }

        // Рисуем дополнительные горизонтальные линии с наклоном
        foreach (float distance in distancesY)
        {
            // Вычисляем позицию по оси Y
            float yPos = Mathf.Lerp(margin, height - margin, Mathf.InverseLerp(maxY, minY, distance));

            // Наклон линии (значение по X влияет на наклон)
            float slope = 64f; // Наклон линии

            // Рисуем наклонную линию
            Vector3 startPoint = new Vector3(margin, yPos);
            Vector3 endPoint = new Vector3(width - margin, yPos);
            DrawSlopedLine(startPoint, endPoint, Color.gray, 2, slope);
        }

        Handles.color = Color.cyan;

        // Рисуем дополнительные горизонтальные линии
        foreach (float distance in distancesYcustom)
        {
            // Вычисляем позицию по оси Y
            float yPos = Mathf.Lerp(margin, height - margin, Mathf.InverseLerp(maxY, minY, distance));

            // Рисуем линию
            Vector3 startPoint = new Vector3(margin, yPos);
            Vector3 endPoint = new Vector3(width - margin, yPos);
            DrawSlopedLine(startPoint, endPoint, Color.cyan, 2, angle);
            // Handles.DrawAAPolyLine(new Vector3[] { new Vector3(margin, yPos), new Vector3(width - margin, yPos) });
        }
    }

    private void DrawSlopedLine(Vector3 startPoint, Vector3 endPoint, Color color, float thickness, float slope)
    {
        Handles.color = color;

        // Вычисляем вектор направления и нормализуем его
        Vector3 direction = (endPoint - startPoint).normalized;

        // Вычисляем вектор наклона
        Vector3 slopeVector = new Vector3(0, slope, 0); // Наклон только по оси Y

        // Сдвигаем начальную и конечную точки линии на вектор наклона
        Vector3 p1 = startPoint + slopeVector / 2;
        Vector3 p2 = endPoint - slopeVector / 2;

        // Рисуем линию с заданными точками
        Handles.DrawAAPolyLine(new Vector3[] { p1, p2 });
    }

    private void DrawCurve(AnimationCurve curve, Color color, float minX, float maxX, float minY, float maxY)
    {
        Vector3[] points = new Vector3[100];
        Color[] colors = new Color[100]; // Массив цветов

        for (int i = 0; i < 100; i++)
        {
            float x = Mathf.Lerp(minX, maxX, (float)i / 99);
            float y = curve.Evaluate(x);
            points[i] = new Vector3(Mathf.Lerp(margin, width - margin, Mathf.InverseLerp(minX, maxX, x)), Mathf.Lerp(margin, height - margin, Mathf.InverseLerp(minY, maxY, y)));
            colors[i] = color; // Заполняем массив цветов одним цветом
        }

        // Рисуем график кривой с массивом цветов
        Handles.DrawAAPolyLine(colors, points);
    }
}