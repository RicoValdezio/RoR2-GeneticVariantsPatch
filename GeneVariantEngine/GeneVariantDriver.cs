using GeneticsArtifact;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VarianceAPI.Components;
using VarianceAPI.ScriptableObjects;
using System.Linq;

namespace GeneticVariantsPatch
{
    public class GeneVariantDriver : NetworkBehaviour
    {
        public static GeneVariantDriver instance;
        internal static List<GeneVariantBehaviour> geneVariantBehaviours;
        public static float timeSinceUpdate;

        #region Hooks
        internal static void RegisterHooks()
        {
            On.RoR2.Run.Start += Run_Start;
            RoR2.Stage.onServerStageBegin += Stage_onServerStageBegin;
            RoR2.Run.onServerGameOver += Run_onServerGameOver; ;
        }

        private static void Run_Start(On.RoR2.Run.orig_Start orig, RoR2.Run self)
        {
            orig(self);
            geneVariantBehaviours = new List<GeneVariantBehaviour>();
            timeSinceUpdate = 0f;
            if (NetworkServer.active)
            {
                instance = self.gameObject.AddComponent<GeneVariantDriver>();
                foreach (GameObject bodyPrefab in BodyCatalog.allBodyPrefabs)
                {
                    VariantSpawnHandler variantSpawnHandler = bodyPrefab.GetComponent<VariantSpawnHandler>();
                    if (variantSpawnHandler != null && variantSpawnHandler.variantInfos.Length > 0)
                    {
                        BodyIndex foundBodyIndex = BodyCatalog.FindBodyIndex(variantSpawnHandler.variantInfos[0].bodyName);
                        foreach (VariantInfo variant in variantSpawnHandler.variantInfos)
                        {
                            geneVariantBehaviours.Add(new GeneVariantBehaviour
                            {
                                bodyIndex = foundBodyIndex,
                                spawnHandler = variantSpawnHandler,
                                variantName = variant.name,
                                baseSpawnRate = variant.spawnRate,
                                healthMult = variant.healthMultiplier,
                                moveSpeedMult = variant.moveSpeedMultiplier,
                                attackSpeedMult = variant.attackSpeedMultiplier,
                                attackDamageMult = variant.damageMultiplier
                            });
                        }
                    }
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

        private static void Run_onServerGameOver(Run arg1, GameEndingDef arg2)
        {
            foreach (GeneVariantBehaviour behaviour in geneVariantBehaviours)
            {
                behaviour.RestoreBaseSpawnRate();
            }
        }
        #endregion

        private void Update()
        {
            if (!NetworkServer.active) return;
            timeSinceUpdate += Time.deltaTime;
            if(timeSinceUpdate >= 60f)
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
                        behaviour.EvaluateFitness();
                        behaviour.UpdateSpawnHandlerRate();
                    }
                }
                timeSinceUpdate = 0f;
            }
        }
    }
}