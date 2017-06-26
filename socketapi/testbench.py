import GameController as gc
con = gc.Controller()
con.connect()

#con.closeUnityLog()

for i in range(10000000):
	n = con.getSpherical()
	if i % 20 == 0:
		print(i)

#con.openUnityLog()
print(con.getPos())

con.close()