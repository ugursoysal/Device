using System;
using System.ComponentModel;

namespace Device.Models
{
    public enum ClientStates
    {
        None,
        Idle,
        Start,
        Login,
        Queue,
        InGame,
    }
    class Client : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int Id { get; set; }
        public int ID
        {
            get { return Id; }
            set
            {
                if (Id != value)
                {
                    Id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        private string Info { get; set; }
        public string LastInfo
        {
            get { return Info; }
            set
            {
                if (Info != value)
                {
                    Info = value;
                    OnPropertyChanged("LastInfo");
                }
            }
        }
        private Account LAccount { get; set; }
        public Account LoadedAccount
        {
            get { return LAccount; }
            set
            {
                if (LAccount != value)
                {
                    LAccount = value;
                    OnPropertyChanged("LoadedAccount");
                }
            }
        }
        private ClientStates Stt { get; set; }
        public ClientStates State
        {
            get { return Stt; }
            set
            {
                if (Stt != value)
                {
                    Stt = value;
                    OnPropertyChanged("State");
                }
            }
        }
        private string LRequest { get; set; }
        public string LastRequest
        {
            get { return LRequest; }
            set
            {
                if (LRequest != value)
                {
                    LRequest = value;
                    OnPropertyChanged("LastRequest");
                }
            }
        }
        public Client(int id)
        {
            ID = id;
            State = ClientStates.Idle;
            LastRequest = DateTime.Now.ToShortTimeString();
        }

        public void OnPropertyChanged(String propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception x)
            {
                Console.Write(x.Message);
            }
        }
    }
}
