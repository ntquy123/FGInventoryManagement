using System;
using erpsolution.service.Interface;

namespace erpsolution.service.FGInventoryMobile
{
    public partial class FGInventoryService : IFGInventoryService
    {
        public FGInventoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string PrimaryKey => throw new NotImplementedException();
    }
}
