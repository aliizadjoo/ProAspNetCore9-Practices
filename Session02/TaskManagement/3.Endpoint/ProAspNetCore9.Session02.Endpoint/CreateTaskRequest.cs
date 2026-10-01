using System;

namespace ProAspNetCore9.Session02.Endpoint;

public class CreateTaskRequest
{
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public string?  Description { get; set; }
}
