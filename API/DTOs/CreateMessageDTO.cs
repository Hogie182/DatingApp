using System;

namespace API.DTOs;

public class CreateMessageDTO
{
    public string RecipientUsername { get; set; } = null!;
    public string Content { get; set; } = null!;
}
