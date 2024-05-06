using ViennaDotNet.DB.Models.Common;

namespace ViennaDotNet.DB.Models.Player.Workshop
{
    public record InputItem(
         string id,
         int count,
         NonStackableItemInstance[] instances
    )
    {
    }
}
