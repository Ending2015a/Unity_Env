import GameController as gc
con = gc.Controller()
con.connect()

con.closeUnityLog()

for i in range(1000):
	con.getPos()
	con.getPos()
	con.setPos(0.052, 3.63, -1.979248)
	con.setPos(0.052, 3.63, -1.979248)
	con.getPos()
	con.getPos()
	if i % 20 == 0:
		print(i)

con.openUnityLog()
print(con.getPos())

con.close()