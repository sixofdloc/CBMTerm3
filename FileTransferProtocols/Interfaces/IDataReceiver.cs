﻿namespace FileTransferProtocols.Interfaces
{
    public interface IDataReceiver
    {
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
