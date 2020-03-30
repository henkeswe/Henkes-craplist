local females = {
    "female", 
    "alyx", 
    "mossman", 
    "chell", 
    "models/player/police_fem.mdl"
}

local males = {
    "male", 
    "arctic", 
    "gasmask", 
    "guerilla",
    "phoenix",
    "riot",
    "swat",
    "urban",
    "soldier_stripped",
    "leet", 
    "barney", 
    "breen", 
    "combine", 
    "models/player/police.mdl", 
    "dod", 
    "gman", 
    "eli", 
    "monk",
    "kleiner",
    "magnusson",
    "odessa",
}

local meta = FindMetaTable("Player")

function meta:GetGender()
    local gen = "no gender"
    local mdl = "error"

    if(self.original_model and self.original_model ~= "")then
        mdl = self.original_model
    else
        mdl = self:GetModel()
    end

    if (string.find(mdl, "models/player/skeleton.mdl")) then
        return "no gender (skeleton)"
    end

    if (string.find(mdl, "zombie")) then
        return "no gender (zombie)"
    end

    if (string.find(mdl, "models/player/corpse1.mdl") or string.find(mdl, "models/player/charple.mdl")) then
        return "no gender (corpse)"
    end

    for k,v in pairs(males) do
        if(string.find(mdl, v)) then
            return "male"
        end
    end

    for k,v in pairs(females) do
        if(string.find(mdl, v)) then
            return "female"
        end
    end

    return gen
end



print(GetGender(miar))