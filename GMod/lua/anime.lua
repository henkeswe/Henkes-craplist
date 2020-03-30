local function arrayContains(table, val)
    for i = 1, #table do
        if table[i] == val then 
            return true
        end
    end
    return false
 end

local lastTime = os.clock();

local spam = 0

local hashTags = {}

hashTags["anime"] = function(str) 
    LocalPlayer():ConCommand("saysound " .. str)
    return string.anime(str)
end

hashTags["fancy"] = function(str)

    local ends = {".", "!", "?"}

    str = string.sub(str, 1,1):upper() .. string.sub(str, 2, string.len(str))

    if(not arrayContains(ends, string.sub(str, string.len(str)))) then
        str = str + "."
    end

    return str
end

hashTags["reverse"] = function(str)
    return string.reverse(str)
end

hook.Add("PlayerSay", "henchat", function(pl, tx, team)

    if(pl ~= LocalPlayer()) then
        return
    end

    if(spam > 3) then
        LocalPlayer():ConCommand("disconnect")
        hook.Remove("PlayerSay", "un") --extra protecc, if dc punishment didn't work for some reason
        return
    end

    if(spam > 2) then
        Say("Something probably went wrong, one more wrong and dc")
    end

    if(os.clock() - lastTime < 1) then
        spam = spam + 1
        print("NO, wtf are u doing, Spam count " .. spam)
        lastTime = os.clock()
        return
    end

    if(spam > 0) then
        spam = spam - 1 --decrease spam, because it moved past the spam check
    end

    lastTime = os.clock()

    local arg = string.Split(string.lower(tx), " ")
    local cmdArg = arg[1]
    if(string.sub(cmdArg, 1, 1) == "#") then
        local cm = string.sub(cmdArg, 2, string.len(cmdArg))

        if(hashTags[cm] ~= nil) then
            tx = string.gsub(tx, cmdArg .. " ", "", 1)

            local call,ret = pcall(hashTags[cm], tx)
            return ret
        end
    end

end)

