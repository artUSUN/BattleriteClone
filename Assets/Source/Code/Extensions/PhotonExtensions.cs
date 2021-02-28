using ExitGames.Client.Photon;

namespace Source.Code.Extensions
{
    public class PhotonExtensions
    { 
        public static T GetValueOrReturnDefault<T> (Hashtable properites, string key)
        {
            if (properites.TryGetValue(key, out object property))
            {
                return (T)property;
            }
            return default;
            
        }
    }

}