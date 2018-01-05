using System.Collections;

namespace Assets.Wulfram3.Scripts.InternalApis.Interfaces
{
    public interface IDiscordApi
    {
        IEnumerator PlayerJoined(string playerName);

        IEnumerator PlayerLeft(string playerName);
    }
}