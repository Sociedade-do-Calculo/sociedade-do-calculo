﻿// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/Fhizban
// =======================================================================================
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

// ===================================================================================
// UCE UI ACCESS REQUIREMENT
// ===================================================================================
public partial class UCE_UI_Requirement : MonoBehaviour
{
    [Header("-=-=-=- Required Assignments -=-=-=-")]
    public GameObject panel;
    public Button interactButton;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;

    [Header("-=-=-=- Configureable Colors -=-=-=-")]
    public Color headingColor;
    public Color textColor;
    public Color errorColor;

    [Header("-=-=-=- Configureable Labels -=-=-=-")]
    public string labelHeading 						= "Interaction requirements:";
    public string labelMinLevel 					= " - Required minimum Level: ";
    public string labelMaxLevel						= " - Required maximum level: ";
    
    public string labelMinHealth					= " - Min. Health Percent: ";
    public string labelMinMana						= " - Min. Mana Percent: ";
    
    public string labelDayStart						= " - Start Day: ";
    public string labelDayEnd						= " - End Day: ";
    public string labelActiveMonth					= " - Active Month: ";
    
    public string labelRequiredSkills 				= " - Required Skill(s): ";
    public string labelLevel 						= "LV";
    public string labelAllowedClasses 				= " - Allowed Class(es): ";
    public string labelRequiresGuild 				= " - Requires guild membership.";
    public string labelRequiresParty 				= " - Requires party membership.";
#if _FHIZPRESTIGECLASSES
    public string labelAllowedPrestigeClasses 		= " - Allowed Prestige Class(es): ";
#endif
#if _FHIZPVP
    public string labelRequiresRealm 				= " - Limited to specific Realm.";
#endif
    public string labelRequiresQuest				= " - Requires Quest: ";
    public string labelInProgressQuest 				= "[Must be in progress]";
#if _FHIZFACTIONS
    public string labelFactionRequirements 			= " - Required faction ratings:";
#endif
    
    public string labelRequiredEquipment 			= " - Required equipment: ";
    public string labelRequiredItems 				= " - Required item(s): ";
    public string labelDestroyItem 					= "[Destroyed on use]";
#if _FHIZHARVESTING
    public string requiredHarvestProfessions 		= " - Requires Harvesting Profession(s):";
#endif
#if _FHIZCRAFTING
    public string requiredCraftProfessions 			= " - Requires Craft Profession(s):";
#endif
#if _FHIZMOUNTS
    public string labelMountedOnly 					= " - Accessible only while mounted.";
    public string labelUnmountedOnly 				= " - Accessible only while unmounted.";
#endif
#if _FHIZTRAVEL
	public string labelTravelroute					= " - Required Travelroute:";
#endif
#if _FHIZWORLDEVENTS
	public string labelWorldEvent 					= " - Required World Event:";
#endif
#if _FHIZGUILDUPGRADES
	public string labelGuildUpgrades 				= " - Required Guild Level:";
#endif
#if _FHIZACCOUNTUNLOCKABLES
	public string labelAccountUnlockable			= " - Required Account Unlockable:";
#endif

	protected UCE_Requirements requirements;
	
    // -----------------------------------------------------------------------------------
    // Show
    // -----------------------------------------------------------------------------------
    public virtual void Show()
    {
    
    }
	
    // -----------------------------------------------------------------------------------
    // updateTextbox
    // -----------------------------------------------------------------------------------
    protected virtual void updateTextbox()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        AddMessage(labelHeading, headingColor);

        // -- Requirements

        if (requirements.minLevel > 0)
            AddMessage(labelMinLevel + requirements.minLevel.ToString(), player.level >= requirements.minLevel ? textColor : errorColor);

        if (requirements.maxLevel > 0)
            AddMessage(labelMaxLevel + requirements.maxLevel.ToString(), player.level <= requirements.maxLevel ? textColor : errorColor);

      	if (requirements.minHealth > 0)
            AddMessage(labelMinHealth + requirements.minHealth.ToString(), player.HealthPercent() >= requirements.minHealth ? textColor : errorColor);

        if (requirements.minMana > 0)
            AddMessage(labelMinMana + requirements.minMana.ToString(), player.ManaPercent() >= requirements.minMana ? textColor : errorColor);

		//TIME
		if (requirements.dayStart > 0)
			AddMessage(labelDayStart + requirements.dayStart.ToString(), requirements.dayStart <= DateTime.UtcNow.Day ? textColor : errorColor);
		if (requirements.dayEnd > 0)
			AddMessage(labelDayEnd + requirements.dayEnd.ToString(), requirements.dayEnd >= DateTime.UtcNow.Day ? textColor : errorColor);
		if (requirements.activeMonth > 0)
			AddMessage(labelActiveMonth + requirements.activeMonth.ToString(), requirements.activeMonth == DateTime.UtcNow.Month ? textColor : errorColor);
		

        if (requirements.requiredSkills.Length > 0)
        {
            AddMessage(labelRequiredSkills, textColor);
            foreach (UCE_SkillRequirement skill in requirements.requiredSkills)
                AddMessage(skill.skill.name + labelLevel + skill.level.ToString(), player.UCE_checkHasSkill(skill.skill, skill.level) ? textColor : errorColor);
        }

        if (requirements.allowedClasses.Length > 0)
        {
            AddMessage(labelAllowedClasses, textColor);
            string temp_classes = "";
            foreach (GameObject classes in requirements.allowedClasses)
                temp_classes += " " + classes.name;
            AddMessage(temp_classes, player.UCE_checkHasClass(requirements.allowedClasses) ? textColor : errorColor);
        }

        if (requirements.requiresParty)
            AddMessage(labelRequiresParty, player.InParty() ? textColor : errorColor);

        if (requirements.requiresGuild)
            AddMessage(labelRequiresGuild, player.InGuild() ? textColor : errorColor);

#if _FHIZPRESTIGECLASSES
        if (requirements.allowedPrestigeClasses.Length > 0)
        {
            AddMessage(labelAllowedPrestigeClasses, textColor);
            string temp_classes = "";
            foreach (UCE_PrestigeClassTemplate classes in requirements.allowedPrestigeClasses)
                temp_classes += " " + classes.name;
            AddMessage(temp_classes, player.UCE_CheckPrestigeClass(requirements.allowedPrestigeClasses) ? textColor : errorColor);
        }
#endif

#if _FHIZPVP
        if (requirements.requiredRealm != null && requirements.requiredAlly != null)
            AddMessage(labelRequiresRealm, requirements.checkRealm(player) ? textColor : errorColor);
#endif

#if _FHIZQUESTS
        if (requirements.requiredQuest != null)
        {
            if (!requirements.questMustBeInProgress)
            {
                AddMessage(labelRequiresQuest, player.UCE_HasCompletedQuest(requirements.requiredQuest.name) ? textColor : errorColor);
            }
            else
            {
                AddMessage(labelRequiresQuest + labelInProgressQuest, player.UCE_HasActiveQuest(requirements.requiredQuest.name) ? textColor : errorColor);
            }
        }
#else
		if (requirements.requiredQuest != null)
			AddMessage(labelRequiresQuest, player.HasCompletedQuest(requirements.requiredQuest.name) ? textColor : errorColor);
#endif

#if _FHIZFACTIONS
        if (requirements.factionRequirements.Length > 0)
        {
            AddMessage(labelFactionRequirements, textColor);
            foreach (UCE_FactionRequirement factionRequirement in requirements.factionRequirements)
                AddMessage(factionRequirement.faction.name, player.UCE_CheckFactionRating(factionRequirement) ? textColor : errorColor);
        }
#endif

        if (requirements.requiredEquipment.Length > 0)
        {
            AddMessage(labelRequiredEquipment, textColor);

            foreach (EquipmentItem item in requirements.requiredEquipment)
            {
                AddMessage(item.name, player.UCE_checkHasEquipment(item) ? textColor : errorColor);
            }
        }

        if (requirements.requiredItems.Length > 0)
        {
            AddMessage(labelRequiredItems, textColor);

            foreach (UCE_ItemRequirement item in requirements.requiredItems)
            {
                AddMessage(item.item.name + " x" + item.amount.ToString(), player.InventoryCount(new Item(item.item)) >= item.amount ? textColor : errorColor);
            }
        }

#if _FHIZHARVESTING
        if (requirements.harvestProfessionRequirements.Length > 0)
        {
            AddMessage(requiredHarvestProfessions, textColor);
            foreach (UCE_HarvestingProfessionRequirement prof in requirements.harvestProfessionRequirements)
            {
                AddMessage(prof.template.name + " " + labelLevel + prof.level, player.HasHarvestingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
            }
        }
#endif

#if _FHIZCRAFTING
        if (requirements.craftProfessionRequirements.Length > 0)
        {
            AddMessage(requiredCraftProfessions, textColor);
            foreach (UCE_CraftingProfessionRequirement prof in requirements.craftProfessionRequirements)
            {
                AddMessage(prof.template.name + " " + labelLevel + prof.level, player.UCE_HasCraftingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
            }
        }
#endif

#if _FHIZMOUNTS
        if (requirements.mountType == UCE_Requirements.MountType.Mounted)
        {
            AddMessage(labelMountedOnly, (player.UCE_mounted) ? textColor : errorColor);
        }
        else if (requirements.mountType == UCE_Requirements.MountType.Unmounted)
        {
            AddMessage(labelUnmountedOnly, (!player.UCE_mounted) ? textColor : errorColor);
        }
#endif


#if _FHIZTRAVEL
		if (!string.IsNullOrWhiteSpace(requirements.requiredTravelrouteName))
		{
			AddMessage(labelTravelroute + requirements.requiredTravelrouteName, player.UCE_travelroutes.Any(t => t.name == requirements.requiredTravelrouteName) ? textColor : errorColor);
		}
#endif
		
#if _FHIZWORLDEVENTS
		if (requirements.worldEvent != null)
		{
			AddMessage(labelWorldEvent, textColor);
			if (player.UCE_CheckWorldEvent(requirements.worldEvent, requirements.minEventCount, requirements.maxEventCount))
			{
				if (requirements.maxEventCount == 0)
					AddMessage(requirements.worldEvent.name + " (" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", textColor);
				else
            		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "]", textColor);
            }
            else
            {
            	if (requirements.maxEventCount == 0)
					AddMessage(requirements.worldEvent.name + " (" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", errorColor);
				else
            		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.UCE_GetWorldEventCount(requirements.worldEvent) + "]", errorColor);
            }
		}
#endif

#if _FHIZGUILDUPGRADES
		if (requirements.minGuildLevel > 0)
		{
			if (player.InGuild())
				AddMessage(labelGuildUpgrades + player.guildLevel.ToString() + "/" + requirements.minGuildLevel.ToString(), textColor);
			else
				AddMessage(labelGuildUpgrades + "0/" + requirements.minGuildLevel.ToString(), errorColor);
		}
#endif

#if _FHIZACCOUNTUNLOCKABLES
		if (!string.IsNullOrWhiteSpace(requirements.accountUnlockable))
		{
			if (player.UCE_HasAccountUnlock(requirements.accountUnlockable))
				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, textColor);
			else
				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, errorColor);
		}
#endif

    }

    // -----------------------------------------------------------------------------------
    // Hide
    // -----------------------------------------------------------------------------------
    public void Hide()
    {
        panel.SetActive(false);
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    protected virtual void Update()
    {
        if (!panel.activeSelf) return;

        Player player = Player.localPlayer;
        if (!player) return;

    }

    // -----------------------------------------------------------------------------------
    // AutoScroll
    // -----------------------------------------------------------------------------------
    protected void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    // -----------------------------------------------------------------------------------
    // AddMessage
    // -----------------------------------------------------------------------------------
    public void AddMessage(string msg, Color color)
    {
        GameObject go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.GetComponent<Text>().text = msg;
        go.GetComponent<Text>().color = color;
        AutoScroll();
    }

    // -----------------------------------------------------------------------------------
}

// =======================================================================================