using RoR2;
using GeneticsArtifact;
using VarianceAPI.Components;
using System.Linq;
using UnityEngine;

namespace GeneticVariantsPatch
{
    public class GeneVariantBehaviour
    {
        public BodyIndex bodyIndex;
        internal MasterGeneBehaviour masterGene;
        internal VariantSpawnHandler spawnHandler;

        public string variantName;
        public float baseSpawnRate, healthMult, moveSpeedMult, attackSpeedMult, attackDamageMult;

        public float currSpawnRate = 0f, fitness;

        public void EvaluateFitness()
        {
            //Split this out because it's ugly math if I don't
            float healthMaster = masterGene.templateGenes[GeneStat.MaxHealth];
            float moveSpeedMaster = masterGene.templateGenes[GeneStat.MoveSpeed];
            float attackSpeedMaster = masterGene.templateGenes[GeneStat.AttackSpeed];
            float attackDamageMaster = masterGene.templateGenes[GeneStat.AttackDamage];

            //Smaller percent of them relative to eachother
            float healthScore = Mathf.Min(healthMaster / healthMult, healthMult / healthMaster);
            float moveSpeedScore = Mathf.Min(moveSpeedMaster / moveSpeedMult, moveSpeedMult / moveSpeedMaster);
            float attackSpeedScore = Mathf.Min(attackSpeedMaster / attackSpeedMult, attackSpeedMult / attackSpeedMaster);
            float attackDamageScore = Mathf.Min(attackDamageMaster / attackDamageMult, attackDamageMult / attackDamageMaster);

            //Potential maximum fitness of 2 if each stat is a perfect match, even at a 50% each it's still a fitness of 1
            fitness = Mathf.Clamp((healthScore + moveSpeedScore + attackSpeedScore + attackDamageScore) / 2, 0.5f, 2f);
        }

        public void UpdateSpawnHandlerRate()
        {
            float prevSpawnRate = currSpawnRate;
            currSpawnRate = Mathf.Clamp(baseSpawnRate * fitness, 0, 100);
#if DEBUG
            GeneticVariantsPatchPlugin.LogSource.LogInfo("Variant " + variantName + "'s Spawn Rate was: " + prevSpawnRate + " and is now: " + currSpawnRate);
#endif
            spawnHandler.variantInfos.First(variant => variant.name.Equals(variantName)).spawnRate = currSpawnRate;
        }

        public void RestoreBaseSpawnRate()
        {
            spawnHandler.variantInfos.First(variant => variant.name.Equals(variantName)).spawnRate = baseSpawnRate;
        }
    }
}
