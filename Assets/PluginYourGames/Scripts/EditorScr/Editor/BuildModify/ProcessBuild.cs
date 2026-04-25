namespace YG.EditorScr.BuildModify
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    public class ProcessBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public static string BuildPath { get; private set; } = string.Empty;
        public int callbackOrder => -1000;
        public void OnPreprocessBuild(BuildReport report)
        {
            int.TryParse(BuildLog.ReadProperty("Build number"), out int buildNumInt);
            buildNumInt += 1;
            YG2.infoYG.Basic.buildNumber = buildNumInt;

            BuildPath = report.summary.outputPath;
#if PLATFORM_WEBGL
            if (!string.IsNullOrEmpty(BuildPath))
            {
                DeleteIfFileExist($"{BuildPath}/index.html");
                DeleteIfFileExist($"{BuildPath}/style.css");
            }
#endif
            if (YG2.infoYG.Basic.platform != null && YG2.infoYG.Basic.autoApplySettings)
                InfoYG.Inst().Basic.platform.ApplyProjectSettings();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            ModifyBuild.ModifyIndex();

            if (report.summary.platform != BuildTarget.WebGL) return;

            string buildPath = report.summary.outputPath;
            string[] loaderFiles = Directory.GetFiles(buildPath, "*.loader.js", SearchOption.AllDirectories);

            if (loaderFiles.Length == 0)
            {
                return;
            }

            string loaderPath = loaderFiles[0];
            string content = File.ReadAllText(loaderPath);
            string newContent = content.Replace("alert", "console.warn");

            if (content != newContent)
            {
                File.WriteAllText(loaderPath, newContent);
            }

            if (YG2.infoYG.Basic.archivingBuild)
                ArchivingBuild.Archiving(BuildPath);

            BuildLog.WritingLog();
        }

#if PLATFORM_WEBGL
        private void DeleteIfFileExist(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
#endif
    }
}