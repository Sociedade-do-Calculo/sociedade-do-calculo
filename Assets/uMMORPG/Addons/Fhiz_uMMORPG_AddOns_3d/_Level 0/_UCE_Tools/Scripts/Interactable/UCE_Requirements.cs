// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/Fhizban
// =======================================================================================
using System;
using System.Linq;
using UnityEngine;
using System.Globalization;

// =======================================================================================
// REQUIREMENTS CLASS
// BASE CLASS FOR ANY KIND OF REQUIREMENTS - can be used for all kinds of requirement checks
// =======================================================================================
[System.Serializable]
public partial class UCE_Requirements
{
    [Header("[-=-=-=- UCE REQUIREMENTS [Checked before interaction allowed]-=-=-=-]")]
    
    [Tooltip("[Optional] One click deactivation (ignores all requirements)")]
    public bool isActive = true;

    [Header("[STATUS & LEVEL]")]
    [Tooltip("[Optional] Player must be alive (health > 0)?")]
    public bool aliveOnly = true;

    public enum StateType { Any, Idle, Casting, Moving, IdleOrCasting, IdleOrMoving, IdleOrCastingOrMoving, Dead }

    [Tooltip("[Optional] Player state limit?")]
    public StateType stateType;

    [Tooltip("[Optional] Minimum player level to interact (0 to disable)")]
    public int minLevel = 0;

    [Tooltip("[Optional] Maximum player level to interact (0 to disable)")]
    public int maxLevel = 0;

    [Tooltip("[Optional] Health has to be equal or higher (0 to disable, 0.5=50%, 1.0=100%)")]
    [Range(0, 1)] public float minHealth;

    [Tooltip("[Optional] Mana has to be equal or higher (0 to disable, 0.5=50%, 1.0=100%)")]
    [Range(0, 1)] public float minMana;

	[Header("[TIME]")]
	[Tooltip("[Optional] To access, must be this day or later in the month (0 to disable)"), Range(0, 31)]
    public int dayStart = 0;
    [Tooltip("[Optional] To access, must be this day or earlier in the month (0 to disable)"), Range(0, 31)]
    public int dayEnd = 0;
    [Tooltip("[Optional]  To access, must be this month (0 to disable)"), Range(0, 12)]
    public int activeMonth = 0;

    [Header("[SKILLS & BUFFS]")]
    [Tooltip("[Optional] Required Skills")]
    public UCE_SkillRequirement[] requiredSkills;

    [Tooltip("[Optional] All Skills are required (otherwise any one of them is enough)")]
    public bool requiresAllSkills;

    [Tooltip("[Optional] Required active Buff (this buff must be active)")]
    public BuffSkill requiredBuff;

    [Tooltip("[Optional] Prohibited active Buff (this buff may not be active)")]
    public BuffSkill prohibitedBuff;

    [Header("[CLASSES, PARTY & GUILD]")]
    [Tooltip("[Optional] Allowed Classes (Player Prefab)")]
    public GameObject[] allowedClasses;

    [Tooltip("[Optional] Must be in a party (any) ?")]
    public bool requiresParty = false;

    [Tooltip("[Optional] Must be in a guild (any) ?")]
    public bool requiresGuild = false;

    [Header("[EQUIPMENT ITEMS]")]
    [Tooltip("[Optional] This item must be in the players equipment")]
    public EquipmentItem[] requiredEquipment;

    [Tooltip("[Optional] All Items are required (otherwise any one of them is enough)")]
    public bool requiresAllEquipment;

    [Header("[INVENTORY ITEMS]")]
    [Tooltip("[Optional] This item must be in the players inventory")]
    public UCE_ItemRequirement[] requiredItems;

    [Tooltip("[Optional] All Equipment items are required (otherwise any one of them is enough)")]
    public bool requiresAllItems;

#if _FHIZPRESTIGECLASSES
    [Header("[UCE PRESTIGE CLASSES REQUIREMENTS]")]
    public UCE_PrestigeClassTemplate[] allowedPrestigeClasses;
#endif

#if _FHIZATTRIBUTES
    [Header("[UCE ATTRIBUTES REQUIREMENTS]")]
    public UCE_AttributeRequirement[] requiredAttributes;
    public bool requiresAllAttributes;
#endif

#if _FHIZPVP

    public enum LootableBy { MyRealmAndAllies, NotMyRealmAndAllies }

    [Header("[UCE PVP REQUIREMENTS]")]
    [Tooltip("[Optional] Allowed Realms")]
    public UCE_Tmpl_Realm requiredRealm;
    public UCE_Tmpl_Realm requiredAlly;

    [Tooltip("[Optional] Accessible by all, allied or enemy Realms?")]
    public LootableBy lootableBy;
#endif

    [Header("[UCE QUEST REQUIREMENTS]")]
    [Tooltip("[Optional] This quest must be completed first")]
#if _FHIZQUESTS
    public UCE_ScriptableQuest requiredQuest;
    public bool questMustBeInProgress;
#else
	public ScriptableQuest requiredQuest;
#endif

#if _FHIZFACTIONS
    [Header("[UCE FACTIONS REQUIREMENTS]")]
    [Tooltip("[Optional] Faction Requirements")]
    public UCE_FactionRequirement[] factionRequirements;
    public bool requiresAllFactionRatings;
#endif

#if _FHIZHARVESTING
    [Header("[UCE HARVESTING REQUIREMENTS]")]
    public UCE_HarvestingProfessionRequirement[] harvestProfessionRequirements;
    public bool requiresAllHarvestingProfessions;
#endif

#if _FHIZCRAFTING
    [Header("[UCE CRAFTING REQUIREMENTS]")]
    public UCE_CraftingProfessionRequirement[] craftProfessionRequirements;
    public bool requiresAllCraftingProfessions;
#endif

#if _FHIZMOUNTS

    public enum MountType { Both, Unmounted, Mounted };

    [Header("[UCE MOUNTS REQUIREMENTS]")]
    [Tooltip("[Optional] Mounts - interactable while mounted or not?")]
    public MountType mountType;
#endif

#if _FHIZTRAVEL
	[Header("[UCE TRAVELROUTE REQUIREMENTS]")]
	public string requiredTravelrouteName;
#endif

#if _FHIZWORLDEVENTS
	[Header("[UCE WORLD EVENT REQUIREMENTS]")]
	[Tooltip("[Optional] This world event will be checked")]
	public UCE_WorldEventTemplate worldEvent;
	[Tooltip("[Optional] Min count the world event has been completed (0 to disable)")]
	public int minEventCount;
	[Tooltip("[Optional] Max count the world event has been completed (0 to disable)")]
	public int maxEventCount;
#endif

#if _FHIZGUILDUPGRADES
	[Header("[UCE GUILD UPGRADES]")]
	[Tooltip("[Optional] Players guild has to be of this level (player has to be in a guild)")]
	public int minGuildLevel;
#endif

#if _FHIZACCOUNTUNLOCKABLES
	[Header("[UCE ACCOUNT UNLOCKABLES]")]
	[Tooltip("[Optional] This must be unlocked on the account (empty to disable)")]
	public string accountUnlockable;
#endif

    // -----------------------------------------------------------------------------------
    // checkRequirements
    // Runs a full check to see if all interaction requirements are met by the player
    // -----------------------------------------------------------------------------------
    public virtual bool checkRequirements(Player player)
    {
        if (!isActive) return true;
        if (player == null) return false;

        if (!hasRequirements()) return true;

        bool valid = true;

        valid = (!aliveOnly || aliveOnly && player.isAlive) ? valid : false;
        valid = checkState(player) ? valid : false;
        valid = (minLevel == 0 || player.level >= minLevel) ? valid : false;
        valid = (maxLevel == 0 || player.level <= maxLevel) ? valid : false;
        valid = (minHealth == 0 || player.HealthPercent() >= minHealth) ? valid : false;
        valid = (minMana == 0 || player.ManaPercent() >= minMana) ? valid : false;
        
        //TIME
        valid = (dayStart == 0 || dayStart <= DateTime.UtcNow.Day) ? valid : false;
        valid = (dayEnd == 0 || dayEnd >= DateTime.UtcNow.Day) ? valid : false;
        valid = (activeMonth == 0 || activeMonth == DateTime.UtcNow.Month) ? valid : false;

        valid = (requiredSkills.Length == 0 || player.UCE_checkHasSkills(requiredSkills, requiresAllSkills)) ? valid : false;
        valid = (requiredBuff == null || player.UCE_checkHasBuff(requiredBuff)) ? valid : false;
        valid = (prohibitedBuff == null || !player.UCE_checkHasBuff(prohibitedBuff)) ? valid : false;
        valid = (allowedClasses.Length == 0 || player.UCE_checkHasClass(allowedClasses)) ? valid : false;
        valid = (!requiresParty || player.InParty()) ? valid : false;
        valid = (!requiresGuild || player.InGuild()) ? valid : false;
        valid = (requiredEquipment.Length == 0 || player.UCE_checkHasEquipment(requiredEquipment)) ? valid : false;
        valid = (requiredItems.Length == 0 || player.UCE_checkHasItems(requiredItems)) ? valid : false;
#if _FHIZPRESTIGECLASSES
        valid = player.UCE_CheckPrestigeClass(allowedPrestigeClasses) ? valid : false;
#endif
#if _FHIZATTRIBUTES
        valid = checkAttributes(player) ? valid : false;
#endif
#if _FHIZPVP
        valid = (checkRealm(player)) ? valid : false;
#endif
#if _FHIZQUESTS
        if (!questMustBeInProgress)
        {
            valid = (requiredQuest == null || player.UCE_HasCompletedQuest(requiredQuest.name)) ? valid : false;
        }
        else
        {
            valid = (requiredQuest == null || player.UCE_HasActiveQuest(requiredQuest.name)) ? valid : false;
        }
#else
		valid = (requiredQuest == null || player.HasCompletedQuest(requiredQuest.name)) ? valid : false;
#endif
#if _FHIZFACTIONS
        valid = player.UCE_CheckFactionRatings(factionRequirements, requiresAllFactionRatings) ? valid : false;
#endif
#if _FHIZHARVESTING
        valid = player.HasHarvestingProfessions(harvestProfessionRequirements, requiresAllHarvestingProfessions) ? valid : false;
#endif
#if _FHIZCRAFTING
        valid = player.UCE_HasCraftingProfessions(craftProfessionRequirements, requiresAllCraftingProfessions) ? valid : false;
#endif
#if _FHIZMOUNTS
        valid = mountType == MountType.Both || (mountType == MountType.Unmounted && !player.UCE_mounted) || (mountType == MountType.Mounted && player.UCE_mounted) ? valid : false;
#endif

#if _FHIZTRAVEL
		valid = string.IsNullOrWhiteSpace(requiredTravelrouteName) || (!string.IsNullOrWhiteSpace(requiredTravelrouteName) && player.UCE_travelroutes.Any(t => t.name == requiredTravelrouteName)) ? valid : false;
#endif

#if _FHIZWORLDEVENTS
		valid = player.UCE_CheckWorldEvent(worldEvent, minEventCount, maxEventCount) ? valid : false;
#endif

#if _FHIZGUILDUPGRADES
		if (minGuildLevel > 0)
			valid = player.InGuild() && player.guildLevel >= minGuildLevel ? valid : false;
#endif

#if _FHIZACCOUNTUNLOCKABLES
		if (!string.IsNullOrWhiteSpace(accountUnlockable))
			valid = player.UCE_HasAccountUnlock(accountUnlockable) ? valid : false;
#endif

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // checkState
    // -----------------------------------------------------------------------------------
    public bool checkState(Player player)
    {
        return
                (stateType == StateType.Any) ||
                (stateType == StateType.Idle && player.State == "IDLE" || player.State == "MOVING") ||
                (stateType == StateType.Casting && player.State == "CASTING") ||
                (stateType == StateType.Moving && player.State == "MOVING") ||
                (stateType == StateType.IdleOrCasting && (player.State == "IDLE" || player.State == "CASTING")) ||
                (stateType == StateType.IdleOrMoving && (player.State == "IDLE" || player.State == "MOVING")) ||
                (stateType == StateType.IdleOrCastingOrMoving && (player.State == "IDLE" || player.State == "CASTING" || player.State == "MOVING")) ||
                (stateType == StateType.Dead && player.State == "DEAD");
    }

    // -----------------------------------------------------------------------------------
    // checkAttributes
    // -----------------------------------------------------------------------------------
    protected bool checkAttributes(Player player)
    {
#if _FHIZATTRIBUTES
        if (requiredAttributes.Length <= 0) return true;

        bool success = false;

        foreach (UCE_AttributeRequirement requirement in requiredAttributes)
        {
            if (requirement.template != null &&
                (requirement.minValue <= 0 || player.UCE_Attributes.FirstOrDefault(x => x.template == requirement.template).points >= requirement.minValue) &&
                (requirement.maxValue <= 0 || player.UCE_Attributes.FirstOrDefault(x => x.template == requirement.template).points <= requirement.maxValue)
                )
            {
                if (!requiresAllAttributes) return true;
                success = true;
            }
            else
            {
                success = false;
            }
        }

        return success;
#else
		return true;
#endif
    }

    // -----------------------------------------------------------------------------------
    // checkRealm
    // -----------------------------------------------------------------------------------
    public bool checkRealm(Player player)
    {
#if _FHIZPVP
        if ((requiredRealm == null && requiredAlly == null) || (player.Realm == null && player.Ally == null))
            return true;

        bool bValid = false;

        if (requiredRealm == player.Realm || requiredAlly == player.Ally || requiredRealm == player.Ally || requiredAlly == player.Realm)
            bValid = true;

        if (lootableBy == LootableBy.MyRealmAndAllies)
            return bValid;

        if (lootableBy == LootableBy.NotMyRealmAndAllies)
            return !bValid;

        return bValid;
#else
		return true;
#endif
    }

    // -----------------------------------------------------------------------------------
    // hasRequirements
    // -----------------------------------------------------------------------------------
    public bool hasRequirements()
    {
        if (!isActive) return false;

        return
                aliveOnly ||
                stateType != StateType.Any ||
                minLevel > 0 ||
                maxLevel > 0 ||
                minHealth > 0 ||
                minMana > 0 ||
                dayStart > 0 ||
                dayEnd > 0 ||
                activeMonth > 0 ||
                requiredSkills.Length > 0 ||
                requiredBuff != null ||
                prohibitedBuff != null ||
                allowedClasses.Length > 0 ||
                requiresParty ||
                requiresGuild ||
                requiredItems.Length > 0 ||
                requiredEquipment.Length > 0
#if _FHIZATTRIBUTES
                || requiredAttributes.Length > 0
#endif
#if _FHIZPVP
                || requiredRealm != null ||
                requiredAlly != null
#endif
#if _FHIZQUESTS
                || requiredQuest != null
#endif
#if _FHIZFACTIONS
                || factionRequirements.Length > 0
#endif
#if _FHIZTRAVEL
				|| !string.IsNullOrWhiteSpace(requiredTravelrouteName)
#endif
#if _FHIZWORLDEVENTS
				|| worldEvent != null && (minEventCount != 0 || maxEventCount != 0)
#endif
#if _FHIZGUILDUPGRADES
				|| minGuildLevel > 0
#endif
                ;
    }

    // -----------------------------------------------------------------------------------
}