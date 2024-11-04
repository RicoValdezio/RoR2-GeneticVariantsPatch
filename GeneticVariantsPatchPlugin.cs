using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API.Utils;

namespace GeneticVariantsPatch
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency("com.RicoValdezio.ArtifactOfGenetics", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.Nebby.VAPI", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class GeneticVariantsPatchPlugin : BaseUnityPlugin
    {
        public const string ModVer = "0.4.0";
        public const string ModName = "GenVarPatch";
        public const string ModGuid = "com.RicoValdezio.GeneticVariantsPatch";
        public static GeneticVariantsPatchPlugin Instance;
        internal static ManualLogSource LogSource;
        public static ConfigEntry<bool> enableChanceTweaking, enableGeneBlocking;
        public static ConfigEntry<float> maxGeneChanceInfluence;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            LogSource = Instance.Logger;

            enableChanceTweaking = Config.Bind<bool>(new ConfigDefinition("General Settings", "Enable Variant Chance Tweaking"), true, new ConfigDescription("If true, the patch will allow the fitness algorithm to change the chance of variants spawning", new AcceptableValueList<bool>(true, false)));
            enableGeneBlocking = Config.Bind<bool>(new ConfigDefinition("General Settings", "Enable Variant Gene Blocking"), true, new ConfigDescription("If true, the patch will prevent variants from receiving genetic bonus and from participating in the fitness algorithm", new AcceptableValueList<bool>(true, false)));
            maxGeneChanceInfluence = Config.Bind<float>(new ConfigDefinition("General Settings", "Maximum Gene Chance Effect"), 0.5f, new ConfigDescription("The max difference percentage (as a decimal) that the algorithm can change the variant spawn rate - 0.5 is ±50%"));
            GeneVariantDriver.RegisterHooks();
        }
    }
}
