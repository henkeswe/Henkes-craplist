--test fok

AddCSLuaFile()

--DEFINE_BASECLASS("base_anim")
easylua.StartEntity("sent_doll")

ENT.Base        = "base_anim"
ENT.Type        = ENT.Type or "anim"
ENT.PrintName   = "Doll"
ENT.Author      = "Henke"
ENT.Information = "No really, It's just a doll."
ENT.Category    = "Other"

ENT.Spawnable   = true
ENT.AdminOnly   = false
ENT.RenderGroup = RENDERGROUP_TRANSLUCENT

local teddySound = Sound("ambient/creatures/teddy.wav")
local model = "models/props_c17/doll01.mdl";
--sound\ambient\voices\playground_memory.wav
--sound\ambient\voices\squeal1.wav

function ENT:SpawnFunction(ply, tr, className)

    if (!tr.Hit) then
        return
    end

	local ent = ents.Create(className)
	ent:SetPos(tr.HitPos + tr.HitNormal)
	ent:Spawn()
	ent:Activate()

	return ent
end

function ENT:Initialize()

    --if CLIENT then
        --return
    --end
    
    self:SetMoveType(MOVETYPE_VPHYSICS)
	self:SetSolid(SOLID_VPHYSICS)
    self:SetModel(model)
    
    if(SERVER) then self:PhysicsInit(SOLID_VPHYSICS) end
    
    self:PhysWake()
    
    local phys = self:GetPhysicsObject()
    if (IsValid(phys)) then
        phys:Wake()
    end
end

--function ENT:PhysicsCollide(data, physobj)
    -- Collision
--end

function ENT:OnTakeDamage(dmginfo)
    self:TakePhysicsDamage(dmginfo)
end

function ENT:Use(activator, caller)

    if (activator:IsPlayer()) then
        if(self:IsPlayerHolding()) then
            activator:DropObject()
        else
            sound.Play(teddySound, self:GetPos())
            activator:PickupObject(self)
        end
	end
end

function ENT:Draw()

    if (SERVER) then
        return
    end

    self:DrawModel()
end

easylua.EndEntity(false, false)
--scripted_ents.Register(ENT,"sent_doll")