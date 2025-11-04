using System;
using service.Interface;

namespace service.Service
{
    public partial class FGInventoryService : IFGInventoryService
    {
        public FGInventoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string PrimaryKey => throw new NotImplementedException();
    }
}
