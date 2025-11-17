using System;
using System.Threading.Tasks;
using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class TransferShippingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;

        public TransferShippingController(
            IFGInventoryService service,
            IServiceProvider serviceProvider,
            AmtContext context,
            ICurrentUser currentUser)
            : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetTransferShippingHeadersAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetTransferShippingHeadersAsync(string whCode, string subwhCode)
        {
            try
            {
                var data = await _service.GetTransferShippingHeadersAsync(whCode, subwhCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetTransferShippingLinesAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetTransferShippingLinesAsync(string invoiceNo)
        {
            try
            {
                var data = await _service.GetTransferShippingLinesAsync(invoiceNo);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanTransferShippingAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ScanTransferShippingAsync([FromBody] TransferShippingScanRequest request)
        {
            try
            {
                var result = await _service.ScanTransferShippingAsync(request);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);
                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }
    }
}
