using erpsolution.dal.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<StByrmstTbl>> GetComBoBoxForBuyer();
        Task<List<MtUccListUpload>> SaveBuyerLabelUpload(DataSaveLableUpload Data);
        Task<string> GetScanIdAsync(bool isExcel, DateTime? date = null, string module = "UCC_UPLOAD", string separator = "@", int width = 6);
    }
}
