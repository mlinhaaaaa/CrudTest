using System;
using System.Collections.Generic;

namespace CrudTest.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int NewsCount { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
