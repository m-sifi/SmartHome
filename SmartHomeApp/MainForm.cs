﻿using SmartHomeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartHomeApp
{
    public partial class MainForm : Form
    {

        public SmartHomeServer Server;

        public MainForm()
        {
            InitializeComponent();

            IPAddress ip = IPAddress.Any;
            int port = 3000;

            Server = new SmartHomeServer(ip, port);
            Server.Start(ProcessCommand);
        }

        private void ProcessCommand(string command)
        {
            Invoke(
            new Action(
                () =>
                {
                    OutputTB.Text += $"[{DateTime.Now}] Received GET Request /{command}{Environment.NewLine}";
                }
            ));
            //MessageBox.Show($"Command Received: {command}", "Smart Home Server");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Server.Stop();
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            AddModule form = new AddModule();
            form.Show();
        }

        private void btnRegisterArduino_Click(object sender, EventArgs e)
        {
            RegisterDevice form = new RegisterDevice();
            form.Show();
        }

        private void lblAddAction_Click(object sender, EventArgs e)
        {
            AddAction form = new AddAction();
            form.Show();
        }

        /* private void btnAddModule_Click(object sender, EventArgs e)
        {
            AddModule form = new AddModule();
            form.Show();
        } */

    }
}
