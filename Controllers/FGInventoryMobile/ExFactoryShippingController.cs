using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class ExFactoryShippingController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private readonly AmtContext _context;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;
        public ExFactoryShippingController(
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
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetExFactoryShippingHeadersAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetExFactoryShippingHeadersAsync(string whCode)
        {
            try
            {
                var data = await _service.GetExFactoryShippingHeadersAsync(whCode);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="shpPkg">MKS-25W47-0001</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetExFactoryShippingLinesAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetExFactoryShippingLinesAsync(string shpPkg)
        {
            try
            {
                var data = await _service.GetExFactoryShippingLinesAsync(shpPkg);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                return new HandleState(false, ex.Message);
            }
        }

        /// <param name="request">
        /// Example:
        /// ```json
        /// {
        ///   "WhCode": "920",
        ///   "SubwhCode": "PGW2",
        ///   "LocCode": "A01-0001",
        ///   "TrAction": 2,
        ///   "TrInfo": "MKS-25W47-0001",
        ///   "CartonId": "MKS1927RGL003001",
        ///   "ContainerNo": "CTN001004",
        ///   "UserId": "2021017"
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanExFactoryShippingAsync))]
        [AllowAnonymous]
        public async Task<HandleState> ScanExFactoryShippingAsync([FromBody] ExFactoryShippingScanRequest request)
        {
            try
            {
                var result = await _service.ScanExFactoryShippingAsync(request);
                var isSuccess = !string.Equals(result.rtnCode, "E", StringComparison.OrdinalIgnoreCase);

                if (!isSuccess)
                {
                    var message = await LogErrorAsync(new Exception(result.rtnMsg ?? "Ex-Factory shipping error"), "ExFactoryShipping", request);
                    result.rtnMsg = message;
                }

                return new HandleState(isSuccess, result.rtnMsg, result);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "ExFactoryShipping");
                return new HandleState(false, message);
            }
        }

        private async Task<string> LogErrorAsync(Exception ex, string menuName, object vm = null)
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            string jsonData = JsonSerializer.Serialize(vm);
            var modelAdd = new ApiLogs
            {
                Method = HttpContext.Request.Method,
                ApiName = currentUrl,
                RequestJson = jsonData,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            string mess = "Error ID:" + log.LogId + ": " + ex.Message;
            return mess;
        }
    }
}
