using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ServiceStudio.Presenter {
    public partial class RuntimeImplementation : Runtime {

        private (Lazy<string> UserDirName, Lazy<string> CommonDirName) resourcesDirNames;

        private static string GetApplicationDataDirectory(bool perUser) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return Environment.GetFolderPath(perUser ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData);
            } else {
                // TODO RICT-2216
                /* If the LocalApplicationData doesn't exist, Environment.SpecialFolder.LocalApplicationData returns an empty string.
                 * GetFolderPath receives an option argument which can return the default path, even if it doesn't exist.
                 * The creation of such directory in such case is handled by SS in GetResourcesDirectory().
                 */
                var destinationFolder = perUser ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.ApplicationData;
                return Path.Combine(Environment.GetFolderPath(destinationFolder, Environment.SpecialFolderOption.DoNotVerify));
            }
        }

        public static string GetDefaultResourcesDirectory(bool perUser = true) {
            return "";
        }

        public string GetResourcesDirectory(bool perUser = true) {
            static string GetPathFreeOfSymbolicLinks(string dir, bool perUser) {
                // #RICT-2849 For security reasons, we can't allow Service Studio to open symlinks.
                // This condition applies for both file and directory symbolic links, hard links and directory junctions.
                if ((File.GetAttributes(dir) & FileAttributes.ReparsePoint) == 0) {
                    var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                    if (files.All(f => (File.GetAttributes(f) & FileAttributes.ReparsePoint) == 0)) {
                        return dir;
                    }
                }

                return "";
            }

            var dir = "";

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
                return dir;
            }

            if (perUser) {
                if (resourcesDirNames.UserDirName == null) {
                    resourcesDirNames.UserDirName = new Lazy<string>(() => GetPathFreeOfSymbolicLinks(dir, perUser));
                }
                return resourcesDirNames.UserDirName.Value;
            }

            if (resourcesDirNames.CommonDirName == null) {
                resourcesDirNames.CommonDirName = new Lazy<string>(() => GetPathFreeOfSymbolicLinks(dir, perUser));
            }

            return resourcesDirNames.CommonDirName.Value;
        }
    }
}
