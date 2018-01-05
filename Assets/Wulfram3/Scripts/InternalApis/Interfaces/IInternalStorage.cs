namespace Assets.Wulfram3.Scripts.InternalApis.Interfaces
{
    public interface IInternalStorage
    {
        string GetValue(string key);

        void SetValue(string key, string data);
    }
}
 