using System;
using System.ComponentModel;

namespace Device.Models
{
    class Account : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string Uname { get; set; }
        public string Username
        {
            get { return Uname; }
            set
            {
                if (Uname != value)
                {
                    Uname = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string Password { get; set; }
        /*public string Password
        {
            get { return Pword; }
            set
            {
                if (Pword != value)
                {
                    Pword = value;
                    OnPropertyChanged("Password");
                }
            }
        }*/
        public Account(string username, string password)
        {
            Username = username;
            Password = password;
        }


        public void OnPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return Username;
        }
    }
}
