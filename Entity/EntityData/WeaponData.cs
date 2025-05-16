using Mirror;

namespace ZHSM
{
    public class WeaponData : EntityData
    {
        public WeaponData(int entityId, int typeId, int weaponId, NetworkConnectionToClient connection) : base(entityId, typeId)
        {
            WeaponId = weaponId;
            Connection = connection;
        }

        public int WeaponId { get; private set; }
        public NetworkConnectionToClient Connection { get; private set; }
    }
}