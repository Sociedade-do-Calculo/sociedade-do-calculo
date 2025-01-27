﻿using UnityEngine;
using UnityEngine.UI;

public partial class UICastBar : MonoBehaviour
{
    public GameObject panel;
    public Slider slider;
    public Text skillNameText;
    public Text progressText;

    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null &&
            player.State == "CASTING" && player.currentSkill != -1 &&
            player.skills[player.currentSkill].showCastBar)
        {
            panel.SetActive(true);

            Skill skill = player.skills[player.currentSkill];
            float ratio = (skill.castTime - skill.CastTimeRemaining()) / skill.castTime;

            slider.value = ratio;
            skillNameText.text = skill.name;
            progressText.text = skill.CastTimeRemaining().ToString("F1") + "s";
        }
        else panel.SetActive(false);
    }
}
