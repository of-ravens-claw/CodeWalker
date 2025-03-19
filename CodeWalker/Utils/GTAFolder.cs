using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using CodeWalker.Properties;
using Microsoft.Win32;
using CodeWalker.GameFiles;
using CodeWalker.Utils;

namespace CodeWalker
{
    public static class GTAFolder
    {
        public static string CurrentGTAFolder { get; private set; } = Settings.Default.GTAFolder;
        public static bool IsGen9 { get; private set; } = Settings.Default.GTAGen9;

        public static bool IsGen9Folder(string folder)
        {
            return File.Exists(folder + @"\gta5_enhanced.exe");
        }

        public static bool ValidateGTAFolder(string folder, bool gen9, out string failReason)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                failReason = "No folder specified";
                return false;
            }

            if (!Directory.Exists(folder))
            {
                failReason = $"Folder \"{folder}\" does not exist";
                return false;
            }

            failReason = "";
	        return true;
        }

        public static bool ValidateGTAFolder(string folder, bool gen9) => ValidateGTAFolder(folder, gen9, out string reason);

        public static bool IsCurrentGTAFolderValid() => ValidateGTAFolder(CurrentGTAFolder, IsGen9);

        public static bool UpdateGTAFolder(bool useCurrentIfValid = false, bool autoDetect = true)
        {
            if (useCurrentIfValid && IsCurrentGTAFolderValid())
            {
                return true;
            }

            var gen9 = IsGen9;
            string origFolder = CurrentGTAFolder;
            string folder = CurrentGTAFolder;
            SelectFolderForm f = new SelectFolderForm();

            f.ShowDialog();
            if (f.Result == DialogResult.Cancel)
            {
                return false;
            }
            if (f.Result == DialogResult.OK && Directory.Exists(f.SelectedFolder))
            {
                folder = f.SelectedFolder;
                gen9 = f.IsGen9;
            }

            if (ValidateGTAFolder(folder, out var failReason))
            {
                SetGTAFolder(folder);
                if (folder != origFolder)
                {
                    MessageBox.Show($"Successfully changed GTA Folder to \"{folder}\"", "Set GTA Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }

            var tryAgain = MessageBox.Show($"Folder \"{folder}\" is not a valid GTA folder:\n\n{failReason}\n\nDo you want to try choosing a different folder?", "Unable to set GTA Folder", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (tryAgain == DialogResult.Retry)
            {
	            return UpdateGTAFolder(false);
            } 
                
            return false;
        }

        public static bool SetGTAFolder(string folder, bool gen9)
        {
            if(ValidateGTAFolder(folder, gen9))
            {
                CurrentGTAFolder = folder;
                IsGen9 = gen9;
                Settings.Default.GTAFolder = folder;
                Settings.Default.GTAGen9 = gen9;
                Settings.Default.Save();
                return true;
            }

            return false;
        }

        public static string GetCurrentGTAFolderWithTrailingSlash() => CurrentGTAFolder.EndsWith(@"\") ? CurrentGTAFolder : CurrentGTAFolder + @"\";
    }
}
