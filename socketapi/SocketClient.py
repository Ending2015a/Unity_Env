import socket


class clientSock:
	def __init__(self, family=socket.AF_INET, protocol=socket.SOCK_STREAM):
		self.sock = socket.socket(family, protocol)

	def connect(self, ip, port):
		self.server_addr = {'ip': ip, 'port': port}
		self.sock.connect((ip, port))

	def connect_self(self, port):
		self.sock.connect(('localhost', port))

	def send(self, message):
		self.sock.send(message)

	def recv(self, btn):
		self.recvData = b''
		while len(self.recvData) < btn:
			data = self.sock.recv(btn - len(self.recvData))
			if not data:
				return data
			self.recvData += data
		return self.recvData;

	def settimeout(self, timeout):
		self.sock.settimeout(timeout)

	def close(self):
		self.sock.close();