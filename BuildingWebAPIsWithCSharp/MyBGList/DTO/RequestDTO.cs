﻿using MyBGList.Attributes;
using System.ComponentModel;

namespace MyBGList.DTO
{
    public class RequestDTO
    {
        [DefaultValue(0)]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        [DefaultValue("Name")]
        [SortColumnValidator(typeof(BoardGameDTO))]
        public string? SortColumn { get; set; } = "Name";

        [DefaultValue("ASC")]
        [SortOrderValidator]
        public string? SortOrder { get; set; } = "ASC";

        [DefaultValue(null)]
        public string? FilterQuery { get; set; } = null;
    }
}
