using Device.Models;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Device
{
    public partial class Manager : Form
    {
        SimpleTcpServer TcpServer = null;
        BindingList<Account> Accounts = null;
        BindingList<Client> Clients = null;

        private delegate void SafeCallDelegate(string text);
        private delegate void SafeClientAdd(int id);

        private readonly OpenFileDialog openFileDialog1;
        private readonly SaveFileDialog saveFileDialog1;

        public int PORT { get { return Convert.ToInt32(portTextBox.Text); } set { portTextBox.Text = value.ToString(); } }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // This BindingSource binds the list to the DataGridView control.
        private BindingSource AccountsBindingSource = new BindingSource();
        private BindingSource ClientsBindingSource = new BindingSource();
        public Manager()
        {
            Accounts = new BindingList<Account>();
            Clients = new BindingList<Client>();
            InitializeComponent();

            //File Dialog
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 60000;
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
            timer.Start();
        }
        /*DateTime GetTimeFromShortString(string str)
        {
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(split[0]), Convert.ToInt32(split[1]), DateTime.Now.Second);
            return now;
        }*/
        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int x = 0; x < Clients.Count; x++)
            {
                try
                {
                    string[] split = Clients[x].LastRequest.Split(':');
                    int hour = Convert.ToInt32(split[0]);
                    int minute = Convert.ToInt32(split[1]);
                    if (hour == DateTime.Now.Hour)
                    {
                        if (Math.Abs(DateTime.Now.Minute - minute) > 2)
                            Clients.RemoveAt(x);
                    }
                    else
                    {
                        if (hour != DateTime.Now.Hour - 1 || Math.Abs(DateTime.Now.Minute - minute) > 2)
                            Clients.RemoveAt(x);
                    }
                }
                catch
                {

                }
                //DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, , Convert.ToInt32(split[1]), DateTime.Now.Second);
            }
        }

        public void StartServer(int port)
        {
            TcpServer = new SimpleTcpServer().Start(port);
            if (TcpServer != null)
            {
                TcpServer.Delimiter = 0x13;
                TcpServer.DelimiterDataReceived += OnClientRequest;
            }
        }

        public void AddClient(int id)
        {
            Client newClient = new Client(id)
            {
                LastInfo = "connecting...",
                LoadedAccount = null
            };
            Clients.Add(newClient);
        }
        public void RemoveClient(int id)
        {
            Client select = Clients.FirstOrDefault(a => a.ID == id);
            Clients.Remove(select);
        }
        public ClientStates GetClientState(int clientID)
        {
            var client = Clients.FirstOrDefault(a => a.ID == clientID);
            if (client == null)
                return ClientStates.None;
            return client.State;
        }
        bool RegisterID(int clientID)
        {
            if (Clients.Count == 30 || Clients.FirstOrDefault(a => a.ID == clientID) != null)
            {
                return false;
            }
            else
            {
                var d = new SafeClientAdd(AddClient);
                this.Invoke(d, clientID);
                return true;
            }
        }
        private void OnClientRequest(object sender, SimpleTCP.Message e)
        {
            if (e != null)
            {
                string response = "default";
                string msg = e.MessageString;
                if (msg.Length > 3)
                {
                    switch (msg)
                    {
                        case "checkID": response = GetEmptyClientID().ToString(); break;
                        default:
                            ClientRequest req = new ClientRequest(msg);
                            var client = Clients.FirstOrDefault(a => a.ID == req.ClientID);
                            if (client != null)
                                client.LastRequest = DateTime.Now.ToShortTimeString();
                            switch (req.Message)
                            {
                                case "register": // add client to the list
                                    response = RegisterID(req.ClientID) ? "true" : "false";
                                    break;
                                case "main": // get state
                                    if (client == null)
                                    {
                                        response = RegisterID(req.ClientID) ? GetClientState(req.ClientID).ToString() : "no_response";
                                    }
                                    else
                                    {
                                        response = GetClientState(req.ClientID).ToString();
                                        client.LastInfo = req.Argument; // set last info
                                        Task.Run(async () => await HttpSimpleClient.SendRequestAsync(req.ClientID, response, req.Argument));
                                    }
                                    break;

                                case "account": // get account data
                                    if (client != null && client.LoadedAccount != null)
                                    {
                                        client.State = ClientStates.Login;
                                        response = client.LoadedAccount.Username + ":" + client.LoadedAccount.Password;
                                        Task.Run(async () => await HttpSimpleClient.SendRequestAsync(req.ClientID, "account", client.LoadedAccount.Username));
                                    }
                                    else
                                    {
                                        response = "no_account";
                                    }
                                    break;

                                case "state": // get account data
                                    if (client != null)
                                    {
                                        switch (req.Argument)
                                        {
                                            case "ingame": client.State = ClientStates.InGame; break;
                                            case "queue": client.State = ClientStates.Queue; break;
                                        }
                                        response = "no_response";
                                    }
                                    break;
                                case "shutdown": // shutting down the client
                                    {
                                        if (Clients.Count == 0 || client == null)
                                        {
                                            response = "no_response";
                                        }
                                        else
                                        {
                                            var d = new SafeClientAdd(RemoveClient);
                                            this.Invoke(d, req.ClientID);
                                            response = "no_response";
                                        }
                                    }
                                    break;
                                case "event": break;
                            }
                            break;
                    }
                }
                if (response != "no_response")
                    e.ReplyLine(response);
            }
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            try
            {
                Clients.FirstOrDefault(a => a.ID == Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value)).State = ClientStates.Idle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
                    Clients.FirstOrDefault(a => a.ID == Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value)).State = ClientStates.Start;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        private void Manager_Load(object sender, EventArgs e)
        {
            StartServer(PORT);

            // Bind the list to the BindingSource.
            this.AccountsBindingSource.DataSource = Accounts;
            this.ClientsBindingSource.DataSource = Clients;

            // Attach the BindingSource to the DataGridView.
            this.listBox1.DataSource = this.AccountsBindingSource;
            this.dataGridView1.DataSource = this.ClientsBindingSource;
        }
        private int GetEmptyClientID()
        {
            if (Clients != null)
            {
                for (int i = 1; i < 30; i++)
                {
                    if (Clients.FirstOrDefault(a => a.ID == i) == null)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        private void SaveAccountsButton_Click(object sender, EventArgs e)
        {
            if (Accounts == null || Accounts.Count < 1)
            {
                MessageBox.Show($"Nothing to be saved.");
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        List<string> contents = new List<string>();
                        foreach (var x in Accounts)
                        {
                            contents.Add($"{x.Username}:{x.Password}");
                        }
                        File.WriteAllLines(saveFileDialog1.FileName, contents);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Can't save file.\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                    }
                }
            }
        }

        private void LoadAccountsButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lines = new List<string>(File.ReadAllLines(openFileDialog1.FileName));
                    foreach (var x in lines)
                    {
                        string[] split = x.Split(':');
                        if (split.Length == 2)
                        {
                            Accounts.Add(new Account(split[0], split[1]));
                        }
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Can't load file.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void ClearAccountsButton_Click(object sender, EventArgs e)
        {
            Accounts.Clear();
        }

        private void AddAccountButton_Click(object sender, EventArgs e)
        {
            if (userTextBox.Text.Length > 2 && passTextBox.Text.Length > 2)
            {
                Accounts.Add(new Account(userTextBox.Text, passTextBox.Text));
            }
        }

        private void RemoveAccountButton_Click(object sender, EventArgs e)
        {
            Accounts.RemoveAt(listBox1.SelectedIndex);
        }

        private void Manager_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var selectedAccount = Accounts.FirstOrDefault(a => a.Username == listBox1.SelectedItem.ToString());
                if (selectedAccount != null)
                {
                    if (dataGridView1.SelectedRows != null)
                    {
                        var selectedClient = Clients.FirstOrDefault(a => a.ID == Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
                        if (selectedAccount != null)
                        {
                            selectedClient.LoadedAccount = selectedAccount;
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("Please select account and ID.");
        }

        private void StartAllButton_Click(object sender, EventArgs e)
        {
            foreach (var x in Clients)
            {
                if (x.LoadedAccount != null && x.LoadedAccount.Username.Length > 2)
                    x.State = ClientStates.Start;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
