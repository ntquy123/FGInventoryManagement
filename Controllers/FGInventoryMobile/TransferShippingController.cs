using System;
using System.Threading.Tasks;
using entities.Common;
using erpsolution.api.Attribute;
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
    public class TransferShippingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;
        public TransferShippingController(
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
                var message = await LogErrorAsync(ex, "Transfer Shipping");
                return new HandleState(false, message);
            }
        }
        /// <param name="invoiceNo">20251107-00001</param>
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
                var message = await LogErrorAsync(ex, "Transfer Shipping");
                return new HandleState(false, message);
            }
        }
        /// <param name="request">
        /// Data input for Transfer Shipping Scan
        /// Example:
        /// ```json
        /// {
        ///   "WhCode": "920",
        ///   "SubwhCode": "PGW2",
        ///   "LocCode": "A01-0001",
        ///   "TrAction": 2, // Note: 2 (Ship), -2 (Cancel Ship)
        ///   "TrInfo": "20251107-00001",
        ///   "CartonId": "MKS1927RGL003001",
        ///   "ContainerNo": "CTN001004",
        ///   "UserId": "2021017"
        /// }
        /// ```
        /// </param>
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
                var message = await LogErrorAsync(ex, "Transfer Shipping");
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
                //RequestJson = jsonData,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            HandlingExceptionError exceptionError = new HandlingExceptionError();
            exceptionError.OnException(ex);
            string mess = "Error ID:" + log.LogId + ": " + ex.Message;
            return mess;
        }
    }
}
