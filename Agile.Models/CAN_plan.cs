﻿using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class CAN_plan : T_base
    {
        [TableField]
        public int? SceneId { get; set; }

        [TableField]
        public int? VocabularyId { get; set; }

        [TableField]
        public int? UserId { get; set; }
    }
}
