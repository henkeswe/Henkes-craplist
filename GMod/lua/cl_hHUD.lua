--include "/inclu.lua"

local hookName = "cl_hHUD"
local fontName = "Font_hHUD"

surface.CreateFont( fontName,
{
	font = "Marlett",
	extended = false,
	size = 60,
	weight = 1000,
	blursize = 0,
	scanlines = 0,
	antialias = true,
	underline = false,
	italic = false,
	strikeout = false,
	symbol = false,
	rotary = false,
	shadow = true,
	additive = false,
	outline = false,
})

--https://wiki.facepunch.com/gmod/HUD_Element_List
local hide = {}
hide["CHudHealth"] = true
hide["CHudBattery"] = true

local curArmorPosX = 0
local curArmorPosY = 0
local padding = 20

local function hHUD()
	surface.SetFont(fontName)

	local health = LocalPlayer():Health()
    local armor = LocalPlayer():Armor()

    local healthTextSizeX, healthTextSizeY = surface.GetTextSize(health)
	local armorTextSizeX, armorTextSizeY = surface.GetTextSize(armor)
	local screenPosX = 0
    local screenPosY = ScrH()

	local armorTargetPosX = screenPosX + healthTextSizeX + padding * 2

    if(curArmorPosX < armorTargetPosX) then
		curArmorPosX = curArmorPosX + armorTargetPosX * FrameTime()
		
		if(curArmorPosX + armorTargetPosX > 1) then
			curArmorPosX = armorTargetPosX
		end
	end
	
    if(curArmorPosX > armorTargetPosX) then
		curArmorPosX = curArmorPosX - armorTargetPosX * FrameTime()

		if(curArmorPosX - armorTargetPosX < 1) then
			curArmorPosX = armorTargetPosX
		end
    end

	surface.SetTextColor(100, 255, 100, 255)
	surface.SetDrawColor(20, 20, 20, 125)
	surface.SetTextPos(screenPosX + padding / 2, screenPosY - healthTextSizeY)
	surface.DrawRect(screenPosX + padding / 2, screenPosY - healthTextSizeY, healthTextSizeX + padding, healthTextSizeY)
    surface.DrawText(health)

    surface.SetTextColor(100, 100, 255, 255)
	surface.SetTextPos(curArmorPosX, screenPosY - armorTextSizeY)
	surface.SetDrawColor(20, 20, 20, 125)
	surface.DrawRect(curArmorPosX, screenPosY - armorTextSizeY, armorTextSizeX, armorTextSizeY)
    surface.DrawText(armor)
end


hook.Add("HUDPaint", hookName, hHUD)

hook.Add("HUDShouldDraw", "Test_HideDefaultHud", function(name)

    if hide[name] then
        return false
    end

end)