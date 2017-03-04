using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class StateObject
{
    public Socket WorkSocket;
    public const int BUFFER_SIZE = 256;
    public byte[] Buffer = new byte[BUFFER_SIZE];
    public StringBuilder Builder = new StringBuilder();
}

public class SocketController
{
    public enum RequestType
    {
        Login
    }

    private readonly Dictionary<RequestType, int> requestTypePorts = new Dictionary<RequestType, int>() {
        {RequestType.Login, 9162}
    };
    
    public string Response;
    public bool Executing;

    private int port;
    private Socket clientSocket;
    private readonly IPAddress serverAddress;
    private IPEndPoint endPoint;
    private string message;

    public SocketController()
    {
        serverAddress = IPAddress.Parse("127.0.0.1");
    }

    private void UpdateEndpoint(int endpointPort)
    {
        port = endpointPort;
        endPoint = new IPEndPoint(serverAddress, port);
    }

    public void StartClient(RequestType type, string msg)
    {
        try
        {
            Executing = true;

            UpdateEndpoint(requestTypePorts[type]);

            message = msg;

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(endPoint, ConnectCallback, clientSocket);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void StopClient()
    {
        try
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            Executing = false;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ConnectCallback(IAsyncResult result)
    {
        try
        {
            clientSocket.EndConnect(result);

            Debug.Log("Connected");

            Send(message);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void Send(string data)
    {
        byte[] str_data = Encoding.ASCII.GetBytes(data);

        clientSocket.BeginSend(str_data, 0, str_data.Length, 0, SendCallback, clientSocket);
    }

    private void SendCallback(IAsyncResult result)
    {
        try
        {
            Socket client_socket = (Socket)result.AsyncState;

            int bytes_sent = client_socket.EndSend(result);
            Debug.Log(string.Format(
                "Sent {0} bytes to server",
                bytes_sent));

            Receive(clientSocket);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void Receive(Socket client)
    {
        try
        {
            StateObject state = new StateObject { WorkSocket = client };

            client.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, ReceiveCallback, state);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            StateObject state = (StateObject)result.AsyncState;
            Socket client = state.WorkSocket;

            int bytes_read = client.EndReceive(result);

            if (bytes_read > 0)
            {
                state.Builder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytes_read));
                client.BeginReceive(state.Buffer, 0, StateObject.BUFFER_SIZE, 0, ReceiveCallback, state);
            }
            else
            {
                if (state.Builder.Length > 1)
                    Response = state.Builder.ToString();

                Debug.Log(Response);

                StopClient();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}
