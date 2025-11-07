using AutoMapper;
using entities.Common;
using entities.Setting;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace erpsolution.api.Controllers.FGInventoryMobile
{
    public class BuyerLabelUploadController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;

        public BuyerLabelUploadController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,

        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;


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
                return new HandleState(false, ex.Message);
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
                return new HandleState(false, ex.Message);
            }
        }
    }

}
