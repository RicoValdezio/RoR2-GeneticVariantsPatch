using GeneticsArtifact;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using VAPI;
using VAPI.Components;

namespace GeneticVariantsPatch
{
    public class GeneVariantDriver : NetworkBehaviour
    {
        public static GeneVariantDriver instance;
        internal static GeneVariantBehaviour[] geneVariantBehaviours;
        public static float timeSinceUpdate;

        #region Hooks
        internal static void RegisterHooks()
        {
            On.RoR2.Run.Start += Run_Start;
            RoR2.Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
            RoR2.Stage.onServerStageBegin += Stage_onServerStageBegin;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private static void Run_Start(On.RoR2.Run.orig_Start orig, RoR2.Run self)
        {
            orig(self);
            timeSinceUpdate = 0f;
            if (NetworkServer.active)
            {
                instance = self.gameObject.AddComponent<GeneVariantDriver>();

                // Nebby changed the name to _registeredVariants, but there's gotta be a better way to read all the variants without regard to their body index
                VariantDef[] catalogVDefs = (VariantDef[])typeof(VariantCatalog).GetField("_registeredVariants", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
                geneVariantBehaviours = new GeneVariantBehaviour[catalogVDefs.Length];

                int index = 0;
                foreach (VariantDef variant in catalogVDefs)
                {
                    BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(variant.bodyName);
                    geneVariantBehaviours[index] = new GeneVariantBehaviour
                    {
                        variantDef = variant,
                        originalSpawnRate = variant.spawnRate,
                        bodyIndex = bodyIndex,
                        variantName = variant.name
                    };
                    index++;
                }
#if DEBUG
                GeneticVariantsPatchPlugin.LogSource.LogInfo("Found " + index + " Variants");
#endif
            }
        }

        private static void Run_onRunDestroyGlobal(Run obj)
        {
            if (NetworkServer.active)
            {
                foreach (GeneVariantBehaviour behaviour in geneVariantBehaviours)
                {
                    behaviour.ResetSpawnRate();
#if DEBUG
                GeneticVariantsPatchPlugin.LogSource.LogMessage("Variant: " + behaviour.variantName + " reset SR " + behaviour.variantDef.spawnRate + ", OSR " + behaviour.originalSpawnRate);
#endif
                }
            }

        }

        private static void Stage_onServerStageBegin(Stage obj)
        {
            if (NetworkServer.active)
            {
                UpdateAllGeneVariants();
            }
        }

        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (NetworkServer.active &&
               GeneticVariantsPatchPlugin.enableGeneBlocking.Value &&
               self.inventory?.GetItemCount(GeneTokens.blockerDef) == 0 &&
               self.GetComponent<BodyVariantManager>()?.variantsInBody?.Count > 0)
            {
                self.inventory.GiveItem(GeneTokens.blockerDef);
#if DEBUG
                GeneticVariantsPatchPlugin.LogSource.LogInfo("Gave GeneBlocker to " + self.name);
#endif
            }
            orig(self);
        }
        #endregion

        private void Update()
        {
            if (!NetworkServer.active) return;
            timeSinceUpdate += Time.deltaTime;
            if (timeSinceUpdate >= 60f)
            {
                UpdateAllGeneVariants();
            }
        }

        public static void UpdateAllGeneVariants()
        {
            if (NetworkServer.active && RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfGenetics.artifactDef))
            {
                foreach (GeneVariantBehaviour behaviour in geneVariantBehaviours)
                {
                    if (behaviour.masterGene != null || GeneEngineDriver.masterGenes.Any(master => master.bodyIndex == behaviour.bodyIndex))
                    {
                        if (behaviour.masterGene == null) behaviour.masterGene = GeneEngineDriver.masterGenes.First(master => master.bodyIndex == behaviour.bodyIndex);
                        if (GeneticVariantsPatchPlugin.enableChanceTweaking.Value)
                        {
                            behaviour.EvaluateFitness();
                        }
#if DEBUG
                        GeneticVariantsPatchPlugin.LogSource.LogMessage("Variant: " + behaviour.variantName + " now has SR " + behaviour.variantDef.spawnRate + " FIT " + behaviour.fitness + " TS  " + behaviour.totalScore + ", OSR " + behaviour.originalSpawnRate);
#endif
                    }
                }

                timeSinceUpdate = 0f;
            }
        }
    }
}