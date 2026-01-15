using System;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;

namespace erpsolution.service.FGInventoryMobile
{
    public partial class FGInventoryService : IFGInventoryService
    {
        private readonly IApiExecutionLockService _ApiExcLockService;
        public FGInventoryService(IServiceProvider serviceProvider, IApiExecutionLockService ApiExcLockService) : base(serviceProvider)
        {
            _ApiExcLockService = ApiExcLockService;
        }

        public override string PrimaryKey => throw new NotImplementedException();
    }
}
