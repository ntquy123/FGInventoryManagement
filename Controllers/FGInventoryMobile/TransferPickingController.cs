using System;
using System.Threading.Tasks;
using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.api.Attribute;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class TransferPickingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;

        public TransferPickingController(
            IFGInventoryService service,
            IServiceProvider serviceProvider,
            AmtContext context,
            IApiExecutionLockService ApiExcLockService,
            ICurrentUser currentUser)
            : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _ApiExcLockService = ApiExcLockService;
        }

        /// <param name="whCode">920</param>
        /// <param name="subwhCode">PGW2</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetTransferPickingHeadersAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetTransferPickingHeadersAsync(string whCode, string subwhCode)
        {
            try
            {
                var data = await _service.GetTransferPickingHeadersAsync(whCode, subwhCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Picking");
                return new HandleState(false, message);
            }
        }

        /// <param name="invoiceNo">20251107-00001</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetTransferPickingLinesAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetTransferPickingLinesAsync(string invoiceNo)
        {
            try
            {
                var data = await _service.GetTransferPickingLinesAsync(invoiceNo);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Picking");
                return new HandleState(false, message);
            }
        }

        /// <param name="request">
        /// Data input for Transfer Picking Scan
        /// Example:
        /// ```json
        /// {
        ///   "WhCode": "920",
        ///   "SubwhCode": "PGW2",
        ///   "LocCode": "A01-0001",
        ///   "TrAction": 1, // Note: 1 (Pick), -1 (Cancel Pick)
        ///   "TrInfo": "20251107-00001",
        ///   "CartonId": "MKS1927RGL003001",
        ///   "ContainerNo": null,
        ///   "UserId": "2021017"
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanTransferPickingAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ScanTransferPickingAsync([FromBody] TransferPickingScanRequest request)
        {
            try
            {
                var result = await _service.ScanTransferPickingAsync(request);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);
                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Picking");
                return new HandleState(false, message);
            }
        }

        private async Task<string> LogErrorAsync(Exception ex, string menuName)
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            var modelAdd = new ApiLogs
            {
                Method = HttpContext.Request.Method,
                ApiName = currentUrl,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            HandlingExceptionError exceptionError = new HandlingExceptionError();
            exceptionError.OnException(ex);
            return "Error ID:" + log.LogId + ": " + ex.Message;
        }
    }
}
