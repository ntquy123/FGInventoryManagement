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
using System.Text.Json;
namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class BuyerLabelUploadController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
        private IApiExecutionLockService _ApiExcLockService;

        public BuyerLabelUploadController(IFGInventoryService service,
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
        [HttpGet(nameof(GetComBoBoxForBuyer))]
        [AllowAnonymous]
        public async Task<HandleState> GetComBoBoxForBuyer()
        {
            try
            {
                var data = await _service.GetComBoBoxForBuyer();
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Buyer Label Upload");
                return new HandleState(false, message);
            }
        }
        [ApiExplorerSettings(GroupName = "fg_inventory_mobile")]
        [HttpPost(nameof(SaveBuyerLabelUpload))]
        [AllowAnonymous]
        public async Task<HandleState> SaveBuyerLabelUpload([FromBody] DataSaveLableUpload Data)
        {
            try
            {
                var data = await _service.SaveBuyerLabelUpload(Data);
                return new HandleState(true, data);
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Buyer Label Upload", Data);
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
            HandlingExceptionError exceptionError = new HandlingExceptionError();
            exceptionError.OnException(ex);
            return "Error ID:" + log.LogId + ": " + ex.Message;
        }
    }

}
