using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
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
        public const string ModVer = "0.2.0";
        public const string ModName = "GenVarPatch";
        public const string ModGuid = "com.RicoValdezio.GeneticVariantsPatch";
        public static GeneticVariantsPatchPlugin Instance;
        internal static ManualLogSource LogSource;
        public static ConfigEntry<bool> enableChanceTweaking, enableGeneBlocking;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            LogSource = Instance.Logger;

            enableChanceTweaking = Config.Bind<bool>(new ConfigDefinition("General Settings", "Enable Variant Chance Tweaking"), true, new ConfigDescription("If true, the patch will allow the fitness algorithm to change the chance of variants spawning", new AcceptableValueList<bool>(true, false)));
            enableGeneBlocking = Config.Bind<bool>(new ConfigDefinition("General Settings", "Enable Variant Gene Blocking"), true, new ConfigDescription("If true, the patch will prevent variants from receiving genetic bonus and from participating in the fitness algorithm", new AcceptableValueList<bool>(true, false)));
            GeneVariantDriver.RegisterHooks();
        }
    }
}
