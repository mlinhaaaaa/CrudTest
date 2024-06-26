﻿using System;
using System.Collections.Generic;

namespace CrudTest.Entities;

public partial class News
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Image { get; set; }

    public string? Content { get; set; }

    public Guid? Category { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Category? CategoryNavigation { get; set; }
}
