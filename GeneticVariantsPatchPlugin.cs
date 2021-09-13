using BepInEx;
using BepInEx.Logging;
using R2API.Utils;

namespace GeneticVariantsPatch
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.RicoValdezio.ArtifactOfGenetics", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.Nebby.VarianceAPI", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticVariantsPatchPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.0.0";
        public const string ModName = "GenVarPatch";
        public const string ModGuid = "com.RicoValdezio.GeneticVariantsPatch";
        public static GeneticVariantsPatchPlugin Instance;
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            LogSource = Instance.Logger;

            GeneVariantDriver.RegisterHooks();
        }
    }
}
