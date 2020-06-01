﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.DtoParameters
{
    public class CompanyDtoParameters
    {
        private const int MaxPageSize = 20;
        public string CompanyName { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        private int _pageSize = 5;

        public string OrderBy { get; set; } = "CompanyName";
        public string Fields { get; set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
