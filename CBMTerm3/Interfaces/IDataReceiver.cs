namespace CBMTerm3.Classes
{
    public interface IDataReceiver
    {
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
