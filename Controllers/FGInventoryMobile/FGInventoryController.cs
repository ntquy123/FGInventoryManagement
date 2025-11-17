using System;
using AutoMapper;
using entities.Common;
using entities.Setting;
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
using Microsoft.Extensions.Options;
namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class FGInventoryController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;

        public FGInventoryController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
        IApiExecutionLockService ApiExcLockService,

        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _ApiExcLockService = ApiExcLockService;


            var option = (IOptions<AppSettings>)_serviceProvider.GetService(typeof(IOptions<AppSettings>));
            if (option != null)
                _appSettings = option.Value;
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetHeaderSubWhReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> GetHeaderSubWhReceipt(string whCode, string toSubwh)
        {
            try
            {
                var data = await _service.GetHeaderSubWhReceipt(whCode, toSubwh);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt");
                return new HandleState(false, message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetDetailSubWhReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh)
        {
            try
            {
                var data = await _service.GetDetailSubWhReceipt(reqNo, whCode, toSubwh);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt");
                return new HandleState(false, message);
            }
        }
        /// <param name="pData">
        /// Data input for QR code scanning
        /// Example:
        /// ```json
        /// {
        ///   "whCode": "920",
        ///   "subwhCode": "PGW2",
        ///   "locCode": "A01-0001", 
        ///   "referInfo": "I-GA2-250730-0003",
        ///   "cartonId": "MKS1854RGL001202507080004",
        ///   "userId": "2021017",
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQrtoSubWHReceipt))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQrtoSubWHReceipt([FromBody] ParamScanQR pData)
        {
            try
            {
                var data = await _service.ScanQrtoSubWHReceipt(pData);
                var isSuccess = !string.Equals(data.rtnCode, "E", StringComparison.OrdinalIgnoreCase);
                return new HandleState(isSuccess, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Sub WH Receipt");
                return new HandleState(false, message);
            }
        }
        /// <param name="cartonId">MKS1854RGL001202507080004</param>
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpGet(nameof(GetUccByCartonAsync))]
        [AllowAnonymous]
        public async Task<HandleState> GetUccByCartonAsync(string cartonId)
        {
            try
            {
                var data = await _service.GetUccByCartonAsync(cartonId);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Location");
                return new HandleState(false, message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(ScanQRtoTransferLocation))]
        [AllowAnonymous]
        public async Task<HandleState> ScanQRtoTransferLocation([FromBody] LocTransferRequest pData)
        {
            try
            {
                var data = await _service.ScanQRtoTransferLocation(pData);
                return new HandleState(true, data.rtnMsg, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Transfer Location");
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
