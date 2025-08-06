﻿using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}