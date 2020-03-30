local pyID64 = "76561197986413226"

hook.Add("EntityTakeDamage", "MetalPython", function(ent, dmginfo)
    if(not dmginfo) then
        return
    end

    if(IsValid(ent) and ent:IsPlayer() and IsValid(dmginfo:GetAttacker()) and dmginfo:GetAttacker():IsPlayer()) then
        local ply = ent
        local att = dmginfo:GetAttacker()
        
        if(ply:SteamID64() == pyID64) then
            
            if(dmginfo:GetDamage() > 0) then
                
                if(dmginfo:IsBulletDamage()) then
                    local dmgPos = dmginfo:GetDamagePosition() or ply:GetPos()
                    local ric = math.random(1, 5)
                    
                    sound.Play("weapons/fx/rics/ric" .. ric .. ".wav", dmgPos, 50, math.random(90, 110), 1)
                    
                    dmginfo:SetDamage(dmginfo:GetDamage() / 2)
                    ply:TakeDamageInfo(dmginfo)
                    att:TakeDamageInfo(dmginfo)
                    dmginfo:SetDamage(0)
                end
            end
        end
    end
end)