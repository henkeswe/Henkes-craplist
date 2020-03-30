--shitty, rewrite

local player = FindMetaTable("Player")
local dir = "hadmin"
local name = "husers"

local fullPath = dir .. "/" .. name .. ".txt"

local userTable = {}

if (!file.Exists(fullPath, "DATA")) then
	if (file.Exists(dir, "DATA") and !file.IsDir(dir, "DATA")) or !file.Exists(dir, "DATA") then
		file.CreateDir(dir)
		print("[H] Created directory: '" .. dir .. "'")
	end
	print("[H] '" .. fullPath .. "' not found, writing blank..")
	file.Write(fullPath, util.TableToJSON(userTable))
else
	userTable = util.JSONToTable(file.Read(fullPath), "DATA")
	print("[H] Loaded '" .. fullPath .. "'")
end


function player:IsUserGroup(group)

	local group = string.lower(group)
	
	if (userTable[group] != nil and table.HasValue(userTable[group], self:SteamID64())) then 
		return true
	else
		return false
	end
end

function player:GetUserGroup()

	local group = "" 

	for k,v in pairs(userTable) do
		for i = 1, table.Count(v) do
			if(v[i] == self:SteamID64()) then
				group = k
			end
		end
	end

	return group
end


function player:SetUserGroup(group, save)


	local group = string.lower(group)

	--Check if group exist
	if (userTable[group] == nil) then
		userTable[group] = {}
		print("[H] Created new group: '" .. group .. "'")
	end
	
	for k,v in pairs(userTable) do
		for i = 1, table.Count(v) do
			if(v[i] == self:SteamID64()) then
				print("[H] Removed entry '" .. v[i] .. "' from group '" .. k .. "'")
				table.remove(v, i)
			end
		end
	end

	--Check if this player already exist in another table
	--for k,v in pairs(userTable) do
	
		--print(userTable[k])
		--for i = 0, 1, table.Count(userTable[k]) do
		--	print(userTable[k][i])
		--end
		--if(k != group and table.HasValue(userTable[k], self:SteamID64())) then
		--	userTable[k][self:SteamID64()] = ""
		--	print("removed..")
		--end
	--end
	
	--Check if player is in group
	if(!table.HasValue(userTable[group], self:SteamID64())) then
		table.insert(userTable[group], self:SteamID64())
		print("[H] Added '" .. self:SteamID64() .. "' to group: '" .. group .. "'")
	end
	
	if (save == false) then
		print("[H] SetUserGroup without saving!")
		return
	end
	
	file.Write(fullPath, util.TableToJSON(userTable))
	print("[H] saved file: '" .. fullPath .. "'")
end


--Override gmod default player_auth initialspawn
hook.Add("PlayerInitialSpawn", "PlayerAuthSpawn", function(ply)
	if(ply:IsUserGroup("admin") or ply:IsUserGroup("superadmin")) then
		print("Admin " .. tostring(ply) .. " connected.")
	else
		ply:SetUserGroup("user")
	end
end)