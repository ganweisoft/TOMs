// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.ExcelHelper;
using IoTCenterCore.RsaEncrypt;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace Ganweisoft.IoTCenter.Module.EquipConfig.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class SystemConfigController : DefaultController
    {
        private readonly IRSAAlgorithm _rsaAlgorithm;
        private readonly ISystemConfigService _systemConfigService;
        private readonly GWDbContext _context;
        private readonly IImportManager _importManager;

        private readonly WebApiConfigOptions _webapiConfigOptions;

        private readonly ILoggingService _apiLog;

        public SystemConfigController(
            IRSAAlgorithm rsaAlgorithm,
            ISystemConfigService systemConfigService,
            GWDbContext context,
            IImportManager importManager,
            IOptions<WebApiConfigOptions> options,
            ILoggingService apiLog)
        {
            _rsaAlgorithm = rsaAlgorithm;
            _systemConfigService = systemConfigService;
            _context = context;
            _importManager = importManager;
            _webapiConfigOptions = options.Value;
            _apiLog = apiLog;
        }

        [HttpPost]
        public OperateResult<PageResult<GetEquipDataDataModel>> GetEquipDataList(GetEquipDataListModel getAllEquipDataListModel)
        {
            return _systemConfigService.GetEquipDataList(getAllEquipDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<Equip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel)
        {
            return await _systemConfigService.GetEquipDataByEquipNo(getEquipDataByNoModel);
        }

        [HttpGet]
        public OperateResult<List<List<string>>> GetEquipColumnData()
        {
            return _systemConfigService.GetEquipColumnData();
        }

        [HttpPost]
        public async Task<OperateResult> AddEquipData([FromQuery] int groupId, [FromBody] EquipDataModel equipDataModel)
        {
            return await _systemConfigService.AddEquipData(equipDataModel, groupId);
        }

        [HttpPost]
        [SkipCustomFilter]
        public async Task<OperateResult> EditEquipData(EquipDataModel equipDataModel)
        {
            return await _systemConfigService.EditEquipData(equipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelEquipData(DelEquipDataModel delEquipDataModel)
        {
            return await _systemConfigService.DelEquipData(delEquipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DeleteEquipData(DelEquipDataModel delEquipDataModel)
        {
            return await _systemConfigService.DelEquipData(delEquipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<YcpResponesModel>>> GetYcpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _systemConfigService.GetYcpDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<Ycp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return await _systemConfigService.GetYcpDataByEquipYcNo(equipYcYxSetNoModel);
        }

        [HttpGet]
        public OperateResult GetYcpColumnData()
        {
            return _systemConfigService.GetYcpColumnData();
        }

        [HttpPost]
        public async Task<OperateResult> AddYcpData(YcpDataModel ycpDataModel)
        {
            return await _systemConfigService.AddYcpData(ycpDataModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<OperateResult> EditYcpData(YcpDataModel ycpDataModel)
        {
            return await _systemConfigService.EditYcpData(ycpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _systemConfigService.DelYcpData(delYcYxSetDataModel);
        }

        [HttpGet]
        public OperateResult<IEnumerable<string>> GetCommunicationDrv()
        {
            return _systemConfigService.GetCommunicationDrv();
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<Yxp>>> GetYxpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _systemConfigService.GetYxpDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<Yxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return await _systemConfigService.GetYxpDataByEquipYxNo(equipYcYxSetNoModel);
        }

        [HttpGet]
        public OperateResult<List<List<string>>> GetYxpColumnData()
        {
            return _systemConfigService.GetYxpColumnData();
        }

        [HttpPost]
        public async Task<OperateResult> AddYxpData(YxpDataModel yxpDataModel)
        {
            return await _systemConfigService.AddYxpData(yxpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditYxpData(YxpDataModel yxpDataModel)
        {
            return await _systemConfigService.EditYxpData(yxpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _systemConfigService.DelYxpData(delYcYxSetDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<SetParmResponesModel>>> GetSetParmDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _systemConfigService.GetSetParmDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<SetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return await _systemConfigService.GetSetParmDataByEquipSetNo(equipYcYxSetNoModel);
        }

        [HttpGet]
        public OperateResult<List<List<string>>> GetSetParmColumnData()
        {
            return _systemConfigService.GetSetParmColumnData();
        }

        [HttpPost]
        [SkipCustomFilter]
        public async Task<OperateResult> AddSetData(SetDataModel setDataModel)
        {
            return await _systemConfigService.AddSetData(setDataModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public async Task<OperateResult> EditSetData(SetDataModel setDataModel)
        {
            return await _systemConfigService.EditSetData(setDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _systemConfigService.DelSetData(delYcYxSetDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<AddEquipFromModelRequest[]>> AddEquipFromModelAsync([FromBody] EquipSetModel equipSetModel)
        {
            return await _systemConfigService.AddEquipFromModelAsync(equipSetModel);
        }

        [HttpPost]
        public async Task<OperateResult> SetEquipToModel(long equipNo)
        {
            return await _systemConfigService.SetEquipToModel(equipNo);
        }

        [HttpPost]
        public async Task<OperateResult> BatchModifyFromEquip(BatchModifyFromEquipModel res)
        {
            if (res == null || res.SourceEquipNo == 0 || !res.TargetEquipNos.Any())
            {
                return OperateResult.Failed();
            }

            var checkList = res.TargetEquipNos.Append(res.SourceEquipNo).Distinct();

            if (await _context.Equip.CountAsync(d => checkList.Contains(d.EquipNo)) != checkList.Count())
            {
                return OperateResult.Failed("设备不存在");
            }

            var sorceEquipModel = await _context.Equip.AsNoTracking().FirstOrDefaultAsync(d => d.EquipNo == res.SourceEquipNo);

            var equips = await _context.Equip.Where(x => checkList.Contains(x.EquipNo)).ToArrayAsync();

            foreach (var equip in equips)
            {
                equip.EquipDetail = sorceEquipModel.EquipDetail;
                equip.AccCyc = sorceEquipModel.AccCyc;
                equip.RelatedPic = sorceEquipModel.RelatedPic;
                equip.ProcAdvice = sorceEquipModel.ProcAdvice;
                equip.OutOfContact = sorceEquipModel.OutOfContact;
                equip.Contacted = sorceEquipModel.Contacted;
                equip.EventWav = sorceEquipModel.EventWav;
                equip.CommunicationDrv = sorceEquipModel.CommunicationDrv;
                equip.LocalAddr = sorceEquipModel.LocalAddr;
                equip.EquipAddr = sorceEquipModel.EquipAddr;
                equip.CommunicationParam = sorceEquipModel.CommunicationParam;
                equip.CommunicationTimeParam = sorceEquipModel.CommunicationTimeParam;
                equip.RawEquipNo = sorceEquipModel.RawEquipNo;
                equip.Tabname = sorceEquipModel.Tabname;
                equip.AlarmScheme = sorceEquipModel.AlarmScheme;
                equip.Attrib = sorceEquipModel.Attrib;
                equip.StaIp = sorceEquipModel.StaIp;
                equip.AlarmRiseCycle = sorceEquipModel.AlarmRiseCycle;
                equip.Reserve1 = sorceEquipModel.Reserve1;
                equip.Reserve2 = sorceEquipModel.Reserve2;
                equip.Reserve3 = sorceEquipModel.Reserve3;
                equip.RelatedVideo = sorceEquipModel.RelatedVideo;
                equip.ZiChanId = sorceEquipModel.ZiChanId;
                equip.PlanNo = sorceEquipModel.PlanNo;
                equip.SafeTime = sorceEquipModel.SafeTime;
                equip.Backup = sorceEquipModel.Backup;
            }

            _context.Equip.UpdateRange(equips);

            if (await _context.SaveChangesAsync() < 0)
            {
                return OperateResult.Failed("更新失败");
            }

            return OperateResult.Success;
        }

        [HttpPost]
        public async Task<OperateResult> BatchModifyEquipParam(BatchOperateEquipModel model)
        {
            if (model == null ||
                model.StaN <= 0 ||
                model.Dicts.Count <= 0 ||
                model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            return await _systemConfigService.BatchModifyEquipParam(model);
        }

        [HttpPost]
        public async Task<OperateResult> BatchModifyYcp(BatchOperateEquipModel model)
        {
            if (model == null ||
               model.StaN <= 0 ||
               model.Dicts.Count <= 0 ||
               model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            return await _systemConfigService.BatchModifyYcp(model);
        }

        [HttpPost]
        public async Task<OperateResult> BatchModifyYxp(BatchOperateEquipModel model)
        {
            if (model == null ||
                model.StaN <= 0 ||
                model.Dicts.Count <= 0 ||
                model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            return await _systemConfigService.BatchModifyYxp(model);
        }

        [HttpPost]
        public async Task<OperateResult> BatchModifyEquipSetting(BatchOperateEquipModel model)
        {
            if (model == null ||
               model.StaN <= 0 ||
               model.Dicts.Count <= 0 ||
               model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            return await _systemConfigService.BatchModifyEquipSetting(model);
        }

        [HttpPost]
        public async Task<OperateResult> BatchDelEquip(BaseBatchOperateEquipModel model)
        {
            if (model == null || model.StaN <= 0 || model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            return await _systemConfigService.BatchDeleteEquip(model);
        }

        [HttpPost]
        public async Task<OperateResult> BatchDeleteEquip(BaseBatchOperateEquipModel model)
        {
            if (model == null || model.StaN <= 0 || model.Ids.Count <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }
            return await _systemConfigService.BatchDeleteEquip(model);
        }

        [HttpPost]
        public OperateResult BatchImportEquip([FromQuery] int? groupId, IFormFile file)
        {
            if (!groupId.HasValue)
            {
                groupId = 1;
            }

            if (!_context.EGroup.Any(d => d.GroupId == groupId))
            {
                return OperateResult.Failed("分组不存在");
            }

            if (file == null || file.Length <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            var fileName = ContentDispositionHeaderValue
                               .Parse(file.ContentDisposition)
                               .FileName.Trim('"');

            if (string.IsNullOrEmpty(fileName))
            {
                return OperateResult.Failed("文件名称不能为空");
            }

            var types = new[] {
                typeof(BatchEquipModel), typeof(BatchYcpModel) ,
                typeof(BatchYxpModel), typeof(BatchSetModel)
            };

            var equips = _importManager.ImportFromXlsx(fileName, file.OpenReadStream(), types);

            var equip = equips[0].OfType<BatchEquipModel>().ToList();
            var ycp = equips[1].OfType<BatchYcpModel>().ToList();
            var yxp = equips[2].OfType<BatchYxpModel>().ToList();
            var set = equips[3].OfType<BatchSetModel>().ToList();

            if (equips.Count <= 0)
            {
                return OperateResult.Failed("设备数量为0，操作终止");
            }

            var tableNames = new[] { "Equip", "Ycp", "Yxp", "SetParm" };
            return _systemConfigService.BatchImportEquipOrTemplate(equip, ycp, yxp, set, tableNames, groupId);
        }

        [HttpPost]
        public OperateResult BatchImportTemplate(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }

            var fileName = ContentDispositionHeaderValue
                               .Parse(file.ContentDisposition)
                               .FileName.Trim('"');

            if (string.IsNullOrEmpty(fileName))
            {
                return OperateResult.Failed("文件名称不能为空");
            }

            var types = new[] {
                typeof(BatchEquipModel), typeof(BatchYcpModel) ,
                typeof(BatchYxpModel), typeof(BatchSetModel)
            };

            var templates = _importManager.ImportFromXlsx(fileName, file.OpenReadStream(), types);
            if (templates.Count <= 0)
            {
                return OperateResult.Failed("导入失败，请确认文件是否存在数据行");
            }

            try
            {
                var equip = templates[0].OfType<BatchEquipModel>().ToList();
                var ycp = templates[1].OfType<BatchYcpModel>().ToList();
                var yxp = templates[2].OfType<BatchYxpModel>().ToList();
                var set = templates[3].OfType<BatchSetModel>().ToList();

                var tableNames = new[] { "IotEquip", "IotYcp", "IotYxp", "IotSetParm" };

                return _systemConfigService.BatchImportEquipOrTemplate(equip, ycp, yxp, set, tableNames);
            }
            catch
            {
                return OperateResult.Failed("导入失败，请确认文件中相关Sheet是否存在");
            }
        }

        [HttpPost]
        public IActionResult BatchExportEquip([FromBody] int[] ids)
        {
            if (ids is null || !ids.Any())
            {
                return BadRequest(OperateResult.Failed("请求参数不能为空"));
            }

            var qids = ids;

            var dr = _systemConfigService.BatchExportEquip(qids.ToList());
            if (dr.Code != 200)
            {
                return BadRequest(OperateResult.Failed(dr.Message));
            }

            var bytes = dr.Data;
            return File(bytes, MimeTypes.TextXlsx);
        }

        [HttpPost]
        public IActionResult BatchExportEquipTemplate([FromBody] int[] ids)
        {
            if (ids is null || ids.Any())
            {
                return BadRequest(OperateResult.Failed("请求参数不能为空"));
            }

            var qids = ids.ToList();

            var dr = _systemConfigService.BatchExportEquip(qids, false);
            if (dr.Code != 200)
            {
                return BadRequest(OperateResult.Failed("请求失败"));
            }

            var bytes = dr.Data;

            return File(bytes, MimeTypes.TextXlsx);
        }

        #region 导出设备（xml、csv）
        [HttpPost]
        public IActionResult BatchXMLOrCSVEquip(ExportEquipModel model)
        {
            if (model.ids is null || !model.ids.Any())
            {
                return BadRequest(OperateResult.Failed("请求参数不能为空"));
            }

            var qids = model.ids;
            if (model.Type == 1)
            {
                var dr = _systemConfigService.BatchExportEquip(qids.ToList());
                if (dr.Code != 200)
                {
                    return BadRequest(OperateResult.Failed(dr.Message));
                }

                var bytes = dr.Data;
                return File(bytes, MimeTypes.TextCsv);
            }
            else
            {
                var dr = _systemConfigService.BatchXmlEquip(qids.ToList());
                if (dr.Code != 200)
                {
                    return BadRequest(OperateResult.Failed(dr.Message));
                }

                var workbook = dr.Data;
                string exportPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
                string filePath = Path.Combine(exportPath, "ExportEquipFile");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var excelSheetName = "Equip" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                string xmlPath = Path.Combine(filePath, excelSheetName);

                var xmlDocument = new XmlDocument();
                var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.AppendChild(xmlDeclaration);

                var root = xmlDocument.CreateElement("Workbook");
                xmlDocument.AppendChild(root);

                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var sheet = (XSSFSheet)workbook.GetSheetAt(i);
                    var sheetName = sheet.SheetName;

                    var sheetElement = xmlDocument.CreateElement("Worksheet");
                    sheetElement.SetAttribute("name", sheetName);
                    root.AppendChild(sheetElement);

                    var tableElement = xmlDocument.CreateElement("Table");
                    sheetElement.AppendChild(tableElement);

                    var rowIterator = sheet.GetRowEnumerator();
                    while (rowIterator.MoveNext())
                    {
                        var row = (XSSFRow)rowIterator.Current;
                        var rowElement = xmlDocument.CreateElement("Row");
                        tableElement.AppendChild(rowElement);

                        var cellIterator = row.GetEnumerator();
                        while (cellIterator.MoveNext())
                        {
                            var cell = (XSSFCell)cellIterator.Current;
                            var cellValue = cell.ToString();

                            var cellElement = xmlDocument.CreateElement("Cell");
                            cellElement.InnerText = cellValue;
                            rowElement.AppendChild(cellElement);
                        }
                    }
                }
                xmlDocument.Save(xmlPath);
                return Content("\\ExportEquipFile\\" + excelSheetName);
            }
        }
        #endregion
    }
}
