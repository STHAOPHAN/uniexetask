﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;

namespace uniexetask.infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<DocumentRepository>, IDocumentRepository
    {
        public DocumentRepository(UniExetaskContext context) : base(context) { }

    }
}
