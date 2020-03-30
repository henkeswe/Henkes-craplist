surface.CreateFont( "Health_Hud_Font", {
	font = "Arial", --  Use the font-name which is shown to you by your operating system Font Viewer, not the file name
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
	shadow = false,
	additive = false,
	outline = false,
})



-- Elements to disable
local disableElement = {
    "CHudHealth",
    "CHudBattery"
}

hook.Add("HUDShouldDraw", "hide_hud", function(name)
    
    for i=1, table.Count(disableElement) do
        if(disableElement[i] == name) then
            return false
        end
    end

    -- No return or it could break other hooks.
end)


hook.Add("HUDPaint", "health_hud", function()
    local health = LocalPlayer():Health()
   -- draw.RoundedBox(0, 5, ScrH() - 15 - 20, health, 15, Color(255,0,0,255))
   -- draw.SimpleText(health, "default", 10, ScrH() - 15 - 40, Color(255,255,255,255))
    draw.RoundedBox(5, 20, ScrH() - 60, 110, 50, Color(0,0,0,200))
    draw.SimpleText(health, "Health_Hud_Font", 40, ScrH() - 60 , Color(50,150,255,255))
end)