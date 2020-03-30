local hookName = "aaaaBAR"
local fontName = "__font_temporary_Henke"
local hookType = "HUDPaint"

surface.CreateFont(fontName,
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

local xPos,yPos = 0,0
local padding = 20
local offsetPosX, offsetPosY = 0,0
local minOffset = -7
local maxOffset = 7
local function aBAR()

    surface.SetFont(fontName)
    local scrW, scrH = ScrW() / 2, ScrH() / 4

    local text = "AAAAA, ZENI CUTE! -Henke"
    local textSizeX, textSizeY = surface.GetTextSize(text)

    xPos,yPos = scrW - textSizeX / 2, scrH
    surface.SetTextColor(255, 100, 100, 255)
	surface.SetDrawColor(35, 35, 35, 125)
	surface.SetTextPos(xPos + offsetPosX, yPos + offsetPosY)
	surface.DrawRect(xPos + offsetPosX - padding/2, yPos + offsetPosY, textSizeX + padding, textSizeY)
    surface.DrawText(text)

    offsetPosX = math.random(minOffset,maxOffset)
    offsetPosY = math.random(minOffset,maxOffset)
end

hook.Add(hookType, hookName, aBAR)

timer.Simple(3, function() 
    hook.Remove(hookType, hookName)
end)