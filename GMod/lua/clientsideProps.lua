--[[
    cache models, pos, angle
    part cache
]]

--[[
hook.Add( "CreateMove", "MouseVector", function()
    if (input.WasMousePressed(MOUSE_LEFT)) then
        print("Left mouse button was pressed")
        print(gui.ScreenToVector(gui.MousePos())) 
    end
end)
]]


local notRun = true

if(notRun) then return end

if(idkyet)then
    idkyet.WipeEntities()
end

idkyet = {}
local ply = LocalPlayer()--ply;
idkyet.parts = {}

idkyet.boneNames = {}
idkyet.boneNames["head"]   = "ValveBiped.Bip01_Head1"
idkyet.boneNames["neck"]   = "ValveBiped.Bip01_Neck1"
idkyet.boneNames["spine4"] = "ValveBiped.Bip01_Spine4"
idkyet.boneNames["spine2"] = "ValveBiped.Bip01_Spine2"
idkyet.boneNames["spine1"] = "ValveBiped.Bip01_Spine1"
idkyet.boneNames["spine"]  = "ValveBiped.Bip01_Spine"
idkyet.boneNames["pelvis"] = "ValveBiped.Bip01_Pelvis"

--[[ structure
idkyet.parts = {}
idkyet.parts["skull"] = {}
idkyet.parts["skull"].model = "models/Gibs/HGIBS.mdl"
idkyet.parts["skull"].pos = Vector(0,0,0)
idkyet.parts["skull"].ang = Angle(0,0,0)
idkyet.parts["skull"].bone = 1
idkyet.parts["skull"].scale = 1
idkyet.parts["skull"].entity = NULL
]]

idkyet.WipeEntities = function()
    for k,v in pairs(idkyet.parts) do
        if IsValid(v.entity) then
            v.entity:Remove()
        end
    end
end

idkyet.Clear = function()
    for k,v in pairs(idkyet.parts) do
        if IsValid(v.entity) then
            v.entity:Remove()
        end
    end

    idkyet.parts = {}
end

local function AddPart(name, model, bone, pos, ang, scale)

    --local tempEnt = ClientsideRagdoll(model)
    --local isRagdoll = tempEnt:IsRagdoll() --clientside ragdoll
    --tempEnt:Remove()

    if(idkyet.parts[name] ~= nil)then
        print("Duplicate part detected!")
        return
    end
    
    name = name or "noname"
    pos = pos or Vector(0,0,0)
    ang = ang or Angle(0,0,0)
    bone = bone or 0
    scale = scale or 1
    model = model or "models/error.mdl"

    util.PrecacheModel(model)

    idkyet.parts[name] = {}
    idkyet.parts[name].model = model
    idkyet.parts[name].pos = pos
    idkyet.parts[name].ang = ang
    idkyet.parts[name].bone = bone
    idkyet.parts[name].scale = scale
    --idkyet.parts[name].isRagdoll = isRagdoll
    idkyet.parts[name].entity = NULL
    
    local part = idkyet.parts[name]
    --if(part.isRagdoll)then
    --    part.entity = ClientsideRagdoll(part.model)
    --else
        part.entity = ents.CreateClientProp(part.model)
    --end

    local ent = part.entity
    ent:SetPos(ply:GetLocalPos() + part.pos)
    ent:SetAngles(ply:GetLocalAngles() + part.ang)
    ent:SetModelScale(scale)
    --if(isRagdoll)then
    --    --ent:SetParent(ply)
    --    local phys = ent:GetPhysicsObject()
    --    phys:EnableMotion(false)
    --    ent:SetNoDraw(false)
    --    ent:FollowBone(pl, part.bone)
    --else
    ent:FollowBone(ply, part.bone)
    --end
end

--local function JigglePart --rag:ManipulateBoneJiggle(i,1) i = bone number

local function ReCreatePartEnt(name)
    if IsValid(idkyet.parts[name].entity) then
        idkyet.parts[name].entity:Remove()
    end

    local part = idkyet.parts[name]

    part.entity = ents.CreateClientProp(part.model)
    local ent = part.entity
    ent:SetPos(ply:GetLocalPos() + part.pos)
    ent:SetAngles(ply:GetLocalAngles() + part.ang)
    ent:FollowBone(ply, part.bone)
end

AddPart("skull", "models/Gibs/HGIBS.mdl", ply:LookupBone(idkyet.boneNames["spine2"]))
AddPart("fok", "models/dav0r/hoverball.mdl", ply:LookupBone(idkyet.boneNames["spine"]))
AddPart("headcrab", "models/nova/w_headcrab.mdl", ply:LookupBone(idkyet.boneNames["head"]), nil, Angle(90 + 180,0,90 + 180))
AddPart("eyeL", "models/Combine_Helicopter/helicopter_bomb01.mdl", ply:LookupBone(idkyet.boneNames["head"]), Vector(5,-7,2), Angle(0,0,45), 0.1)
AddPart("eyeR", "models/Combine_Helicopter/helicopter_bomb01.mdl", ply:LookupBone(idkyet.boneNames["head"]), Vector(5,-7,-2), Angle(0,0,-45), 0.1)
AddPart("ragdoll", "models/player/alyx.mdl", ply:LookupBone(idkyet.boneNames["neck"]))


PrintTable(idkyet.parts["ragdoll"])
--local rag = idkyet.parts["ragdoll"].entity
--for i = 0, rag:GetBoneCount() do
--    rag:ManipulateBoneJiggle(i,1)
--end

--ReCreatePartEnt("fok")
--[[
idkyet.Add = function(model, bone, positionOffset, angleOffset)
    
    if not positionOffset then
        positionOffset  = Vector(0,0,0)
    end

    if not angleOffset then
        angleOffset = Angle(0,0,0)
    end

    local pl = ply
    
    local prop = ents.CreateClientProp(model)

    local bone = pl:LookupBone(idkyet.boneNames[bone])
    prop:SetPos(pl:GetLocalPos() + positionOffset)
    prop:SetAngles(pl:GetLocalAngles() + angleOffset)
    prop:FollowBone(pl, bone)

    table.insert(idkyet.props, prop)
end

idkyet.Clear = function()
    for k,v in pairs(idkyet.props) do
        v:Remove()
    end
end

idkyet.Add("models/nova/w_headcrab.mdl", "head", Vector(0,0,0), Angle(90 + 180,0,90 + 180))

idkyet.Add("models/hunter/blocks/cube025x025x025.mdl", "spine")
]]

local ent = FindMetaTable("Entity")

function ent:GetBoneNameTable()

    local bCount = self:GetBoneCount()
    local tab = {}

    for i = 0, bCount do
        tab[i] = self:GetBoneName(i)
    end
    
    return tab
end


--[[
--PrintTable(pl:GetBoneNameTable())
pl:SetupBones()
local plHead = pl:LookupBone(boneNames["head"])
local plPelvis = pl:LookupBone(boneNames["spine"])

local offset = Vector(0,0,0)
local angOffset = Angle(90 + 180,0,90 + 180)

local clientProp = ents.CreateClientProp("models/nova/w_headcrab.mdl")
local clientProp2 = ents.CreateClientProp("models/hunter/blocks/cube025x025x025.mdl")

clientProp:SetPos(pl:GetLocalPos() + offset)
clientProp:SetAngles(pl:GetLocalAngles() + angOffset)
clientProp:FollowBone(pl, plHead)

clientProp2:SetPos(pl:GetLocalPos())
clientProp2:SetAngles(pl:GetLocalAngles())
clientProp2:FollowBone(pl, plPelvis)
clientProp:Spawn()
clientProp2:Spawn()
pl:SetupBones()
timer.Simple(5, function()
    clientProp:Remove()
    clientProp2:Remove()
end)
]]
--clientProp.SetPos(position)
--[[


	[9] = "ValveBiped.Bip01_Spine",
	[10] = "ValveBiped.Bip01_Spine1",
	[11] = "ValveBiped.Bip01_Spine2",
	[12] = "ValveBiped.Bip01_Spine4",
	[13] = "ValveBiped.Bip01_Neck1",
	[14] = "ValveBiped.Bip01_Head1",


The name of the bone.
Common generic bones ( for player models and some HL2 models ):

ValveBiped.Bip01_Head1
ValveBiped.Bip01_Spine
ValveBiped.Anim_Attachment_RH
Common hand bones (left hand equivalents also available, replace R with L)

ValveBiped.Bip01_R_Hand
ValveBiped.Bip01_R_Forearm
ValveBiped.Bip01_R_Foot
ValveBiped.Bip01_R_Thigh
ValveBiped.Bip01_R_Calf
ValveBiped.Bip01_R_Shoulder
ValveBiped.Bip01_R_Elbow

]]