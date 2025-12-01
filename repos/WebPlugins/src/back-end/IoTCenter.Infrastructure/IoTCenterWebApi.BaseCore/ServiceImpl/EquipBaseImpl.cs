// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterWebApi.BaseCore;

public class EquipBaseImpl
{
    private IEnumerable<EquipNameDto> _equipList = new List<EquipNameDto>();

    private IEnumerable<YcNameDto> _ycList = new List<YcNameDto>();

    private IEnumerable<YxNameDto> _yxList = new List<YxNameDto>();

    private IEnumerable<SetNameDto> _setList = new List<SetNameDto>();

    private string _controlEquips = string.Empty;

    private string _controlEquipUnits = string.Empty;

    private string _browseEquips = string.Empty;

    private string _browsePages = string.Empty;

    private readonly IMemoryCacheService _memoryCacheService;
    private readonly GWDbContext _context;

    private readonly Session _session;

    public EquipBaseImpl(IMemoryCacheService memoryCacheService,
        Session session,
        GWDbContext context)
    {
        _session = session;
        _memoryCacheService = memoryCacheService;
        _context = context;
    }

    private class EquipNameDto
    {
        public string EquipNm { get; set; }
        public string EquipNo { get; set; }
    }
    public async Task<string> GetEquipName(string equipNo)
    {
        string equip_nm;

        if (_equipList == null || !_equipList.Any())
        {
            var equipsQuery = _context.Equip.AsNoTracking()
                .OrderBy(d => d.EquipNo)
                .Select(d =>
                    new EquipNameDto()
                    {
                        EquipNo = d.EquipNo.ToString(),
                        EquipNm = d.EquipNm
                    });

            _equipList = await equipsQuery.ToListAsync();
        }

        equip_nm = _equipList.Where(d => d.EquipNo == equipNo).Select(d => d.EquipNm).FirstOrDefault();

        return equip_nm;
    }

    private class YcNameDto
    {
        public string YcNo { get; set; }
        public string YcNm { get; set; }
        public string EquipNo { get; set; }
        public string EquipNm { get; set; }
    }
    public async Task<string> GetYcName(string equipNo, string ycyxNo)
    {
        string yc_nm;

        if (_ycList == null || !_ycList.Any())
        {
            var ycQuery = from y in _context.Ycp.AsNoTracking()
                          join e in _context.Equip.AsNoTracking() on y.EquipNo equals e.EquipNo
                          orderby y.YcNo, y.EquipNo
                          select new YcNameDto
                          {
                              YcNo = y.YcNo.ToString(),
                              YcNm = y.YcNm,
                              EquipNo = e.EquipNo.ToString(),
                              EquipNm = e.EquipNm,
                          };
            _ycList = await ycQuery.ToListAsync();
        }
        yc_nm = _ycList.Where(d => d.EquipNo == equipNo && d.YcNo == ycyxNo).Select(d => d.YcNm).FirstOrDefault();


        return yc_nm;
    }

    private class YxNameDto
    {
        public string YxNo { get; set; }
        public string YxNm { get; set; }
        public string EquipNo { get; set; }
        public string EquipNm { get; set; }
    }
    public async Task<string> GetYxName(string equipNo, string ycyxNo)
    {
        string yx_nm;

        if (_yxList == null || !_yxList.Any())
        {
            var yxQuery = from y in _context.Yxp.AsNoTracking()
                          join e in _context.Equip.AsNoTracking() on y.EquipNo equals e.EquipNo
                          orderby y.YxNo, y.EquipNo
                          select new YxNameDto
                          {
                              YxNo = y.YxNo.ToString(),
                              YxNm = y.YxNm,
                              EquipNo = e.EquipNo.ToString(),
                              EquipNm = e.EquipNm,
                          };
            _yxList = await yxQuery.ToListAsync();
        }
        yx_nm = _yxList.Where(d => d.EquipNo == equipNo && d.YxNo == ycyxNo).Select(d => d.YxNm).FirstOrDefault();

        return yx_nm;
    }


    private class SetNameDto
    {
        public string EquipNo { get; set; }
        public string EquipNm { get; set; }
        public string SetNo { get; set; }
        public string SetNm { get; set; }
        public string SetType { get; set; }
        public string MinorInstruction { get; set; }
        public string MainInstruction { get; set; }
        public string Value { get; set; }
    }
    public async Task<string> GetSetName(string equipNo, string setNo, string dataName)
    {
        string dataValue = string.Empty;
        if (_setList == null || !_setList.Any())
        {
            var setQuery =
                from s in _context.SetParm.AsNoTracking()
                join e in _context.Equip.AsNoTracking() on s.EquipNo equals e.EquipNo into temp
                from t in temp.DefaultIfEmpty()   // left jon
                select new SetNameDto
                {
                    EquipNo = s.EquipNo.ToString(),
                    EquipNm = t.EquipNm,
                    SetNo = s.SetNo.ToString(),
                    SetNm = s.SetNm,
                    SetType = s.SetType,
                    MinorInstruction = s.MinorInstruction,
                    MainInstruction = s.MainInstruction,
                    Value = s.Value
                };
            _setList = await setQuery.ToListAsync();
        }

        var dataObj = _setList.FirstOrDefault(d => d.EquipNo == equipNo && d.SetNo == setNo);
        if (dataObj != null)
        {
            dataValue = (string)dataObj
                .GetType() // 获取类型
                .GetProperty(dataName) // 获取指定的名称属性
                ?.GetValue(dataObj, null); // param1 将返回其属性值的对象，param2索引化属性的可选索引值。 对于非索引化属性，此值应为null
        }


        return dataValue;
    }

    public async Task<string> GetRoleEquipList()
    {
        string result = string.Empty;

        if (string.IsNullOrWhiteSpace(_browseEquips))
        {
            var browseEquipQuery = (await _context.Gwrole.AsNoTracking().ToArrayAsync());

            var a = browseEquipQuery
            .Where(d => d.Name.ToLower(CultureInfo.CurrentCulture) == _session.RoleName.ToLower(CultureInfo.CurrentCulture))
            .Select(d => d.BrowseEquips);
            _browseEquips = a.FirstOrDefault();
        }

        if (!string.IsNullOrWhiteSpace(_browseEquips))
        {
            string realBrowseEquips = _browseEquips;
            result = realBrowseEquips.Replace("#", ",", StringComparison.Ordinal);
        }
        return result;
    }

    public async Task<string> GetRoleControlEquips()
    {
        string result = string.Empty;

        if (string.IsNullOrWhiteSpace(_controlEquips))
        {
            var controlEquipQuery = (await _context.Gwrole.AsNoTracking().ToArrayAsync())
                .Where(d => d.Name.ToLower(CultureInfo.CurrentUICulture) == _session.RoleName.ToLower(CultureInfo.CurrentUICulture))
                .Select(d => d.ControlEquips);
            _controlEquips = controlEquipQuery.FirstOrDefault();
        }

        if (!string.IsNullOrWhiteSpace(_controlEquips))
        {
            string realBrowseEquips = _controlEquipUnits;
            result = realBrowseEquips.Replace("#", ",", StringComparison.Ordinal);
        }
        return result;
    }

    public async Task<string> GetRoleControlEquipsUnit()
    {
        string result = string.Empty;

        if (string.IsNullOrWhiteSpace(_controlEquipUnits))
        {
            var controlEquipUnitQuery = (await _context.Gwrole.AsNoTracking().ToArrayAsync())
                .Where(d => d.Name.ToLower(CultureInfo.CurrentUICulture) == _session.RoleName.ToLower(CultureInfo.CurrentUICulture))
                .Select(d => d.ControlEquipsUnit);
            _controlEquipUnits = controlEquipUnitQuery.FirstOrDefault();
        }

        if (!string.IsNullOrWhiteSpace(_controlEquipUnits))
        {
            string realBrowseEquips = _controlEquipUnits;
            result = realBrowseEquips.Replace("#", ",", StringComparison.Ordinal);
        }

        return result;
    }

    public string GetRoleBrowsePages()
    {
        string result = string.Empty;

        if (string.IsNullOrWhiteSpace(_browsePages))
        {
            var browsePageQuery = (_context.Gwrole.AsNoTracking().ToArray())
                .Where(d => d.Name.ToLower(CultureInfo.CurrentUICulture) == _session.RoleName.ToLower(CultureInfo.CurrentUICulture))
                .Select(d => d.BrowsePages);
            _browsePages = browsePageQuery.FirstOrDefault();
        }

        if (!string.IsNullOrWhiteSpace(_browsePages))
        {
            string realBrowseEquips = _browsePages;
            result = realBrowseEquips.Replace("#", ",", StringComparison.Ordinal);
        }


        return result;
    }

    public void SysEvtLog(string userName, string eventMsg)
    {
        string csEvent = $"用户-{userName}-{eventMsg}-成功!";
        _context.SysEvt.Add(new SysEvt()
        {
            StaN = 1,
            Event = csEvent,
            Time = DateTime.Now,
            Confirmtime = DateTime.Now,
            Guid = Guid.NewGuid().ToString("N")
        });
        _context.SaveChanges();
    }

}
