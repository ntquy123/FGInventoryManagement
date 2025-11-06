using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using entities.Common;
using entities.Setting;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities.Common;
using FGInventoryManagement.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;
using service.Common.Base.Interface;
using service.Interface;

namespace service.Service
{
    public partial class FGInventoryService : ServiceBase<FgRequestRow>
    {
        public async Task<List<StByrmstTblView>> GetComBoBoxForBuyer()
        {
            return await _amtContext.StByrmstTbl
                .AsNoTracking()
                .Where(x => x.Useyn == "Y")
                .Select(x => new StByrmstTblView
                {
                    ByrCd = x.Byrcd,
                    ByrNm = x.Byrnm
                })
                .ToListAsync();
        }

        public async Task<List<MtUccListUpload>> SaveBuyerLabelUpload(DataSaveLableUpload Data)
        {
            string scanId = await GetScanIdAsync(isExcel: false);
            var nowDate = DateTime.Now;
            int seq = 0;
            var dataSave = Data.lstCartonId.Select(x => new MtUccListUpload
            {
                XlsId = scanId,
                XlsSq = seq++,
                WhCode = Data.WhCode,
                CartonId = x,
                ByrCd = Data.BuyerCd,
                LabelType = "B",
                Status = "N",
                CrtDat = nowDate,
                UptDat = nowDate,
                CrtId = Data.UserId,
                UptId = Data.UserId,

            }).ToList();
            await _amtContext.MtUccListUpload.AddRangeAsync(dataSave);
            await _amtContext.SaveChangesAsync();
            return dataSave;
        }

        public async Task<string> GetScanIdAsync(
       bool isExcel,
       DateTime? date = null,
       string module = "UCC_UPLOAD",
       string separator = "@",
       int width = 6)
        {
            string typeChar = isExcel ? "E" : "S";

            DateTime useDate = date ?? DateTime.UtcNow;
            string dateStr = useDate.ToString("yyyyMMdd");

            const string sql = "SELECT PK_NUMBERING_RULES_FC(:P_MODULE, :P_SEP, :P_WIDTH, :P_TYPE, :P_DATE) FROM DUAL";

            var connection = _amtContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                var pModule = command.CreateParameter();
                pModule.ParameterName = "P_MODULE";
                pModule.DbType = DbType.String;
                pModule.Value = module;
                command.Parameters.Add(pModule);

                var pSep = command.CreateParameter();
                pSep.ParameterName = "P_SEP";
                pSep.DbType = DbType.String;
                pSep.Value = separator;
                command.Parameters.Add(pSep);

                var pWidth = command.CreateParameter();
                pWidth.ParameterName = "P_WIDTH";
                pWidth.DbType = DbType.Int32;
                pWidth.Value = width;
                command.Parameters.Add(pWidth);

                var pType = command.CreateParameter();
                pType.ParameterName = "P_TYPE";
                pType.DbType = DbType.String;
                pType.Value = typeChar;
                command.Parameters.Add(pType);

                var pDate = command.CreateParameter();
                pDate.ParameterName = "P_DATE";
                pDate.DbType = DbType.String;
                pDate.Value = dateStr;
                command.Parameters.Add(pDate);

                var result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToString(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
