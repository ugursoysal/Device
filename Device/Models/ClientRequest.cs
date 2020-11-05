using System;

namespace Device.Models
{
    class ClientRequest
    {
        public int ClientID { get; set; }
        public string Message { get; set; }
        public string Argument { get; set; }

        public ClientRequest(string msg)
        {
            string[] arr = msg.Split('|');
            ClientID = Convert.ToInt32(arr[0]);
            Message = arr[1].ToString();
            Argument = arr[2].ToString();
        }
    }
}
