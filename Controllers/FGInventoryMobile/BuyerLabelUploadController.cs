namespace FGInventoryManagement.Controllers.FGInventoryMobile
{
    public class BuyerLabelUploadController : ControllerBaseEx<IFGInventoryService, OspAppusrTbl, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        private AppSettings _appSettings;
        public IServiceProvider _serviceProvider;
        private readonly IPkTbMasDeviceService _deviceService;
        public BuyerLabelUploadController(IFGInventoryService service,
        IServiceProvider serviceProvider,
        AmtContext context,
        IPkTbMasDeviceService deviceService,
        ICurrentUser currentUser) : base(service, currentUser)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _deviceService = deviceService;

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
