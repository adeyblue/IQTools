-- Script used in Bizhawk 1.12 for displaying additional info
-- like input display on the right side of the screen
-- I've included the referenced images in the repo
-- but if you actually use this, you'll need to change the paths
-- since when run, they're relative to the Bizhawk's current directory
-- not the scripts, so Bizhawk probably won't find them as is

-- globals
local g_width = client.bufferwidth()
local g_height = client.bufferheight()
local g_xDrawPos = g_width - 10
local g_yDrawPos = 10
local g_origYDrawPos = 10
local g_firstTime = true
local g_runningLevelScore = 0
local g_runningIQ = 0
local g_highestIQ = 0
local g_stageSeed = 0
local g_currentStage = 0
local g_lastGameScreen = 0
local g_checkForState15 = false
local g_stateChangeEventGuid = 0
local g_stageChangeEventGuid = 0

--- addresses/constants
-- These are only for IQ Japan SCPS-10010
local g_charXAddr = 0xA0334
local g_charYAddr = 0xA033c
local g_curPuzzleAddr = 0x76F68
local g_levelScoreAddr = 0x76DD8
local g_currentScreenAddr = 0x76EE8
local g_srandWriteAddr = 0x76BD0
local g_frameCountAddr = 0x76be0
local g_stageRowsAddr = 0x8fe8c
local g_curIQAddr = 0x6D5F0
local g_completedStagesAddr = 0x76E48
local g_pressedButtonColour = 0x7fff0000

function IncYDrawPos()
	g_yDrawPos = g_yDrawPos + 20
end

function DrawXYPos()

	local xPos = mainmemory.read_s16_le(g_charXAddr)
	local yPos = math.abs(mainmemory.read_s16_le(g_charYAddr))

	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("X Pos:%d", xPos), null, null, 16)
	IncYDrawPos()
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Y Pos:%d", yPos), null, null, 16)
	IncYDrawPos()

end

function DrawLevelScoreAndPuzzle()
  
	local puzzle = mainmemory.read_s16_le(g_curPuzzleAddr) + 1
	local levelScore = mainmemory.read_s32_le(g_levelScoreAddr)
	
	if(levelScore > g_runningLevelScore) then
		g_runningLevelScore = levelScore
	end

	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Puzzle:%03d", puzzle), null, null, 16)
	IncYDrawPos()
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Stage Seed:%d", g_stageSeed), null, null, 16)
	IncYDrawPos()
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Stage Pt:%d",  levelScore), null, null, 16)
	IncYDrawPos()
	local numRows = mainmemory.readbyte(g_stageRowsAddr)
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Stage Rows:%d", numRows), null, null, 16)
	IncYDrawPos()

end

function CalculateIQ(rowScore)
	local totalScore = g_runningLevelScore + rowScore
	
	local adjustedLevelScore = math.floor(totalScore * 1.5)
	local multiplierSubtractor = 0
	if(g_currentStage > 0) then
		multiplierSubtractor = 0.005 * (g_currentStage - 1)
	end
	local stageIQMultiplier = 0.06 - multiplierSubtractor
	local stageIQ = math.floor(math.floor(adjustedLevelScore * stageIQMultiplier) / 100)
	local totalIQ = stageIQ + g_runningIQ
	
	if(totalIQ > g_highestIQ) then
		g_highestIQ = totalIQ
	end
	
	return {stage=stageIQ, total=g_highestIQ}
end

function DrawIQ()	
	local iqs = CalculateIQ(0)
	
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Stage IQ:%d", iqs.stage), null, null, 16)
	IncYDrawPos()
	gui.drawText(g_xDrawPos, g_yDrawPos, string.format("Total IQ:%d", iqs.total), null, null, 16)
	IncYDrawPos()
end

function CheckForStageEnd()
	if(g_checkForState15 == true) then
		local gameState = mainmemory.readbyte(g_currentScreenAddr)
		--console.log("CheckForStageEnd called with state " .. gameState .. ", seenStageEnd = " .. tostring(g_seenStageEnd))
		if(gameState == 15) then
			-- calculate iq including stage rows
			local numRows = mainmemory.readbyte(g_stageRowsAddr)
			local iqs = CalculateIQ(numRows * 1000)
			--console.log(string.format("CheckForStageEnd IQ calc for cur stage %d returned stage %d,total:%d,numRows:%d,g_runningLevelScore=%d", g_currentStage, iqs.stage, iqs.total, numRows, g_runningLevelScore));
		end
		g_checkForState15 = false
	end
end

function OnStageChange()
	if(emu.framecount() > 3000) then
		g_stageSeed = mainmemory.read_s32_le(g_frameCountAddr)
		g_currentStage = g_currentStage + 1
		g_runningLevelScore = 0
		g_runningIQ = g_highestIQ
	end
end

-- this event fires on the write, but the readbyte returns the value before the write
-- so we set a flag to read it the next frame
function OnGameStateChange()
	if(emu.framecount() > 6800) then
		local gameState = mainmemory.readbyte(g_currentScreenAddr)
		if(gameState == 4) then
			g_checkForState15 = true
		end
	end
end

local psxControllerButtons = {
	["P1 Circle"]={x1=143,y1=35,x2=155,y2=49,circle=true},
	["P1 Square"]={x1=113,y1=35,x2=126,y2=49,circle=true},
	["P1 Triangle"]={x1=128,y1=21,x2=141,y2=35,circle=true},
	["P1 Cross"]={x1=128,y1=49,x2=141,y2=63,circle=true},
	["P1 Up"]={points={{33,27},{33,36},{36,39},{41,36},{41,27}},circle=false},
	["P1 Down"]={points={{33,57},{40,57},{40,50},{35,45},{33,49}},circle=false},
	["P1 Left"]={points={{21,37},{29,37},{34,43},{29,47},{21,47}},circle=false},
	["P1 Right"]={points={{50,37},{50,47},{42,47},{39,43},{43,37}},circle=false}
}

function HighlightButton(coords)
	if (coords ~= nil) then
		if (coords.circle) then
			local width = coords.x2 - coords.x1
			local g_height = coords.y2 - coords.y1
			gui.drawEllipse(coords.x1, coords.y1, width, g_height, g_pressedButtonColour, g_pressedButtonColour)
		else
			gui.drawPolygon(coords.points, g_pressedButtonColour, g_pressedButtonColour)
		end
	end
end

function MapControllerCoords(contX, contY, controllerButtons)
	for key,value in pairs(controllerButtons) do
		if(value.circle) then
			value.x1 = value.x1 + contX
			value.x2 = value.x2 + contX
			value.y1 = value.y1 + contY
			value.y2 = value.y2 + contY
		else
			for i=1,#value.points do
				local thisPoint = value.points[i]
				thisPoint[1] = thisPoint[1] + contX
				thisPoint[2] = thisPoint[2] + contY
			end
		end
	end
end

function DrawController()
	local contX = g_xDrawPos
	local contY = g_yDrawPos
	if(g_firstTime) then
		MapControllerCoords(contX, contY, psxControllerButtons)
		g_firstTime = false
	end
	gui.drawImage("controller.png", contX, contY)
	local frame = emu.framecount()
	local inputs = nil
	if(movie.isloaded()) then
		inputs = movie.getinput(frame)
	else
		inputs = joypad.get()
	end
	for key,value in pairs(inputs) do
		if value then
			HighlightButton(psxControllerButtons[key]) 
		end
	end
	g_yDrawPos = g_yDrawPos + 108
end

function DrawSquashPicture()
	gui.drawImage("iqsquash3.jpg", g_xDrawPos, g_yDrawPos)
end

function main()

	client.SetGameExtraPadding(0, 0, 160, 0)
	g_stageChangeEventGuid = event.onmemorywrite(OnStageChange, 0x80000000 + g_srandWriteAddr)
	g_stateChangeEventGuid = event.onmemorywrite(OnGameStateChange, 0x80000000 + g_currentScreenAddr)
	console.log("Registered events " .. g_stageChangeEventGuid .. " and " .. g_stateChangeEventGuid)
	
	while true do
		g_yDrawPos = g_origYDrawPos
		
		CheckForStageEnd()

		DrawXYPos()
		DrawLevelScoreAndPuzzle()
		DrawIQ()
		DrawController()
		DrawSquashPicture()

		emu.frameadvance()
	end

end

main()