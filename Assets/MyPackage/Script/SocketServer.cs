using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class ServerThread{
	private struct Struct_Internet{
		public string ip;
		public int port;
	}

	private TcpListener serverSocket = null;
	//private Socket serverSocket;
	private TcpClient clientSocket;
	//private Socket clientSocket;
	private Struct_Internet internet;
	//public byte[] receiveMessage;
	public Queue<byte> receiveMessage;
	public byte[] sendMessage;

	private Thread threadConnect;
	private Thread threadReceive;

	private bool isConnected = false;
	private System.Object queuelock = new System.Object();

	public ServerThread(/*AddressFamily family, SocketType socketType, ProtocolType protocolType,*/ string ip="127.0.0.1", int port=4567){

		//serverSocket = new Socket (family, socketType, protocolType);
		internet.ip = ip;
		internet.port = port;
		//serverSocket.Bind (new IPEndPoint (IPAddress.Parse (internet.ip), internet.port));
		serverSocket = new TcpListener (IPAddress.Parse (internet.ip), internet.port);
		receiveMessage = new Queue<byte> ();
	}

	public void Listen(){
		serverSocket.Start ();
	}

	public void StartConnect(){
		threadConnect = new Thread (Accept);
		threadConnect.IsBackground = true;
		threadConnect.Start ();
	}

	public void StopConnect(){
		try{
			if(isConnected){
				clientSocket.Close();
			}
			serverSocket.Stop();
			isConnected = false;
		}catch(Exception e){
			Debug.LogError(e.ToString());
		}
	}


	public void Send(byte []message){
		if (message == null) {
			throw new NullReferenceException ("message is null");
		} else {
			sendMessage = new byte[message.Length];
			message.CopyTo (sendMessage, 0);
		}
		SendMessage ();
	}

	public void Send(string message){
		if (message == null) {
			return;
		} else {
			Debug.Log (message);
			sendMessage = Encoding.ASCII.GetBytes (message);
		}
		SendMessage ();
	}

	public void Receive(){
		if (threadReceive != null && threadReceive.IsAlive == true)
			return;
		threadReceive = new Thread (ReceiveMessage);
		threadReceive.IsBackground = true;
		threadReceive.Start ();
	}

	public string GetString(int size = -1, bool block = false){
		return Encoding.UTF8.GetString (GetByte (size, block));
	}

	public Byte[] GetByte(long size = -1, bool block = false){
		byte[] msg;
		if (size == -1) {
			lock (queuelock) {
				msg = new byte[receiveMessage.Count];
				for (int i=0;receiveMessage.Count>0;i++) {
					msg [i] = receiveMessage.Dequeue ();
				}
			}
		} else {
			msg = new byte[size];
			for (long i = 0; i < size; i++) {
				if (block) {
					while (receiveMessage.Count <= 0)
						ReceiveMessage ();
				} else if (receiveMessage.Count <= 0)
					return msg;
				lock (queuelock) {
					msg [i] = receiveMessage.Dequeue ();
				}
			}
		}
		return msg;
	}

	private void Accept(){
		try{
			clientSocket = serverSocket.AcceptTcpClient();
			IPEndPoint client = clientSocket.Client.RemoteEndPoint as IPEndPoint;
			LogWriter.Log ("Client Connect: " + client.Address + ":" + client.Port);
			Debug.Log("Client Connect: " + client.Address + ":" + client.Port);
			isConnected = true;
		}catch(Exception e){
			LogWriter.Error ("Client Connection Error: " + e.ToString());
			Debug.LogError ("Client Connection Error: " + e.ToString());
		}
	}

	private void SendMessage(){
		try{
			if(clientSocket.Connected == true){
				clientSocket.GetStream().Write(sendMessage, 0, sendMessage.Length);
				//clientSocket.GetStream.write(sendMessage);
			}else{
				throw new Exception("Connection Error : Client Socket Connection: " + clientSocket.Connected + "/n/t in SendMessage");
			}
		}catch(Exception e){
			LogWriter.Error (e.ToString());
			Debug.LogException (e);
		}
	}

	private void ReceiveMessage(){  //receive data
		if (clientSocket.Connected == true) {
			byte[] bytes = new byte[clientSocket.ReceiveBufferSize];
			try{
				long dataLength = clientSocket.GetStream ().Read (bytes, 0, bytes.Length);

				//long dataLength = clientSocket.Receive (bytes);
				byte[] recvmsg = new byte[dataLength];

				Array.Copy (bytes, recvmsg, dataLength);
				foreach (byte b in recvmsg) {
					lock (queuelock) {
						receiveMessage.Enqueue (b);
					}
				}
			}catch(Exception e){
				LogWriter.Error ("Receive Error: " + e.ToString ());
				Debug.LogException (e);
			}

			//receiveMessage = bytes;
		}
	}

	public bool Connected{   //check if connected to client
		get{ return isConnected; }
	}
}

public class SocketServer : MonoBehaviour {

	public string ip;
	public int port;

	private ServerThread st;
	private CommandParser parser;

	// Use this for initialization

	void Awake(){
		Application.runInBackground = true;
	}

	void Start () {
		st = new ServerThread (/*AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp,*/ ip, port);
		st.Listen ();
		st.StartConnect ();
		parser = this.GetComponent<CommandParser> ();
	}

	public void Rebind(){
		st.StopConnect ();
		st = new ServerThread (/*AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, */ip, port);
		st.Listen ();
		st.StartConnect ();
	}

	public void Restart(){
		st.StopConnect();
		st.Listen ();
		st.StartConnect ();
	}

	public byte[] GetByte(int size=-1, bool block=false){
		return st.GetByte (size, block);
	}

	public string GetString(int size = -1, bool block = false){
		return st.GetString (size, block);
	}
	
	// Update is called once per frame
	void Update () {
		if (st.Connected) {


			if (st.receiveMessage.Count >=2) {
				byte[] message = st.GetByte (2);
				string msg = parser.Parse (message);
				LogWriter.Log ("Client: " + msg);
				Debug.Log ("Client: " + msg);
			}

			st.Receive ();
		}

	}

	public void Send(string message){
		st.Send (message);
	}

	public void Send(byte[] message){
		st.Send (message);
	}

	public void Send(int value){
		st.Send (BitConverter.GetBytes (value));
	}

	private void OnApplicationQuit(){
		st.StopConnect ();
	}
}
