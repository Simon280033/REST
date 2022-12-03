using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using NuGet.Protocol;
using Properties;
using Sociolite.Models;
using System.Net;
using System.Web.Http;

namespace WebAPI.Model.DisccusionFolder
{
    public class DiscussionContext : IDiscussionContext
    {
        private DatabaseContext ctx;

        public DiscussionContext(DatabaseContext db)
        {
            ctx = db;
        }

        public async Task<HttpStatusCode> DeleteDiscussion(int id)
        {
            var discussion = await ctx.CustomDiscussions.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (discussion != null)
            {
                ctx.Remove(discussion);
                var res = await ctx.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            return HttpStatusCode.NotFound;
        }

        public async Task<List<CustomDiscussionProperty>> GetAllDiscussions()
        {
            List<CustomDiscussionProperty> discussions = (from a in ctx.CustomDiscussions select a).ToList();
            return discussions;
        }

        public async Task<HttpStatusCode> PostDiscussion(CustomDiscussionProperty disucssion)
        {
            if (disucssion != null)
            {
                await ctx.CustomDiscussions.AddAsync(disucssion);
                await ctx.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            return HttpStatusCode.NotAcceptable;
        }
    }
}
