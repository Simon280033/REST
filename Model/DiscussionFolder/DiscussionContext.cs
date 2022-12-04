﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using NuGet.Protocol;
using Properties;
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

        public async Task<HttpResponseMessage> DeleteDiscussion(List<CustomDiscussionProperty> discussions)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                foreach(var item in discussions)
                {
                    var discussion = await ctx.CustomDiscussions.Where(c => c.Id == item.Id).FirstOrDefaultAsync();
                    if (discussion != null)
                    {
                        ctx.Remove(discussion);
                        var res = await ctx.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Succesfully deleted discussion!");
                return response;
            } catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to delete discussion!");
                return response;
            }
        }

        public async Task<List<CustomDiscussionProperty>> GetAllDiscussions(int teamId)
        {
            List<CustomDiscussionProperty> discussions = ctx.CustomDiscussions.Where(c => c.TeamId == teamId).ToList();
            return discussions;
        }

        public async Task<HttpResponseMessage> PostDiscussions(List<CustomDiscussionProperty> discussions)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
            if (discussions != null)
            {
                    foreach (var discussion in discussions)
                    {
                        await ctx.CustomDiscussions.AddAsync(discussion);
                        await ctx.SaveChangesAsync();
                    }
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Succesfully added discussions!");
                return response;
            }
            } 
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("Failed to add discussions!");
                return response;
            }
            response.StatusCode = HttpStatusCode.NotAcceptable;
            response.Content = new StringContent("Failed to add discussions!");
            return response;
        }
    }
}
