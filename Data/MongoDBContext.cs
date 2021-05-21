using System;
using System.Collections.Generic;
using System.Text;
using dotnetCoreWithJWTAuth.Data.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnetCoreWithJWTAuth.Data
{
    public class MongoDBContext : IMongoDBContext
    {
    }
}