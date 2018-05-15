﻿using System;

namespace DC.Web.Ui.Services.Models
{
    public class IlrSubmissionMessage
    {
        public string Filename { get; set; }

        public string ContainerReference { get; set; }

        public Guid CorrelationId { get; set; }
    }
}