-- Gives a cross hair UI for the stylus for DS games

local upColor = 'green'
local downColor = 'red'
local dotColor = 'black'

function Draw(x, y, maxX, maxY, isDown)
	color = upColor
	if isDown then
		color = downColor
	end

	gui.drawLine(0, y, maxX, y, color)
	gui.drawLine(x, 0, x, maxY, color)

	if isDown then
		gui.drawPixel(x, y, dotColor)
	end
end

while true do
	if emu.getsystemid() ~= "NDS" then
		console.log('This script is for Nintendo DS only')
		break
	end
	
	local btns = joypad.get()
	
	if movie.mode() == "PLAY" and emu.framecount() > 0 then
		btns = movie.getinput(emu.framecount() - 1)
	end

	local x = btns['Touch X']
	local y = btns['Touch Y']
	local isDown = btns['Touch']

	-- A bit of a hack to ensure it is not drawing while mouse moving
	-- on the top screen
	if y == 0 then
		x = 0
	end

	pts = client.transformPoint(x, y)
	local tx = pts["x"];
	local ty = pts["y"];
	Draw(tx, ty, 10000, 10000, isDown)

	emu.frameadvance()
end