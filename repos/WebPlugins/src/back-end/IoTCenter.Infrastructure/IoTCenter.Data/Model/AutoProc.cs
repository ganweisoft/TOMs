// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace IoTCenter.Data.Model
{
    [Table("autoproc")]
    public class AutoProc
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("iequip_no")]
        public int IequipNo { get; set; }
        [Column("iycyx_no")]
        public int IycyxNo { get; set; }
        [Column("iycyx_type")]
        public string IycyxType { get; set; }
        [Column("delay")]
        public int Delay { get; set; }
        [Column("oequip_no")]
        public int OequipNo { get; set; }
        [Column("oset_no")]
        public int OsetNo { get; set; }
        [Column("value")]
        public string Value { get; set; }
        [Column("procdesc")]
        public string ProcDesc { get; set; }
        [Column("enable")]
        public bool Enable { get; set; }

        [Column("reserve")]
        public string Reserve { get; set; }


        private IEnumerable<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
        [NotMapped]
        public IEnumerable<KeyValuePair<string, string>> ReserveParameters
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Reserve))
                {
                    try
                    {
                        _parameters = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(this.Reserve);
                    }
                    catch (Exception)
                    {
                    }
                }

                return _parameters;
            }
            set
            {
                var keyDuplicates = value.GroupBy(x => x.Key).Where(x => x.Count() > 1);
                if (keyDuplicates.Any())
                {
                    return;
                }

                var valueIsEmpties = value.Where(x => string.IsNullOrEmpty(x.Value));
                if (valueIsEmpties.Any())
                {
                    return;
                }

                var parameters = _parameters.ToList();
                foreach (var valuePair in value)
                {
                    var item = parameters.Find(p => p.Key == valuePair.Key);
                    if (!item.Equals(default(KeyValuePair<string, string>)))
                    {
                        parameters.Remove(item);
                    }

                    parameters.Add(valuePair);
                }
                _parameters = parameters;

                this.Reserve =
                    System.Text.Json.JsonSerializer.Serialize(_parameters.ToDictionary(x => x.Key, x => x.Value));
            }
        }
    }
}
