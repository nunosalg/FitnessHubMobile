﻿namespace FitnessHubMobile.Models
{
    public class ResetPassword
    {
        public string? Email { get; set; }

        public string? Token { get; set; }

        public string? NewPassword { get; set; }

        public string? Confirm { get; set; }
    }
}
