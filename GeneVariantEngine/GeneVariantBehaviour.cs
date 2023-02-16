using GeneticsArtifact;
using RoR2;
using System;
using UnityEngine;
using VAPI;

namespace GeneticVariantsPatch
{
    public class GeneVariantBehaviour
    {
        public BodyIndex bodyIndex;
        internal MasterGeneBehaviour masterGene;
        internal VariantDef variantDef;

        public string variantName;
        public float originalSpawnRate;

        public float fitness;
        public double totalScore;

        public void EvaluateFitness()
        {
            //Split this out because it's ugly math if I don't
            float healthMaster = masterGene.templateGenes[GeneStat.MaxHealth];
            float moveSpeedMaster = masterGene.templateGenes[GeneStat.MoveSpeed];
            float attackSpeedMaster = masterGene.templateGenes[GeneStat.AttackSpeed];
            float attackDamageMaster = masterGene.templateGenes[GeneStat.AttackDamage];

            //Smaller percent of them relative to eachother
            float healthScore = Mathf.Min(healthMaster / variantDef.healthMultiplier, variantDef.healthMultiplier / healthMaster);
            float moveSpeedScore = Mathf.Min(moveSpeedMaster / variantDef.moveSpeedMultiplier, variantDef.moveSpeedMultiplier / moveSpeedMaster);
            float attackSpeedScore = Mathf.Min(attackSpeedMaster / variantDef.attackSpeedMultiplier, variantDef.attackSpeedMultiplier / attackSpeedMaster);
            float attackDamageScore = Mathf.Min(attackDamageMaster / variantDef.damageMultiplier, variantDef.damageMultiplier / attackDamageMaster);

            //New fitness uses a hyperbolic secant centered around 4, since 4 is the max score
            totalScore = healthScore + moveSpeedScore + attackSpeedScore + attackDamageScore;
            fitness = (float)(2 / Math.Pow(Math.Cosh(totalScore - 4), 1.6));
            variantDef.spawnRate = Mathf.Clamp(originalSpawnRate * fitness, originalSpawnRate * (1 - GeneticVariantsPatchPlugin.maxGeneChanceInfluence.Value), originalSpawnRate * (1 + GeneticVariantsPatchPlugin.maxGeneChanceInfluence.Value));
        }

        public void ResetSpawnRate()
        {
            variantDef.spawnRate = originalSpawnRate;
        }
    }
}
