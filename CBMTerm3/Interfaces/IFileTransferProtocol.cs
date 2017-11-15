namespace CBMTerm3.Interfaces
{
    interface IFileTransferProtocol
    {
        void Initialize(IDataSender dataout, IStatusRecipient statusrecipient);
        int SendFile(byte[] data); //Send File
        byte[] ReceiveFile(); //Receive File
        void Cancel();
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
