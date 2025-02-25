﻿using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FacebookApplication.Models.Data
{
    [Table("tblMessages")]
    public class MessageDTO
    {
        [Key]
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }

        [ForeignKey("From")]
        public virtual UserDTO FromUsers { get; set; }
        [ForeignKey("To")]
        public virtual UserDTO ToUsers { get; set; }
    }
}