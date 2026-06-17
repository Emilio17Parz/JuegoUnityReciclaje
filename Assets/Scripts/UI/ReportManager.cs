using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReportManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject reportPanel;
    public TMP_InputField reportInputField;
    public TMP_Text statusText;

    private void Start()
    {
        if (reportPanel != null)
            reportPanel.SetActive(false);

        if (statusText != null)
            statusText.text = "";
    }

    public void OpenReportPanel()
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
            reportPanel.transform.SetAsLastSibling();
        }

        if (statusText != null)
            statusText.text = "";
    }

    public void CloseReportPanel()
    {
        Debug.Log("CERRANDO PANEL DE REPORTE");

        if (reportPanel != null)
            reportPanel.SetActive(false);
    }

    public void SubmitReport()
    {
        try
        {
            string reportText = BuildReportText();

            string baseFolder = GetGameFolder();
            string reportsFolder = Path.Combine(baseFolder, "Reports");

            if (!Directory.Exists(reportsFolder))
                Directory.CreateDirectory(reportsFolder);

            string fileName = "reporte_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
            string filePath = Path.Combine(reportsFolder, fileName);

            File.WriteAllText(filePath, reportText);
            GUIUtility.systemCopyBuffer = reportText;

            if (statusText != null)
                statusText.text = "Se subió tu reporte.";

            if (reportInputField != null)
                reportInputField.text = "";

            Debug.Log("Reporte guardado en: " + filePath);
        }
        catch (Exception ex)
        {
            if (statusText != null)
                statusText.text = "Error al guardar reporte.";

            Debug.LogError("Error al guardar reporte: " + ex.Message);
        }
    }

    private string BuildReportText()
    {
        string userComment = reportInputField != null ? reportInputField.text : "";

        return
            "===== REPORTE DE INCIDENCIA =====\n\n" +
            "Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
            "Escena actual: " + SceneManager.GetActiveScene().name + "\n" +
            "Resolución: " + Screen.width + "x" + Screen.height + "\n" +
            "FPS aproximado: " + GetApproxFPS() + "\n" +
            "Tiempo desde inicio de app: " + Time.realtimeSinceStartup.ToString("0.00") + " segundos\n" +
            "TimeScale: " + Time.timeScale + "\n\n" +
            "Sistema operativo: " + SystemInfo.operatingSystem + "\n" +
            "Dispositivo: " + SystemInfo.deviceModel + "\n" +
            "GPU: " + SystemInfo.graphicsDeviceName + "\n" +
            "Versión Unity: " + Application.unityVersion + "\n" +
            "Carpeta del juego: " + GetGameFolder() + "\n\n" +
            "Comentario del jugador:\n" +
            (string.IsNullOrWhiteSpace(userComment) ? "Sin comentario." : userComment) + "\n";
    }

    private string GetGameFolder()
    {
#if UNITY_EDITOR
        return Directory.GetParent(Application.dataPath).FullName;
#else
        return Path.GetDirectoryName(Application.dataPath);
#endif
    }

    private int GetApproxFPS()
    {
        if (Time.unscaledDeltaTime <= 0f)
            return 0;

        return Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
    }
}