
--redo, AG, know how it works

surface.CreateFont( "RollerMon",
{
	font = "Arial", --  Use the font-name which is shown to you by your operating system Font Viewer, not the file name
	extended = false,
	size = 50,
	weight = 500,
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


secret = {}

--timerName = "RepeatingTimer"

local rm = secret

rm.render = {}
rm.render.tX = 0
rm.render.tY = 0
rm.render.tInvert = false
rm.render.tSpeed = 100
rm.render.startTimer = 15
rm.events = {}

rm.events.CurrentEvent = function() end

rm.events.StartTimer = function()

    timer.Create("RM_StartTimer", 1, 15, function()
        rm.render.startTimer = rm.render.startTimer - 1
        rm.events.PopUpText("Game starts in " + rm.render.startTimer, 0.9)
    end)
end

rm.events.RollerOut = function(ply, roller)
    rm.events.PopUpText("Player " .. ply:GetNick() .. " with their roller was taken out!", 2)
end

rm.events.StartGame = function()
    rm.events.PopUpText("Start!")
end

rm.events.WinGame = function(ply, roller)
    rm.events.PopUpText("Player " .. ply:GetNick() .. " WON!", 5)
end

rm.events.PopUpText = function(str, time)
    
    rm.events.CurrentEvent = function()
        local scrW = ScrW() * 0.5
        local scrH = ScrH() * 0.25
        local rX = rm.render.tX
        local rY = rm.render.tY

        local rectPadding = 15.5

        surface.SetFont( "RollerMon" )
        local textSizeX,textSizeY = surface.GetTextSize(str)

        surface.SetDrawColor(50, 50, 50, 255)
        surface.DrawRect(scrW + rX - textSizeX / 2 - rectPadding / 2, scrH + rY - rectPadding/2, textSizeX + rectPadding, textSizeY + rectPadding)

        surface.SetTextColor( 255, 255, 255 )
        surface.SetTextPos( scrW + rX - textSizeX / 2, scrH + rY) 

        surface.DrawText( str )

        --draw.DrawText(str, "RollerMon", scrW + rX, scrH + rY, Color(255,255,255,255), TEXT_ALIGN_CENTER)

        rm.render.tSpeed = 100

        if(not rm.render.tInvert and rm.render.tY < 100) then
            rm.render.tY = rm.render.tY + rm.render.tSpeed * FrameTime()
    
            if (rm.render.tY > 99) then
                rm.render.tInvert = true
            end
    
        elseif(rm.render.tInvert and rm.render.tY > 0) then
            rm.render.tY = rm.render.tY - rm.render.tSpeed * FrameTime()
            
            if(rm.render.tY < 1) then 
                rm.render.tInvert = false
            end
    
        end
    end

    rm.events.Destroy(time)
end

rm.events.Destroy = function(time)
    if(not time) then
        time = 0
    end

    timer.Simple(time, function()
        rm.events.OnDestroy()
    end)
end

rm.events.OnDestroy = function()
    rm.events.CurrentEvent = nil
    --perhaps some extra, but rn it just puts it as nothing
end


--rm.events.WinGame(LocalPlayer(), "")
rm.events.StartTimer()

hook.Add("HUDPaint", "RollerMonHUD", function()

    if (rm and rm.events and rm.events.CurrentEvent) then
        rm.events.CurrentEvent()
    else
        rm.render.tY = 0
        rm.render.tInvert = false
        rm.render.tSpeed = 1
        rm.render.tX = 0
    end
    
end)