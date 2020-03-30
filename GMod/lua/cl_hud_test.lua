local hookName = "cl_hud_test"
local fontName = "FontTest"

surface.CreateFont( fontName,
{
	font = "Marlett",
	extended = false,
	size = 50,
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

local hide = {}
hide["CHudHealth"] = true
hide["CHudBattery"] = true


hook.Add("HUDShouldDraw", "Test_HideDefaultHud", function(name)

    if hide[name] then
        return false
    end

end)

local arCurPosX = 0

local function healthHud()
    local health = LocalPlayer():Health()
    local armor = LocalPlayer():Armor()
    local padding = 20
    surface.SetFont(fontName)

    local hpTextSizeX, hpTextSizeY = surface.GetTextSize(health)
    local arTextSizeX, arTextSizeY = surface.GetTextSize(armor)
    local screenPosX = 0 --textSizeX / 2
    local screenPosY = ScrH() -- textSizeY - textSizeY / 2;

    local arTargetPosX = screenPosX + hpTextSizeX + padding * 2

    if(arCurPosX < arTargetPosX) then
        arCurPosX = arCurPosX + arTargetPosX * FrameTime()
    end

    if(arCurPosX > arTargetPosX) then
        arCurPosX = arCurPosX - arTargetPosX * FrameTime()
    end

    surface.SetTextColor(100, 255, 100, 255)
    surface.SetTextPos(screenPosX + padding, screenPosY - hpTextSizeY)
    surface.DrawText(health)

    surface.SetTextColor(100, 100, 255, 255)
    surface.SetTextPos(arCurPosX, screenPosY - arTextSizeY)---screenPosX + hpTextSizeX + padding * 2, screenPosY - arTextSizeY)
    surface.DrawText(armor)
end

local function textHud()
    local str = "Victory!"
    surface.SetFont(fontName)

    local scrW = ScrW() * 0.5
    local scrH = ScrH() * 0.25

    local textSizeX,textSizeY = surface.GetTextSize(str)

    local rectPadding = 15.5

    local rectPosX = scrW - textSizeX / 2 - rectPadding / 2
    local rextPosY = scrH - rectPadding / 2

    surface.SetDrawColor(50, 50, 50, 255)
    surface.DrawRect(rectPosX, rextPosY, textSizeX + rectPadding, textSizeY + rectPadding)

    surface.SetTextColor(255, 255, 255)
    surface.SetTextPos(scrW - textSizeX / 2, scrH)

    surface.DrawText(str)
end

local function hookFunc()
    healthHud()
end

hook.Add("HUDPaint", hookName, hookFunc)