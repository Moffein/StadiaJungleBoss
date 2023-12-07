﻿using RoR2;
using System.Linq;

namespace StadiaJungleBoss.Setup
{
    internal static class Tokens
    {
        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(StadiaJungleBossPlugin.pluginInfo.Location);
            }
        }
        internal static string languageRoot => System.IO.Path.Combine(assemblyDir, "language");

        internal static void LoadLanguage()
        {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}
