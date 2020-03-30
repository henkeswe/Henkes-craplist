local bugbait_anim_Translates = {}

--Commented = not used by bugbait, as far as i know
bugbait_anim_Translates[ACT_MP_STAND_IDLE] = "idle_slam"
bugbait_anim_Translates[ACT_MP_WALK] =       "walk_slam"
bugbait_anim_Translates[ACT_MP_RUN] =        "run_slam"
bugbait_anim_Translates[ACT_MP_CROUCH_IDLE] = "cidle_slam"
bugbait_anim_Translates[ACT_MP_CROUCHWALK] = "cwalk_slam"
--bugbait_anim_Translates[ACT_MP_ATTACK_STAND_PRIMARYFIRE] = "range_melee"
--bugbait_anim_Translates[ACT_MP_ATTACK_CROUCH_PRIMARYFIRE] = "range_melee"
--bugbait_anim_Translates[ACT_MP_RELOAD_STAND] = "reload_pistol"
--bugbait_anim_Translates[ACT_MP_RELOAD_CROUCH] = "reload_pistol"
bugbait_anim_Translates[ACT_MP_JUMP] = "jump_slam"
bugbait_anim_Translates[ACT_LAND] = "jump_land"
--bugbait_anim_Translates[ACT_RANGE_ATTACK1] = "range_slam"
bugbait_anim_Translates[ACT_MP_SWIM_IDLE] = "swim_idle_slam"
bugbait_anim_Translates[ACT_MP_SWIM] = "swimming_slam"

hook.Add("TranslateActivity","bb", function(ply, act)

    if(!IsValid(ply) or ply != me) then
        return
    end

    local wep = ply:GetActiveWeapon()

    if(!IsValid(wep)) then
        return
    end

    if(wep:GetClass() != "weapon_bugbait") then
        return
    end


    if(bugbait_anim_Translates[act]) then
        return ply:GetSequenceActivity(ply:LookupSequence(bugbait_anim_Translates[act]))
    end

end)

