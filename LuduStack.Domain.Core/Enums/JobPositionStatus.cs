﻿using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum JobPositionStatus
    {
        [Display(Name = "Draft")]
        Draft = 1,

        [Display(Name = "Cancelled")]
        Cancelled = 2,

        [Display(Name = "Open for Application")]
        OpenForApplication = 3,

        [Display(Name = "Closed for Application")]
        ClosedForApplication = 4,

        [Display(Name = "In Analysis")]
        InAnalysis = 5,

        [Display(Name = "Fulfilled")]
        Fulfilled = 6
    }
}