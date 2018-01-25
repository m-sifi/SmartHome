﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHomeLib
{
    public class SmartHomeServer
    {
        public delegate void OnReceiveCommand(string command); // Callback Method

        public readonly string Ip;
        public readonly int Port;

        private Thread listenerThread;
        private TcpListener Listener;
        private OnReceiveCommand onReceiveCommand;
        private int count;

        public SmartHomeServer(string ip, int port)
        {
            Ip = ip;
            Port = port;
            Listener = new TcpListener(IPAddress.Parse(ip), port);
        }

        public SmartHomeServer(IPAddress ip, int port)
        {
            Ip = ip.ToString();
            Port = port;
            Listener = new TcpListener(ip, port);
        }

        public void Start(OnReceiveCommand onReceiveCommand)
        {
            this.onReceiveCommand = onReceiveCommand;
            count = 0;

            listenerThread = new Thread(RunListenerThread);
            listenerThread.Start();
        }


        public void Stop()
        {
            Listener.Stop();
            listenerThread.Abort();
        }

        private void RunListenerThread()
        {
            Listener.Start();

            while (true)
            {
                Socket client = Listener.AcceptSocket();
                var childSocketThread = new Thread(() =>
                {
                    ++count;
                    String request = "";

                    byte[] data = new byte[256];
                    int size = client.Receive(data);

                    for (int i = 0; i < size; i++)
                    {
                        Char c = Convert.ToChar(data[i]);
                        request += c;
                        if (c == '\n') break;
                    }

                    string startStr = "/";
                    string endStr = "HTTP";

                    int start = request.IndexOf(startStr) + startStr.Length;
                    int end = request.IndexOf(endStr) - (endStr.Length + 1);

                    request = request.Substring(start, end).Trim();

                    if (request != "favicon.ico")
                    {
                        client.Send(Encoding.ASCII.GetBytes($"Received Command '{request}'"));
                        client.Close();

                        //if (count == 0)
                        //{
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            onReceiveCommand(request);
                        }, request);
                        count = 0;
                        //}
                    }
                });
                childSocketThread.Start();
            }
        }
    }
}
