using System;
using System.Collections.Generic;

namespace MyApplication.Components.Model.AWS;

public partial class AwsAgent
{
    public short Id { get; set; }

    public string AwsGuid { get; set; } = null!;

    public string AwsUserName { get; set; } = null!;

    public int? EmployeeId { get; set; }
}
