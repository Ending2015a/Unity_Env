using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class ServerThread{
	private struct Address{
		public string ip;
		public int port;
	}

	private TcpListener host = null;
	private TcpClient client;
	private Address addr;
	public Queue<byte> receiveMessage;
	public byte[] sendMessage;

	private Thread threadReceive =null;
	private Thread threadAccept =null;

	private bool isConnected = false;
	private System.Object queuelock = new System.Object();

	public ServerThread(string ip="127.0.0.1", int port=4567){
		addr.ip = ip;
		addr.port = port;
		host = new TcpListener (IPAddress.Parse (addr.ip), addr.port);
		receiveMessage = new Queue<byte> ();
	}

	public void Start(){
		host.Start ();
		LogWriter.Log ("Socket Host Start");
		Debug.Log ("Socket Host Start");
		threadAccept = new Thread (Accept);
		threadAccept.IsBackground = true;
		threadAccept.Start ();
	}

	public void Stop(){
		try{
			if(threadAccept != null && threadAccept.IsAlive)
				threadAccept.Abort();
			if(threadReceive != null && threadReceive.IsAlive)
				threadReceive.Abort();
			if(isConnected)
				client.Close();
			host.Stop();
			isConnected = false;
			LogWriter.Log("Socket Host Stopped");
			Debug.Log("Socket Host Stopped");
		}catch(Exception e){
			LogWriter.Error ("Stop Connection Error: " + e.ToString ());
			Debug.LogException(e);
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
		if (host == null)
			return;
		client = host.AcceptTcpClient();
		IPEndPoint client_addr = client.Client.RemoteEndPoint as IPEndPoint;
		LogWriter.Log ("Client Connect: " + client_addr.Address + ":" + client_addr.Port);
		Debug.Log("Client Connect: " + client_addr.Address + ":" + client_addr.Port);
		isConnected = true;
	}

	private void SendMessage(){
		try{
			if(client.Connected == true){
				client.GetStream().Write(sendMessage, 0, sendMessage.Length);
			}else{
				throw new Exception("Connection Error : Client Socket Connection: " + client.Connected + "/n/t in SendMessage");
			}
		}catch(Exception e){
			LogWriter.Error (e.ToString());
			Debug.LogException (e);
		}
	}

	private void ReceiveMessage(){  //receive data
		if (client.Connected == true) {
			byte[] bytes = new byte[client.ReceiveBufferSize];
			try{
				long dataLength = client.GetStream ().Read (bytes, 0, bytes.Length);
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
		}
	}

	public bool Connected{   //check if connected to client
		get{ return isConnected && client.Connected; }
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
		st = new ServerThread (ip, port);
		st.Start();
		parser = this.GetComponent<CommandParser> ();
	}

	public void Rebind(){
		st.Stop();
		st = new ServerThread (ip, port);
		st.Start();
	}

	public void Restart(){
		st.Stop();
		st.Start();
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
			if (st.receiveMessage.Count >= 2) {
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
		st.Stop();
	}
}
